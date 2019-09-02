﻿using System;
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
            var collective_EF = await _userRepository.GetCollective_EF(DateTime.Today);
            if (collective_EF != null)
            {
                return Ok(collective_EF);
            } else
            {
                return NotFound("CollectiveEF has not yet been commputed for today date");
            }
        }

        // GET api/values/5
        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            var collective_EF = await _userRepository.GetCollective_EF(date);
            if (collective_EF != null)
            {
                return Ok(collective_EF);
            } else
            {
                return  NotFound("CollectiveEF has not yet been commputed");
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(DateTime date)
        {
            CollectiveEF cEF = null;
            float collective_EF = await Compute_Collective_EFAsync(date);
            if (collective_EF != -1)
            {
                cEF = new CollectiveEF()
                {
                    Date_taken = date,
                    Collective_EF = collective_EF
                };
                int result = await _userRepository.AddCollective_EF(cEF);
                if (result == 1)
                {
                    return Ok();
                } else
                {
                    return BadRequest("Saving Collective EF encounter DB issue.");
                }
            } else
            {
                return BadRequest("Computing Collective EF encounter DB issue.");
            }


        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        { 
            /*** NOT NEEDED   ****/
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            /*** NOT NEEDED   ****/
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
                    total_collective_Ef += user.EcologicalMeasurements.Find(x => x.Date_taken == date).EcologicalFootprint;
                    size += 1;
                }
                return (float)total_collective_Ef / size;
            }
            else
                return -1;
        }
    }
}
