using TradeUp.Client.Models;

namespace TradeUp.Client.ViewModels.Shares
{
    public class NotificationViewModel: BaseViewModel, IDisposable
    {
        public int ActiveNotificationIndex { get; private set; }
        public bool HasAppNotifications
        {
            get
            {
                return (NotificationService?.AppNotifications.Count ?? 0) > 0;
            }
        }

        public bool HasUserNotifications
        {
            get
            {
                return (NotificationService?.UserNotifications.Count ?? 0) > 0;
            }
        }

        public List<Notification> UserNotifications
        {
            get
            {
                return NotificationService?.UserNotifications ?? new List<Notification>();
            }
        }

        public List<Notification> AppNotifications
        {
            get
            {
                //TODO get from db
                return NotificationService?.AppNotifications ?? new List<Notification>();
            }
        }

        private Timer? _timer;

        public override void Initialize()
        {
            NotificationService?.AddListener(this);
            ServiceHasChanged();

            _timer = new Timer(_ =>
            {
                Task.Run(() =>
                {
                    ActiveNotificationIndex = (ActiveNotificationIndex + 1) % AppNotifications.Count;
                    OnPropertyChanged(nameof(ActiveNotificationIndex));
                });
            }, null, 3000, 3000);
        }

        public override void ServiceHasChanged()
        {
            OnPropertyChanged(nameof(UserNotifications));
            OnPropertyChanged(nameof(AppNotifications));
        }

        public void DismissNotification(string id)
        {
            NotificationService?.RemoveNotification(id);

            ServiceHasChanged();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
