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
    }
}