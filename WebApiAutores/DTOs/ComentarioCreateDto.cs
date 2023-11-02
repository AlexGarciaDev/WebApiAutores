using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class ComentarioCreateDto
    {
        [Required]
        public string Contenido { get; set; }
    }
}
