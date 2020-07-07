using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace PLNKTNv2.Models
{
#if DEBUG
    [DynamoDBTable("DEV-Bin")]
#else
    [DynamoDBTable("Bin")]
#endif
    public class Bin
    {
        public int Count { get; set; } = 0;

        public IList<UserGrantedRewardProject> Projects { set; get; }

        [DynamoDBHashKey]
        public string Region_name { get; set; }
    }
}