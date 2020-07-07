using Amazon.DynamoDBv2.DataModel;
using PLNKTNv2.BusinessLogic.AttributeDecorators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models
{
#if DEBUG
    [DynamoDBTable("DEV-Rewards")]
#else
    [DynamoDBTable("Rewards")]
#endif
    public class Reward
    {
        [DynamoDBHashKey]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public RewardGridPosition GridPosition { get; set; }

        [Required]
        public string Text_When_Completed { get; set; }

        [Required]
        public string Text_When_Not_Completed { get; set; }

        [Required]
        public string Source { get; set; }

        [RewardChallenge]
        public List<RewardChallenge> Challenges { get; set; }

        public string Country { get; set; }

        [Required]
        public string Overview { get; set; }

        [Required]
        public string Impact { get; set; }

        [Required]
        public string Tree_species { get; set; }
    }
}
