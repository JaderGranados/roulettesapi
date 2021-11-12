using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoulettesAPI.Dtos
{
    public class LoginDto
    {
        [JsonPropertyName("user_name")]
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
    }
}