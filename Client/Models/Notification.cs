namespace TradeUp.Client.Models
{
    public class Notification
    {
        public string Id { get; set; } = new Guid().ToString();

        public string Text { get; set; } = string.Empty;

        public string Class { get
            {
                if (Type == NotificationType.AppInfo)
                {
                    return Severity switch
                    {
                        NotificationSeverity.Info => "text-light",
                        NotificationSeverity.Warning => "text-warning",
                        NotificationSeverity.Error => "text-danger",
                        _ => "text-light",
                    };
                }

                return Severity switch
                {
                    NotificationSeverity.Info => "bg-info border border-success text-light",
                    NotificationSeverity.Warning => "bg-warning border border-warning text-light",
                    NotificationSeverity.Error => "bg-danger border border-danger text-light",
                    _ => "bg-info border border-success text-light",
                };
            }
        }

        public NotificationType Type { get; set; } = NotificationType.UserInfo;

        public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;
    }

    public enum NotificationType
    {
        UserInfo,
        AppInfo
    }

    public enum NotificationSeverity
    {
        Info,
        Warning,
        Error
    }
}
