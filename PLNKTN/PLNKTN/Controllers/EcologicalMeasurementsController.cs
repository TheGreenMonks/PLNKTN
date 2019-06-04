using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.DTOs;
using PLNKTN.Models;
using PLNKTN.Repositories;

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcologicalMeasurementsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public EcologicalMeasurementsController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/EcologicalMeasurements/5
        [HttpGet("GetMeasurements/{id}")]
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
                return Ok(user.EcologicalMeasurements);
            }
            else
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + id + "' does not exist.");
            }
        }

        // GET: api/EcologicalMeasurements/5
        [HttpGet("GetMeasure/{id}/{date}")]
        public async Task<IActionResult> Get(string id, DateTime date)
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
                var ecologicalMeasure = user.EcologicalMeasurements.Find(
                    delegate (EcologicalMeasurement em) {
                        return DateTime.Equals(em.Date_taken, date);
                    });
                if (ecologicalMeasure != null)
                {
                    return Ok(ecologicalMeasure);
                }
                else
                {
                    // return HTTP 404 as ecologicalMeasure with date cannot be found in DB
                    return NotFound("User with ID '" + id + "' does not have ecological measure on " + date);
                }
            }
            else
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + id + "' does not exist.");
            }
        }

        // POST: api/EcologicalMeasurements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EcologicalMeasurementDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Measurement information formatted incorrectly.");
            }

            var ecologicalMeasurement = new EcologicalMeasurement
            {
                Date_taken = dto.Date_taken,
                Transport = dto.Transport,
                Diet = dto.Diet,
                Electronics = dto.Electronics,
                Clothing = dto.Clothing,
                Footwear = dto.Footwear
            };

            int result = await _userRepository.AddEcologicalMeasurement(dto.UserId, ecologicalMeasurement);

            if (result == 1)
            {
                // return HTTP 201 Created with user object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("GetMeasure", new { id = dto.UserId, date = dto.Date_taken }, ecologicalMeasurement);
            }
            else if (result == -7)
            {
                // return HTTP 409 as ecologicalMeasure with date already exists - conflict
                return Conflict("User with ID '" + dto.UserId + "' already has an ecological measure on " + dto.Date_taken);
            }
            else if(result == -9)
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + dto.UserId + "' does not exist.");
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // PUT: api/EcologicalMeasurements
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EcologicalMeasurementDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Measurement information formatted incorrectly.");
            }

            var ecologicalMeasurement = new EcologicalMeasurement
            {
                Date_taken = dto.Date_taken,
                Transport = dto.Transport,
                Diet = dto.Diet,
                Electronics = dto.Electronics,
                Clothing = dto.Clothing,
                Footwear = dto.Footwear
            };

            var result = await _userRepository.UpdateEcologicalMeasurement(dto.UserId, ecologicalMeasurement);

            if (result == 1)
            {
                return Ok();
            }
            else if (result == -8)
            {
                // return HTTP 404 as ecologicalMeasure with date cannot be found in DB
                return NotFound("User with ID '" + dto.UserId + "' does not have ecological measure on " + dto.Date_taken);
            }
            else
            {
                // return HTTP 404 as user cannot be found in DB
                return NotFound("User with ID '" + dto.UserId + "' does not exist.");
            }

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<IActionResult> Delete(EcologicalMeasurementDeleteDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Measurement information formatted incorrectly.");
            }

            var elementsDeleted = await _userRepository.DeleteEcologicalMeasurement(dto.UserId, dto.Date_taken);
            if (elementsDeleted > 0)
            {
                return Ok(elementsDeleted + " measurement(s) deleted.");
            }
            else if (elementsDeleted == 0)
            {
                return NotFound("No measurement(s) on date " + dto.Date_taken.ToShortDateString() + " available to be deleted.");
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }
    }
}
