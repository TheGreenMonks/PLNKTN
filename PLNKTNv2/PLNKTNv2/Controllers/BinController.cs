using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.Models;
using PLNKTNv2.Repositories;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
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