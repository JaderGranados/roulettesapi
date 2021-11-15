using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoulettesAPI.Dtos
{
    public class CloseRouletteDto
    {
        [JsonPropertyName("roulette_id")]
        public string RoueletteId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("opening_date")]
        public DateTime OpeningDate { get; set; }

        [JsonPropertyName("closing_date")]
        public DateTime ClosingDate { get; set; }

        [JsonPropertyName("winner_number")]
        public int WinnerNumber { get; set; }

        [JsonPropertyName("bets")]
        public IEnumerable<BettingDto> Bets { get; set; }
    }
}