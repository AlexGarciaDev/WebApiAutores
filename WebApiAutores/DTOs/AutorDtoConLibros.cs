namespace WebApiAutores.DTOs
{
    public class AutorDtoConLibros : AutorFindDto
    {

        public List<LibroDto> Libros { get; set; }
    }
}
