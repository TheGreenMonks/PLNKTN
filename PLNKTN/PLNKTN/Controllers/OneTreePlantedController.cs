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
        [HttpGet("GetAllRegionsFromCountry/country_name")]
        public async Task<IActionResult> Get(string region_name)
        {
            if (String.IsNullOrWhiteSpace(region_name))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            var pojects = await _rewardRepository.GetAllProjects(region_name);
            if (pojects != null)
            {
                // return HTTP 200
                return Ok(pojects);
            }
            else
            {
                // return HTTP 404 as region cannot be found in DB
                return NotFound("Country with name '" + region_name + "' does not exist.");
            }
        }

        // GET api/values/5
        [HttpGet("GetRegionInfo/country_name/region_name")]
        public async Task<IActionResult> Get(string region_name, string project_name)
        {
            if (String.IsNullOrWhiteSpace(region_name) || String.IsNullOrWhiteSpace(project_name))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country or Region information formatted incorrectly.");
            }

            var project = await _rewardRepository.GetProjectInfo(region_name, project_name);

            if (project != null)
            {
                // return HTTP 200
                return Ok(project);
            }
            else
            {
                // return HTTP 404 as region cannot be found in DB
                return NotFound("Region with name '" + region_name + "' does not exist.");
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RewardRegion region)
        {
            if (region == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            var rgn = new RewardRegion()
            {
                Region_name = region.Region_name,
                Projects = region.Projects
            };

            // Save the new user to the DB
            var result = await _rewardRepository.CreateRegion(rgn);

            if (result == 1)
            {
                // return HTTP 201 Created with country object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { name = region.Projects }, rgn);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as user already exists in DB
                return Conflict("country with name '" + region.Projects + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // PUT api/values/5
        [HttpPut("AddRegionIntoCountry/country_name/region_name")]
        public async Task<IActionResult> Put(string region_name, [FromBody] Project project)
        {
            if (region_name == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            var prj = new Project()
            {
                Project_name = project.Project_name,
                Description = project.Description,
                Impact = project.Impact,
                Tree_species = project.Tree_species,
                Images = project.Images
            };

            var result = await _rewardRepository.AddProject(region_name, prj);
            if (result == 1)
            {
                return Ok();
            } else
            {
                return NotFound("Country with name '" + region_name + "' does not exist.");
            }

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
