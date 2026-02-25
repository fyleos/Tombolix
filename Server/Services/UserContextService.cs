using System.Security.Claims;

namespace TradeUp.Server.Services
{
    public class UserContextService
    {
        private IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal == null || principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return null;
            }
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? principal.FindFirst("sub")?.Value;
            return userId;
        }

        public bool IsUserAuthenticated()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
        }
    }
}
