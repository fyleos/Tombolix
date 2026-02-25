using TradeUp.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace TradeUp.Client.Services
{
    public class UserOptionsService: BaseService
    {

        private StateContainerService StateContainerService { get; set; }
        protected HttpClient HttpClient { get; set; }
        public UserLoggedService UserLoggedService { get;private set; }

        public SharedUserOption _userOption;
        public SharedUserOption UserOption
        {
            get
            {
                return _userOption;
            }
            private set
            {
                _userOption = value;
                UpdateSharedUserOptionAsync(UserLoggedService.UserId, UserOption);
            }
        }

        public UserOptionsService(HttpClient http,UserLoggedService userLogService, StateContainerService stateContainerService)
        {
            HttpClient = http;
            UserLoggedService = userLogService;
            StateContainerService = stateContainerService;
            StateContainerService.OnStateChanged += Refresh;

            using var _ = OnInitializedAsync();
        }

        public void Refresh()
        {
            if(UserOption is null)
            {
                TryGetUserOption();
            }
        }

        protected async Task OnInitializedAsync()
        {
            Console.WriteLine($"OnInitializedAsync {nameof(UserOptionsService)}");
            if (UserLoggedService.UserId is not null)
            {
                Console.WriteLine($"Get User Options");
                UserOption = await GetSharedUserOptionAsync(UserLoggedService.UserId);
            }
            Console.WriteLine($"User Options should ok");
        }

        private async Task<SharedUserOption> GetSharedUserOptionAsync(string userId)
        {
            if(UserLoggedService is null || !UserLoggedService.IsUserLogged())
            {
                return null;
            }

            string request = $"{API_V1_BASE_ROUTE}/users/getoptions/{userId}";
            Console.WriteLine($"getoption with request: {request}");
            return await HttpClient.GetFromJsonAsync<SharedUserOption>(request);
        }

        public async Task UpdateSharedUserOptionAsync(string userId,SharedUserOption sharedUserOption)
        {
            var response = await HttpClient.PutAsJsonAsync($"{API_V1_BASE_ROUTE}/users/setoptions/{userId}", sharedUserOption);
            Console.WriteLine($"Response status for PutUserOptions: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update SharedUserOption");
            }
        }

        internal async void TryGetUserOption()
        {
            if(UserLoggedService is not null && UserLoggedService.UserId is not null)
            {
                var tmp_userOption = await GetSharedUserOptionAsync(UserLoggedService.UserId);
                if (tmp_userOption is not null)
                {
                    UserOption = tmp_userOption;
                    StateContainerService.ShouldRefresh = true;
                }
            }
        }

        public void UpdateUserOptionItem(string optionName,string optionValue)
        {
            PropertyInfo propertyInfo = UserOption.GetType().GetProperty(optionName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(UserOption, optionValue);
                Console.WriteLine($"Property {optionName} updated");

                UpdateSharedUserOptionAsync(UserLoggedService.UserId, UserOption);
                StateContainerService.ShouldRefresh = true;
                return;
            }

            Console.WriteLine($"Property {optionName} not found in {nameof(UserOptionsService)}");
        }
    }
}
