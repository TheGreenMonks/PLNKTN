﻿using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(User user);
        Task<bool> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement);
        Task<int> DeleteEcologicalMeasurement(string userId, DateTime date_taken);
        Task<User> GetUser(string userId);
        Task<int> DeleteUser(string userId);
    }
}
