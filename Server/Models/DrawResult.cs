using TradeUp.Shared.Models;

namespace TradeUp.Server.Models
{
    public class DrawResult
    {
        public string Id { get; set; } = new Guid().ToString();
        public string ContextId { get; set; } = string.Empty;
        public int TirageIndex { get; set; }
        public string DataRawId { get; set; } = string.Empty;
    }
}
