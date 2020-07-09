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
    /// The GrantedRewards Controller holds methods to retrieve and manipulate data held about a user's Granted Rewards
    /// in the database on AWS.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token before they will execute.
    /// </remarks>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class GrantedRewardsController : Controller
    {
        private readonly IAccount _account;
        private readonly string _appTotalTreesPlantedId = "AppTotalTreesPlanted";
        private readonly IBinService _binService;
        private readonly IGrantedRewardService _grantedRewardService;
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// Constructor to create GrantedRewardsController with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and Repositories.</param>
        /// <param name="account">Provides access to authenticated user data.</param>
        /// <param name="grantedRewardService">Provides business logic for processing data related to granted rewards.</param>
        /// <param name="binService">Provides business logic for processing data related to Bin items.</param>
        public GrantedRewardsController(
            IUnitOfWork unitOfWork,
            IAccount account,
            IGrantedRewardService grantedRewardService,
            IBinService binService
            )
        {
            _unitOfWork = unitOfWork;
            _account = account;
            _grantedRewardService = grantedRewardService;
            _binService = binService;
        }

        /// <summary>
        /// Get a user's granted reward from the database by Id.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns granted reward data for logged in user.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserGrantedReward))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{region_name}")]
        public async Task<IActionResult> Get(string region_name)
        {
            string id = _account.GetAccountId(this.User);
            User user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            UserGrantedReward grantedReward = _grantedRewardService.GetUserGrantedReward(user, region_name);

            if (grantedReward != null)
            {
                return Ok(grantedReward);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get all granted rewards for a user from the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns list of granted rewards data.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<UserGrantedReward>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string id = _account.GetAccountId(this.User);
            User user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            ICollection<UserGrantedReward> grantedRewards = _grantedRewardService.GetUserGrantedRewards(user);

            if (grantedRewards.Count > 0)
            {
                return Ok(grantedRewards);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get total number of trees planted by all users in the application.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns object with count of total trees planted.</response>
        /// <response code="400">Poorly formed request.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppTotalTreesPlanted))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("Count")]
        public async Task<IActionResult> GetCount()
        {
            AppTotalTreesPlanted totalTreesPlanted = await _unitOfWork.Repository<AppTotalTreesPlanted>()
                                                                            .GetByIdAsync(_appTotalTreesPlantedId);
            return Ok(totalTreesPlanted);
        }

        /// <summary>
        /// Create new granted reward for logged in user in the database. The granted reward will also be added
        /// to the Bin table ready to be sent to the One Tree Planted organisation.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials. Multiple trees can be planted in
        /// the same region and same project, therefore calling this function multiple times with the same data will have
        /// a knock on effect of planting multiple trees for the user.
        /// </remarks>
        /// <param name="grantedRewardDto">Object with region name and project info to be added for the user.</param>
        /// <returns><c>IActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RewardRegion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserGrantedRewardDto grantedRewardDto)
        {
            string id = _account.GetAccountId(this.User);

            // Get all object froms the DB
            User user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            AppTotalTreesPlanted totalTreesPlanted = await _unitOfWork.Repository<AppTotalTreesPlanted>()
                                                                            .GetByIdAsync(_appTotalTreesPlantedId);
            Bin dbBin = await _unitOfWork.Repository<Bin>().GetByIdAsync(grantedRewardDto.Region_name);

            // Ensure objects exists in DB
            string errorMessage = user == null ? "User not found. " : "";
            errorMessage += dbBin == null ? "Reward Region not found in Bin. " : "";
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return NotFound(errorMessage);
            }

            // Mutate all objects to reflect new state
            _grantedRewardService.InsertUserGrantedReward(user, grantedRewardDto);
            _binService.InsertUserTreeToBin(grantedRewardDto.Project, dbBin);
            totalTreesPlanted.TreesCount++;

            // Persist all changes to the DB
            await _unitOfWork.Repository<AppTotalTreesPlanted>().UpdateAsync(totalTreesPlanted);
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.Repository<Bin>().UpdateAsync(dbBin);

            // Return to the client
            return CreatedAtAction("Get", new { grantedRewardDto.Region_name }, grantedRewardDto.Project);
        }
    }
}