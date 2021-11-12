using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoulettesAPI.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [JsonPropertyName("names")]
        public string Names { get; set; }

        [Required]
        [JsonPropertyName("last_names")]
        public string LastNames { get; set; }

        [Required]
        [JsonPropertyName("user_name")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("birthdate")]
        public DateTime Birthdate { get; set; }
    }
}