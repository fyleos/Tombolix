namespace TradeUp.Client.Services
{
    public class AssetService
    {
        private readonly string _version;

        public AssetService(IConfiguration config)
        {
            // On récupère la version ou on met un timestamp par défaut
            _version = config["AppSettings:Version"] ?? DateTime.Now.Ticks.ToString();
        }

        public string GetVersionedUrl(string path)
        {
            string separator = path.Contains("?") ? "&" : "?";
            return $"{path}{separator}v={_version}";
        }
    }
}
