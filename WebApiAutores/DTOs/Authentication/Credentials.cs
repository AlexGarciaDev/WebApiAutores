using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs.Authentication
{
    public class Credentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
