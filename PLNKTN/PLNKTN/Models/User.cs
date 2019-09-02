using System;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PLNKTN.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        public string First_name { get; set; }

        public string Last_name { get; set; }

        public DateTime? Created_at { get; set; }

        public string Email { get; set; }

        public string Level { get; set; }

        public List<EcologicalMeasurement> EcologicalMeasurements { get; set; }

        public int? LivingSpace { get; set; }

        public int? NumPeopleHousehold { get; set; }

        public float? CarMPG { get; set; }

        public bool? ShareData { get; set; }

        public float? EcologicalFootprint { get; set; }

        public string Country { get; set; } = null;

        public List<UserReward> UserRewards { get; set; }

        public User()
        {
        }

    }
}
