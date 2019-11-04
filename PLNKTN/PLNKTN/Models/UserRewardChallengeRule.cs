using PLNKTN.BusinessLogic;
using System.ComponentModel.DataAnnotations;

namespace PLNKTN.Models
{
    public class UserRewardChallengeRule
    {
        [Range(1, 365)]
        public int Time { get; set; }

        [Category]
        public string Category { get; set; }

        [SubCategory]
        public string SubCategory { get; set; }

        [EnumDataType(typeof(ChallengeType), ErrorMessage = "RestrictionType type value doesn't exist, must be - 0 = 'Skip', 1 = 'Only_This'.")]
        public ChallengeType RestrictionType { get; set; }
    }
}
