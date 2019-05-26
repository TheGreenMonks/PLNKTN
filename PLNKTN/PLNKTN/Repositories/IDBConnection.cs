using Amazon.DynamoDBv2.DataModel;

namespace PLNKTN.Repositories
{
    public interface IDBConnection
    {
        IDynamoDBContext Context(DynamoDBContextConfig config = null);
    }
}
