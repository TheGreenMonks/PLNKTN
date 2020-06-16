using PLNKTNv2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Repositories
{
    public interface ICollectiveEFRepository
    {
        void Create(CollectiveEF collectiveEF);

        Task<IList<CollectiveEF>> GetAll();

        Task<CollectiveEF> GetById(DateTime collectiveEFDate);
    }
}