namespace TradeUp.Server.Models
{
    public class DrawItem
    {
        public string Id { get; set; } = new Guid().ToString();
        public string DrawContextId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public override int GetHashCode()
        {
            return Id.GetHashCode() + DrawContextId.GetHashCode() + Name.GetHashCode();
        }
    }
}
