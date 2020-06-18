using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Authentication;
using PLNKTNv2.Models;
using PLNKTNv2.Repositories;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{

    /// <summary>
    /// The UserRewards Controller holds methods to retrieve and manipulate data held about user's rewards in the database on AWS.
    /// </summary>
    /// <remarks>
    /// Each User Reward maps to a game reward, and its associated challenges, and holds data relating to completion and notification.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class UserRewardsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccount _account;

        /// <summary>
        /// UserRewards constructor with DI assets.
        /// </summary>
        /// <param name="userRepository">Repository provides database access to User information.</param>
        /// <param name="account">Provides access to authenticated user data.</param>
        public UserRewardsController(IUserRepository userRepository, IAccount account)
        {
            _userRepository = userRepository;
            _account = account;
        }


        /// <summary>
        /// PUT updates a user reward with data provided in the request's HTTP body (user Id retrieved fron JWT token).
        /// </summary>
        /// <param name="model"></param>
        /// <returns><c>Task/<IActionResult/></c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="404">Item not found in database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] UserReward model)
        {
            var id = _account.GetAccountId(this.User);
            int result = await _userRepository.UpdateUserReward(id, model);

            if (result == 1)
            {
                // return HTTP 200 Ok item was updated.  PUT does not require the item to be returned in HTTP body.
                // Not done to save bandwidth.
                return Ok();
            }
            else if (result == -9)
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound();
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest();
            }
        }
    }
}