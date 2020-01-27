using Amazon.DynamoDBv2.DataModel;
using PLNKTN.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IDynamoDBContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
        }

        //public IUserRepository Users { get; private set; }
        //public IRewardRepository Rewards { get; private set; }

        public UnitOfWork(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _dbContext = _dbContextFactory.DbContext;
            //Users = new UserRepository(_dbContext);
            //Rewards = new RewardRepository(_dbContext);
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> repo = new Repository<T>(_dbContext);
            Repositories.Add(typeof(T), repo);
            return repo;
        }

        public async Task Commit(BatchWrite[] batchWrites)
        {
            _dbContext.CreateMultiTableBatchWrite(batchWrites);
            await _dbContext.ExecuteBatchWriteAsync(batchWrites);
        }
    }
}
