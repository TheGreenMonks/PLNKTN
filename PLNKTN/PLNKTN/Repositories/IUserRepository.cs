using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public interface IUserRepository
    {
        // General User tasks
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(User user);
        Task<User> GetUser(string userId);
        Task<int> DeleteUser(string userId);
        Task<IList<User>> GetAllUsers();

        // Ecological Measurement tasks
        Task<int> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);
        Task<int> UpdateEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);
        // TODO: This code below is not needed as per PLNKTN-44 & 45 - DELETE
        Task<int> DeleteEcologicalMeasurement(string userId, DateTime date_taken);

        Task<int> AddUserRewardToAllUsers(UserReward reward);
        Task<int> AddUserRewardChallenge(string userId, string rewardId, UserRewardChallenge challenge);
    }
}
