using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class AutorCreateDto
    {
        [Required(ErrorMessage = "El parametro nombre es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "EL parametro nombre no puede contener mas de 5 caracteres")]
        [LetraMayuscula]
        public string Nombre { get; set; }
    }
}
