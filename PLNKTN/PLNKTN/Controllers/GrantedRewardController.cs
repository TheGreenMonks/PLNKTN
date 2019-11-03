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
    public class GrantedRewardController : Controller
    {
        private readonly IUserRepository _userRepository;

        public GrantedRewardController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET api/values/5
        [HttpGet("GetUserGrantedRewards/{id}/{region_name}")]
        public async Task<IActionResult> Get(string id, string region_name)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("User ID information formatted incorrectly.");
            }

            var granted = await _userRepository.GetUserGrantedReward(id, region_name);

            if (granted != null)
            {
                return Ok(granted);
                
            }
            else
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("Either the User with ID '" + id + "' does not exist or this User does not have any " +
                    "assigned Rewards in region '" + region_name + "' yet.");
            }

        }

        // POST api/values
        [HttpPost("PostGrantedReward/{id}/{region_name}")]
        public async Task<IActionResult> Post(string id, string region_name, [FromBody] Rgn project)
        {
            if (project == null)
            {
                return BadRequest("project formatted incorrectly.");
            }

            int result = await _userRepository.AddUserGrantedReward(id, region_name, project);

            if (result == 1)
            {
                // return HTTP 201 Created with project object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = id, region_name = region_name }, project);
            }
            else if (result == -7)
            {
                return Ok("User already planted in this area. It is ok to plant again. Region count has been incremented");
            }
            else if (result == -9)
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + id + "' does not exist.");
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }
    }
}
