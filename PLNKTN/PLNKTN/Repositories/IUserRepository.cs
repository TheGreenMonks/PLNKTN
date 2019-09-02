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

        // Challenge tasks
        Task<int> AddCompletedChallengeUser(string userId, EcologicalMeasurement ecologicalMeasurement);

        // Reward tasks
    }
}
