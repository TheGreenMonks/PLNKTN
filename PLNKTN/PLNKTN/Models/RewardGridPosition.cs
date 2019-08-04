using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class RewardGridPosition
    {
        [DynamoDBProperty]
        public int x { get; set; }

        [DynamoDBProperty]
        public int y { get; set; }
    }
}
