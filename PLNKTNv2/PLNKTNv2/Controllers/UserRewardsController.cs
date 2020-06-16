using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.Models;
using PLNKTNv2.Repositories;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRewardsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserRewardsController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // PUT: api/UserRewards/<string>
        [HttpPut("{userId}")]
        public async Task<IActionResult> Put(string userId, [FromBody] UserReward model)
        {
            if (model == null || string.IsNullOrWhiteSpace(userId))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("User Reward information formatted incorrectly.");
            }

            int result = await _userRepository.UpdateUserReward(userId, model);

            if (result == 1)
            {
                // return HTTP 200 Ok item was updated.  PUT does not require the item to be returned in HTTP body.
                // Not done to save bandwidth.
                return Ok();
            }
            else if (result == -9)
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User does not exist.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }
    }
}