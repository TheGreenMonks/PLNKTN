using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models
{
    public class RewardChallenge
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public string Text_When_Completed { get; set; }

        [Required]
        public string Text_When_Not_Completed { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public RewardChallengeRule Rule { get; set; }
    }
}
