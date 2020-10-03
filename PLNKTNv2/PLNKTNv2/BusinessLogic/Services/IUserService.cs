using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using System.Collections.Generic;

namespace PLNKTNv2.BusinessLogic.Services
{
    public interface IUserService
    {
        void AddUserRewardToAllUsers(Reward reward, IEnumerable<User> users);

        void CalculateUserRewardCompletion(List<User> users);

        void CalculateMyRewardCompletion(User user);

        User CreateUser(ICollection<Reward> rewards, CreateUserDetailsDTO userDto, string id);

        void DeleteUserRewardFromAllUsers(string rewardId, IEnumerable<User> users);

        void UpdateUserRewards(User user, ICollection<UserRewardDto> userRewards);

        void UpdateUserRewardInAllUsers(Reward reward, IEnumerable<User> users);
    }
}