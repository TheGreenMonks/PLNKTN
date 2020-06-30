using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Persistence.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDynamoDBContext _dbContext;

        public Repository(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteByIdAsync(string id)
        {
            await _dbContext.DeleteAsync<T>(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
            // as the DB increases in size.
            // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

            // Defins scan conditions - there are none as we want all rewards
            IList<ScanCondition> conditions = new List<ScanCondition>();

            // TODO ******************** DEBUG ONLY REMOVE FROM TESTING *****************************
            //conditions.Add(new ScanCondition("Id", ScanOperator.Equal, "test"));

            // Gets rewards from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
            return await _dbContext.ScanAsync<T>(conditions).GetRemainingAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbContext.LoadAsync<T>(id);
        }

        public async Task InsertAsync(T entity)
        {
            await _dbContext.SaveAsync(entity);
        }

        public async Task InsertAsync(ICollection<T> entities)
        {
            BatchWrite<T> batchWrite = _dbContext.CreateBatchWrite<T>();
            batchWrite.AddPutItems(entities);
            await _dbContext.ExecuteBatchWriteAsync(new[] { batchWrite });
        }

        public async Task UpdateAllAsync(IEnumerable<T> entities)
        {
            foreach (var item in entities)
            {
                await UpdateAsync(item);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            DynamoDBOperationConfig config = new DynamoDBOperationConfig()
            {
                IgnoreNullValues = true
            };
            await _dbContext.SaveAsync(entity, config);
        }
    }
}