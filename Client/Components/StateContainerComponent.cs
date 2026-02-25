using Microsoft.AspNetCore.Components;
using TradeUp.Client.Services;
using TradeUp.Client.ViewModels.Shares;

namespace TradeUp.Client.Components
{
    public abstract class StateContainerComponent<TViewModel> : ExtendedComponentBase where TViewModel : BaseViewModel
    {
        [Inject] protected TViewModel ViewModel { get; set; } = default!;

        [Inject] protected StateContainerService StateContainerService { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] protected ToolBarService ToolBarService { get; set; }
        [Inject] protected TranslationService T { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IConfiguration AppConfig { get; set; }
        [Inject] protected UserLoggedService UserLoggedService{ get; set; }

        protected string CdnBaseUrl { get; private set; } = string.Empty;

        protected override void OnInitialized()
        {
            CdnBaseUrl = AppConfig["AppSettings:CdnBaseUrl"] ?? string.Empty;

            ViewModel.Component = this;

            if(UserLoggedService is null)
            {
                NotificationService.AddUserErrorNotification("Error ULS_404.");
            }

            ViewModel.UserLoggedService = UserLoggedService;

            StateContainerService.OnStateChanged += OnStateChanged;

            ViewModel.NotificationService = NotificationService;
            ViewModel.ToolBarService = ToolBarService;

            ViewModel.ToolBarService.AddListener(ViewModel);
            ViewModel.NotificationService.AddListener(ViewModel);

            ViewModel.Initialize();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await T.Initialize();

            OnStateChanged();
        }

        private void OnStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            StateContainerService.OnStateChanged -= OnStateChanged;
        }
    }
}
