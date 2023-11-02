using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs.Authentication
{
    public class EditAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
