using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TradeUp.Server.Models;

namespace TradeUp.Server.Services
{
    public class LicenceService
    {
        private readonly HttpClient _http;
        private readonly UserContextService userContextService; 
        private readonly UserManager<ApplicationUser> _userManager;
        private DateTime? lastRefresh;

        private bool getLicenceFromServer = false;

        private List<LicenceDto> _licences = new();

        public LicenceService( UserContextService userContext, UserManager<ApplicationUser> userManager) 
        {
            _http = new HttpClient();
            userContextService = userContext;
            _userManager = userManager;

            RefreshLicenceList();
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var userId = userContextService.GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await _userManager.FindByIdAsync(userId);
        }

        private void RefreshLicenceList()
        {
            if(lastRefresh != null && (DateTime.Now - lastRefresh).Value.Hours < 8)
            {
                return;
            }

            if (!getLicenceFromServer)
            {
                var json = """
                    [
                      {
                        "ExpiryDate": "2026-02-27T23:59:59",
                        "CompagnyName": "mg-software",
                        "LicenceQty": 1
                      }
                    ]
                    """;
                _licences = System.Text.Json.JsonSerializer.Deserialize<List<LicenceDto>>(json) ?? new List<LicenceDto>();
                lastRefresh = DateTime.Now;
                return;
            }

            string query = "url to licence server";
            _http.GetFromJsonAsync<List<LicenceDto>>(query).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    _licences = task.Result;
                    lastRefresh = DateTime.Now;
                }
            });
        }

        public async Task<bool> IsLicenceValidAsync(string licenceKey)
        {
            //var user = Environment.UserName;
            var connectedUser = await GetCurrentUserAsync();

            if(connectedUser == null) 
            {
                return false; 
            }

            string compagnyId = "mg-software";//connectedUser?.CompagnyId ?? "mg-software";

            return LicenceCheck(compagnyId);
        }

        private bool LicenceCheck(string compagnyId)
        {
            LicenceDto? compagnyLicence = _licences.FirstOrDefault(l => l.CompagnyName?.Equals(compagnyId, StringComparison.OrdinalIgnoreCase) == true);

            if (compagnyLicence == null)
            {
                return false;
            }

            if (compagnyLicence.ExpiryDate != null && compagnyLicence.ExpiryDate < DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}
