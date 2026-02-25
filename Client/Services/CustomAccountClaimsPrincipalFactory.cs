using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;
using System.Text.Json;

namespace TradeUp.Client.Services
{
    public class CustomAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public CustomAccountClaimsPrincipalFactory(
        IAccessTokenProviderAccessor accessor)
        : base(accessor)
        {
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);

            if (user.Identity is ClaimsIdentity identity)
            {
                var roleClaims = identity.FindAll("role").ToList();

                foreach (var claim in roleClaims)
                {
                    identity.RemoveClaim(claim);

                    var roles = JsonSerializer.Deserialize<string[]>(claim.Value);

                    if (roles == null)
                        continue;

                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
            }

            return user;
        }
    }
}
