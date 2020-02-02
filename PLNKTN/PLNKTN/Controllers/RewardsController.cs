using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.Models;
using PLNKTN.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RewardsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Calculate Reward Completion

        [AcceptVerbs("CalcRewards")]
        public async void CalculateUserRewardCompletion()
        {
            IEmailHelper emailHelper = new EmailHelper();
            ICollection<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            BatchWrite<User> batchUsers = null;

            foreach (var _user in users)
            {
                User updatedUser = RewardCalculation.CalculateUserRewardCompletion(_user, ref emailHelper);

                if (updatedUser != null)
                {
                    batchUsers = _unitOfWork.Repository<User>().Update(updatedUser);
                }
            }

            if (batchUsers != null)
            {
                await _unitOfWork.Commit(new BatchWrite[] { batchUsers });
            }
            // Send email with results of this method (Dexter has email originator address and recipients)
            emailHelper.SendEmail("Reward");
        }

        #endregion

        #region Calculate Challenge Completion

        [AcceptVerbs("CalcChallenges")]
        public async void CalculateUserChallengeCompletion()
        {
            IEmailHelper emailHelper = new EmailHelper();
            ICollection<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            BatchWrite<User> batchUsers = null;

            foreach (var _user in users)
            {
                User updatedUser = ChallengeCalculation.CalculateUserChallengeCompletion(_user, ref emailHelper);

                if (updatedUser != null)
                {
                    batchUsers = _unitOfWork.Repository<User>().Update(updatedUser);
                }
            }

            if (batchUsers != null)
            {
                await _unitOfWork.Commit(new BatchWrite[] { batchUsers });
            }

            // Send email with results of this method (Dexter has email originator address and recipients)
            emailHelper.SendEmail("Challenge");
        }

        #endregion


        // GET: api/Rewards
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.Repository<Reward>().GetAllAsync();

            if (result != null && result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("No Rewards are present in the DB.");
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            Reward result = await _unitOfWork.Repository<Reward>().GetByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("Reward does not exist.");
            }
        }

        // POST api/Rewards
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Reward reward)
        {
            if (reward == null)
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            var batchReward = _unitOfWork.Repository<Reward>().Insert(reward);
            var userReward = UserRewards.GenerateUserReward(reward);
            IList<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            ICollection<User> updatedUsers = UserRewards.AddUserRewardToAllUsers(userReward, users);
            BatchWrite<User> batchUsers = _unitOfWork.Repository<User>().Update(updatedUsers);

            await _unitOfWork.Commit(new BatchWrite[] { batchReward, batchUsers });

            return CreatedAtAction("Get", new { id = reward.Id }, reward);
        }

        // PUT api/Rewards
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Reward reward)
        {
            if (reward == null)
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            BatchWrite<Reward> batchReward = _unitOfWork.Repository<Reward>().Update(reward);
            var userReward = UserRewards.GenerateUpdatedUserReward(reward);
            IList<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            ICollection<User> updatedUsers = UserRewards.UpdateUserRewardInAllUsers(userReward, users);
            BatchWrite<User> batchUsers = _unitOfWork.Repository<User>().Update(updatedUsers);

            await _unitOfWork.Commit(new BatchWrite[] { batchReward, batchUsers });

            return Ok();
        }

        // DELETE api/Rewards/rewardId
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            BatchWrite<Reward> batchReward = _unitOfWork.Repository<Reward>().DeleteById(id);
            IList<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            ICollection<User> updatedUsers = UserRewards.DeleteUserRewardFromAllUsers(id, users);
            BatchWrite<User> batchUsers = _unitOfWork.Repository<User>().Update(updatedUsers);

            await _unitOfWork.Commit(new BatchWrite[] { batchReward, batchUsers });

            return Ok();
        }
    }
}
