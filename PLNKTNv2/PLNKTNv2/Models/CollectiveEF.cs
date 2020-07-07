using Amazon.DynamoDBv2.DataModel;
using System;

namespace PLNKTNv2.Models
{
#if DEBUG
    [DynamoDBTable("DEV-CollectiveEF")]
#else
    [DynamoDBTable("CollectiveEF")]
#endif
    public class CollectiveEF
    {
        public DateTime Date_taken { get; set; }
        public float Collective_EF { get; set; }
    }
}