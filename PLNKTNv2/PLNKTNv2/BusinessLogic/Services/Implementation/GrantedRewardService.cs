using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace PLNKTNv2.BusinessLogic.Services.Implementation
{
    public class GrantedRewardService : IGrantedRewardService
    {
        public UserGrantedReward GetUserGrantedReward(User user, string region_name)
        {
            return user.GrantedRewards.Find(gr => gr.Region_name == region_name);
        }

        public ICollection<UserGrantedReward> GetUserGrantedRewards(User user)
        {
            return user.GrantedRewards;
        }

        public void InsertUserGrantedReward(User user, UserGrantedRewardDto grantedRewardDto)
        {
            UserGrantedReward existingGrantedReward = user.GrantedRewards.Find(ugr => ugr.Region_name == grantedRewardDto.Region_name);

            if (existingGrantedReward == null)
            {
                UserGrantedReward rewardRegion = new UserGrantedReward
                {
                    Region_name = grantedRewardDto.Region_name,
                    Projects = new List<UserGrantedRewardProject>() { grantedRewardDto.Project },
                    Count = 1
                };
                user.GrantedRewards.Add(rewardRegion);
            }
            else
            {
                // user already planted here(it is okay to plant again). Increment count.
                existingGrantedReward.Count++;
                bool project_exists = existingGrantedReward.Projects.Any(x => x.Project_name == grantedRewardDto.Project.Project_name);
                if (!project_exists)
                {
                    existingGrantedReward.Projects.Add(grantedRewardDto.Project);
                }
            }

            /* Set large data lists to null as these aren't updated here, thus sending them
            back to the DB is a waste of BW and DyDB processing time.*/
            user.UserRewards = null;
            user.EcologicalMeasurements = null;
        }
    }
}