using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Tools.AutoMappper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreateDto, Autor>();
            CreateMap<Autor, AutorFindDto>();

            CreateMap<Autor, AutorDtoConLibros>()
            .ForMember(autorDto => autorDto.Libros, opciones => opciones.MapFrom(MapAutorDtoLibros));

            CreateMap<LibroCreateDto, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDto>();
            CreateMap<Libro, LibroDtoConAutores>()
                .ForMember(libroDto => libroDto.Autores, opciones => opciones.MapFrom(MapLibroDtoAutores));
            CreateMap<LibroPatchDto, Libro>().ReverseMap();

            CreateMap<ComentarioCreateDto, Comentarios>();
            CreateMap<Comentarios, ComentarioFindDto>();
        }

        private List<LibroDto> MapAutorDtoLibros(Autor autor, AutorFindDto autorDto)
        {
            var resultado = new List<LibroDto>();

            if (autor.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDto()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }

        private List<AutorFindDto> MapLibroDtoAutores(Libro libro, LibroDto libroDto)
        {
            var resultado = new List<AutorFindDto>();

            if (libro.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorFindDto()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreateDto librocreatedto,Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if (librocreatedto.AutoresId==null)
            {
                return resultado;
            }

            foreach (var autorId in librocreatedto.AutoresId)
            {
                resultado.Add(new AutorLibro()
                {
                    AutorId = autorId
                });
            }

            return resultado;
        }

    }
}
