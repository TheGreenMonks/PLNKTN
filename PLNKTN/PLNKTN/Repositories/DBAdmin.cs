using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Repositories
{
    public class DBAdmin
    {
        public DBAdmin()
        {
            
        }

        private async int createUserTable()
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
                      AttributeType = "N"
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
        }
    }
}
