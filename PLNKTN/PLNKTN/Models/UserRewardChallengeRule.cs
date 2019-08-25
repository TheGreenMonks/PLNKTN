﻿using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class UserRewardChallengeRule
    {
        [DynamoDBProperty]
        public int Time { get; set; }

        [DynamoDBProperty]
        public string Category { get; set; }

        [DynamoDBProperty]
        public string SubCategory { get; set; }

        [DynamoDBProperty]
        public string RestrictionType { get; set; }
    }
}
