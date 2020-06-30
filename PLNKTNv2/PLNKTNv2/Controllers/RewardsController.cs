using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.Models;
using PLNKTNv2.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    /// <summary>
    /// The Rewards Controller holds methods to retrieve and manipulate data held about rewards in the database on AWS.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token and special credentials before they will execute.
    /// </remarks>
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor to create Controller with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and the generic repository.</param>
        /// <param name="userService">Provides business logic for processing data related to users.</param>
        public RewardsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        /// <summary>
        /// Remove a reward and its associated data from the database.
        /// </summary>
        /// <remarks>
        /// Calling this function will also remove all UserRewards related to this reward from all users in the DB.
        /// Special administration priviledges required to execute this function.
        /// </remarks>
        /// <param name="id">The <c>string</c> id of the reward to be removed.</param>
        /// <returns><c>ActionResult</c> with appropriate code</returns>
        /// <response code="200">Item removed from the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _unitOfWork.Repository<Reward>().DeleteByIdAsync(id);
            IEnumerable<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            _userService.DeleteUserRewardFromAllUsers(id, users);
            return Ok();
        }

        /// <summary>
        /// Get all rewards from the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns rewards data.</response>
        /// <response code="400">Poorly formed request.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Reward>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Reward> result = await _unitOfWork.Repository<Reward>().GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get a reward from the database by Id.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns reward data.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Reward))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            Reward result = await _unitOfWork.Repository<Reward>().GetByIdAsync(id);

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
        /// Create new reward in the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// </remarks>
        /// <param name="reward">Reward entry for reward creation.</param>
        /// <returns><c>Task/<IActionResult/></c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="409">Item already exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Reward))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] Reward reward)
        {
            var exists = await _unitOfWork.Repository<Reward>().GetByIdAsync(reward.Id);
            if (exists != null)
            {
                return Conflict();
            }

            IEnumerable<User> users = await _unitOfWork.Repository<User>().GetAllAsync();

            _userService.AddUserRewardToAllUsers(reward, users);

            // TODO: This may be better implemented using DyDB's new transaction functionality.
            await _unitOfWork.Repository<User>().UpdateAllAsync(users);
            await _unitOfWork.Repository<Reward>().InsertAsync(reward);

            return CreatedAtAction("Get", new { id = reward.Id }, reward);
        }

        /// <summary>
        /// Replaces reward data with that provided in the request's HTTP body.
        /// </summary>
        /// <remarks>
        /// User must be logged in with special administrative credentials to execute. Full reward information must be sent.
        /// </remarks>
        /// <param name="reward">Representation of reward object with fields that can be manipulated by this request.</param>
        /// <returns><c>ActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found in the database.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] Reward reward)
        {
            var exists = await _unitOfWork.Repository<Reward>().GetByIdAsync(reward.Id);
            if (exists == null)
            {
                return NotFound();
            }

            IEnumerable<User> users = await _unitOfWork.Repository<User>().GetAllAsync();

            _userService.UpdateUserRewardInAllUsers(reward, users);
            await _unitOfWork.Repository<User>().UpdateAllAsync(users);
            await _unitOfWork.Repository<Reward>().UpdateAsync(reward);
            return Ok();
        }
    }
}