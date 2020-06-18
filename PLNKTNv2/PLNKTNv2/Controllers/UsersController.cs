using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Authentication;
using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using PLNKTNv2.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    /// <summary>
    /// The Users Controller holds methods to retrieve and manipulate data held about users in the database on AWS.
    /// </summary>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccount _account;
        private readonly IRewardRepository _rewardRepository;
        private readonly IUserRepository _userRepository;


        /// <summary>
        /// Constructor to create UsersController with DI assets.
        /// </summary>
        /// <param name="userRepository">Repository provides database access to User information.</param>
        /// <param name="rewardRepository">Repository provides database access to Reward and Challenge information.</param>
        /// <param name="account">Provides access to authenticated user data.</param>
        public UsersController(
            IUserRepository userRepository,
            IRewardRepository rewardRepository,
            IAccount account
            )
        {
            _userRepository = userRepository;
            _rewardRepository = rewardRepository;
            _account = account;
        }

        /// <summary>
        /// DELETE method to remove a user and their associated data from the database.
        /// </summary>
        /// <param name="id">The <c>string</c> id of the user to be removed.</param>
        /// <returns></returns>
        /// <response code="200">Item removed from the database successfully.</response>
        /// <response code="404">Item not found in the database.</response>
        /// <response code="400">Poorly formed request.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            var userDeleted = await _userRepository.DeleteUser(id);

            if (userDeleted > 0)
            {
                return Ok();
            }
            else if (userDeleted == -9)
            {
                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all user data from the database by Id (user Id retrieved fron JWT token).
        /// </summary>
        /// <returns><c>Task/<IActionResult/></c> HTTP response with HTTP code and user details in body.</returns>
        /// <response code="200">Returns authenticated user data.</response>
        /// <response code="404">Item not found in the database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByIdAsync()
        {
            var id = _account.GetAccountId(this.User);
            var user = await _userRepository.GetUser(id);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get the total number of users in the database.
        /// </summary>
        /// <returns><c>IActionResult</c> HTTP response with HTTP code and user count as int in body.</returns>
        /// <response code="200">Returns number of users in the database.</response>
        [HttpGet("Count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public IActionResult GetUserCount()
        {
            var userCount = _userRepository.GetUserCount("UserCount");
            return Ok(userCount);
        }

        /// <summary>
        /// POST method to create new user in the database.
        /// </summary>
        /// <param name="userDto">DTO representation of a user entry for user creation.</param>
        /// <returns><c>Task/<IActionResult/></c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="409">Item already exists in database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] UserDetailsDTO userDto)
        {
            // Generate the 'user rewards' for this new 'user' ready for insertion to the DB so, that the user has a complete
            // list of rewards and challenges so, they can participate in reward and challenge completion.
            var rewards = await _rewardRepository.GetAllRewards();
            var userRewards = (List<UserReward>)GenerateUserRewards(rewards);

            // Create new user
            var user = new User()
            {
                Id = userDto.Id,
                First_name = userDto.First_name,
                Last_name = userDto.Last_name,
                Created_at = DateTime.UtcNow,
                Email = userDto.Email,
                Level = userDto.Level,
                EcologicalMeasurements = new List<EcologicalMeasurement>(),
                LivingSpace = userDto.LivingSpace,
                NumPeopleHousehold = userDto.NumPeopleHousehold,
                CarMPG = userDto.CarMPG,
                ShareData = userDto.ShareData,
                Country = userDto.Country,
                UserRewards = userRewards,
                GrantedRewards = new List<Bin>()
            };

            // Save the new user to the DB
            var result = await _userRepository.CreateUser(user);

            if (result == 1)
            {
                return CreatedAtAction("Get", new { id = userDto.Id }, user);
            }
            else if (result == -10)
            {
                return Conflict();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// PUT replaces current user data with that provided in the request's HTTP body.
        /// </summary>
        /// <param name="dto">Partial representation of user object with fields that can be manipulated by this request.</param>
        /// <returns></returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="404">Item not found in database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] UserDetailsDTO dto)
        {
            var user = new User()
            {
                Id = dto.Id,
                First_name = dto.First_name,
                Last_name = dto.Last_name,
                Created_at = dto.Created_at,
                Email = dto.Email,
                Level = dto.Level,
                LivingSpace = dto.LivingSpace,
                NumPeopleHousehold = dto.NumPeopleHousehold,
                CarMPG = dto.CarMPG,
                ShareData = dto.ShareData,
                Country = dto.Country
            };

            var result = await _userRepository.UpdateUser(user);

            if (result == 1)
            {
                return Ok();
            }
            else if (result == -9)
            {
                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        //  Adds all reward and challenge data required by the user object to a
        internal static ICollection<UserReward> GenerateUpdateUserRewards(ICollection<Reward> rewards)
        {
            ICollection<UserReward> generatedUserRewards = new List<UserReward>();

            foreach (var _reward in rewards)
            {
                var userRewardChallenge = new List<UserRewardChallenge>();

                foreach (var challenge in _reward.Challenges)
                {
                    userRewardChallenge.Add(new UserRewardChallenge
                    {
                        Id = challenge.Id,
                        Rule = new UserRewardChallengeRule
                        {
                            Category = challenge.Rule.Category,
                            RestrictionType = challenge.Rule.RestrictionType,
                            SubCategory = challenge.Rule.SubCategory,
                            Time = challenge.Rule.Time,
                            AmountToConsume = challenge.Rule.AmountToConsume
                        },
                    });
                }

                var userReward = new UserReward
                {
                    Id = _reward.Id,
                    Challenges = userRewardChallenge,
                };

                generatedUserRewards.Add(userReward);
            }

            return generatedUserRewards;
        }

        //  Adds all reward and challenge data required by the user object to a
        internal static ICollection<UserReward> GenerateUserRewards(ICollection<Reward> rewards)
        {
            ICollection<UserReward> generatedUserRewards = new List<UserReward>();

            foreach (var _reward in rewards)
            {
                var userRewardChallenge = new List<UserRewardChallenge>();

                foreach (var challenge in _reward.Challenges)
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
                            Time = challenge.Rule.Time,
                            AmountToConsume = challenge.Rule.AmountToConsume
                        },
                        Status = UserRewardChallengeStatus.Incomplete,
                        NotificationStatus = NotificationStatus.Not_Complete
                    });
                }

                var userReward = new UserReward
                {
                    Id = _reward.Id,
                    Challenges = userRewardChallenge,
                    DateCompleted = null,
                    Status = UserRewardStatus.Incomplete,
                    NotificationStatus = NotificationStatus.Not_Complete,
                    IsRewardGranted = false
                };

                generatedUserRewards.Add(userReward);
            }

            return generatedUserRewards;
        }
    }
}