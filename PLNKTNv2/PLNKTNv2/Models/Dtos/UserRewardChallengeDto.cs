using PLNKTNv2.BusinessLogic.AttributeDecorators;
using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models
{
    public class UserRewardChallengeDto
    {
        [Required]
        public string Id { get; set; }

        [NotificationStatus]
        [Required]
        public NotificationStatus? NotificationStatus { get; set; }
    }
}