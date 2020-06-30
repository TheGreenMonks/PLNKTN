using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using System.Collections.Generic;

namespace PLNKTNv2.BusinessLogic.Services
{
    public interface IUserService
    {
        void AddUserRewardToAllUsers(Reward reward, IEnumerable<User> users);

        void DeleteUserRewardFromAllUsers(string rewardId, IEnumerable<User> users);

        void UpdateUserReward(User user, UserReward model);

        void UpdateUserRewardInAllUsers(Reward reward, IEnumerable<User> users);

        User CreateUser(ICollection<Reward> rewards, UserDetailsDTO userDto, string id);
    }
}