using Amazon.DynamoDBv2.DataModel;
using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDBConnection _dbConneciton;

        public UserRepository(IDBConnection dbConnection)
        {
            _dbConneciton = dbConnection;
        }

        public async void Add(User user)
        {
            using (var context = _dbConneciton.Context())
            {
                await context.SaveAsync(user);
            }
        }
    }
}
