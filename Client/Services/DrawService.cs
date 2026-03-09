using System.Net.Http.Json;
using TradeUp.Shared.Models;

namespace TradeUp.Client.Services
{
    public class DrawService: BaseService
    {
        private HttpClient _httpClient;
        private UserLoggedService _userLoggedService;
        private NotificationService _notificationService;
        public DrawService(HttpClient httpClient, UserLoggedService userService, NotificationService notificationService    )
        {
            _httpClient = httpClient;
            _userLoggedService = userService;
            _notificationService = notificationService;
        }

        public async void SaveContextAsync(DrawContextDTO context)
        {
            if (context == null || !_userLoggedService.IsUserLogged())
            {
                return;
            }

            var query = $"{API_V1_BASE_ROUTE}/draw/context/save";
            var response = await _httpClient.PostAsJsonAsync(query, context);

            if (!response.IsSuccessStatusCode)
            {
                var message = response.Content.ReadAsStringAsync();
                _notificationService.AddUserErrorNotification($"Error: {message}");
                return;
            }

            return;
        }
    }
}
