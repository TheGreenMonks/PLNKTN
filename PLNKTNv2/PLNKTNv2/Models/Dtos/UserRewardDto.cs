using PLNKTNv2.BusinessLogic.AttributeDecorators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models
{
    public class UserRewardDto
    {
        public List<UserRewardChallengeDto> Challenges { get; set; }

        [Required]
        public string Id { get; set; }

        public bool? IsRewardGranted { get; set; }

        [NotificationStatus]
        public NotificationStatus? NotificationStatus { get; set; }
    }
}