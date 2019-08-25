using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public interface IRewardRepository
    {
        // General Reward tasks
        Task<IList<Reward>> GetAllRewards();
    }
}
