using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models
{
    public class RewardChallenge
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }

        public string ImageURL { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public string Text_When_Completed { get; set; }

        public string Text_When_Not_Completed { get; set; }

        public string Source { get; set; }

        [Required]
        public RewardChallengeRule Rule { get; set; }
    }
}
