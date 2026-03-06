namespace TradeUp.Server.Models
{
    public class DrawData
    {
        public string Id { get; set; } = new Guid().ToString();
        public string DrawContextId { get; set; } = string.Empty;
        public string RawId { get; set; } = string.Empty;
        public string DataKey { get; set; } = string.Empty;
        public string DataValue { get; set; } = string.Empty;
    }
}
