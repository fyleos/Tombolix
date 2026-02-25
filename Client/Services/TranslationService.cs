using System.Text.Json;

namespace TradeUp.Client.Services
{
    public class TranslationService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly AssetService Assets;
        private Dictionary<string, string> _translations = new();
        public string CurrentLanguage { get; private set; } = "fr";

        public TranslationService(IConfiguration config, IHttpClientFactory clientFactory, AssetService assetService)
        {
            _http = clientFactory.CreateClient("Public");
            Assets = assetService;
            _config = config;
        }

        public async Task Initialize()
        {
            try
            {
                if (_translations.Any()) return;

                string lang = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                var supportedLanguages = _config["AppSettings:SupportedLanguages"]?.Split(',') ?? new[] { "en" };
                if (!supportedLanguages.Contains(lang)) lang = "en";

                CurrentLanguage = lang;

                await LoadLanguageAsync(CurrentLanguage);
            }
            catch
            {
            }

        }

        private async Task LoadLanguageAsync(string lang)
        {
            try
            {
                var url = Assets.GetVersionedUrl($"i18n/{lang}.json");
                var json = await _http.GetStringAsync(url);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (data is not null)
                    _translations = data;
            }
            catch 
            {
            }
        }

        public string TEXT(string key)
        {
            if (_translations.TryGetValue(key, out var value))
            {
                return value;
            }

            return key; 
        }
    }
}
