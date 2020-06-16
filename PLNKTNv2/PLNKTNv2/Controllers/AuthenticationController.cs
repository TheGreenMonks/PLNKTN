using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.Models.Dtos;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string _clientId = "4s9n3lg2ptogi5o4pj899332kj";
        private const string _userPoolId = "us-west-2_yt7xxSRrl";
        private readonly RegionEndpoint _region = RegionEndpoint.USWest2;

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<string>> Register(UserAuthDto user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            var customRoleAttribute = new AttributeType
            {
                Name = "custom:role",
                Value = "EndUser"
            };
            request.UserAttributes.Add(customRoleAttribute);

            // TODO: Client needs to ensure user is signed up and confirmed successfully
            var response = await cognito.SignUpAsync(request);

            var confResponse = await cognito.AdminConfirmSignUpAsync(new AdminConfirmSignUpRequest
            {
                Username = user.Username,
                UserPoolId = _userPoolId
            });

            return Ok();
        }

        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<string>> SignIn(UserAuthDto user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _userPoolId,
                ClientId = _clientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            return Ok(response.AuthenticationResult.IdToken);
        }
    }
}