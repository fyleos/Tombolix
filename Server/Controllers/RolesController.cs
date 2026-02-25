using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradeUp.Server.Services;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Controllers
{
    public class RolesController : BaseApiV1Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager, LicenceService licenceService, UserContextService userContextService) : base(licenceService, userContextService)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetGetRolesList()
        {
            if (!_userContextService.IsUserAuthenticated())
            {
                return Problem(
                    detail: "Vous devez être authentifié pour accéder à cette ressource.",
                    instance: HttpContext.Request.Path,
                    statusCode: StatusCodes.Status401Unauthorized, 
                    title: "Unauthorized"
                );
            }
            
            if (IsLicenceNotValid())
            {
                return Problem(
                     detail: "Votre licence annuelle est arrivée à échéance le 01/02/2026.",
                     instance: HttpContext.Request.Path,
                     statusCode: StatusCodes.Status402PaymentRequired, 
                     title: "Invalid Licence"
                 );
            }

            List<RoleDto> roles =  _roleManager.Roles.Select(role => new RoleDto
            {
                Id = role.Id,
                RoleName = role.Name
            }).ToList();
            
            return Ok(roles);
        }
    }
}
