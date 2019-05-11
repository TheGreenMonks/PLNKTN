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
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public class DBConnection : IDBConnection
    {
        // Set DB connection variables here
        // AWS Toolkit profile name to locate the keys of.
        private static readonly string _profileName = "Dexter";
        // Location in file system where the local credentials file is stored.
        private static readonly string _localCredsUri = "C:\\";
        // Name of teh local credentials file.
        private static readonly string _localCredsFilename = "Ahmed.csv";

        public IDynamoDBContext Context()
        {
            IDynamoDBContext context;

            if (tryGetLocalDBContext(out context))
            {
                return context;
            }
            else if (tryGetServerDBContext(out context))
            {
                return context;
            }
            else
            {
                return null;
            }
        }

        private bool tryGetLocalDBContext(out IDynamoDBContext context)
        {
            var profileStore = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials = null;

            if (profileStore.TryGetAWSCredentials(_profileName, out awsCredentials))
            {
                var credentials = awsCredentials.GetCredentials();
                context = getDBClientOnLocal(credentials.AccessKey, credentials.SecretKey);
                return true;
            }
            else if (File.Exists(_localCredsUri + _localCredsFilename))
            {
                ICollection<string> keys = new List<string>();

                using (StreamReader stream = File.OpenText(_localCredsUri + _localCredsFilename))
                {
                    keys.Add(stream.ReadLine());
                    keys.Add(stream.ReadLine());

                    context = getDBClientOnLocal(keys.ElementAt<string>(0), keys.ElementAt<string>(1));
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
            else
            {
                context = null;
                return false;
            }
        }

        private IDynamoDBContext getDBClientOnLocal(string accessKey, string secretKey)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            // This client will access the US West 1 region (N Cali).
            clientConfig.RegionEndpoint = RegionEndpoint.USWest1;
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretKey, clientConfig);

            return getDBContext(client);
        }

        private bool tryGetServerDBContext(out IDynamoDBContext context)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            // This client will access the US West 1 region (N Cali).
            clientConfig.RegionEndpoint = RegionEndpoint.USWest1;
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);

            context = getDBContext(client);
            if (context != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private IDynamoDBContext getDBContext(AmazonDynamoDBClient client)
        {
            try
            {
                return new DynamoDBContext(client);
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
    }
}
