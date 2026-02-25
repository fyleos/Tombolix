using System.Net.Http.Json;
using TradeUp.Client.ViewModels.Shares;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Pages
{
    public class UserEditViewModel: BaseViewModel
    {
        private readonly HttpClient Http;

        public string Id { get; set; } = string.Empty;

        public UserDto? user;
        public List<string>? availableRole;

        public UserEditViewModel(HttpClient httpClient) : base(httpClient)
        {
            Http = httpClient;
        }

        public async Task InitializeAsync(string id)
        {
            Id = id;

            ToolBarService?.SetBackButton("/users");
            ToolBarService?.SetReloadButton();
            await GetUserInfo();

            return;
        }

        private async Task GetUserInfo()
        {
            if (UserLoggedService is null || !UserLoggedService.IsUserLogged())
            {
                NotificationService?.AddUserErrorNotification("ERROR_USER_NOT_LOGGED");
                return;
            }

            if (string.IsNullOrWhiteSpace(Id))
            {
                var query = BuildQueryApiV1String("users/me");
                user = await Http.GetFromJsonAsync<UserDto>(query);
            }
            else
            {
                

                var query = BuildQueryApiV1String($"users/{Id}");
                user = await Http.GetFromJsonAsync<UserDto>(query);
            }

            var roleTypesQuery = BuildQueryApiV1String("users/roletypes");
            availableRole = await Http.GetFromJsonAsync<List<string>>(roleTypesQuery);

            OnPropertyChanged(nameof(user));
            OnPropertyChanged(nameof(availableRole));
        }

        public override async void Initialize()
        {
            base.Initialize();
        }

        public void AddRole(string role)
        {
            if(user is null || string.IsNullOrWhiteSpace(role))             
                return;

            user.Roles.Add(role);

            SaveUser();
        }

        public void SaveUser()
        {
            if (UserLoggedService is null || !UserLoggedService.IsUserLogged())
            {
                NotificationService?.AddUserErrorNotification("ERROR_USER_NOT_LOGGED");
                return;
            }

            if (user is null)             
                return;

            Task.Run(async () => await SaveUserAsync());
            OnPropertyChanged(nameof(user));

            NotificationService?.AddUserInfoNotification("SUCCESS_USER_UPDATE");
        }

        private async Task SaveUserAsync()
        {
            if (user is null)
                return;

            var query = BuildQueryApiV1String($"users/{user.Id}");
            await Http.PutAsJsonAsync(query, user);
        }

        public void UpdateUser(string Id)
        {
            if (UserLoggedService is null || !UserLoggedService.IsUserLogged())
            {
                NotificationService?.AddUserErrorNotification("ERROR_USER_NOT_LOGGED");
                return;
            }

            if (user is null || string.IsNullOrWhiteSpace(Id))
                return;

            if(user.Id != Id)
            {
                NotificationService?.AddUserErrorNotification("ERROR_USER_UPDATE");
                return;
            }

            SaveUser();
        }
    }
}
