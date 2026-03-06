using TradeUp.Shared.Models;

namespace TradeUp.Server.Models
{
    public class DrawContext
    {
        public string ID { get; set; } = new Guid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
