using Amazon.DynamoDBv2.DataModel;
using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public class CollectiveEFRepository : ICollectiveEFRepository
    {
        private readonly IDBConnection _dbConnection;

        public CollectiveEFRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async void Create(CollectiveEF collectiveEF)
        {
            using (IDynamoDBContext context = _dbConnection.Context())
            {
                await context.SaveAsync(collectiveEF);
            }
        }

        public async Task<IList<CollectiveEF>> GetAll()
        {
            using (var context = _dbConnection.Context())
            {
                var conditions = new List<ScanCondition>();
                List<CollectiveEF> collectiveEFs = await context.ScanAsync<CollectiveEF>(conditions).GetRemainingAsync();
                return collectiveEFs;
            }
        }

        public async Task<CollectiveEF> GetById(DateTime collectiveEFDate)
        {
            using (var context = _dbConnection.Context())
            {
                var conditions = new List<ScanCondition>();
                List<CollectiveEF> collectiveEFs = await context.ScanAsync<CollectiveEF>(conditions).GetRemainingAsync();
                var collective_EF = collectiveEFs.SingleOrDefault(cef => cef.Date_taken.Date == collectiveEFDate.Date);
                return collective_EF;
            }
        }
    }
}
