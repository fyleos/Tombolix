using Microsoft.AspNetCore.Components;
using TradeUp.Client.ViewModels.Shares;

namespace TradeUp.Client.Services
{
    public class StateContainerService
    {
        public event Action? OnStateChanged;

        private bool _shouldRefresh;
        public bool ShouldRefresh
        {
            get => _shouldRefresh;
            set
            {
                _shouldRefresh = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnStateChanged?.Invoke();
    }
}
