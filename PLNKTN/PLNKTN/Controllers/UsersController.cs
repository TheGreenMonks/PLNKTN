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
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "PLNKTN", "app is up and running" };
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserCreateDTO userDto)
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

            if (await _userRepository.Add(user))
            {
                return Ok();
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
            
        }



        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]string value)
        {
            return BadRequest("users PUT is not implemented");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return BadRequest("users DELETE is not implemented");
        }
    }
}
