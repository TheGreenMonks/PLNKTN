using Amazon.DynamoDBv2.DataModel;
using PLNKTN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Persistence.Repositories
{
    public interface IRewardRepository
    {
        // General Reward tasks
        BatchWrite<Reward> Insert(Reward reward, BatchWrite<Reward> batchWrite = null);
        Task<IList<Reward>> GetAllAsync();
        Task<Reward> GetByIdAsync(string id);
        BatchWrite<Reward> Update(Reward reward, BatchWrite<Reward> batchWrite = null);
        BatchWrite<Reward> DeleteById(string id, BatchWrite<Reward> batchWrite = null);

        /*** Functions for OneTreePlanted table ***/
        Task<int> CreateRegion(RewardRegion region);
        Task<int> AddProject(string region_name, Project project);
        Task<List<string>> GetAllRegionNames();
        Task<List<Project>> GetAllProjects(string region_name);
        Task<Project> GetProjectInfo(string region_name, string project_name);

        /***Bin***/
        Task<int> ThrowTreeInBin(string region_name, Rgn project);
    }
}
