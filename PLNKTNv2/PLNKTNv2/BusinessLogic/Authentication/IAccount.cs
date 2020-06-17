using System.Security.Claims;

namespace PLNKTNv2.BusinessLogic.Authentication
{
    public interface IAccount
    {
        string GetAccountId(ClaimsPrincipal user);
    }
}