using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Authentication;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using PLNKTNv2.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    /// <summary>
    /// The Users Controller holds methods to retrieve and manipulate data held about users in the database on AWS.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token before they will execute.
    /// </remarks>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccount _account;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor to create Controller with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and the generic repository.</param>
        /// <param name="account">Provides access to authenticated user data.</param>
        /// <param name="userService">Provides business logic for processing data related to users.</param>
        public UsersController(IUnitOfWork unitOfWork, IAccount account, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _account = account;
            _userService = userService;
        }

        /// <summary>
        /// DELETE method to remove a user and its associated data from the database.
        /// </summary>
        /// <remarks>
        /// Special administration priviledges required to execute this function.
        /// </remarks>
        /// <param name="id">The <c>string</c> id of the user to be removed.</param>
        /// <returns><c>ActionResult</c> with appropriate code</returns>
        /// <response code="200">Item removed from the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _unitOfWork.Repository<User>().DeleteByIdAsync(id);
            return Ok();
        }

        /// <summary>
        /// Get all user data from the database by Id (user Id retrieved fron JWT token).
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with end user credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code</returns>
        /// <response code="200">Returns authenticated user data.</response>
        /// <response code="404">Item not found in the database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            string id = _account.GetAccountId(this.User);
            User result = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get the total number of users in the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with end user credentials.
        /// </remarks>
        /// <returns><c>IActionResult</c> HTTP response with HTTP code and user count as int in body.</returns>
        /// <response code="200">Returns number of users in the database.</response>
        [HttpGet("Count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> GetUserCount()
        {
            AppTotalUsers totalUserRow = await _unitOfWork.Repository<AppTotalUsers>().GetByIdAsync("UserCount");
            return Ok(totalUserRow.UserRecordCount);
        }

        /// <summary>
        /// Create new user in the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with end user credentials. Id of the user to be created
        /// is retrieved from the JWT token of the currenly logged in user as the Cognito user Id and
        /// the User Id stored in the DB must match.
        /// </remarks>
        /// <param name="userDto">DTO representation of a user entry for user creation.</param>
        /// <returns><c>Task/<IActionResult/></c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="409">Item already exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] CreateUserDetailsDTO userDto)
        {
            string id = _account.GetAccountId(this.User);

            var exists = await _unitOfWork.Repository<Reward>().GetByIdAsync(id);
            if (exists != null)
            {
                return Conflict();
            }

            ICollection<Reward> rewards = (ICollection<Reward>) await _unitOfWork.Repository<Reward>().GetAllAsync();
            User user = _userService.CreateUser(rewards, userDto, id);

            await _unitOfWork.Repository<User>().InsertAsync(user);
            return CreatedAtAction("Get", user);
        }

        /// <summary>
        /// Replaces current user data with that provided in the request's HTTP body.
        /// </summary>
        /// <remarks>
        /// User must be logged in with end user credentials to execute. Partial user information can be sent.
        /// The function replaces any data that is send in the request body and ignores any fields that are not present.
        /// </remarks>
        /// <param name="dto">Partial representation of user object with fields that can be manipulated by this request.</param>
        /// <returns></returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Patch([FromBody] PatchUserDetailsDto dto)
        {
            var id = _account.GetAccountId(this.User);

            var user = new User()
            {
                Id = id,
                First_name = dto.First_name,
                Last_name = dto.Last_name,
                Email = dto.Email,
                Level = dto.Level,
                LivingSpace = dto.LivingSpace,
                NumPeopleHousehold = dto.NumPeopleHousehold,
                CarMPG = dto.CarMPG,
                ShareData = dto.ShareData,
                Country = dto.Country
            };

            await _unitOfWork.Repository<User>().UpdateAsync(user);
            return Ok();
        }

        /// <summary>
        /// Uupdates a complete user reward with data provided in the request's HTTP body (user Id retrieved from JWT token).
        /// </summary>
        /// <remarks>
        /// User must be logged in with end user credentials to execute. The full userReward unit of information must be sent.
        /// This includes all of the challenge infomration that is not being updated. The function basically removes the user
        /// reward and replaces it.
        /// </remarks>
        /// <param name="model"></param>
        /// <returns><c>Task ActionResult </c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="404">Item not found in database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPut("Rewards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] UserReward model)
        {
            var id = _account.GetAccountId(this.User);
            User user = _unitOfWork.Repository<User>().GetByIdAsync(id).Result;

            if (user != null)
            {
                _userService.UpdateUserReward(user, model);
                await _unitOfWork.Repository<User>().UpdateAsync(user);
                return Ok();
            }
            return NotFound("User not found.");
        }
    }
}