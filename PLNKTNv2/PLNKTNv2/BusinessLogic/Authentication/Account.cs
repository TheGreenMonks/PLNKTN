using System.Security.Claims;

namespace PLNKTNv2.BusinessLogic.Authentication
{
    public class Account : IAccount
    {
        public string GetAccountId(ClaimsPrincipal user)
        {
            return user.FindFirst("cognito:username")?.Value;
        }
    }
}