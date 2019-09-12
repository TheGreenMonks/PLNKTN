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
        Task<int> CreateReward(Reward reward);
        Task<int> UpdateReward(Reward reward);
        Task<ICollection<Reward>> GetAllRewards();
        Task<Reward> GetReward(string id);
    }
}
