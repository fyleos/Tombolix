using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using TradeUp.Client.ViewModels.Shares;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Pages
{
    public class UsersViewModel : BaseViewModel
    {
        private string _userId = string.Empty;
        public List<UserDto>? Users;
        public Dictionary<string, bool> UserRoles = new();

        private readonly NavigationManager Navigation;
        private readonly AuthenticationStateProvider AuthenticationStateProvider;

        private readonly HttpClient Http;

        public UsersViewModel(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigation)
        {
            Http = httpClient;
            AuthenticationStateProvider = authenticationStateProvider;
            Navigation = navigation;
        }

        public void NavigateToUpdateUser(string id) => Navigation.NavigateTo($"user/{id}");
        public bool IsUserIdMatchConnectedId(string userId)
        {
            return userId == _userId;
        }

        public async override void Initialize()
        {
            if (!UserLoggedService.IsUserLogged())
            {
                return;
            }

            base.Initialize();
            var usersQuery = BuildQueryApiV1String("users");
            Users = await Http.GetFromJsonAsync<List<UserDto>>(usersQuery);
            if (Users is not null)
            {
                foreach (var user in Users)
                {
                    string request = $"{API_V1_BASE_ROUTE}/users/issuperadmin/{user.Id}";
                    bool tmp_result = await Http.GetFromJsonAsync<bool>(request);
                    UserRoles.TryAdd(user.Id, tmp_result);
                }
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var authUser = authState.User;

            if (authUser is not null && authUser.Identity is not null && authUser.Identity.IsAuthenticated)
            {
                _userId = (authUser.FindFirst(c => c.Type == "sub")?.Value) ?? "Utilisateur non authentifié";
            }
            else
            {
                _userId = "Utilisateur non authentifié";
            }

            ToolBarService?.AddListener(this);
            ToolBarService?.SetReloadButton();

            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(UserRoles));
        }
    }
}
