using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public class DBConnection : IDBConnection
    {
        public DynamoDBContext Context()
        {
            var profileStore = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials = null;
            DynamoDBContext context = null;

            if (profileStore.TryGetAWSCredentials("Dexter", out awsCredentials))
            {
                var credentials = awsCredentials.GetCredentials();
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
                // This client will access the US West 1 region (N Cali).
                clientConfig.RegionEndpoint = RegionEndpoint.USWest1;
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials.AccessKey, credentials.SecretKey, clientConfig);

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

            return context;
        }
    }
}
