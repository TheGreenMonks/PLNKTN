using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.Models.Dtos;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string _clientId = "4s9n3lg2ptogi5o4pj899332kj";
        private const string _userPoolId = "us-west-2_yt7xxSRrl";
        private readonly RegionEndpoint _region = RegionEndpoint.USWest2;

        /// <summary>
        /// Register a user for the service with a valid username and password.
        /// </summary>
        /// <param name="user">The <c>UserAuthDto</c> with username, password and valid email address of user to register.</param>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns authentication data.</response>
        /// <response code="400">Poorly formed request.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserAuthDto user)
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

        /// <summary>
        /// Sign a user into the service via a valid username and password.
        /// </summary>
        /// <param name="user">The <c>UserAuthDto</c> with username and password of user to sign in.</param>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns authentication data.</response>
        /// <response code="400">Poorly formed request.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminInitiateAuthResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(UserAuthDto user)
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

            return Ok(response.AuthenticationResult);
        }
    }
}