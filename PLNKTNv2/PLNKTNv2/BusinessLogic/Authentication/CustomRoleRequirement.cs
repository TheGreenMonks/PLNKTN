using Microsoft.AspNetCore.Authorization;

namespace PLNKTNv2.BusinessLogic.Authentication
{
    internal class CustomRoleRequirement : IAuthorizationRequirement
    {
        public string CustomRole { get; private set; }

        public CustomRoleRequirement(string customRole)
        {
            CustomRole = customRole;
        }
    }
}