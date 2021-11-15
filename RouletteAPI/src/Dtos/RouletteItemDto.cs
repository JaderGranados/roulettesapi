using System;
using System.Text.Json.Serialization;

namespace RoulettesAPI.Dtos
{
    public class RouletteItemDto
    {
        [JsonPropertyName("roulette_id")]
        public string RouletteId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("last_turn_number")]
        public int? LastTurnNumber { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDate { get; set; }
    }
}