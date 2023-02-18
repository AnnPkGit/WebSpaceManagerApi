using Microsoft.AspNetCore.Mvc;
using WebSpaceManager.DTOs;
using WebSpaceManager.Entities;
using WebSpaceManager.Helpers;
using WebSpaceManager.Models;

namespace WebSpaceManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractsHelper _contractsHelper;
        private readonly IContractsDevHelper _contractsDevHelper;
        private readonly IAuthoriztionValidator _authoriztionValidator;
        private const string _headersKey = "ApiSecret";
        private const string _headersDevKey = "DevApiSecret";

        public ContractsController(IContractsHelper contractsHelper, IAuthoriztionValidator authoriztionValidator, IContractsDevHelper contractsDevHelper)
        {
            _contractsHelper = contractsHelper ?? throw new NullReferenceException();
            _contractsDevHelper = contractsDevHelper ?? throw new NullReferenceException();
            _authoriztionValidator = authoriztionValidator ?? throw new NullReferenceException();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<GetContractDto>> GetContracts()
        {
            HttpContext.Request.Headers.TryGetValue(_headersKey, out var key);
            if (!_authoriztionValidator.IsValidKey(key))
                return Unauthorized();

            return _contractsHelper.GetContracts();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult PostContract([FromBody] PostContractDto contract)
        {
            HttpContext.Request.Headers.TryGetValue(_headersKey, out var key);
            if (!_authoriztionValidator.IsValidKey(key))
                return Unauthorized();

            var res =_contractsHelper.CreateContract(contract);
            if (!res.IsSuccessful)
            {
                return BadRequest(res.ErrorMessage);
            }
            return Ok(res.Value);
        }

        [HttpGet]
        [Route("dev")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<List<Contract>> GetContractsRaw()
        {
            HttpContext.Request.Headers.TryGetValue(_headersDevKey, out var devKey);
            if (!_authoriztionValidator.IsValidDevKey(devKey))
                return Unauthorized();

            return _contractsDevHelper.GetContractsRaw();
        }
    }
}
