namespace TradeUp.Shared.Models
{
    public class DrawContextDTO
    {
        public string ID { get; set; } = new Guid().ToString();
        public string Name { get; set; } = string.Empty;
        public List<string> DrawnItems { get; set; } = new List<string>();
        public TombolaData? DrawInfos { get; set; }
        public List<TombolaData>? DrawnItemsDatas { get; set; } 
        public List<ResultDTO> Results { get; set; } = new List<ResultDTO>();

        public DrawContextDTO Clone()
        {
            DrawContextDTO clone = new DrawContextDTO
            {
                ID = ID,
                Name = Name,
                DrawInfos = DrawInfos,
                DrawnItems = [.. DrawnItems],
                DrawnItemsDatas = [.. DrawnItemsDatas!],
                Results = [.. Results]
            };

            return clone;
        }
    }
}
