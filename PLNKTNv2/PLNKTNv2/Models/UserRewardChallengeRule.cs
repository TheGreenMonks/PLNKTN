using PLNKTNv2.BusinessLogic.AttributeDecorators;
using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models
{
    public class UserRewardChallengeRule
    {
        public int? AmountToConsume { get; set; }

        [Category]
        public string Category { get; set; }

        [EnumDataType(typeof(ChallengeType), ErrorMessage = "RestrictionType type value doesn't exist, must be - 0 = 'Skip', 1 = 'Only_This' or 2 = 'Any'.")]
        public ChallengeType RestrictionType { get; set; }

        [SubCategory]
        public string SubCategory { get; set; }

        [Range(1, 365)]
        public int? Time { get; set; }
    }
}