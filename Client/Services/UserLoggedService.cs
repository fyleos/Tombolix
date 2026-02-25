using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace TradeUp.Client.Services
{
    public class UserLoggedService
    {
        private const string noUserId = "no_user_logged";
        protected AuthenticationStateProvider AuthStateProvider { get; set; }
        private StateContainerService StateContainerService { get; set; }

        public string? UserId { get; private set; }

        public UserLoggedService(AuthenticationStateProvider authStateProvider, StateContainerService stateContainerService)
        {
            StateContainerService = stateContainerService;
            AuthStateProvider = authStateProvider;
            OnInitializedAsync();
        }

        protected async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var authUser = authState.User;

            if (authUser is not null && authUser.Identity is not null && authUser.Identity.IsAuthenticated)
            {
                var id = authUser.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrWhiteSpace(id))
                {
                    UserId = id;
                    StateContainerService.ShouldRefresh = true;
                }
                else
                {
                    UserId = noUserId;
                }
            }
            else
            {
                UserId = noUserId;
            }
        }

        public bool IsUserLogged()
        {
            if(UserId is null || UserId == noUserId)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
