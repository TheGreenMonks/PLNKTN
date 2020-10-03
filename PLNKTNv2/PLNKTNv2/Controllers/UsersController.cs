using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Authentication;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using PLNKTNv2.Persistence;
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

            var exists = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (exists != null)
            {
                return Conflict();
            }

            ICollection<Reward> rewards = (ICollection<Reward>)await _unitOfWork.Repository<Reward>().GetAllAsync();
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
        /// Updates a / multiple user reward(s) with data provided in the request's HTTP body (user Id retrieved from JWT token).
        /// </summary>
        /// <remarks>
        /// User must be logged in with end user credentials to execute. Partial objects are allowed. Refer to object
        /// schemas for required structure.
        /// </remarks>
        /// <param name="userRewards">A Collection of UserRewards to be updated</param>
        /// <returns><c>Task ActionResult </c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="404">Item not found in database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPut("Rewards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] ICollection<UserRewardDto> userRewards)
        {
            var id = _account.GetAccountId(this.User);
            User user = _unitOfWork.Repository<User>().GetByIdAsync(id).Result;

            if (user != null)
            {
                _userService.UpdateUserRewards(user, userRewards);
                await _unitOfWork.Repository<User>().UpdateAsync(user);
                return Ok();
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Calculate User Reward and Challenge completion for all users in the DB.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// For every user the call will check all reward's challenges to see if they are complete by checking the user's
        /// submitted ecological measurements against the challenge's rule condition for completion.  Once all challenges
        /// for a reward are checked (6 per reward) the reward itself will be checked to see if it is complete. A reward
        /// is complete when all its challenges are themselves complete. The call will do this for every challenge in every
        /// reward in every user in the database.
        /// </remarks>
        /// <returns><c>Task/<IActionResult/></c> HTTP response with HTTP code.</returns>
        /// <response code="200">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPost("CalculateUserRewardCompletion")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post()
        {
            List<User> users = (List<User>)await _unitOfWork.Repository<User>().GetAllAsync();
            LambdaLogger.Log("**** All Users retrieved from DB ****");

            _userService.CalculateUserRewardCompletion(users);
            LambdaLogger.Log("**** All User's challenge and reward status' have bene calculated ****");

            await _unitOfWork.Repository<User>().UpdateAllAsync(users);
            LambdaLogger.Log("**** All modified User data saved to DB ****");

            return Ok();
        }

        /// <summary>
        /// Calculate User Reward and Challenge completion for the specified user.
        /// </summary>
        /// <remarks>
        /// User must be logged in with end user credentials to execute.
        /// Will check all reward's and challenges to see if they are complete by checking the user's
        /// submitted ecological measurements against the challenge's rule condition for completion.  Once all challenges
        /// for a reward are checked (6 per reward) the reward itself will be checked to see if it is complete. A reward
        /// is complete when all its challenges are themselves complete. The call will do this for every challenge in every
        /// reward in every user in the database.
        /// </remarks>
        /// <returns><c>Task ActionResult </c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="404">Item not found in database.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpPost("CalculateMyRewardCompletion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalculateMyRewardCompletion()
        {
            var id = _account.GetAccountId(this.User);
            User user = _unitOfWork.Repository<User>().GetByIdAsync(id).Result;

            if (user != null)
            {
                _userService.CalculateMyRewardCompletion(user);
                await _unitOfWork.Repository<User>().UpdateAsync(user);
                return Ok();
            }
            return NotFound("User not found.");
        }
    }
}