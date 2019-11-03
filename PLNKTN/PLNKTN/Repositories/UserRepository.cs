using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
        private readonly DynamoDBContextConfig _config;

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


                        if (dbEcoMeasure != null)
                        {
                            // Remove old ecological measurement from DB User
                            user.EcologicalMeasurements.Remove(dbEcoMeasure);

                            // Update DB updatedEcoMeasure with new values
                            dbEcoMeasure.EcologicalFootprint = updatedEcoMeasure.EcologicalFootprint;
                            dbEcoMeasure.Diet = updatedEcoMeasure.Diet;
                            dbEcoMeasure.Clothing = updatedEcoMeasure.Clothing;
                            dbEcoMeasure.Electronics = updatedEcoMeasure.Electronics;
                            dbEcoMeasure.Footwear = updatedEcoMeasure.Footwear;
                            dbEcoMeasure.Transport = updatedEcoMeasure.Transport;

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


        public async Task<CollectiveEF> GetCollective_EF(DateTime date_taken)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    // Define scan conditions
                    var conditions = new List<ScanCondition>();

                    // Gets items from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var collective_EF = await context.ScanAsync<CollectiveEF>(conditions).GetRemainingAsync();

                    var result = collective_EF.FindAll(cf => cf.Date_taken.Date == date_taken.Date);

                    return result.Count > 0 ? result[0] : null;
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

        public async Task<List<User>> GetUsers()
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var conditions = new List<ScanCondition>();
                    List<User> users = await context.ScanAsync<User>(conditions).GetRemainingAsync();
                    if (users != null)
                    {
                        return users;
                    }
                    else
                    {
                        return null;
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
        /*Function below are added new*/
        public async Task<List<CollectiveEF>> GetAllCollective_EFs()
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var conditions = new List<ScanCondition>();

                    List<CollectiveEF> collectiveEFs = await context.ScanAsync<CollectiveEF>(conditions).GetRemainingAsync();

                    if (collectiveEFs != null)
                    {
                        return collectiveEFs;
                    }
                    else
                    {
                        return null;
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
        public async Task<int> AddCollective_EF(CollectiveEF cEF)
        {
            using (IDynamoDBContext context = _dbConnection.Context())
            {
                try
                {
                    var alreadyHasCEF = await GetCollective_EF(cEF.Date_taken);

                    if (alreadyHasCEF == null)
                    {
                        await context.SaveAsync(cEF);
                        return 1;
                    }
                    else
                    {
                        // item already exists
                        return -7;
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
                    var conditions = new List<ScanCondition>
                    {
                        new ScanCondition("UserRewards", ScanOperator.IsNotNull),
                        new ScanCondition("EcologicalMeasurements", ScanOperator.IsNotNull)
                    };

                    // TODO ******************** DEBUG ONLY REMOVE FROM TESTING *****************************
                    //conditions.Add(new ScanCondition("Id", ScanOperator.Equal, "2019/8/31/13/31/00/000"));

                    // Makes the read a strong consistent one to ensure latest values are retrieved.
                    var dbConfig = new DynamoDBOperationConfig
                    {
                        ConsistentRead = true
                    };

                    // Gets users from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var users = await context.ScanAsync<User>(conditions, dbConfig).GetRemainingAsync();

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

        public async Task<int> UpdateUserRewardInAllUsers(UserReward reward)
        {
            using (IDynamoDBContext context = _dbConnection.Context(_config))
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

                            if (dbReward != null)
                            {
                                // Get each challenge that is currently stored in the DB so we can get some of its data
                                foreach (var challenge in dbReward.Challenges)
                                {
                                    // Find the updated challenge that has been sent into this method
                                    var newRewardChallenge = reward.Challenges.FirstOrDefault(c => c.Id == challenge.Id);
                                    // Remove it from the updated reward, ready for changes to be made before re-insertion later
                                    reward.Challenges.Remove(newRewardChallenge);

                                    newRewardChallenge.DateCompleted = challenge.DateCompleted;
                                    newRewardChallenge.NotificationStatus = challenge.NotificationStatus;
                                    newRewardChallenge.Status = challenge.Status;
                                    reward.Challenges.Add(newRewardChallenge);
                                }

                                // Update the reward information
                                reward.DateCompleted = dbReward.DateCompleted;
                                reward.NotificationStatus = dbReward.NotificationStatus;
                                reward.Status = dbReward.Status;
                                reward.IsRewardGranted = dbReward.IsRewardGranted;

                                // Remove the old reward entry and add the new one and save
                                user.UserRewards.Remove(dbReward);
                                user.UserRewards.Add(reward);
                                await context.SaveAsync(user);
                            }
                            else
                            {
                                user.UserRewards.Add(reward);
                                await context.SaveAsync(user);
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

        /* Delete the specified 'userReward' in all 'users' in the DB.  This is used when a 'Reward' needs to be removed.
         * 'Reward' - Refers to the information required by a user object in the DB in reference to
         * rewards and challenges.
         * 
         */
        public async Task<int> DeleteUserRewardFromAllUsers(string rewardId)
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
                            // Find reward object in User object from the DB where the IDs match
                            UserReward dbUserReward = user.UserRewards.FirstOrDefault(r => r.Id == rewardId);

                            if (dbUserReward != null)
                            {
                                user.UserRewards.Remove(dbUserReward);
                                await context.SaveAsync(user);
                            }
                            else
                            {
                                // This user doesn't have the associated dbUserReward in its UserReward collection - Do nothing
                            }
                        }
                        // OK All saves complete
                        return 1;
                    }
                    else
                    {
                        // 404 - No users in the DB
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

        public Task<int> AddCompletedChallengeUser(string userId, EcologicalMeasurement ecologicalMeasurement)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Rgn>> GetUserGrantedReward(string userId, string region_name)
        {
            using (IDynamoDBContext context = _dbConnection.Context(_config))
            {
                try
                {
                    User user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        if (user.GrantedRewards != null)
                        {
                            Bin grantedRewards = user.GrantedRewards.FirstOrDefault(r => r.Region_name == region_name);

                            if (grantedRewards != null)
                            {
                                return grantedRewards.Projects;
                            }
                            else
                            {
                                // 404 grantedReward list with region_name do not exist
                                return null;
                            }
                        }
                        else
                        {
                            // 404 There are no grantedRewards yet
                            return null;
                        }
                    }
                    else
                    {
                        // 404 - User with specified userId doesn't exist
                        return null;
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
        public async Task<int> AddUserGrantedReward(string userId, Bin rewardRegion)
        {
            using (IDynamoDBContext context = _dbConnection.Context())
            {
                try
                {
                    User user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {

                        Bin dbgrantedReward = user.GrantedRewards.FirstOrDefault(r => r.Region_name == rewardRegion.Region_name);

                        if (dbgrantedReward == null)
                        {
                            user.GrantedRewards.Add(rewardRegion);
                            await context.SaveAsync(user);
                            return 1;
                        }
                        else
                        {
                            // user already planted here(it is okay to plant again).  Increate count
                            dbgrantedReward.Count++;
                            await context.SaveAsync(user);
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

        public async Task<int> UpdateUserReward(string userId, UserReward model)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var user = await context.LoadAsync<User>(userId);

                    if (user != null)
                    {
                        user.UserRewards.RemoveAll(ur => ur.Id == model.Id);
                        user.UserRewards.Add(model);

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
    }
}
