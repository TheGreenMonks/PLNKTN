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
        Task<int> DeleteReward(string id);
      
        /*** Functions for OneTreePlanted table ***/
        Task<int> CreateRegion(RewardRegion region);
        Task<int> AddProject(string region_name, Project project);
        Task<List<Project>> GetAllProjects(string region_name);
        Task<Project> GetProjectInfo(string region_name, string project_name);

        /***Bin***/
        Task<int> ThrowTreeInBin(string region_name, Rgn project);
    }
}
