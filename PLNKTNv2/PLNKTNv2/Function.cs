using Amazon;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using PLNKTNv2.Models.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PLNKTNv2
{
    public class Function
    {
        /// <summary>
        /// Gets security credentials of admin user from AWS secrets store, gets and ID token from AWS Cognito 
        /// and calls /Prod/api/users/CalculateUserRewardCompletion as admin.
        /// </summary>
        /// <param name="context">The Lambda context information sent to the function.</param>
        public async Task FunctionHandler(ILambdaContext context)
        {
            LambdaLogger.Log("**** PLNKTNv2 START Auto Invocation of /Prod/api/signin ****");

            string userPasswordJSONRaw = GetSecret("dexter-admin");
            Dictionary<string, string> pw = JsonConvert.DeserializeObject<Dictionary<string, string>>(userPasswordJSONRaw);
            LambdaLogger.Log("**** Secret retrieved ****");

            UserAuthDto user = new UserAuthDto()
            {
                Password = pw["password"],
                Username = "dexter-admin"
            };

            HttpClient _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://a38pr2cfl6.execute-api.us-west-2.amazonaws.com/Prod/")
                //BaseAddress = new Uri("https://localhost:44356/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/signin")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage webApiResponse;

            try
            {
                webApiResponse = await _httpClient.SendAsync(requestMessage);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            finally
            {
                requestMessage.Dispose();
            }

            string responseBodyAsString = await webApiResponse.Content.ReadAsStringAsync();
            AuthenticationResultType cognitoDetails = JsonConvert.DeserializeObject<AuthenticationResultType>(responseBodyAsString);
            requestMessage.Dispose();
            LambdaLogger.Log("**** Admin user login complete ****");

            LambdaLogger.Log("**** PLNKTNv2 END Auto Invocation of /Prod/api/signin ****");



            LambdaLogger.Log("**** PLNKTNv2 START Auto Invocation of /Prod/api/users/CalculateUserRewardCompletion ****");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cognitoDetails.IdToken);

            requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/users/CalculateUserRewardCompletion");
            try
            {
                webApiResponse = await _httpClient.SendAsync(requestMessage);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            finally
            {
                requestMessage.Dispose();
                _httpClient.Dispose();
            }

            LambdaLogger.Log("**** CalculateUserRewardCompletion call complete with status code result: " + webApiResponse.StatusCode + " ****");

            LambdaLogger.Log("**** PLNKTNv2 END Auto Invocation of /Prod/api/users/CalculateUserRewardCompletion ****");
        }

        private string GetSecret(string secretName)
        {
            string region = "us-west-2";

            MemoryStream memoryStream = new MemoryStream();

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (Amazon.SecretsManager.Model.InvalidParameterException)
            {
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                throw;
            }
            catch (InvalidRequestException)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (Amazon.SecretsManager.Model.ResourceNotFoundException)
            {
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (System.AggregateException)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                return response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }
        }
    }
}