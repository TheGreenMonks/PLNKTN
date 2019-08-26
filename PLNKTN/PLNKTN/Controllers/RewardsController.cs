using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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

        // DELETE api/Rewards/5
        [HttpDelete("{id}")]
        public void Delete(int id)
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
                    Status = "INCOMPLETE"
                });
            }

            var userReward = new UserReward
            {
                Id = reward.Id,
                Challenges = userRewardChallenge,
                DateCompleted = null,
                Status = "INCOMPLETE"
            };

            _userRepository.AddUserReward(userReward);
        }
    }
}
