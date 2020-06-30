using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.Models;
using PLNKTNv2.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTNv2.Controllers
{
    /// <summary>
    /// The RewardRegionsController holds methods to retrieve and manipulate data held about the
    /// reward regions in the database on AWS.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token before they will execute.
    /// </remarks>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RewardRegionsController : Controller
    {
        private readonly IRewardRegionService _rewardRegionService;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor to create Controller with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and the generic repository.</param>
        /// <param name="rewardRegionService">Provides business logic for processing data related to reward regions.</param>
        public RewardRegionsController(IUnitOfWork unitOfWork, IRewardRegionService rewardRegionService)
        {
            _unitOfWork = unitOfWork;
            _rewardRegionService = rewardRegionService;
        }

        /// <summary>
        /// Remove a reward region from the database.
        /// </summary>
        /// <remarks>
        /// Special administration priviledges required to execute this function.
        /// </remarks>
        /// <param name="id">The <c>string</c> id of the reward region where the proejct is to be removed.</param>
        /// <param name="project">The specific <c>Project</c> instance to be removed for a region.</param>
        /// <returns><c>ActionResult</c> with appropriate code</returns>
        /// <response code="200">Item removed from the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("Projects")]
        public async Task<IActionResult> Delete(string id, [FromBody] Project project)
        {
            var region = await _unitOfWork.Repository<RewardRegion>().GetByIdAsync(id);
            _rewardRegionService.DeleteProject(region, project);
            await _unitOfWork.Repository<RewardRegion>().UpdateAsync(region);
            return Ok();
        }

        /// <summary>
        /// Get all reward regions and their data from the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns list of reward region data.</response>
        /// <response code="400">Poorly formed request.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RewardRegion>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var regions = await _unitOfWork.Repository<RewardRegion>().GetAllAsync();
            return Ok(regions);
        }

        /// <summary>
        /// Get a reward region from the database by Id.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <param name="id">The <c>string</c> id of the reward region to retrieve.</param>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns reward region data.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RewardRegion))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _unitOfWork.Repository<RewardRegion>().GetByIdAsync(id);
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
        /// Create new reward region in the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// </remarks>
        /// <param name="region">Reward region entry for reward region creation.</param>
        /// <returns><c>IActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="409">Item already exists.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RewardRegion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RewardRegion region)
        {
            RewardRegion exists = await _unitOfWork.Repository<RewardRegion>().GetByIdAsync(region.Region_name);
            if (exists != null)
            {
                return Conflict();
            }

            await _unitOfWork.Repository<RewardRegion>().InsertAsync(region);
            return CreatedAtAction("Get", new { region_name = region.Region_name }, region);
        }

        /// <summary>
        /// Replaces whole reward region data with that provided in the request's HTTP body.
        /// </summary>
        /// <remarks>
        /// User must be logged in with special administrative credentials to execute. Full reward region information must be sent.
        /// </remarks>
        /// <param name="region">Representation of reward region object with fields that can be manipulated by this request.</param>
        /// <returns><c>ActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found in the database.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] RewardRegion region)
        {
            RewardRegion exists = await _unitOfWork.Repository<RewardRegion>().GetByIdAsync(region.Region_name);
            if (exists == null)
            {
                return NotFound();
            }

            await _unitOfWork.Repository<RewardRegion>().UpdateAsync(region);
            return Ok();
        }

        /// <summary>
        /// Replaces a reward region's project data with that provided in the request's HTTP body.
        /// </summary>
        /// <remarks>
        /// User must be logged in with special administrative credentials to execute. Full reward region project information must be sent.
        /// </remarks>
        /// <param name="id">The ID ('region_name') of the reward region where the project is to be updated.</param>
        /// <param name="project">Representation of reward region project object with fields that can be manipulated by this request.</param>
        /// <returns><c>ActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found in the database.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("Projects")]
        public async Task<IActionResult> Put(string id, [FromBody] Project project)
        {
            RewardRegion exists = await _unitOfWork.Repository<RewardRegion>().GetByIdAsync(id);
            if (exists == null)
            {
                return NotFound();
            }

            _rewardRegionService.AddProject(exists, project);
            await _unitOfWork.Repository<RewardRegion>().UpdateAsync(exists);
            return Ok();
        }
    }
}