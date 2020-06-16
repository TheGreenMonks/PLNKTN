using Amazon.DynamoDBv2.DataModel;
using System;

namespace PLNKTNv2.Models
{
    public class UserRewardChallenge
    {
        public string Id { get; set; }

        public DateTime? DateCompleted { get; set; }

        public UserRewardChallengeStatus Status { get; set; }

        public UserRewardChallengeRule Rule { get; set; }

        public NotificationStatus NotificationStatus { get; set; }
    }
}
