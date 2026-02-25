using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Net.Http.Json;
using TradeUp.Client.Models;
using TradeUp.Client.Services;

namespace TradeUp.Client.ViewModels.Shares
{
    public class BaseViewModel : INotifyPropertyChanged, IServiceListener
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ExtendedComponentBase? Component { get; set; }

        public ToolBarService? ToolBarService { get; set; }

        public NotificationService? NotificationService { get; set; }

        public UserLoggedService UserLoggedService { get; set; }

        protected readonly HttpClient? _httpClient;

        protected const string API_V1_BASE_ROUTE = "api/v1";

        public BaseViewModel() { }

        public BaseViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public virtual void Initialize()
        {
            ToolBarService?.CleanItems();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (Component is not null)
            {
                Component.PublicInvokeAsync(Component.PublicStateHasChanged);
            }
        }

        protected string BuildQueryApiV1String(string baseUrl, Dictionary<string, string>? parameters = null)
        {
            if(baseUrl.StartsWith("/"))
            {
                baseUrl = baseUrl[1..];
            }

            var baseRoute = $"{API_V1_BASE_ROUTE}/{baseUrl}";

            if (parameters == null || parameters.Count == 0)
                return baseRoute;

            var query = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            return $"{baseRoute}?{query}";
        }

        protected virtual void Refresh()
        {
        }

        protected void LogInfo(string message)
        {
            string log = $"[INFO] [{GetType()}] {DateTime.Now}: {message}";
            Log(log);
        }

        protected void LogWarning(string message)
        {
            string log = $"[WARNING] [{GetType()}] {DateTime.Now}: {message}";
            Log(log);
        }

        protected void LogError(string message)
        {
            string log = $"[ERROR] [{GetType()}] {DateTime.Now}: {message}";
            Log(log);
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        protected async Task<T?> GetJsonFromApiAsync<T>(string url) where T : class
        {
            if(_httpClient == null)
            {
                LogError("HttpClient is not initialized.");
                return default(T);
            }

            var usersQuery = BuildQueryApiV1String(url);
            T? result = await _httpClient.GetFromJsonAsync<T>(usersQuery);

            return result;
            //var Users = await Http.GetFromJsonAsync<T>(usersQuery);
        }

        public virtual void ServiceHasChanged()
        {
        }

        protected async Task<T?> HandleRequest<T>(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return default(T);

            var response = await _httpClient.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                LogInfo("Successful response from API.");
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                NotificationService?.AddSystemErrorNotification($"Access denied: {errorMsg}");
            }
            else
            {
                var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
                {
                    NotificationService?.AddSystemErrorNotification($"{problem?.Title}: {problem?.Detail}");
                }
            }

            return default(T);
        }
    }
    public class ExtendedComponentBase : ComponentBase
    {
        public Task PublicInvokeAsync(Action workItem)
        {
            return InvokeAsync(workItem);
        }

        public void PublicStateHasChanged()
        {
            StateHasChanged();
        }

        public virtual void Dispose() { }
    }
}
