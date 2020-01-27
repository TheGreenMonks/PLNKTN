using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.DTOs;
using PLNKTN.Models;
using PLNKTN.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> Get()
        { 
            var result = await _unitOfWork.Repository<User>().GetAllAsync();

            if (result != null && result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("No users are present in the DB.");
            }
        }

        // GET api/users/test
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("User information formatted incorrectly.");
            }

            User result = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("User does not exist.");
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

            // Generate the 'user rewards' for this new 'user' ready for insertion to the DB so, that the user has a complete
            // list of rewards and challenges so, they can participate in reward and challenge completion.
            IList<Reward> rewards = await _unitOfWork.Repository<Reward>().GetAllAsync();
            List<UserReward> userRewards = (List<UserReward>)UserRewards.GenerateUserRewards(rewards);

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

            BatchWrite<User> batch = _unitOfWork.Repository<User>().Insert(user);
            await _unitOfWork.Commit(new BatchWrite[] { batch });

            // return HTTP 201 Created with user object in body of return and a 'location' header with URL of newly created object
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }

        // PUT api/users/test
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]UserDetailsDTO dto)
        {
            if (dto == null)
            {
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

            BatchWrite<User> batch = _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.Commit(new BatchWrite[] { batch });

            return Ok();
        }

        // DELETE api/users/test
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("User information formatted incorrectly.");
            }

            BatchWrite<User> batch = _unitOfWork.Repository<User>().DeleteById(id);
            await _unitOfWork.Commit(new BatchWrite[] { batch });

            return Ok("user with ID '" + id + "' deleted.");
        }
    }
}
