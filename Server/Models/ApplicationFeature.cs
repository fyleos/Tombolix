namespace TradeUp.Server.Models
{
    public class ApplicationFeature
    {
        public int Id { get; set; }
        public string Name { get; set; } = "No name";
        public string ShortDescription { get; set; } = "No short description";
        public string[] EnabledGroups { get; set; } = Array.Empty<string>();
    }
}
