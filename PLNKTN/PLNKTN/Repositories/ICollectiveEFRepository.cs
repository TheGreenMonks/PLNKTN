using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public interface ICollectiveEFRepository
    {
        // General User tasks
        void Create(CollectiveEF collectiveEF);
        Task<CollectiveEF> GetById(DateTime collectiveEFDate);
        Task<IList<CollectiveEF>> GetAll();
    }
}
