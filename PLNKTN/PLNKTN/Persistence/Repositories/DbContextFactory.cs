using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PLNKTN.Persistence.Repositories
{
    /// <summary>
    /// A Factory class to create a <c>DynamoDbContext</c> instance.  On creation it will check if the system has the 
    /// AWS SDK profile store where i can get credentials for the DB.  If this fails it will try a custom reversionary
    /// mode we created to allow Mac users to remotely login to the DB.  Finally it will assume the code is running
    /// on an EC2 instance and create a DB context for that environment that can access the DB.
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        public IDynamoDBContext DbContext { get; private set; }

        // Set DB connection variables here
        // AWS Toolkit profile name to locate the keys of.
        private readonly string _profileName = "Ahmed";
        // Location in file system where the local credentials file is stored.
        private readonly string _localCredsUri = "/Users/ahmedali89/Documents";
        // Name of the local credentials file.
        private readonly string _localCredsFilename = "Ahmed.csv";
        // Caller can specify DbContext config as required as per below documentation, defaults to null.
        // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DotNetDynamoDBContext.html#OptionalConfigParams
        //private DynamoDBContextConfig _config = null;

        public DbContextFactory()
        {
            DbContext = CreateDbContext();
        }

        private IDynamoDBContext CreateDbContext()
        {
            var profileStore = new CredentialProfileStoreChain();

            if (profileStore.TryGetAWSCredentials(_profileName, out AWSCredentials awsCredentials))
            {
                var credentials = awsCredentials.GetCredentials();
                return GetDBClientOnLocal(credentials.AccessKey, credentials.SecretKey);
            }
            else if (File.Exists(_localCredsUri + _localCredsFilename))
            {
                ICollection<string> keys = new List<string>();

                using (StreamReader stream = File.OpenText(_localCredsUri + _localCredsFilename))
                {
                    keys.Add(stream.ReadLine());
                    keys.Add(stream.ReadLine());

                    return GetDBClientOnLocal(keys.ElementAt<string>(0), keys.ElementAt<string>(1));
                }
            }
            else
            {
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
                {
                    // This client will access the US West 1 region (N Cali).
                    RegionEndpoint = RegionEndpoint.USWest1
                };
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
                return new DynamoDBContext(client, null);
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
            return new DynamoDBContext(client, new DynamoDBContextConfig {
                IgnoreNullValues = true
            });
        }
    }
}
