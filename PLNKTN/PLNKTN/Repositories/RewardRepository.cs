using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using PLNKTN.Models;

namespace PLNKTN.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly IDBConnection _dbConnection;
        private readonly DynamoDBContextConfig _config;

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

                    // TODO ******************** DEBUG ONLY REMOVE FROM TESTING *****************************
                    //conditions.Add(new ScanCondition("Id", ScanOperator.Equal, "test"));

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

        public async Task<Reward> GetReward(string id)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var reward = await context.LoadAsync<Reward>(id);

                    return reward;
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

        public async Task<int> DeleteReward(string id)
        {
        using (var context = _dbConnection.Context())
            {
                try
                {
                var reward = await context.LoadAsync<Reward>(id);
                    if (reward != null)
                    {
                        await context.DeleteAsync(reward);
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

        public async Task<int> CreateRegion(RewardRegion region)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var exits = await context.LoadAsync<RewardRegion>(region.Region_name);
                    if (exits != null)
                    {
                        return -10;
                    }
                    else
                    {
                        await context.SaveAsync(region);
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

        public async Task<int> AddProject(string region_name, Project project)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    RewardRegion region = await context.LoadAsync<RewardRegion>(region_name);
                    if (region != null)
                    {
                        region.Projects.Add(project);
                        await context.SaveAsync(region);
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

        public async Task<List<string>> GetAllRegionNames()
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    // Define scan conditions
                    var conditions = new List<ScanCondition>();

                    // Gets all regions from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var regions = await context.ScanAsync<RewardRegion>(conditions).GetRemainingAsync();
                    List<string> names = new List<string>();
                    foreach (RewardRegion region in regions)
                    {
                        names.Add(region.Region_name);
                    }
                    return names.Count > 0 ? names : null;

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

        public async Task<List<Project>> GetAllProjects(string region_name)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var region = await context.LoadAsync<RewardRegion>(region_name);
                    if (region != null)
                    {
                        return region.Projects;
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

        public async Task<Project> GetProjectInfo(string region_name, string project_name)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    var region = await context.LoadAsync<RewardRegion>(region_name);
                    if (region == null)
                    {
                        return null;
                    }

                    var project = region.Projects.FirstOrDefault(e => e.Project_name == project_name);
                    if (project != null)
                    {
                        return project;
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

        public async Task<int> ThrowTreeInBin(string region_name, Rgn project)
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    Bin region = await context.LoadAsync<Bin>(region_name);
                    if (region != null)
                    {
                        if (region.Projects != null)
                        {
                            region.Projects.Add(project);
                            await context.SaveAsync(region);
                            return 1;
                        }
                        else
                        {
                            region.Projects = new List<Rgn>();
                            region.Projects.Add(project);
                            await context.SaveAsync(region);
                            return 1;
                        }
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
    }
}
