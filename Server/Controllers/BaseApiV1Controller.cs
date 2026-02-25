using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeUp.Server.Services;

namespace TradeUp.Server.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseApiV1Controller : ControllerBase
    {
        private LicenceService _licenceService;
        protected readonly UserContextService _userContextService;

        public BaseApiV1Controller(LicenceService licenceService, UserContextService userContextService)
        {
            _licenceService = licenceService;
            _userContextService = userContextService;
        }

        protected bool IsLicenceNotValid()
        {
            if(_licenceService is null)
            {
                return true;
            }

            return Task.Run(async () =>await _licenceService.IsLicenceValidAsync("debugLicence") == false).Result;
        }
    }
}
