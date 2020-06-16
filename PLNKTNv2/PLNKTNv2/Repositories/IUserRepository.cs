using PLNKTNv2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Repositories
{
    public interface IUserRepository
    {
        Task<int> AddCompletedChallengeUser(string userId, EcologicalMeasurement ecologicalMeasurement);

        Task<int> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);

        Task<int> AddUserGrantedReward(string userId, string region_name, Rgn project, string treeCountId);

        Task<int> AddUserRewardChallenge(string userId, string rewardId, UserRewardChallenge challenge);

        Task<int> AddUserRewardToAllUsers(UserReward reward);

        Task<int> CreateUser(User user);

        Task<int> DeleteUser(string userId);

        Task<int> DeleteUserRewardFromAllUsers(string rewardId);

        Task<IList<User>> GetAllUsers();

        int GetTotalTreesPlantedCount(string treeCountId);

        Task<User> GetUser(string userId);

        int GetUserCount(string userCountId);

        Task<IList<Rgn>> GetUserGrantedReward(string userId, string region_name);

        Task<List<User>> GetUsers();

        Task<int> UpdateEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);

        Task<int> UpdateUser(User user);

        Task<int> UpdateUserReward(string userId, UserReward model);

        Task<int> UpdateUserRewardInAllUsers(UserReward reward);
    }
}