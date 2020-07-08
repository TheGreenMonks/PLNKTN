using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Security.Claims;

namespace PLNKTNv2.BusinessLogic.Authentication
{
    /// <summary>
    /// Contains all methods for data retrieval and manipulation of a User's authentication and
    /// authorisation information (JWT token).
    /// </summary>
    internal class Account : IAccount
    {
        public string GetAccountId(ClaimsPrincipal user)
        {
            return user.FindFirst("cognito:username")?.Value;
        }

        public bool TryGetLocalAwsCredentials(RegionEndpoint region, out AWSCredentials credentials)
        {
            var profileStore = new CredentialProfileStoreChain();
            /* Info: Environment variables for the release/live version of this serverless app (i.e. when hosted on AWS) are defined in the serverless.template file
             * under the lambda application.  For further details see https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/sam-resource-function.html#sam-function-environment
             * and https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/aws-properties-lambda-function-environment.html that describe how to add environment variables to the 
             * live application. For local testing purposes env vars are still defined locally in the 'launchSettings.json'.
             */
            if (profileStore.TryGetAWSCredentials(Environment.GetEnvironmentVariable("AWS_PROFILE_NAME"),
                out AWSCredentials _credentials))
            {
                credentials = _credentials;
                return true;
            }
            credentials = _credentials;
            return false;
        }
    }
}