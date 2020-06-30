using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic;
using PLNKTNv2.BusinessLogic.Authentication;
using PLNKTNv2.Models;
using PLNKTNv2.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    /// <summary>
    /// The Collective Ecological Footprint Controller holds methods to retrieve and manipulate data held about user's EFs
    /// in the database on AWS.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token before they will execute.
    /// </remarks>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectiveEFController : ControllerBase
    {
        private readonly ICollectiveEfService _collectiveEfService;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor to create CollectiveEFController with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and Repositories.</param>
        /// <param name="collectiveEfService">Provides business logic for processing data related to Collective Ecological Footprints.</param>
        public CollectiveEFController(
            IUnitOfWork unitOfWork,
            ICollectiveEfService collectiveEfService
            )
        {
            _unitOfWork = unitOfWork;
            _collectiveEfService = collectiveEfService;
        }

        /// <summary>
        /// Get all CollectiveEFs and their data from the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns list of CollectiveEFs data.</response>
        /// <response code="400">Poorly formed request.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CollectiveEF>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<CollectiveEF> collectiveEFs = await _unitOfWork.Repository<CollectiveEF>().GetAllAsync();
            return Ok(collectiveEFs);
        }

        /// <summary>
        /// Get a CollectiveEF from the database by date.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns CollectiveEF data.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CollectiveEF))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            CollectiveEF collectiveEF = await _unitOfWork.Repository<CollectiveEF>().GetByIdAsync(date.ToString());

            if (collectiveEF != null)
            {
                return Ok(collectiveEF);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create new CollectiveEF in the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// </remarks>
        /// <param name="date">The date and time the CollectiveEF is to be calculated for.</param>
        /// <returns><c>IActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="409">Item already exists.</response>
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CollectiveEF))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{date}")]
        public async Task<IActionResult> Post(DateTime date)
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            CollectiveEF collectiveEf = _collectiveEfService.GenerateCollectiveEF(date, users);

            await _unitOfWork.Repository<CollectiveEF>().InsertAsync(collectiveEf);
            return CreatedAtAction("Get", new { date }, collectiveEf);
        }
    }
}