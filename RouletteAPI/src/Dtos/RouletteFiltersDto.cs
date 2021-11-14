using System.Text.Json.Serialization;
using RoulettesAPI.Types;

namespace RoulettesAPI.Dtos
{
    public class RouletteFiltersDto
    {
        [JsonPropertyName("roulette_id")]
        public string RouletteId { get; set; }

        [JsonPropertyName("status_id")]
        public RouletteStatus StatusId { get; set; }

        [JsonPropertyName("items_per_page")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }
}