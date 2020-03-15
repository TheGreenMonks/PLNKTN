using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string userEmail)
        {
            string userId = _userRepository.GetUserIdByEmail(userEmail);
            if (String.IsNullOrEmpty(userId))
            {
                // Return 200 even though this is an error condition. This helps prevent attackers using
                // brute force from finding out what email addresses are valid or not.
                return Ok("A security code has been sent to the email address provided.");
            }

            UInt16[] rndNumbers = new UInt16[2];
            DateTime secureCodeExpiration = DateTime.UtcNow.AddHours(1);

            if (!String.IsNullOrWhiteSpace(userId))
            {
                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                {
                    for (int i = 0; i < 2; i++)
                    {
                        byte[] tokenData = new byte[2];
                        UInt16 codePart = 0;
                        do
                        {
                            rng.GetBytes(tokenData);
                            codePart = BitConverter.ToUInt16(tokenData);
                        } while (codePart < 1000);

                        rndNumbers[i] = codePart;
                    }
                }
            }

            User user = await _userRepository.GetUser(userId);
            user.SecureCode = String.Join("", rndNumbers);
            user.SecureCodeExpires = secureCodeExpiration;

            var result = await _userRepository.UpdateUser(user);

            if (result == 1 || result == -9)
            {
                // return HTTP 200 Ok user was updated.  PUT does not require user to be returned in HTTP body.
                // Not done to save bandwidth.
                return Ok("A security code has been sent to the email address provided.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An internal error occurred.  Please contact the system administrator.");
            }
        }
    }
}