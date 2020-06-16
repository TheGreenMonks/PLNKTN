using PLNKTNv2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Repositories
{
    public interface IRewardRepository
    {
        Task<int> AddProject(string region_name, Project project);

        Task<int> CreateRegion(RewardRegion region);

        Task<int> CreateReward(Reward reward);

        Task<int> DeleteReward(string id);

        Task<List<Project>> GetAllProjects(string region_name);

        Task<List<string>> GetAllRegionNames();

        Task<ICollection<Reward>> GetAllRewards();

        Task<Project> GetProjectInfo(string region_name, string project_name);

        Task<Reward> GetReward(string id);

        Task<int> ThrowTreeInBin(string region_name, Rgn project);

        Task<int> UpdateReward(Reward reward);
    }
}