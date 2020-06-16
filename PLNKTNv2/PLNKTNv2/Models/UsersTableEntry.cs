using Amazon.DynamoDBv2.DataModel;

namespace PLNKTNv2.Models
{
    [DynamoDBTable("Users")]
    public class UsersTableEntry
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
    }
}
