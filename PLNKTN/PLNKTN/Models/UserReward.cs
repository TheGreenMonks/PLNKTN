using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class UserReward
    {
        [DynamoDBProperty]
        public string Id { get; set; }

        [DynamoDBProperty]
        public List<UserRewardChallenge> Challenges { get; set; }

        [DynamoDBProperty]
        public DateTime? DateCompleted { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; }
    }
}
