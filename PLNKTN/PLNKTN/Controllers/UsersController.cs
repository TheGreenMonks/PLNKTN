using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.DTOs;
using PLNKTN.Models;
using PLNKTN.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "PLNKTN", "app is up and running" };
        }

        // GET api/users/test
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("User ID information formatted incorrectly.");
            }

            var user = await _userRepository.GetUser(id);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("User with ID '" + id + "' does not exist.");
            }
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDetailsDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User information formatted incorrectly.");
            }

            var user = new User()
            {
                Id = userDto.Id,
                First_name = userDto.First_name,
                Last_name = userDto.Last_name,
                Created_at = DateTime.UtcNow,
                Email = userDto.Email,
                Level = userDto.Level,
                LivingSpace = userDto.LivingSpace,
                NumPeopleHousehold = userDto.NumPeopleHousehold,
                CarMPG = userDto.CarMPG,
                ShareData = userDto.ShareData,
                Country = userDto.Country
            };

            var result = await _userRepository.CreateUser(user);

            if (result == 1)
            {
                return CreatedAtAction("Get", new { id = userDto.Id }, userDto);
            }
            else if (result == -10)
            {
                return Conflict("User with ID '" + userDto.Id + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
            
        }



        // PUT api/users/test
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]string value)
        {
            return BadRequest("users PUT is not implemented");
        }

        // DELETE api/users/test
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return BadRequest("users DELETE is not implemented");
        }
    }
}
