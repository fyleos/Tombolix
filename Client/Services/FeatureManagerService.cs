using System.Collections.ObjectModel;
using System.Net.Http.Json;
using TradeUp.Shared.Models;

namespace TradeUp.Client.Services
{
    public class FeatureManagerService
    {
        //private ObservableCollection<FeatureDto>? _features;
        protected HttpClient HttpClient { get; set; }
        //private StateContainerService StateContainerService { get; set; }
        public FeatureManagerService(StateContainerService stateContainerService,  HttpClient httpClient)
        {
            //StateContainerService = stateContainerService;
            HttpClient = httpClient;
        }

        //public ObservableCollection<FeatureDto> GetFeatures()
        //{
        //    Task.Run( async () => TryGetFeaturesFromServer());

        //    return _features;
        //}

        //public async Task TryGetFeaturesFromServer()
        //{
        //    _features = await GetFeaturesAsync();
        //    StateContainerService.ShouldRefresh = true;

        //    if (_features is null)
        //    {
        //        _features = new ObservableCollection<FeatureDto>();
        //    }
        //}

        public async Task<ObservableCollection<FeatureDto>> GetFeaturesAsync()
        {
            string request = $"api/admin/features";
            return await HttpClient.GetFromJsonAsync<ObservableCollection<FeatureDto>>(request);
        }

        internal async Task<bool> AddFeatureAsync(FeatureDto updatedFeature)
        {
            string request = $"api/admin/features/add";
            var result = await HttpClient.PutAsJsonAsync<FeatureDto>(request, updatedFeature);

            if(!result.IsSuccessStatusCode)
            {
                return false;
            }

            updatedFeature = null;

            return true;
        }

        internal async Task<bool> UpdateFeatureAsync(FeatureDto updatedFeature)
        {
            string request = $"api/admin/features/update";
            var result = await HttpClient.PutAsJsonAsync<FeatureDto>(request, updatedFeature);

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            updatedFeature = null;

            return true;
        }
    }
}
