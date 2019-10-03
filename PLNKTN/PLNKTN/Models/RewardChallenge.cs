﻿using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class RewardChallenge
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ImageURL { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public string Text_When_Completed { get; set; }

        public string Text_When_Not_Completed { get; set; }

        public string Source { get; set; }

        public RewardChallengeRule Rule { get; set; }
    }
}
