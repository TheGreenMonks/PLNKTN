using PLNKTNv2.Models;

namespace PLNKTNv2.BusinessLogic.Services.Implementation
{
    public class RewardRegionService : IRewardRegionService
    {
        public void AddProject(RewardRegion region, Project project)
        {
            region.Projects.Add(project);
        }

        public void DeleteProject(RewardRegion region, Project project)
        {
            region.Projects.RemoveAll(p => p.Project_name == project.Project_name);
        }
    }
}