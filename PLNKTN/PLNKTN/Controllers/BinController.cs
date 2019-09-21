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
    public class DumpsterController : Controller
    {
        private readonly IRewardRepository _rewardRepository;

        public DumpsterController(IRewardRepository rewardRepository)
        {

            _rewardRepository = rewardRepository;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]int id)
        {
        }

        // PUT api/values/5
        [HttpPut("ThrowTreeInBin/region_name")]
        public async Task<IActionResult> Put(string region_name, [FromBody] Rgn project)
        {
            if (region_name == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            var prj = new Rgn()
            {
                Project_name = project.Project_name,
                Tree_species = project.Tree_species
            };

            var result = await _rewardRepository.ThrowTreeInBin(region_name, prj);
            if (result == 1)
            {
                return Ok();
            }
            else
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
