using Amazon.DynamoDBv2.DataModel;

namespace PLNKTNv2.Models
{
#if DEBUG
    [DynamoDBTable("DEV-Users")]
#else
    [DynamoDBTable("Users")]
#endif
    public class UsersTableEntry
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
    }
}