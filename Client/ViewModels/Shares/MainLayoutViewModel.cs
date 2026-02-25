using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using TradeUp.Client.Services;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Shares
{
    public class MainLayoutViewModel: BaseViewModel, IDisposable
    {
        public ThemeManagerService ThemeManagerService { get;private set; }
        private StateContainerService StateContainerService { get; set; }

        public MainLayoutViewModel(ThemeManagerService themeManagerService, StateContainerService stateContainer)
        {
            StateContainerService = stateContainer;
            StateContainerService.OnStateChanged += Refresh;

            ThemeManagerService = themeManagerService;
        }

        protected override void Refresh()
        {
            OnPropertyChanged(nameof(ClasseTheme));
        }

        public string ClasseTheme
        {
            get
            {
                if (ThemeManagerService is not null)
                {
                    return ThemeManagerService.CurrentTheme;
                }
                else
                {
                    return "No-Theme-Manager";
                }
            }
        }

        public void SwitchTheme()
        {
            ThemeManagerService.SwitchTheme();
            OnPropertyChanged(nameof(ClasseTheme));
        }

        public void Dispose()
        {
            StateContainerService.OnStateChanged -= Refresh;
        }
    }
}
