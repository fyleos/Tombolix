using Microsoft.AspNetCore.Components;
using TradeUp.Client.Models.Components;

namespace TradeUp.Client.Services
{
    public class ToolBarService: BaseService
    {
        public List<ToolBarButton> Buttons = new List<ToolBarButton>();
        private ToolBarButton? reloadButton = null;

        private NavigationManager NavigationManager { get; set; } = null!;

        public ToolBarService(NavigationManager navigationManager)
        {
            NavigationManager = navigationManager;
        }

        public void AddButton(ToolBarButton button)
        {
            var oldBtn = Buttons.FirstOrDefault(b => b.Name == button.Name);

            if (oldBtn is not null)
            {
                Buttons.Remove(oldBtn);
            }

            Buttons.Add(button);
            NotifyListeners();
        }

        public ToolBarButton BtnBuilder(string name, string icon, string tooltip, string @class, ToolBarButton.OnClickAction onClick)
        {
            return new ToolBarButton
            {
                Name = name,
                Icon = icon,
                Tooltip = tooltip,
                Class = @class,
                OnClick = onClick
            };
        }

        public ToolBarButton BtnBuilder(string name, bool hideName, string icon,bool hideIcon ,string tooltip, string @class, ToolBarButton.OnClickAction onClick)
        {
            return new ToolBarButton
            {
                Name = name,
                HideText = hideName,
                Icon = icon,
                HideIcon = hideIcon,
                Tooltip = tooltip,
                Class = @class,
                OnClick = onClick
            };
        }

        public void SetBackButton(string url)
        {
            var backButton = BtnBuilder(
                "Back",
                true,
                "oi oi-arrow-left",
                false,
                "Go back to the previous page",
                "toolbar-button hidden-text show-icon",
                () => NavigationManager.NavigateTo(url)
            );
            AddButton(backButton);
            NotifyListeners();
        }

        public void SetReloadButton()
        {
            if(reloadButton is not null && Buttons.Contains(reloadButton))
            {
                Buttons.Remove(reloadButton);
            }

            reloadButton = BtnBuilder(
                "Reload",
                true,
                "oi oi-reload",
                false,
                "Reload the page",
                "toolbar-button hidden-text show-icon",
                () => {
                    NavigationManager.Refresh(true);
                }
            );
            AddButton(reloadButton);
            NotifyListeners();
        }

        internal void CleanItems()
        {
            Buttons.Clear();
            NotifyListeners();
        }
    }
}
