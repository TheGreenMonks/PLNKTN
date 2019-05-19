using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class RewardUser
    {
        [DynamoDBProperty]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Title { get; set; }

        [DynamoDBProperty]
        public List<ChallengeUser> Challenges_to_complete { get; set; } = new List<ChallengeUser>();

        [DynamoDBProperty]
        public DateTime Date_completed { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; }
    }
}
