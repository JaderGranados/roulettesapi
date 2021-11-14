using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RoulettesAPI.Types;

namespace RoulettesAPI.Dtos
{
    public class CreateRouletteBetDto
    {
        [JsonPropertyName("roulette_id")]
        public string RouletteId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("amount")]
        [Required(ErrorMessage = "La cantidad a apostar es requerida")]
        public decimal Amount { get; set; }

        [JsonPropertyName("value")]
        [Required(ErrorMessage = "Debe especificar el número al que le va a apostar")]
        public int Value { get; set; }

        [JsonPropertyName("bet_type_id")]
        [Required(ErrorMessage = "Debe especificar si apostará por un número o apostará por un color")]
        public BetType BetTypeId { get; set; }
    }
}