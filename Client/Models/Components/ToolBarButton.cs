namespace TradeUp.Client.Models.Components
{
    public class ToolBarButton
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool Disabled { get; set; } = false;
        public bool HideText { get; set; } = false;
        public bool HideIcon { get; set; } = false;
        public string Tooltip { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public OnClickAction OnClick { get; set; } = null!;

        public delegate void OnClickAction();
    }
}
