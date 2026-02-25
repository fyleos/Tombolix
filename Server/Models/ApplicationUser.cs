using Microsoft.AspNetCore.Identity;

namespace TradeUp.Server.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}

