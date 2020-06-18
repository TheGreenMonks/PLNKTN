using System.Security.Claims;

namespace PLNKTNv2.BusinessLogic.Authentication
{

    /// <summary>
    /// Contains all methods for data retrieval and manipulation of a User's authentication and
    /// authorisation information (JWT token).
    /// </summary>
    internal class Account : IAccount
    {
        /// <summary>
        /// Get the AWS Cognito user name (Id) of the current authenticated user.
        /// </summary>
        /// <param name="user">The ClaimsPrinciple implementation for JWT Bearer</param>
        /// <returns>Cognito user name as string</returns>
        public string GetAccountId(ClaimsPrincipal user)
        {
            return user.FindFirst("cognito:username")?.Value;
        }
    }
}