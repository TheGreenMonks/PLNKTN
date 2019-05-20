using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public interface IUserRepository
    {
        Task<bool> Add(User user);
        Task<bool> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);
    }
}
