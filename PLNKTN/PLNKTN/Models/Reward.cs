using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PLNKTN.Models
{
    [DynamoDBTable("Rewards")]
    public class Reward
    {
        [DynamoDBHashKey]
        [Required]
        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public RewardGridPosition GridPosition { get; set; }

        public string Text_When_Completed { get; set; }

        public string Text_When_Not_Completed { get; set; }

        public string Source { get; set; }

        [Required]
        public List<RewardChallenge> Challenges { get; set; }

        public string Country { get; set; }

        public string Overview { get; set; }

        public string Impact { get; set; }

        public string Tree_species { get; set; }
    }
}
