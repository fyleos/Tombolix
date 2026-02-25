using System.Text.Json.Serialization;

namespace TradeUp.Client.Models
{
    internal class ValidationProblemDetails
    {
        [JsonPropertyName("title")]
        public string? Title { get;  set; }

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("instance")]
        public string? Instance { get; set; }
    }
}