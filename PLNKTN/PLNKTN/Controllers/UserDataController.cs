using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PLNKTN.DTOs;
using PLNKTN.Models;

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        // GET: api/UserData
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "PLNKTN", "app is up and running" };
        }

        // GET: api/UserData/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "Requested user information with ID of " + id;
        }

        // POST: api/UserData
        [HttpPost]
        public IActionResult Post([FromBody] UserDataDTO userData)
        {
            return Ok(userData);
        }

        // PUT: api/UserData/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return BadRequest("userdata PUT is not implemented");
        }

        // DELETE: api/UserData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Task<IActionResult>
            await ExecuteAsync();
            return BadRequest("userdata DELETE is not implemented");
        }

        public static async Task ExecuteAsync()
        {
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            if (chain.TryGetAWSCredentials("Dexter", out awsCredentials))
            {
                var a = awsCredentials.GetCredentials();
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
                // This client will access the US West 1 region (N Cali).
                clientConfig.RegionEndpoint = RegionEndpoint.USWest1;
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(a.AccessKey, a.SecretKey, clientConfig);

                using (var context = new DynamoDBContext(client))
                {
                    User user = new User
                    {
                        Email = "b@b.com",
                        First_name = "d",
                        Last_name = "c",
                        Created_at = "20190430T122330Z",
                        Level = "L2"
                    };

                    await context.SaveAsync(user);
                }
            }
        }
    }
}
