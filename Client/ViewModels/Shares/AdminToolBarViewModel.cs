using TradeUp.Client.Models.Components;
using TradeUp.Client.Services;

namespace TradeUp.Client.ViewModels.Shares
{
    public class AdminToolBarViewModel: BaseViewModel
    {
        public List<ToolBarButton> ToolBarItems {
            get
            {
                if(ToolBarService is null || ToolBarService.Buttons is null)
                {
                    return new List<ToolBarButton>();
                }

                return ToolBarService?.Buttons!;
            }
        } 

        private bool forceShowToolBar = false;

        public bool IsToolBarVisible
        {
            get
            {
                return  ToolBarItems.Count > 0 || forceShowToolBar;
            }
        }

        public override void Initialize()
        {
            ToolBarService?.AddListener(this);
            OnPropertyChanged(nameof(ToolBarItems));
        }

        public override void ServiceHasChanged()
        {
            OnPropertyChanged(nameof(ToolBarItems));
        }

        public void OnToolBarItemClicked(ToolBarButton btn)
        {
            if(btn.OnClick != null)
            {
                btn.OnClick();
            }
        }
    }
}
