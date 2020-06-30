using Amazon.DynamoDBv2.DataModel;
using System;

namespace PLNKTNv2.Persistence.Repositories
{
    public interface IDbContextFactory : IDisposable
    {
        IDynamoDBContext DbContext { get; }
    }
}
