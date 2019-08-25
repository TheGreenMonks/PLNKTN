using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class RewardChallenge
    {
        [DynamoDBProperty]
        public string c_Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string ImageURL { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public string Link { get; set; }

        [DynamoDBProperty]
        public string Text_When_Completed { get; set; }

        [DynamoDBProperty]
        public string Text_When_Not_Completed { get; set; }

        [DynamoDBProperty]
        public string Source { get; set; }

        [DynamoDBProperty]
        public RewardChallengeRule Rule { get; set; }
    }
}
