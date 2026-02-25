using Microsoft.AspNetCore.Identity;

namespace TradeUp.Server.Models
{
    public class ApplicationRole: IdentityRole
    {
    }

    public enum Roles
    {
        SuperAdmin,
        Essentiel,
        Pro,
        Premium
    }
}
