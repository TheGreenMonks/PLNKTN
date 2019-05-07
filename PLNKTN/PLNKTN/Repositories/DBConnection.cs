using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Collections.Generic;
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

            if (tryGetToolkitAWSCredentials(_profileName, out context))
            {
                return context;
            }
            else if (tryGetLocalAWSCredentials(_localCredsUri, _localCredsFilename, out context))
            {
                return context;
            }
            else
            {
                return null;
            }
        }

        private bool tryGetToolkitAWSCredentials(string profileName, out IDynamoDBContext context)
        {
            var profileStore = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials = null;

            if (profileStore.TryGetAWSCredentials(profileName, out awsCredentials))
            {
                var credentials = awsCredentials.GetCredentials();
                context = getDBClient(credentials.AccessKey, credentials.SecretKey);
                return true;
            }
            else
            {
                context = null;
                return false;
            }
        }

        private bool tryGetLocalAWSCredentials(string localCredsUri, string localCredsFilename, out IDynamoDBContext context)
        {
            ICollection<string> keys = new List<string>();

            using (StreamReader stream = File.OpenText(localCredsUri + localCredsFilename))
            {
                keys.Add(stream.ReadLine());
                keys.Add(stream.ReadLine());

                context = getDBClient(keys.ElementAt<string>(0), keys.ElementAt<string>(1));
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

        private IDynamoDBContext getDBClient(string accessKey, string secretKey)
        {
            IDynamoDBContext context;
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            // This client will access the US West 1 region (N Cali).
            clientConfig.RegionEndpoint = RegionEndpoint.USWest1;
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretKey, clientConfig);

            try
            {
                context = new DynamoDBContext(client);
                return context;
            }
            catch
            {
                // TODO: Implement explicit exceptions relevant to 'DynamoDBContext'
                return null;
            }
        }
    }
}
