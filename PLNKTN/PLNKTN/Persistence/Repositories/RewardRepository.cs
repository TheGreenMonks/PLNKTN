using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Persistence.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly IDynamoDBContext _dbContext;

        public RewardRepository(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BatchWrite<Reward> Insert(Reward reward, BatchWrite<Reward> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<Reward>();
            }
            batchWrite.AddPutItem(reward);
            return batchWrite;
        }

        public async Task<IList<Reward>> GetAllAsync()
        {
            // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
            // as the DB increases in size.
            // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

            // Defins scan conditions - there are none as we want all rewards
            IList<ScanCondition> conditions = new List<ScanCondition>();

            // TODO ******************** DEBUG ONLY REMOVE FROM TESTING *****************************
            //conditions.Add(new ScanCondition("Id", ScanOperator.Equal, "test"));

            // Gets rewards from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
            IList<Reward> rewards = await _dbContext.ScanAsync<Reward>(conditions).GetRemainingAsync();
            return rewards;
        }

        public async Task<Reward> GetByIdAsync(string id)
        {
            return await _dbContext.LoadAsync<Reward>(id);
        }

        public BatchWrite<Reward> Update(Reward reward, BatchWrite<Reward> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<Reward>();
            }
            batchWrite.AddPutItem(reward);
            return batchWrite;
        }

        public BatchWrite<Reward> DeleteById(string id, BatchWrite<Reward> batchWrite = null)
        {
            if (batchWrite == null)
            {
                batchWrite = _dbContext.CreateBatchWrite<Reward>();
            }
            batchWrite.AddDeleteKey(id);
            return batchWrite;
        }

        public async Task<int> CreateRegion(RewardRegion region)
        {
            using (var context = _dbConnection.CreateDbContext())
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
                    Debug.WriteLine("CreateDbContext obj for DynamoDB set to null");
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
            using (var context = _dbConnection.CreateDbContext())
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
                    Debug.WriteLine("CreateDbContext obj for DynamoDB set to null");
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
            using (var context = _dbConnection.CreateDbContext())
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
                    Debug.WriteLine("CreateDbContext obj for DynamoDB set to null");
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
            using (var context = _dbConnection.CreateDbContext())
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
                    Debug.WriteLine("CreateDbContext obj for DynamoDB set to null");
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
            using (var context = _dbConnection.CreateDbContext())
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
                    Debug.WriteLine("CreateDbContext obj for DynamoDB set to null");
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
            using (var context = _dbConnection.CreateDbContext())
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
                    Debug.WriteLine("CreateDbContext obj for DynamoDB set to null");
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
