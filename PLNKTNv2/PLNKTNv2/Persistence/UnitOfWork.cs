using Amazon.DynamoDBv2.DataModel;
using PLNKTNv2.Persistence.Repositories;
using PLNKTNv2.Persistence.Repositories.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PLNKTNv2.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.DbContext;
        }

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
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
    }
}