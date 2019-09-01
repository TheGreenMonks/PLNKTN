using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDBConnection _dbConnection;
        private DynamoDBContextConfig _config;

        public UserRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;

            // make context not delete attributes that are null, thus save operation will only 
            // update values that have been set by user.
            _config = new DynamoDBContextConfig
            {
                IgnoreNullValues = true
            };
        }

        public async Task<int> CreateUser(User user)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var userExists = await context.LoadAsync<User>(user.Id);
                    if (userExists != null)
                    {
                        return -10;
                    }
                    else
                    {
                        await context.SaveAsync(user);
                        return 1;
                    }
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        public async Task<int> UpdateUser(User user)
        {
            using (var context = _dbConnection.Context(_config))
            {
                try
                {
                    var existingUser = await context.LoadAsync<User>(user.Id);
                    if (existingUser != null)
                    {
                        await context.SaveAsync(user);
                        // Success
                        return 1;
                    }
                    else
                    {
                        // Object not found in db
                        return -9;
                    }
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        public async Task<int> DeleteUser(string userId)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        await context.DeleteAsync<User>(userId);
                        // Success
                        return 1;
                    }
                    else
                    {
                        // Object not found in db
                        return -9;
                    }
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    // Exception
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        public async Task<int> AddEcologicalMeasurement(string userId, EcologicalMeasurement ecologicalMeasurement)
        {
            using (IDynamoDBContext context = _dbConnection.Context())
            {
                try
                {
                    User user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        // Find ecological measurement object in User object from the DB where the dates match
                        EcologicalMeasurement dbEcoMeasure = user.EcologicalMeasurements.FirstOrDefault(e => e.Date_taken.Date == ecologicalMeasurement.Date_taken.Date);

                        if (dbEcoMeasure == null)
                        {
                            user.EcologicalMeasurements.Add(ecologicalMeasurement);
                            await context.SaveAsync(user);
                            return 1;
                        }
                        else
                        {
                            // 409 - EcologicalMeasurement with specified date already exists, conflict
                            return -7;
                        }
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return -9;
                    }

                    
                    
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        public async Task<int> UpdateEcologicalMeasurement(string userId, EcologicalMeasurement updatedEcoMeasure)
        {
            using (var context = _dbConnection.Context(_config))
            {
                try
                {
                    var user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        // Find ecological measurement object in User object from the DB where the dates match
                        var dbEcoMeasure = user.EcologicalMeasurements.FirstOrDefault(e => e.Date_taken.Date == updatedEcoMeasure.Date_taken.Date);


                        if (dbEcoMeasure !=  null)
                        {
                            // Remove old ecological measurement from DB User
                            user.EcologicalMeasurements.Remove(dbEcoMeasure);

                            // Update DB updatedEcoMeasure with new values if not null
                            if (updatedEcoMeasure.Diet != null)
                            {
                                dbEcoMeasure.Diet = updatedEcoMeasure.Diet;
                            }
                            if (updatedEcoMeasure.Clothing != null)
                            {
                                dbEcoMeasure.Clothing = updatedEcoMeasure.Clothing;
                            }
                            if (updatedEcoMeasure.Electronics != null)
                            {
                                dbEcoMeasure.Electronics = updatedEcoMeasure.Electronics;
                            }
                            if (updatedEcoMeasure.Footwear != null)
                            {
                                dbEcoMeasure.Footwear = updatedEcoMeasure.Footwear;
                            }
                            if (updatedEcoMeasure.Transport != null)
                            {
                                dbEcoMeasure.Transport = updatedEcoMeasure.Transport;
                            }
                            
                            user.EcologicalMeasurements.Add(dbEcoMeasure);
                            await context.SaveAsync<User>(user);
                            // 200 - Ok save complete
                            return 1;
                        }
                        else
                        {
                            // 404 - EcologicalMeasurement with specified date doesn't exist
                            return -8;
                        }
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return -9;
                    }

                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        // TODO: This code is not needed as per PLNKTN-44 & 45 - DELETE
        public async Task<int> DeleteEcologicalMeasurement(string userId, DateTime date_taken)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    User user = await context.LoadAsync<User>(userId);
                    if (user != null)
                    {
                        int numElementsRemoved = user.EcologicalMeasurements.RemoveAll(em => em.Date_taken.Date == date_taken.Date);

                        if (numElementsRemoved == 0)
                        {
                            return numElementsRemoved;
                        }
                        else
                        {
                            await context.SaveAsync<User>(user);
                            return numElementsRemoved;
                        }
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return -9;
                    }
                    
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        public async Task<User> GetUser(string userId)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var user = await context.LoadAsync<User>(userId);
                    return user;
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return null;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return null;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return null;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return null;
                }
            }
        }

        public async Task<IList<User>> GetAllUsers()
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
                    // as the DB increases in size.
                    // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

                    // Defins scan conditions - there are none as we want all users
                    var conditions = new List<ScanCondition>();
                    conditions.Add(new ScanCondition("UserRewards", ScanOperator.IsNotNull));
                    conditions.Add(new ScanCondition("EcologicalMeasurements", ScanOperator.IsNotNull));

                    // TODO ******************** DEBUG ONLY REMOVE FROM TESTING *****************************
                    //conditions.Add(new ScanCondition("Id", ScanOperator.Equal, "2019/8/31/13/31/00/000"));


                    // Gets users from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var users = await context.ScanAsync<User>(conditions).GetRemainingAsync();

                    return users;
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return null;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return null;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return null;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return null;
                }
            }
        }

        /* Add the 'userReward' to all 'users' in the DB.  This is used when a new 'Reward' is created.
         * 'Reward' - Refers to the information required by a user object in the DB in reference to
         * rewards and challenges.
         * 
         */
        public async Task<int> AddUserRewardToAllUsers(UserReward reward)
        {
            using (IDynamoDBContext context = _dbConnection.Context())
            {
                try
                {
                    // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
                    // as the DB increases in size.
                    // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

                    // Defins scan conditions - there are none as we want all users
                    var conditions = new List<ScanCondition>();

                    // Gets users from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var users = await context.ScanAsync<User>(conditions).GetRemainingAsync();


                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            if (user.UserRewards == null)
                            {
                                user.UserRewards = new List<UserReward>();
                            }
                            // Find reward object in User object from the DB where the IDs match
                            UserReward dbReward = user.UserRewards.FirstOrDefault(r => r.Id == reward.Id);

                            if (dbReward == null)
                            {
                                user.UserRewards.Add(reward);
                                await context.SaveAsync(user);
                            }
                            else
                            {
                                // 409 - reward with specified ID already exists, conflict
                            }
                        }
                        // OK All saves complete
                        return 1;
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return -9;
                    }
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }


        /* Adds all 'userRewards' to a single user.  This is used when a new User is created.
         * 'userRewards' - Refers to the infomration required by a user object in the DB in reference to
         * rewards and challenges.
         * 
         */
        public async Task<int> AddAllUserRewardsToAUser(string userId, ICollection<UserReward> userRewards)
        {
            using (IDynamoDBContext context = _dbConnection.Context())
            {
                try
                {
                    // Gets useru from table.
                    var user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        user.UserRewards.AddRange(userRewards);
                        await context.SaveAsync(user);
                        // OK All saves complete
                        return 1;
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return -9;
                    }
                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }

        public async Task<int> AddUserRewardChallenge(string userId, string rewardId, UserRewardChallenge challenge)
        {
            using (IDynamoDBContext context = _dbConnection.Context(_config))
            {
                try
                {
                    User user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        // Find reward object in User object from the DB where the IDs match
                        UserReward dbReward = user.UserRewards.FirstOrDefault(e => e.Id == rewardId);

                        if (dbReward != null)
                        {
                            user.UserRewards.Remove(dbReward);
                            dbReward.Challenges.Add(challenge);
                            user.UserRewards.Add(dbReward);
                            await context.SaveAsync(user);
                            return 1;
                        }
                        else
                        {
                            // 409 - reward with specified ID already exists, conflict
                            return -7;
                        }
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return -9;
                    }



                }
                catch (AmazonServiceException ase)
                {
                    Debug.WriteLine("Could not complete operation");
                    Debug.WriteLine("Error Message:  " + ase.Message);
                    Debug.WriteLine("HTTP Status:    " + ase.StatusCode);
                    Debug.WriteLine("AWS Error Code: " + ase.ErrorCode);
                    Debug.WriteLine("Error Type:     " + ase.ErrorType);
                    Debug.WriteLine("Request ID:     " + ase.RequestId);
                    return -1;
                }
                catch (AmazonClientException ace)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + ace.Message);
                    return -1;
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine("Context obj for DynamoDB set to null");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Internal error occurred communicating with DynamoDB");
                    Debug.WriteLine("Error Message:  " + e.Message);
                    Debug.WriteLine("Inner Exception:  " + e.InnerException);
                    return -1;
                }
            }
        }
    }
}
