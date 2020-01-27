using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDynamoDBContext _dbContext;

        public Repository(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BatchWrite<T> DeleteById(string id, BatchWrite<T> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<T>();
            }
            batchWrite.AddDeleteKey(id);
            return batchWrite;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
            // as the DB increases in size.
            // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

            // Defins scan conditions - there are none as we want all rewards
            IList<ScanCondition> conditions = new List<ScanCondition>();

            // TODO ******************** DEBUG ONLY REMOVE FROM TESTING *****************************
            //conditions.Add(new ScanCondition("Id", ScanOperator.Equal, "test"));

            // Gets rewards from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
            IList<T> entities = await _dbContext.ScanAsync<T>(conditions).GetRemainingAsync();
            return entities;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbContext.LoadAsync<T>(id);
        }

        public BatchWrite<T> Insert(T entity, BatchWrite<T> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<T>();
            }
            batchWrite.AddPutItem(entity);
            return batchWrite;
        }

        public BatchWrite<T> Update(T entity, BatchWrite<T> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<T>();
            }
            batchWrite.AddPutItem(entity);
            return batchWrite;
        }

        public BatchWrite<T> Update(ICollection<T> entities, BatchWrite<T> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<T>();
            }
            batchWrite.AddPutItems(entities);
            return batchWrite;
        }
    }
}
