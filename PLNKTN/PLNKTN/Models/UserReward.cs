using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;

namespace PLNKTN.Models
{
    public class UserReward
    {
        public string Id { get; set; }

        public List<UserRewardChallenge> Challenges { get; set; }

        public DateTime? DateCompleted { get; set; }

        public UserRewardStatus Status { get; set; }
    }
}
