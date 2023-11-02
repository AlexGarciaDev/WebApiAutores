using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entidades
{
    public class Autor
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="El parametro nombre es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage ="EL parametro nombre no puede contener mas de 5 caracteres")]
        [LetraMayuscula]
        public string Nombre { get; set; }

        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
