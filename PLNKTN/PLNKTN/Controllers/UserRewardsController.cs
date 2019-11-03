using Microsoft.AspNetCore.Mvc;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Controllers
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

        // GET: api/UserRewards
        [HttpGet]
        public IActionResult Get()
        {
            return NotFound();
        }

        // GET: api/UserRewards/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            return NotFound();
        }

        // POST: api/UserRewards
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return NotFound();
        }

        // PUT: api/UserRewards/5
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

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NotFound();
        }
    }
}
