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

        public List<DrawContextDTO> UserDraws { get; internal set; }
        public DrawContextDTO? CurrentDraw { get; internal set; }

        public async Task SetCurrentDrawContextAsync(DrawContextDTO context)
        {
            CurrentDraw = context;
            await FeedContextDatasAsync(CurrentDraw);

            return;
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

        public async void RemoveContextAsync(DrawContextDTO context)
        {
            if (context == null || !_userLoggedService.IsUserLogged())
            {
                return;
            }

            var query = $"{API_V1_BASE_ROUTE}/draw/context/{context.ID}";
            var response = await _httpClient.DeleteAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                var message = response.Content.ReadAsStringAsync();
                _notificationService.AddUserErrorNotification($"Error: {message}");
                return;
            }

            return;
        }

        internal async Task RefreshUserDrawsAsync()
        {
            var query = $"{API_V1_BASE_ROUTE}/draw/contexts/me";
            var draws = await _httpClient.GetFromJsonAsync<List<DrawContextDTO>>(query);

            if( draws is not null)
            {
                UserDraws = draws;
            }
            else
                UserDraws = new List<DrawContextDTO>();

            return;
        }

        private async Task FeedContextDatasAsync(DrawContextDTO context)
        {
            string datasQuery = $"{API_V1_BASE_ROUTE}/draw/context/datas/{context.ID}";
            context.DrawnItemsDatas = await _httpClient.GetFromJsonAsync<List<TombolaData>>(datasQuery) ?? new List<TombolaData>();
            Console.WriteLine($"Data count: {context.DrawnItemsDatas.Count}");

            string itemsQuery = $"{API_V1_BASE_ROUTE}/draw/context/items/{context.ID}";
            context.DrawnItems = await _httpClient.GetFromJsonAsync<List<string>>(itemsQuery) ?? new List<string>();
            Console.WriteLine($"Items count: {context.DrawnItems.Count}");

            string resultsQuery = $"{API_V1_BASE_ROUTE}/draw/context/results/{context.ID}";
            context.Results = await _httpClient.GetFromJsonAsync<List<ResultDTO>>(resultsQuery) ?? new List<ResultDTO>();
            Console.WriteLine($"result count: {context.Results.Count}");

            string headersQuery = $"{API_V1_BASE_ROUTE}/draw/context/headers/{context.ID}";
            context.DrawInfos = await _httpClient.GetFromJsonAsync<TombolaData>(headersQuery) ?? new TombolaData();
            Console.WriteLine($"header count: {context.DrawInfos.Details.Length}");
            return;
        }

        internal DrawContextDTO NewDraw()
        {
            string tmp_name = $"draft-{DateTime.Now:yyyyMMdd-HHmmss}";

            string? tmp_id = null;

            Task.Run(async () =>
            {
                tmp_id = await GetNewIdAsync();
            });

            if ( string.IsNullOrEmpty(tmp_id)) 
            {
                tmp_id = new Guid().ToString();
            }

            var ctx = new DrawContextDTO()
            {
                Name = tmp_name,
                DrawnItems = new(),
                DrawnItemsDatas = null,
                Results = new List<ResultDTO>()
            };

            CurrentDraw = ctx;

            if(_userLoggedService.IsUserLogged())
            {
                SaveContextAsync(CurrentDraw);
            }

            return CurrentDraw;
        }

        private async Task<string> GetNewIdAsync()
        {
            string tmp_id = string.Empty;

            string query = $"{API_V1_BASE_ROUTE}/draw/newId";
            tmp_id = await _httpClient.GetFromJsonAsync<string>(query) ?? string.Empty;

            return tmp_id;
        }
    }
}
