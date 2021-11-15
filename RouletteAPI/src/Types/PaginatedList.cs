using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoulettesAPI.Types
{
    public class PaginatedList<T> where T : class
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("items_per_page")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("total_items")]
        public int TotalItems { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; }
    }
}