using Microsoft.AspNetCore.Mvc;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System.Threading.Tasks;

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

        // PUT api/values/5
        [HttpPut("ThrowTreeInBin/{region_name}")]
        public async Task<IActionResult> Put(string region_name, [FromBody] Rgn project)
        {
            if (region_name == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Country information formatted incorrectly.");
            }

            var result = await _rewardRepository.ThrowTreeInBin(region_name, project);

            if (result == 1)
            {
                return Ok();
            }
            else
            {
                return NotFound("Country with name '" + region_name + "' does not exist.");
            }

        }
    }
}
