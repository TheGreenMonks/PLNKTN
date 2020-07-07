using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using PLNKTNv2.BusinessLogic.Authentication;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PLNKTNv2.Persistence.Repositories.Implementation
{
    /// <summary>
    /// A Factory class to create a <c>DynamoDbContext</c> instance.  On creation it will check if the system has the
    /// AWS SDK profile store where i can get credentials for the DB.  If this fails it will try a custom reversionary
    /// mode we created to allow Mac users to remotely login to the DB.  Finally it will assume the code is running
    /// on an EC2 instance and create a DB context for that environment that can access the DB.
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        // Name of the local credentials file.
        private readonly string _localCredsFilename = "Ahmed.csv";

        // Location in file system where the local credentials file is stored.
        private readonly string _localCredsUri = "/Users/ahmedali89/Documents";

        // Set DB connection variables here
        private bool disposedValue;

        private readonly RegionEndpoint debugDb = RegionEndpoint.USWest1;
        private readonly RegionEndpoint releaseDb = RegionEndpoint.USWest1;
        private readonly IAccount _account;

        public DbContextFactory(IAccount account)
        {
            _account = account;

            CreateDbContext();
        }

        public IDynamoDBContext DbContext { get; private set; }

        /// <summary>
        /// Creates a DynamoDB context to enable communication between the app and DyDB.
        /// </summary>
        /// <remarks>
        /// Creates context using either 1-the credentials stored in the AWS SDK installed in Visual Studio,
        /// 2-the credentials stored in a file stored locally on your machine OR, 3-the credentials granted to the
        /// service running in the AWS ecosystem.
        /// DB Endpoint is set as USWEST1 to represent the DB location.
        /// </remarks>
        private void CreateDbContext()
        {
            if (_account.TryGetLocalAwsCredentials(debugDb, out AWSCredentials awsCredentials))
            {
                var credentials = awsCredentials.GetCredentials();
                DbContext = GetDBClientOnLocal(credentials.AccessKey, credentials.SecretKey);
            }
            else if (File.Exists(_localCredsUri + _localCredsFilename))
            {
                ICollection<string> keys = new List<string>();

                using (StreamReader stream = File.OpenText(_localCredsUri + _localCredsFilename))
                {
                    keys.Add(stream.ReadLine());
                    keys.Add(stream.ReadLine());

                    DbContext = GetDBClientOnLocal(keys.ElementAt<string>(0), keys.ElementAt<string>(1));
                }
            }
            else
            {
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
                {
                    // This client will access the US West 1 region (N Cali).
                    RegionEndpoint = releaseDb
                };
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
                DbContext = new DynamoDBContext(client, new DynamoDBContextConfig
                {
                    IgnoreNullValues = true
                });
            }
        }

        private IDynamoDBContext GetDBClientOnLocal(string accessKey, string secretKey)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
            {
                // This client will access the US West 1 region (N Cali).
                RegionEndpoint = debugDb
            };
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretKey, clientConfig);
            return new DynamoDBContext(client, new DynamoDBContextConfig
            {
                IgnoreNullValues = true
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DbContextFactory()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}