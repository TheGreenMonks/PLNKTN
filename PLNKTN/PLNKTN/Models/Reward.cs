using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    [DynamoDBTable("Rewards")]
    public class Reward
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Title { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public string Description_not_unlocked { get; set; }

        [DynamoDBProperty]
        public List<Challenge> Challenges_to_complete { get; set; } = new List<Challenge>();

        [DynamoDBProperty]
        public string Country { get; set; }

        [DynamoDBProperty]
        public string Overview { get; set; }

        [DynamoDBProperty]
        public string Impact { get; set; }

        [DynamoDBProperty]
        public string Tree_species { get; set; }
    }
}
