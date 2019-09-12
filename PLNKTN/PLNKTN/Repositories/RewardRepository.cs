using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using PLNKTN.Models;

namespace PLNKTN.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly IDBConnection _dbConnection;
        private DynamoDBContextConfig _config;

        public RewardRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;

            // make context not delete attributes that are null, thus save operation will only 
            // update values that have been set by user.
            _config = new DynamoDBContextConfig
            {
                IgnoreNullValues = true
            };
        }

        public async Task<int> CreateReward(Reward reward)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var rewardExists = await context.LoadAsync<Reward>(reward.Id);
                    if (rewardExists != null)
                    {
                        // Error - Reward already exists
                        return -10;
                    }
                    else
                    {
                        await context.SaveAsync(reward);
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

        public async Task<ICollection<Reward>> GetAllRewards()
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
                    // as the DB increases in size.
                    // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

                    // Defins scan conditions - there are none as we want all rewards
                    var conditions = new List<ScanCondition>();

                    // Gets rewards from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var rewards = await context.ScanAsync<Reward>(conditions).GetRemainingAsync();

                    return rewards;
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

        public async Task<int> UpdateReward(Reward reward)
        {
            using (var context = _dbConnection.Context(_config))
            {
                try
                {
                    var existingReward = await context.LoadAsync<Reward>(reward.Id);
                    if (existingReward != null)
                    {
                        await context.SaveAsync(reward);
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

        public async Task<int> CreateRegion(RewardCountry country)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var countryExists = await context.LoadAsync<User>(country.name);
                    if (countryExists != null)
                    {
                        return -10;
                    }
                    else
                    {
                        await context.SaveAsync(country);
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

        public async Task<int> AddRegionIntoCountry(string country_name,Region region)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    RewardCountry ctr = await context.LoadAsync<RewardCountry>(country_name);
                    if (ctr != null)
                    {
                        ctr.Regions.Add(region);
                        await context.SaveAsync(ctr);
                        return 1;
                    }
                    else
                    {
                        // 404 - Country with specified name doesn't exist
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

        public async Task<List<Region>> GetAllRegionsFromCountry(string country_name)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var country = await context.LoadAsync<RewardCountry>(country_name);
                    if (country != null)
                    {
                        return country.Regions;
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

        public async Task<Region> GetRegionInfo(string country_name, string region_name)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var country = await context.LoadAsync<RewardCountry>(country_name);
                    var region = country.Regions.FirstOrDefault(e => e.Region_name == region_name);
                    if (region != null)
                    {
                        return region;
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
    }
}
