using Amazon.DynamoDBv2.DataModel;

namespace PLNKTN.Models
{
    [DynamoDBTable("Users")]
    public class UsersTableEntry
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
    }
}
