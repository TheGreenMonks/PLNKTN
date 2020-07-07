using Amazon;
using Amazon.Runtime;
using System.Security.Claims;

namespace PLNKTNv2.BusinessLogic.Authentication
{

    /// <summary>
    /// Interface defining all methods for data retrieval and manipulation of a User's authentication and
    /// authorisation information.
    /// </summary>
    public interface IAccount
    {

        /// <summary>
        /// Get the user name (Id) of the current authenticated user.
        /// </summary>
        /// <param name="user">The ClaimsPrinciple implementation for a user</param>
        /// <returns>User name as string</returns>
        string GetAccountId(ClaimsPrincipal user);

        /// <summary>
        /// Tries to get AWS user credentials from local store using the value of environment variable
        /// "AWS_PROFILE_NAME" as the profile name.
        /// </summary>
        /// <param name="region">The RegionEndpoint to use for credentials</param>
        /// <param name="credentials">Local AWSCredentials of user (this is out param)</param>
        /// <returns>True for successful retrieval of credentials or false for no local credentials found.
        /// Out param returns credentials object on successful retrieval.</returns>
        bool TryGetLocalAwsCredentials(RegionEndpoint region, out AWSCredentials credentials);
    }
}