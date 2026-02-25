using TradeUp.Client.Models;

namespace TradeUp.Client.Services
{
    public class NotificationService: BaseService
    {
        private readonly TranslationService T;

        public NotificationService(TranslationService translationService)
        {
            T = translationService;
        }

        private List<Notification> Notifications { get; set; } = new List<Notification>();

        #region addNotificationMethods
        public void AddUserInfoNotification(string text)
        {
            var notification = CreateNotification(text, NotificationType.UserInfo, NotificationSeverity.Info);
            AddNotification(notification);
        }

        public void AddUserWarningNotification(string text)
        {
            var notification = CreateNotification(text, NotificationType.UserInfo, NotificationSeverity.Warning);
            AddNotification(notification);
        }
        public void AddUserErrorNotification(string text)
        {
            var notification = CreateNotification(text, NotificationType.UserInfo, NotificationSeverity.Error);
            AddNotification(notification);
        }

        public void AddSystemInfoNotification(string text)
        {
            var notification = CreateNotification(text, NotificationType.AppInfo, NotificationSeverity.Info);
            AddNotification(notification);
        }

        public void AddSystemWarningNotification(string text)
        {
            var notification = CreateNotification(text, NotificationType.AppInfo, NotificationSeverity.Warning);
            AddNotification(notification);
        }

        public void AddSystemErrorNotification(string text)
        {
            var notification = CreateNotification(text, NotificationType.AppInfo, NotificationSeverity.Error);
            AddNotification(notification);
        }
        #endregion

        public List<Notification> UserNotifications
        {
            get
            {
                return Notifications.Where(n => n.Type == NotificationType.UserInfo).ToList();
            }
        }

        public List<Notification> AppNotifications
        {
            get
            {
                return Notifications.Where(n => n.Type == NotificationType.AppInfo).ToList();
            }
        }

        private Notification CreateNotification(string text,NotificationType type, NotificationSeverity severity )
        {
            string tmp_text = T.TEXT(text);

            return new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Text = tmp_text,
                Type = type,
                Severity = severity
            };
        }

        public void AddNotification(Notification notification)
        {
            Notifications.Add(notification);
            NotifyListeners();
        }

        public void RemoveNotification(string notificationId) 
        {
            Notifications.RemoveAll(n => n.Id == notificationId);
            NotifyListeners();
        }
    }
}
