using System.Text.Json.Serialization;

namespace HandsOn.Api.Models
{
    public class ClienteToken
    {
        [JsonPropertyName("token_type")]
        public string Tipo { get; set; } = string.Empty;
        
        [JsonPropertyName("expires_in")]
        public int Expiracao { get; set; }
        
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = string.Empty;
    }
}
