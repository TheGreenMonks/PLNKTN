using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectiveEFController : ControllerBase
    {
        private readonly ICollectiveEFRepository _collectiveEFRepository;
        private readonly IUserRepository _userRepository;

        public CollectiveEFController(ICollectiveEFRepository collectiveEFRepository, IUserRepository userRepository)
        {
            _collectiveEFRepository = collectiveEFRepository;
            _userRepository = userRepository;
        }

        // GET: api/CollectiveEF
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var collectiveEFs = await _collectiveEFRepository.GetAll();
            return Ok(collectiveEFs);
        }

        // GET api/CollectiveEF/2020-03-26T00:01:00.000Z
        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            var collective_EF = await _collectiveEFRepository.GetById(date);

            if (collective_EF != null)
            {
                return Ok(collective_EF);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/CollectiveEF
        [HttpPost("{date}")]
        public async Task<IActionResult> Post(DateTime date)
        {
            DateTime dayToCalculate = date.AddDays(-1);

            if (await _collectiveEFRepository.GetById(dayToCalculate) != null)
            {
                return Conflict("A Collective EF with that date already exists.");
            }

            var users = await _userRepository.GetUsers();
            CollectiveEF collectiveEf = CollectiveEfLogic.GenerateCollectiveEF(dayToCalculate, users);

            _collectiveEFRepository.Create(collectiveEf);
            return Ok();
        }
    }
}
