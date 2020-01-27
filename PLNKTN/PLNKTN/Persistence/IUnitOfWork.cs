using Amazon.DynamoDBv2.DataModel;
using PLNKTN.Persistence.Repositories;
using System.Threading.Tasks;

namespace PLNKTN.Persistence
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
        //IUserRepository Users { get; }
        //IRewardRepository Rewards { get; }
        Task Commit(BatchWrite[] batchWrites);
    }
}
