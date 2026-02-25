using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;
using TradeUp.Shared.Models;

namespace TradeUp.Client.Services
{
    public class UsersService
    {
        private readonly HttpClient _httpClient;
        private readonly NotificationService _notificationService;
        private const string API_V1_BASE_ROUTE = "api/v1";

        public UsersService(HttpClient httpClient, NotificationService notificationService)
        {
            _httpClient = httpClient;
            _notificationService = notificationService;
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<UserDto>($"{API_V1_BASE_ROUTE}/users/me");
                return user;
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return null;
            }
        }

        public bool SaveUser(UserDto user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
                return false;
            _httpClient.PutAsJsonAsync($"{API_V1_BASE_ROUTE}/users/{user.Id}", user);

            return true;
        }

        internal async Task<HttpResponseMessage> SaveUserAsync(UserDto user)
        {
            var response = await _httpClient.PutAsJsonAsync($"{API_V1_BASE_ROUTE}/users/{user.Id}", user);

            if(response.IsSuccessStatusCode)
            {
                _notificationService.AddUserInfoNotification("SUCCESS_USER_UPDATE");
            }
            else
            {
                _notificationService.AddUserErrorNotification("ERROR_USER_UPDATE");
            }

            return response;
        }
    }
}
