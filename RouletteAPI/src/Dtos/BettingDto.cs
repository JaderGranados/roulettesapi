using System.Text.Json.Serialization;

namespace RoulettesAPI.Dtos
{
    public class BettingDto
    {
        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("is_winner")]
        public bool IsWinner { get; set; }

        [JsonPropertyName("bet_type")]
        public string BetType { get; set; }
    }
}