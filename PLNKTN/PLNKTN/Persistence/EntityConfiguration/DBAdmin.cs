using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace PLNKTN.Persistence.EntityConfiguration
{
    /* 
     * TODO
     * This class is here to represent the programmatic way to create the tables required for this application.
     * It doesn't currently set up the tables programmatically, however, with some small modifications,
     * could be programmed to do so.  It could also be used to check the DBs are 'active' before the application
     * launches to ensure integrity and help automate the startup procedure.
     * 
     */
    public class DBAdmin
    {
        public DBAdmin()
        {
            InitDbTables();
        }

        private async void InitDbTables()
        {
            var client = new AmazonDynamoDBClient(RegionEndpoint.USWest1);
            var status = "";

            do
            {
                // Wait 5 seconds before checking (again).
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

                try
                {
                    var response = await client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = "Rewards"
                    });

                    Debug.WriteLine("Table = {0}, Status = {1}",
                      response.Table.TableName,
                      response.Table.TableStatus);

                    status = response.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    //   get resource not found. 
                }

            } while (status != TableStatus.ACTIVE);
        }

        private async Task<HttpStatusCode> CreateUserTable()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            var request = new CreateTableRequest
            {
                TableName = "Users",
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                      AttributeName = "Id",
                      AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                      AttributeName = "Id",
                      KeyType = "HASH"  //Partition key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 1
                }
            };

            var response = await client.CreateTableAsync(request);

            return response.HttpStatusCode;
        }

        private async Task<HttpStatusCode> CreateRewardTable()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            var request = new CreateTableRequest
            {
                TableName = "Rewards",
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                      AttributeName = "Id",
                      AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                      AttributeName = "Id",
                      KeyType = "HASH"  //Partition key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 1
                }
            };

            var response = await client.CreateTableAsync(request);

            Debug.WriteLine("HTTP Status Code: " + response.HttpStatusCode.ToString());
            return response.HttpStatusCode;
        }
    }
}
