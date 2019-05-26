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
        // GET: api/EcologicalMeasurements
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/EcologicalMeasurements/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
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

            if (await _userRepository.AddEcologicalMeasurement(dto.UserId, ecologicalMeasurement))
            {
                return Ok();
            }
            else
            {
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // PUT: api/EcologicalMeasurements/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
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
