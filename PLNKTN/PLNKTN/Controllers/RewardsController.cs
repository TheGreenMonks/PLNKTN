using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.Models;
using PLNKTN.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardRepository _rewardRepository;
        private readonly IUserRepository _userRepository;

        public RewardsController(IRewardRepository rewardRepository, IUserRepository userRepository)
        {
            _rewardRepository = rewardRepository;
            _userRepository = userRepository;
        }

        // GET: api/Rewards
        [HttpGet]
        public async void Get()
        {
            /* Algorithm
             * Foreach User check each Reward's Challenge status to see if the User has completed all Challenges.  If all Challenges in a
             * Reward are complete then set the status of the UserReward to complete and set the date of completion.
             * 
             * TODO - Efficiency Improvements -- 1. Only get the required fields from the db (i.e. eco measurments aren't needed)
             */

            // Get Users from DB

            /* TODO
             * This action will need to be paginated and parallelised as the max transfer size is 1MB, which will will be exceed quickly.
             * this functionality will enable multiple app threads to work on crunching the data via multiple 1MB requests
             * Reference -> https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_Scan.html
             * and -> https://stackoverflow.com/questions/48631715/retrieving-all-items-in-a-table-with-dynamodb
             * and -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html
             */

            // Get all users from the DB who have NOT NULL Reward arrays
            var dbUsers = await _userRepository.GetAllUsers();

            // Iterate over each user that has rewards in their DB record.
            foreach (var _user in dbUsers.Where(u => u.UserRewards != null))
            {
                // Flag to indicate if changes to challenge status have been made
                var changesMade = false;
                // Sort eco measurements by date for easier processing in all challenge rule checks.
                _user.EcologicalMeasurements.OrderBy(e => e.Date_taken);

                // Iterate over all incomplete rewards.
                foreach (var _reward in _user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
                {
                    var incompleteChallenges = _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete).ToList();
                    
                    // if all challenges are complete then set the reward status to complete
                    if (incompleteChallenges.Count() == 0)
                    {
                        _reward.DateCompleted = DateTime.UtcNow;
                        _reward.Status = UserRewardStatus.Complete;
                        changesMade = true;
                    }
                }

                // Save changes to DB (removes eco emasurements as they don't all need to be sent back to the server.
                if (changesMade)
                {
                    _user.EcologicalMeasurements = null;
                    await _userRepository.UpdateUser(_user);
                }

            }

            sendEmail();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/Rewards
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Reward reward)
        {
            if (reward == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Reward information formatted incorrectly.");
            }

            var user = new Reward()
            {
                Id = reward.Id,
                Title = reward.Title,
                ImageURL = reward.ImageURL,
                Description = reward.Description,
                Link = reward.Link,
                GridPosition = reward.GridPosition,
                Text_When_Completed = reward.Text_When_Completed,
                Text_When_Not_Completed = reward.Text_When_Not_Completed,
                Source = reward.Source,
                Challenges = reward.Challenges,
                Country = reward.Country,
                Overview = reward.Overview,
                Impact = reward.Impact,
                Tree_species = reward.Tree_species
            };

            var result = await _rewardRepository.CreateReward(reward);

            // Add reward to the users in the Database.
            postUserRewards(reward);

            if (result == 1)
            {
                // return HTTP 201 Created with reward object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = reward.Id }, reward);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as reward already exists in DB
                return Conflict("Reward with ID '" + reward.Id + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // PUT api/Rewards
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Reward reward)
        {
            if (reward == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Reward information formatted incorrectly.");
            }

            var user = new Reward()
            {
                Id = reward.Id,
                Title = reward.Title,
                ImageURL = reward.ImageURL,
                Description = reward.Description,
                Link = reward.Link,
                GridPosition = reward.GridPosition,
                Text_When_Completed = reward.Text_When_Completed,
                Text_When_Not_Completed = reward.Text_When_Not_Completed,
                Source = reward.Source,
                Challenges = reward.Challenges,
                Country = reward.Country,
                Overview = reward.Overview,
                Impact = reward.Impact,
                Tree_species = reward.Tree_species
            };

            var result = await _rewardRepository.UpdateReward(reward);

            if (result == 1)
            {
                // return HTTP 201 Created with reward object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = reward.Id }, reward);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as reward already exists in DB
                return Conflict("Reward with ID '" + reward.Id + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // DELETE api/Rewards
        [HttpDelete]
        public void Delete()
        {
        }

        /* Business Logic
         * Methods here are stubs to transfer data further than just the rewards table.  It is a way of updating the user rewards and the rewards table at the same time.
         */

        private void postUserRewards(Reward reward)
        {
            var userRewardChallenge = new List<UserRewardChallenge>();

            foreach (var challenge in reward.Challenges)
            {
                userRewardChallenge.Add(new UserRewardChallenge
                {
                    Id = challenge.Id,
                    DateCompleted = null,
                    Rule = new UserRewardChallengeRule
                    {
                        Category = challenge.Rule.Category,
                        RestrictionType = challenge.Rule.RestrictionType,
                        SubCategory = challenge.Rule.SubCategory,
                        Time = challenge.Rule.Time
                    },
                    Status = UserRewardChallengeStatus.Incomplete
                });
            }

            var userReward = new UserReward
            {
                Id = reward.Id,
                Challenges = userRewardChallenge,
                DateCompleted = null,
                Status = UserRewardStatus.Incomplete
            };

            _userRepository.AddUserReward(userReward);
        }

        private void sendEmail()
        {
            var pwFile = "C:\\gmpw.txt";
            string fromEmail = "";
            string toEmail = "";
            string pw = "";

            // Code to get pw from local file to keep it out of the code.
            if (System.IO.File.Exists(pwFile))
            {
                using (StreamReader stream = System.IO.File.OpenText(pwFile))
                {
                    fromEmail = stream.ReadLine();
                    toEmail = stream.ReadLine();
                    pw = stream.ReadLine();
                }

                // Code to set upi email and send it.
                var fromAddress = new MailAddress("skippy6263@gmail.com", "PLNKTN Web App");
                var toAddress = new MailAddress("dextercunningham@hotmail.co.uk", "Developers");
                string subject = "Reward and Challenge Completion Calculation Execution";
                string body = "The Reward and Challenge Completion Calculation methods have been executed at " +
                    DateTime.UtcNow.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n\n" +
                    "Best regards,\n\n\n" + "The PLNKTN Web App";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, pw),
                    Timeout = 20000
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            else
            {
                Debug.WriteLine("Error: Email send error");
                Debug.WriteLine("Location: RewardsController in 'sendEmail()' method.");
                Debug.WriteLine("Cause: Could not send email, possibly due to bad password file, bad access or bad email set up.");
            }
        }
    }
}
