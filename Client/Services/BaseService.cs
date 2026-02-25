namespace TradeUp.Client.Services
{
    public class BaseService
    {
        protected List<IServiceListener> listeners = new List<IServiceListener>();

        protected const string API_V1_BASE_ROUTE = "api/v1";

        public void AddListener(IServiceListener listenner)
        {
            if (!listeners.Contains(listenner))
            {
                listeners.Add(listenner);
                NotifyListeners();
            }
        }

        protected void NotifyListeners()
        {
            foreach (var listener in listeners)
            {
                listener.ServiceHasChanged();
            }
        }
    }
}
