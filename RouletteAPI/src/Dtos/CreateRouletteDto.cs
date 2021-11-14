using System.Text.Json.Serialization;
using RoulettesAPI.Types;

namespace RoulettesAPI.Dtos
{
    public class CreateRouletteDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status_id")]
        public RouletteStatus Status { get; set; } = RouletteStatus.CLOSED;
    }
}