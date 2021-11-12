using System.Text.Json.Serialization;

namespace RoulettesAPI.Dtos
{
    public class DefaultResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}