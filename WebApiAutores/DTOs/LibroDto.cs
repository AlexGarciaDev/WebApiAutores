using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class LibroDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El parametro nombre es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "EL parametro nombre no puede contener mas de 5 caracteres")]
        public string Titulo { get; set; }

        public DateTime fechapulicacion { get; set; }

    }
}
