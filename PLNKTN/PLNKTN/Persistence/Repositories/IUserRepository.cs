using Amazon.DynamoDBv2.DataModel;
using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Persistence.Repositories
{
    public interface IUserRepository
    {
        // General User tasks
        public BatchWrite<User> Insert(User user, BatchWrite<User> batchWrite = null);
        Task<int> UpdateUser(User user);
        Task<User> GetUser(string userId);
        Task<int> DeleteUser(string userId);
        Task<IList<User>> GetAllUsers();

        // Ecological Measurement tasks
        Task<int> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);
        Task<int> UpdateEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);

        // TODO: This code below is not needed as per PLNKTN-44 & 45 - DELETE
        Task<int> DeleteEcologicalMeasurement(string userId, DateTime date_taken);

        Task<List<User>> GetUsers();
        /*Function below are added new*/
        Task<CollectiveEF> GetCollective_EF(DateTime date_taken);
        Task<int> AddCollective_EF(CollectiveEF cEF);
        Task<List<CollectiveEF>> GetAllCollective_EFs();

        // UserReward Tasks
        Task<int> UpdateUserReward(string userId, UserReward model);

        // Challenge tasks
        Task<int> AddCompletedChallengeUser(string userId, EcologicalMeasurement ecologicalMeasurement);

        // Reward tasks

        Task<int> AddUserRewardToAllUsers(UserReward reward);
        Task<int> UpdateUserRewardInAllUsers(UserReward reward);
        Task<int> AddUserRewardChallenge(string userId, string rewardId, UserRewardChallenge challenge);
        Task<int> DeleteUserRewardFromAllUsers(string rewardId);

        //Granted Rewards
        Task<IList<Rgn>> GetUserGrantedReward(string userId, string region_name);
        Task<int> AddUserGrantedReward(string userId, string region_name, Rgn project);
    }
}
