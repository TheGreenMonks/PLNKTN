using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace PLNKTNv2.Models
{
#if DEBUG
    [DynamoDBTable("DEV-RewardRegion")]
#else
    [DynamoDBTable("RewardRegion")]
#endif
    public class RewardRegion
    {
        public string Region_name { get; set; }
        public List<Project> Projects { get; set; }
    }
}