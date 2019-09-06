using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
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
        private readonly string ctrlName = "Reward";

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
            // New List to hold all email messages generated
            ICollection<string> emailMessages = new List<string>();

            // Iterate over each user that has rewards in their DB record.
            foreach (var _user in dbUsers.Where(u => u.UserRewards != null))
            {
                // Flag to indicate if changes to challenge status have been made
                var changesMade = false;

                // Iterate over all incomplete rewards.
                foreach (var _reward in _user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
                {
                    var incompleteChallenges = _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete).ToList();
                    
                    // if all challenges are complete then set the reward status to complete
                    if (incompleteChallenges.Count() == 0)
                    {
                        _reward.DateCompleted = DateTime.UtcNow;
                        _reward.Status = UserRewardStatus.Complete;
                        _reward.NotificationStatus = NotificationStatus.Not_Notified;
                        changesMade = true;
                        emailMessages.Add(EmailHelper.EmailMessage(_user.Id, ctrlName, _reward.Id));
                    }
                }

                // Save changes to DB (removes eco emasurements as they don't all need to be sent back to the server.
                if (changesMade)
                {
                    _user.EcologicalMeasurements = null;
                    await _userRepository.UpdateUser(_user);
                }
            }

            // If there are no item completions this time then confirm this in the email with a message.
            if (emailMessages.Count == 0)
            {
                emailMessages.Add("No " + ctrlName + "s completed this time...\n");
            }

            EmailHelper.SendEmail(emailMessages, ctrlName);
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

            // Generate a 'user reward' and add it to all users in the DB.
            var rewards = new List<Reward>();
            rewards.Add(reward);
            var userReward = UsersController.GenerateUserRewards(rewards);
            await _userRepository.AddUserRewardToAllUsers(userReward.First());

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
    }
}
