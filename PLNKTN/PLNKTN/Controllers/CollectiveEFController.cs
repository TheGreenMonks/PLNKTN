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
    public class CollectiveEFController : Controller
    {
        private readonly IUserRepository _userRepository;

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {   
            /* To get average Collective EF */
            var average_CEFs = await Compute_Average_Collective_EF();
            return Ok(average_CEFs);
        }

        // GET api/values/5
        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            var collective_EF = _userRepository.GetCollective_EF(date);
            if (collective_EF != null)
            {
                return Ok(collective_EF);
            } else
            {
                /*What happen when trying to get Collective_EF for today date but it was not computed yet 
                So I thought here is a great place to do the computation and then return it to the front end
                but at this moment we also need to post the newly computed value need to be post to the DB 
                and this is where I am stuck is how to post the new computed value into the DB while doing get function*/
                float cef = await this.Compute_Collective_EF();
                collective_EF = new CollectiveEF
                {
                    Date_taken : date,
                    collective_ef : cef,
                };

                return Ok(collective_EF);
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
        /*Function below are added new*/
        /*** HELPER FUNCTION TO COMPUTE THE COLLECTIVE_EF ***/
        private async Task<float> Compute_Collective_EF()
        {
            var users = await _userRepository.GetUsers();
            if (users != null)
            {
                float? total_collective_Ef = 0;
                int size = 0;
                foreach (var user in users)
                {
                    total_collective_Ef += user.EcologicalFootprint;
                    size += 1;
                }
                return (float)total_collective_Ef / size;
            }
            else
                return -1;
        }

        private async Task<float> Compute_Average_Collective_EF()
        {
            var CollectiveEFs = await _userRepository.GetAllCollective_EFs();
            if (CollectiveEFs != null)
            {
                float? total_collective_Efs = 0;
                int size = 0;
                foreach (var CollectiveEF in CollectiveEFs)
                {
                    total_collective_Efs += CollectiveEF.Collective_EF;
                    size += 1;
                }
                return (float)total_collective_Efs / size;
            }
            else
                return -1;
        }
    }
}
