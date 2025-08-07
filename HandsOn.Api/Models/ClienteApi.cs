using System.Text.Json.Serialization;

namespace HandsOn.Api.Models
{
    public class ClienteApi
    {
        [JsonPropertyName("grant_type")]
        public string? Tipo { get; set; }

        [JsonPropertyName("client_id")]
        public string? Id { get; set; }

        [JsonPropertyName("client_secret")]
        public string? Secret { get; set; }
    }
}
