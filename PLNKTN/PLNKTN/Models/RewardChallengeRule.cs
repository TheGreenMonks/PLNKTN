using Amazon.DynamoDBv2.DataModel;

namespace PLNKTN.Models
{
    public class RewardChallengeRule
    {
        [DynamoDBProperty]
        public int Time { get; set; }

        [DynamoDBProperty]
        public string Category { get; set; }

        [DynamoDBProperty]
        public string SubCategory { get; set; }

        [DynamoDBProperty]
        public ChallengeType RestrictionType { get; set; }
    }
}