using System.Collections.ObjectModel;
using TradeUp.Client.Services;
using TradeUp.Client.ViewModels.Shares;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Pages
{
    public class FeaturesViewModel: BaseViewModel
    {
        public FeatureDto? UpdatedFeature;

        private ObservableCollection<FeatureDto>? _features;
        public ObservableCollection<FeatureDto> Features
        {
            get
            {
                return _features;
            }
            private set
            {
                _features = value;
            }
        }

        private FeatureManagerService _featureManagerService { get; set; }
        private readonly StateContainerService _stateContainerService;

        public FeaturesViewModel(FeatureManagerService featureManagerService, StateContainerService stateContainerService)
        {
            _stateContainerService = stateContainerService;
            _featureManagerService = featureManagerService;
            Task.Run(async () => await RefreshFeatures());
        }
        public async Task RefreshFeatures()
        {
            Features = await _featureManagerService.GetFeaturesAsync();
            _stateContainerService.ShouldRefresh = true;
        }

        public void AddFeature()
        {
            UpdatedFeature = new()
            {
                Id = -1,
                Name = "Feature name",
                ShortDescription = "Short desc",
                EnabledRoles = Array.Empty<string>()
            };

            _stateContainerService.ShouldRefresh = true;
        }

        public async Task SaveFeature()
        {
            if(UpdatedFeature is null || _featureManagerService is null)
            {
                return;
            }
            if(UpdatedFeature.Id == -1)
            {
                var result = await _featureManagerService.AddFeatureAsync(UpdatedFeature);

                if (result)
                {
                    UpdatedFeature = null;
                    RefreshFeatures();
                }
            }
            else
            {
                var result = await _featureManagerService.UpdateFeatureAsync(UpdatedFeature);

                if (result)
                {
                    UpdatedFeature = null;
                    RefreshFeatures();
                }
            }
        }

        public void UpdateFeature(int id)
        {
            var item = Features.FirstOrDefault(f => f.Id == id);
            if(item is null)
            {
                return;
            }

            UpdatedFeature = new FeatureDto() 
            {
                Id = item.Id,
                Name = item.Name,
                ShortDescription = item.ShortDescription,
                EnabledRoles = item.EnabledRoles
            };

            _stateContainerService.ShouldRefresh = true;
        }

        public void NavigateToUpdateFeature(string feature)
        {
            //navigate to update user page
        }
    }
}
