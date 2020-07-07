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