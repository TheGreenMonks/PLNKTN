﻿using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    [DynamoDBTable("Rewards")]
    public class Reward
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Title { get; set; }

        [DynamoDBProperty]
        public string ImageURL { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public string Link { get; set; }

        [DynamoDBProperty]
        public RewardGridPosition GridPosition { get; set; }

        [DynamoDBProperty]
        public string Text_When_Completed { get; set; }

        [DynamoDBProperty]
        public string Text_When_Not_Completed { get; set; }

        [DynamoDBProperty]
        public string Source { get; set; }

        [DynamoDBProperty]
        public List<RewardChallenge> Challenges { get; set; }

        [DynamoDBProperty]
        public string Country { get; set; }

        [DynamoDBProperty]
        public string Overview { get; set; }

        [DynamoDBProperty]
        public string Impact { get; set; }

        [DynamoDBProperty]
        public string Tree_species { get; set; }
    }
}
