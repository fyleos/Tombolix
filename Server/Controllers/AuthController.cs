using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradeUp.Server.Models;
using TradeUp.Server.Services;

namespace TradeUp.Server.Controllers
{
    public class AuthController : BaseApiV1Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(LicenceService licenceService, SignInManager<ApplicationUser> signInManager, UserContextService userContextService) : base(licenceService, userContextService)
        {
            _signInManager = signInManager;
        }


        [HttpGet("logout")]
        public async Task<ActionResult> Get()
        {
            //if (IsLicenceNotValid())
            //{
            //    return Problem(
            //         detail: "Votre licence annuelle est arrivée à échéance le 01/02/2026.",
            //         instance: HttpContext.Request.Path,
            //         statusCode: StatusCodes.Status402PaymentRequired,
            //         title: "Invalid Licence"
            //     );
            //}

            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
