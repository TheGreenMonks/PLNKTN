using Amazon.DynamoDBv2.DataModel;

namespace PLNKTN.Models
{
    public class UserRewardChallengeRule
    {
        public int Time { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public ChallengeType RestrictionType { get; set; }
    }
}
