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

        /*** Functions for OneTreePlanted table ***/
        Task<int> CreateRegion(RewardCountry country);
        Task<int> AddRegionIntoCountry(string country_name, Region region);
        Task<List<Region>> GetAllRegionsFromCountry(string country_name);
        Task<Region> GetRegionInfo(string country_name, string region_name);
    }
}
