using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PLNKTNv2.BusinessLogic.Authentication
{
    internal class CustomRoleAuthorizationHandler : AuthorizationHandler<CustomRoleRequirement>
    {
        private readonly string _cognitoAuthority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_yt7xxSRrl";
        private readonly string _customVariable = "custom:role";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleRequirement requirement)
        {
            // Fail Authorisation if the JWT custom claim isn't present
            if (!context.User.HasClaim(c => c.Issuer == _cognitoAuthority && c.Type == _customVariable))
            {
                return Task.CompletedTask;
            }

            // Fail Authorisation if we can't read a string value from _customVariable
            string customRole = context.User.FindFirst(c => c.Issuer == _cognitoAuthority && c.Type == _customVariable).Value;
            if (customRole == null)
            {
                return Task.CompletedTask;
            }

            // Validate the role received from JWT is same as specified in _customVariable
            if (customRole.Contains(requirement.CustomRole))
            {
                // Mark the requirement as satisfied
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}