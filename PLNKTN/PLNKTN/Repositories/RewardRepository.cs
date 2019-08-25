﻿using System;
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

        public async Task<IList<Reward>> GetAllRewards()
        {
            using (var context = _dbConnection.Context())
            {
                try
                {
                    // TODO - This needs to be correctly designed as performace at scale is a VERY large issue
                    // as the DB increases in size.
                    // ref -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html

                    // Defins scan conditions - there are none as we want all rewards
                    IEnumerable<ScanCondition> conditions = new List<ScanCondition>();

                    // Gets rewards from table.  .GetRemainingAsync() is placeholder until sequential or parallel ops are programmed in.
                    var _rewards = await context.ScanAsync<Reward>(conditions).GetRemainingAsync();

                    return _rewards;
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