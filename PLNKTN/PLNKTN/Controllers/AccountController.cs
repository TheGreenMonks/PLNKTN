using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;
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

            string secureCodeTemp = String.Join("", rndNumbers);

            user.SecureCode = GetHashString(secureCodeTemp);
            user.SecureCodeExpires = secureCodeExpiration;
            EmailHelper.SendEmailSecureCode(secureCodeTemp, userEmail);

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

        [HttpGet("{userEmail}")]
        public async Task<IActionResult> Get(string userEmail, [FromBody] string secureCode)
        {
            string userId = _userRepository.GetUserIdByEmail(userEmail);

            if (String.IsNullOrWhiteSpace(secureCode) || String.IsNullOrWhiteSpace(userId))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Information incorrect.");
            }

            var user = await _userRepository.GetUser(userId);
            //int dateComparison = DateTime.Compare(user.SecureCodeExpires.Value, DateTime.UtcNow);
            string storedSecureCode = GetHashString(secureCode);

            if (DateTime.Compare(user.SecureCodeExpires.Value, DateTime.UtcNow) > 0 && 
                String.Equals(user.SecureCode, storedSecureCode, StringComparison.OrdinalIgnoreCase))
            {
                // Reset secure code so that it is not unnecessarily exposed
                user.SecureCode = null;
                // return HTTP 200
                return Ok(user);
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return StatusCode(StatusCodes.Status400BadRequest,
                    "An internal error occurred.  Please contact the system administrator.");
            }
        }

        private byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}