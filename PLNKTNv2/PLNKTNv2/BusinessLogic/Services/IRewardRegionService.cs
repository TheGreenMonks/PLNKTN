using PLNKTNv2.Models;

namespace PLNKTNv2.BusinessLogic.Services
{
    public interface IRewardRegionService
    {
        void AddProject(RewardRegion region, Project project);

        void DeleteProject(RewardRegion region, Project project);
    }
}