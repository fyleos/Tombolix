using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("current")]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new UserDto
                {
                    Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Email = User.Identity.Name,
                    Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                });
            }
            return Unauthorized();
        }
    }
}
