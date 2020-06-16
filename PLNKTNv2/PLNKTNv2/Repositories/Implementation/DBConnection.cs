using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PLNKTNv2.Repositories
{
    public class DBConnection : IDBConnection
    {
        // Name of the local credentials file.
        private static readonly string _localCredsFilename = "Ahmed.csv";

        // Location in file system where the local credentials file is stored.
        private static readonly string _localCredsUri = "/Users/ahmedali89/Documents";

        // Set DB connection variables here
        // AWS Toolkit profile name to locate the keys of.
        private static readonly string _profileName = "Ahmed";
        // Caller can specify context config as required as per below documentation, defaults to null.
        // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DotNetDynamoDBContext.html#OptionalConfigParams
        private static DynamoDBContextConfig _config = null;

        public IDynamoDBContext Context(DynamoDBContextConfig config = null)
        {
            _config = config;

            if (TryGetLocalDBContext(out IDynamoDBContext context))
            {
                return context;
            }
            else if (TryGetServerDBContext(out context))
            {
                return context;
            }
            else
            {
                return null;
            }
        }

        private IDynamoDBContext GetDBClientOnLocal(string accessKey, string secretKey)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
            {
                // This client will access the US West 1 region (N Cali).
                RegionEndpoint = RegionEndpoint.USWest1
            };
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretKey, clientConfig);

            return GetDBContext(client);
        }

        private IDynamoDBContext GetDBContext(AmazonDynamoDBClient client)
        {
            try
            {
                return new DynamoDBContext(client, _config);
            }
            catch (AmazonDynamoDBException e)
            {
                Debug.WriteLine("Could not complete operation");
                Debug.WriteLine("Error Message:  " + e.Message);
                Debug.WriteLine("HTTP Status:    " + e.StatusCode);
                Debug.WriteLine("AWS Error Code: " + e.ErrorCode);
                Debug.WriteLine("Error Type:     " + e.ErrorType);
                Debug.WriteLine("Request ID:     " + e.RequestId);
                return null;
            }
            catch (AmazonServiceException e)
            {
                Debug.WriteLine("Could not complete operation");
                Debug.WriteLine("Error Message:  " + e.Message);
                Debug.WriteLine("HTTP Status:    " + e.StatusCode);
                Debug.WriteLine("AWS Error Code: " + e.ErrorCode);
                Debug.WriteLine("Error Type:     " + e.ErrorType);
                Debug.WriteLine("Request ID:     " + e.RequestId);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not complete operation");
                Debug.WriteLine("Error Message:  " + e.Message);
                Debug.WriteLine("HTTP Status:    " + e.InnerException);
                return null;
            }
        }

        private bool TryGetLocalDBContext(out IDynamoDBContext context)
        {
            var profileStore = new CredentialProfileStoreChain();

            if (profileStore.TryGetAWSCredentials(_profileName, out AWSCredentials awsCredentials))
            {
                var credentials = awsCredentials.GetCredentials();
                context = GetDBClientOnLocal(credentials.AccessKey, credentials.SecretKey);
                return true;
            }
            else if (File.Exists(_localCredsUri + _localCredsFilename))
            {
                ICollection<string> keys = new List<string>();

                using StreamReader stream = File.OpenText(_localCredsUri + _localCredsFilename);
                keys.Add(stream.ReadLine());
                keys.Add(stream.ReadLine());

                context = GetDBClientOnLocal(keys.ElementAt<string>(0), keys.ElementAt<string>(1));
                if (context != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                context = null;
                return false;
            }
        }
        private bool TryGetServerDBContext(out IDynamoDBContext context)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
            {
                // This client will access the US West 1 region (N Cali).
                RegionEndpoint = RegionEndpoint.USWest1
            };
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);

            context = GetDBContext(client);
            if (context != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}