using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class LibroPatchDto
    {

        [Required(ErrorMessage = "El parametro nombre es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "EL parametro nombre no puede contener mas de 50 caracteres")]
        public string Titulo { get; set; }
        public DateTime fechapulicacion { get; set; }
    }
}
