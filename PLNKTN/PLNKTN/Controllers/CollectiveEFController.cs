using Microsoft.AspNetCore.Mvc;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    public class CollectiveEFController : Controller
    {
        private readonly IUserRepository _userRepository;

        public CollectiveEFController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/CollectiveEF
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var collective_EF = await _userRepository.GetAllCollective_EFs();

            if (collective_EF != null)
            {
                return Ok(collective_EF);
            }
            else
            {
                return NotFound("No collective EFs have been calculated yet.");
            }
        }

        // GET api/CollectiveEF/2020-03-26T00:01:00.000Z
        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            var collective_EF = await _userRepository.GetCollective_EF(date);

            if (collective_EF != null)
            {
                return Ok(collective_EF);
            }
            else
            {
                return NotFound("No collective EFs have been calculated yet.");
            }
        }

        // POST api/CollectiveEF
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            DateTime timestamp = DateTime.UtcNow.AddDays(-1);
            CollectiveEF cEF = null;
            float collective_EF = await Compute_Collective_EFAsync(timestamp);

            if (collective_EF != -1)
            {
                cEF = new CollectiveEF()
                {
                    Date_taken = timestamp,
                    Collective_EF = collective_EF
                };

                int result = await _userRepository.AddCollective_EF(cEF);

                if (result == 1)
                {
                    return Ok();
                }
                else if (result == -7)
                {
                    return Conflict("A Collective EF with that date already exists.");
                }
                else
                {
                    return StatusCode(500, "Internal server error.  Please contact the administrator.");
                }
            }
            else
            {
                return NotFound("No Ecological Footprints recorded by Users on '" + timestamp.Date.ToShortDateString() + "'.");
            }
        }

        /*Function below are added new*/
        /*** HELPER FUNCTION TO COMPUTE THE COLLECTIVE_EF ***/
        private async Task<float> Compute_Collective_EFAsync(DateTime date)
        {
            var users = await _userRepository.GetUsers();

            if (users != null)
            {
                float? total_collective_Ef = 0;
                int size = 0;

                foreach (var user in users)
                {
                    var user_ef = user.EcologicalMeasurements.SingleOrDefault(x => x.Date_taken.Date == date.Date);

                    if (user_ef != null)
                    {
                        total_collective_Ef += user_ef.EcologicalFootprint;
                        size += 1;
                    }
                }

                if (total_collective_Ef == 0)
                    return -1;

                return (float)total_collective_Ef / size;
            }
            else
                return -1;
        }
    }
}
