using System.Net.Http.Json;
using System.Reflection.Metadata;
using TradeUp.Client.Pages.Frontend;
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

        public void SetCurrentDrawContext(DrawContextDTO context)
        {
            CurrentDraw = context;
            Task.Run(async () => await FeedContextDatasAsync(CurrentDraw));
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

            string itemsQuery = $"{API_V1_BASE_ROUTE}/draw/context/items/{context.ID}";
            context.DrawnItems = await _httpClient.GetFromJsonAsync<List<string>>(itemsQuery) ?? new List<string>();

            string resultsQuery = $"{API_V1_BASE_ROUTE}/draw/context/results/{context.ID}";
            context.Results = await _httpClient.GetFromJsonAsync<List<ResultDTO>>(resultsQuery) ?? new List<ResultDTO>();

            string headersQuery = $"{API_V1_BASE_ROUTE}/draw/context/headers/{context.ID}";
            context.DrawInfos = await _httpClient.GetFromJsonAsync<TombolaData>(headersQuery) ?? new TombolaData();

            Console.WriteLine($"Data count: {context.DrawnItemsDatas.Count}");
            Console.WriteLine($"Items count: {context.DrawnItems.Count}");
            Console.WriteLine($"result count: {context.Results.Count}");
            Console.WriteLine($"header count: {context.DrawInfos.Details.Length}");

            return;
        }

        private TombolaData? DefineDataHeaders(List<TombolaData> drawnItemsDatas)
        {
            foreach (var info in drawnItemsDatas) 
            {
            }
            return new TombolaData();

        }

        internal DrawContextDTO NewDraw()
        {
            string tmp_name = $"draft-{DateTime.Now:yyyyMMdd-HHmmss}";

            var ctx = new DrawContextDTO()
            {
                Name = tmp_name,
                DrawnItems = new(),
                DrawnItemsDatas = null,
                Results = new List<ResultDTO>()
            };

            CurrentDraw = ctx;
            return CurrentDraw;
        }
    }
}
