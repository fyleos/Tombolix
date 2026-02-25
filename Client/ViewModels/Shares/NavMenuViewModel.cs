using TradeUp.Client.Services;

namespace TradeUp.Client.ViewModels.Shares
{
    public class NavMenuViewModel: BaseViewModel
    {
        private bool collapseNavMenu = true;

        public string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        public NavMenuViewModel()
        {
        }
    }
}
