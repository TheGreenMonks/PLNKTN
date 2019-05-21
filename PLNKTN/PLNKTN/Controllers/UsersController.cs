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
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("User ID information formatted incorrectly.");
            }

            var user = await _userRepository.GetUser(id);

            if (user != null)
            {
                // return HTTP 200
                return Ok(user);
            }
            else
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + id + "' does not exist.");
            }
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDetailsDTO userDto)
        {
            if (userDto == null)
            {
                // return HTTP 400 badrequest as something is wrong
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
                // return HTTP 201 Created with user object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = userDto.Id }, userDto);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as user already exists in DB
                return Conflict("User with ID '" + userDto.Id + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }



        // PUT api/users/test
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]UserDetailsDTO dto)
        {
            if (dto == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("User information formatted incorrectly.");
            }

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
                // return HTTP 200 Ok user was updated.  PUT does not require user to be returned in HTTP body.
                // Not done to save bandwidth.
                return Ok();
            }
            else if (result == -9)
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + dto.Id + "' does not exist.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // DELETE api/users/test
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("User information formatted incorrectly.");
            }

            var userDeleted = await _userRepository.DeleteUser(id);

            if (userDeleted > 0)
            {
                return Ok("user with ID '" + id + "' deleted.");
            }
            else if (userDeleted == -9)
            {
                return NotFound("No user with ID '" + id + "' available to be deleted.");
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }
    }
}
