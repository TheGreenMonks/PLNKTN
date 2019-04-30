using System;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string Email { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Created_at { get; set; }
        public string Level { get; set; }

        public User()
        {
        }

    }
}
