﻿using PLNKTN.Models;
using System;
using System.Collections.Generic;
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
        int GetUserCount(string userCountId);

        // Ecological Measurement tasks
        Task<int> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);
        Task<int> UpdateEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);

        // TODO: This code below is not needed as per PLNKTN-44 & 45 - DELETE
        Task<int> DeleteEcologicalMeasurement(string userId, DateTime date_taken);

        Task<List<User>> GetUsers();

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
        Task<int> AddUserGrantedReward(string userId, string region_name, Rgn project, string treeCountId);
        int GetTotalTreesPlantedCount(string treeCountId);
    }
}
