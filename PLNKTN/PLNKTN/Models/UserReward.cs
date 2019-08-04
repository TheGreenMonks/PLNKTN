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
        public string Title { get; set; }

        [DynamoDBProperty]
        public List<UserRewardChallenge> ChallengesUnderway { get; set; } = new List<UserRewardChallenge>();

        [DynamoDBProperty]
        public DateTime DateCompleted { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; }
    }
}
