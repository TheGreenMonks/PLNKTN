using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLNKTNv2.BusinessLogic.Authentication;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.Models;
using PLNKTNv2.Persistence;
using System.Threading.Tasks;

namespace PLNKTNv2.Controllers
{
    /// <summary>
    /// The Bin Controller holds methods to retrieve and manipulate data held about trees that have been planted by users.
    /// The 'Bin' is a separate data store used to notify the One Tree Planted organisation how many trees
    /// have been awarded and where. The table will be trawled periodically for trees planted and cleared when
    /// the data is sent to OTP.  This prevents duplicate tree planting requests going to OTP.
    /// </summary>
    /// <remarks>
    /// All functions require the user to be authenticated with JWT token before they will execute.
    /// </remarks>
    [Authorize(Policy = "EndUser")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BinController : Controller
    {
        private readonly IAccount _account;
        private readonly IBinService _binService;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor to create EcologicalMeasurementsController with DI assets.
        /// </summary>
        /// <param name="unitOfWork">Abstraction layer between the controller and DB Context and Repositories.</param>
        /// <param name="account">Provides access to authenticated user data.</param>
        /// <param name="binService">Provides business logic for processing data related to Bin items.</param>
        public BinController(
            IUnitOfWork unitOfWork,
            IAccount account,
            IBinService binService
            )
        {
            _unitOfWork = unitOfWork;
            _account = account;
            _binService = binService;
        }

        // PUT api/values/5
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] Bin bin)
        {
            Bin dbBin = await _unitOfWork.Repository<Bin>().GetByIdAsync(bin.Region_name);
            Status result = _binService.InsertUserTreeToBin(bin.Projects[0], dbBin);
            return Ok();
        }
    }
}