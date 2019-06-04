using Amazon.DynamoDBv2.DataModel;
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

        public async Task<int> DeleteEcologicalMeasurement(string userId, DateTime date_taken)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var user = await context.LoadAsync<User>(userId);
                    var numElementsRemoved = user.EcologicalMeasurements.RemoveAll(x => x.Date_taken == date_taken);

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
    }
}
