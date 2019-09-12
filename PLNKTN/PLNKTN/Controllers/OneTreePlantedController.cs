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
    public class OneTreePlantedController : Controller
    {
        private readonly IRewardRepository _rewardRepository;

        public OneTreePlantedController(IRewardRepository rewardRepository)
        {

            _rewardRepository = rewardRepository;
        }
        // GET: api/countryname
        [HttpGet]
        public async Task<IActionResult> Get(string country_name)
        {
            if (String.IsNullOrWhiteSpace(country_name))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            var regions = await _rewardRepository.GetAllRegionsFromCountry(country_name);
            if (regions != null)
            {
                // return HTTP 200
                return Ok(regions);
            }
            else
            {
                // return HTTP 404 as region cannot be found in DB
                return NotFound("Country with name '" + country_name + "' does not exist.");
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string country_name, string region_name)
        {
            if (String.IsNullOrWhiteSpace(country_name) || String.IsNullOrWhiteSpace(region_name))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country or Region information formatted incorrectly.");
            }

            var region = await _rewardRepository.GetRegionInfo(country_name, region_name);

            if (region != null)
            {
                // return HTTP 200
                return Ok(region);
            }
            else
            {
                // return HTTP 404 as region cannot be found in DB
                return NotFound("Region with name '" + region_name + "' does not exist.");
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RewardCountry country)
        {
            if (country == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            // Create new user
            var ctr = new RewardCountry()
            {
                name = country.name,
                Regions = country.Regions
            };

            // Save the new user to the DB
            var result = await _rewardRepository.CreateRegion(ctr);

            if (result == 1)
            {
                // return HTTP 201 Created with country object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { name = country.name }, ctr);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as user already exists in DB
                return Conflict("country with name '" + country.name + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
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
