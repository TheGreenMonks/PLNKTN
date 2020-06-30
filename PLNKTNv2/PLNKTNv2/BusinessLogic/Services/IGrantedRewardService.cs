using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using System.Collections.Generic;

namespace PLNKTNv2.BusinessLogic.Services
{
    public interface IGrantedRewardService
    {
        UserGrantedReward GetUserGrantedReward(User user, string region_name);

        ICollection<UserGrantedReward> GetUserGrantedRewards(User user);

        void InsertUserGrantedReward(User user, UserGrantedRewardDto grantedRewardDto);
    }
}