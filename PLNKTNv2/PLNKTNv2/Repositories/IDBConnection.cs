using Amazon.DynamoDBv2.DataModel;

namespace PLNKTNv2.Repositories
{
    public interface IDBConnection
    {
        IDynamoDBContext Context(DynamoDBContextConfig config = null);
    }
}
