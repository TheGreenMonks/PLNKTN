using Amazon.DynamoDBv2.DataModel;

namespace PLNKTN.Persistence.Repositories
{
    public interface IDbContextFactory
    {
        IDynamoDBContext DbContext { get; }
    }
}