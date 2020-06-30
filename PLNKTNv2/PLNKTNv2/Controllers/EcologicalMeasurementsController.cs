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
    /// The Ecological Measurements Controller holds methods to retrieve and manipulate data held about user's EMs
    /// in the database on AWS.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token before they will execute.
    /// </remarks>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EcologicalMeasurementsController : ControllerBase
    {
        private readonly IAccount _account;
        private readonly IEcologicalMeasurementService _ecologicalMeasurementService;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor to create EcologicalMeasurementsController with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and Repositories.</param>
        /// <param name="account">Provides access to authenticated user data.</param>
        /// <param name="ecologicalMeasurementService">Provides business logic for processing data related to Ecological Measurements.</param>
        public EcologicalMeasurementsController(
            IUnitOfWork unitOfWork,
            IAccount account,
            IEcologicalMeasurementService ecologicalMeasurementService
            )
        {
            _unitOfWork = unitOfWork;
            _account = account;
            _ecologicalMeasurementService = ecologicalMeasurementService;
        }

        /// <summary>
        /// Get all ecological measurements for logged in user.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns list of ecological measurements data.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">User not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EcologicalMeasurement>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string id = _account.GetAccountId(this.User);
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (user != null)
            {
                return Ok(user.EcologicalMeasurements);
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Get an ecological measurement for logged in user from the database by date.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with credentials.
        /// </remarks>
        /// <returns><c>ActionResult</c> with appropriate code and data in the body.</returns>
        /// <response code="200">Returns ecological measurement data.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EcologicalMeasurement))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            string id = _account.GetAccountId(this.User);
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (user != null)
            {
                EcologicalMeasurement tempEm = _ecologicalMeasurementService.GetEcologicalMeasurement(user, date);
                if (tempEm != null)
                {
                    return Ok(tempEm);
                }
                return NotFound("Ecological Measurement not found.");
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Create new Ecological Measurement for logged in user in the database.
        /// </summary>
        /// <remarks>
        /// Requires user to be logged in with special administrative credentials.
        /// </remarks>
        /// <param name="dto">Ecological Measurement entry (plus user Id) for Ecological Measurement creation.</param>
        /// <returns><c>IActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="201">Returns newly created item.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="409">Item already exists.</response>
        /// <response code="404">Item not found.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EcologicalMeasurement))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EcologicalMeasurementDTO dto)
        {
            var ecologicalMeasurement = new EcologicalMeasurement
            {
                Date_taken = dto.Date_taken,
                EcologicalFootprint = dto.EcologicalFootprint,
                Transport = dto.Transport,
                Diet = dto.Diet,
                Electronics = dto.Electronics,
                Clothing = dto.Clothing,
                Footwear = dto.Footwear
            };

            string id = _account.GetAccountId(this.User);
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (user != null)
            {
                Status status = _ecologicalMeasurementService.InsertEcologicalMeasurement(user, ecologicalMeasurement);
                if (status.Equals(Status.CREATED_AT))
                {
                    await _unitOfWork.Repository<User>().UpdateAsync(user);
                    return CreatedAtAction("Get", new { date = dto.Date_taken }, ecologicalMeasurement);
                }
                return Conflict("Ecological Measurement already exists.");
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Replaces whole Ecological Measurement data with that provided in the request's HTTP body.
        /// </summary>
        /// <remarks>
        /// User must be logged in with credentials to execute. Full EcologicalMeasurement information must be sent.
        /// </remarks>
        /// <param name="dto">Representation of EcologicalMeasurement object with fields that can be manipulated by this request.</param>
        /// <returns><c>ActionResult</c> HTTP response with HTTP code.</returns>
        /// <response code="200">Item updated in the database successfully.</response>
        /// <response code="400">Poorly formed request.</response>
        /// <response code="404">Item not found in the database.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] EcologicalMeasurementDTO dto)
        {
            var ecologicalMeasurement = new EcologicalMeasurement
            {
                Date_taken = dto.Date_taken,
                EcologicalFootprint = dto.EcologicalFootprint,
                Transport = dto.Transport,
                Diet = dto.Diet,
                Electronics = dto.Electronics,
                Clothing = dto.Clothing,
                Footwear = dto.Footwear
            };

            string id = _account.GetAccountId(this.User);
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (user != null)
            {
                Status status = _ecologicalMeasurementService.UpdateEcologicalMeasurement(user, ecologicalMeasurement);
                if (status.Equals(Status.OK))
                {
                    await _unitOfWork.Repository<User>().UpdateAsync(user);
                    return Ok();
                }
                return NotFound("Ecological Measurement not found.");
            }
            return NotFound("User not found.");
        }
    }
}