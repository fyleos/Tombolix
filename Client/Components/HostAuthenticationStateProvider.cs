using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using TradeUp.Shared.Models;

namespace TradeUp.Client.Components
{
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _client;
        private static readonly TimeSpan UserCacheRefreshInterval = TimeSpan.FromSeconds(60);
        private DateTimeOffset _userLastCheck = DateTimeOffset.MinValue;
        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

        public HostAuthenticationStateProvider(HttpClient client)
        {
            _client = client;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return new AuthenticationState(await GetUser());
        }

        private async Task<ClaimsPrincipal> GetUser()
        {
            if (DateTimeOffset.Now < _userLastCheck + UserCacheRefreshInterval)
                return _cachedUser;

            try
            {
                var userInfo = await _client.GetFromJsonAsync<UserDto>("api/user/current");

                if (userInfo != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                    new Claim(ClaimTypes.Name, userInfo.Email),
                    new Claim(ClaimTypes.Email, userInfo.Email)
                };

                    claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    var id = new ClaimsIdentity(claims, "Server Identity");
                    _cachedUser = new ClaimsPrincipal(id);
                }
                else
                {
                    _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch
            {
                _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            _userLastCheck = DateTimeOffset.Now;
            return _cachedUser;
        }
    }
}
