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

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
            var granted = await _userRepository.GetUserReward(id, region_name);
            if (granted != null)
            {
                return Ok(granted);
                
            }
            else
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + id + "' does not have rewards yet.");
            }

        }

        // POST api/values
        [HttpPost("PostGrantedReward/{id}")]
        public async Task<IActionResult> Post(string id, [FromBody] Bin grantedReward)
        {
            if (grantedReward == null)
            {
                return BadRequest("granted reward formatted incorrectly.");
            }

            var granted = new Bin
            {
                Region_name = grantedReward.Region_name,
                Projects = grantedReward.Projects
            };

            int result = await _userRepository.AddUserReward(id, granted);

            if (result == 1)
            {
                /***  HERE MIGHT CAL Collective_EF ***/

                // return HTTP 201 Created with user object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = id}, granted);

            }
            else if (result == -7)
            {
                return Ok("User already planted in this area. It is ok to plant again");
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
