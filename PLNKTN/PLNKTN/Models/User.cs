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

        [DynamoDBProperty]
        public string First_name { get; set; }

        [DynamoDBProperty]
        public string Last_name { get; set; }

        [DynamoDBProperty]
        public DateTime? Created_at { get; set; }

        [DynamoDBProperty]
        public string Email { get; set; }

        [DynamoDBProperty]
        public string Level { get; set; }

        [DynamoDBProperty]
        public List<EcologicalMeasurement> EcologicalMeasurements { get; set; }

        [DynamoDBProperty]
        public int? LivingSpace { get; set; }

        [DynamoDBProperty]
        public int? NumPeopleHousehold { get; set; }

        [DynamoDBProperty]
        public float? CarMPG { get; set; }

        [DynamoDBProperty]
        public bool? ShareData { get; set; }

        [DynamoDBProperty]
        public float? EcologicalFootprint { get; set; }

        [DynamoDBProperty]
        public string Country { get; set; } = null;

        [DynamoDBProperty]
        public List<RewardUser> RewardsUser { get; set; }

        public User()
        {
        }

    }
}
