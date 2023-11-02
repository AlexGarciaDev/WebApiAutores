using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [Route("api/libros")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDtoConAutores>> Get(int id)
        {
            var existe = await context.Libros.
                Include(libroDB => libroDB.AutoresLibros).
                ThenInclude(autorLibroDB => autorLibroDB.Autor).
                FirstOrDefaultAsync(x=>x.Id==id);


            if (existe != null)
            {
                return mapper.Map<LibroDtoConAutores>(existe);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreateDto librodto)
        {
            if (librodto.AutoresId==null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresId = await context.Autores.Where(autorBd => librodto.AutoresId.Contains(autorBd.Id)).Select(x => x.Id).ToListAsync();

            if (librodto.AutoresId.Count!=autoresId.Count)
            {
                return BadRequest("No se encontrarons a todos los autores");
            }

            var libro = mapper.Map<Libro>(librodto);

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreateDto librodto)
        {
            var libroDB = await context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id==id);

            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = mapper.Map(librodto,libroDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDto> patchDocument)
        {
            if (patchDocument==null)
            {
                return BadRequest();
            }

            var libroDB = await context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            var libroDto = mapper.Map<LibroPatchDto>(libroDB);
            patchDocument.ApplyTo(libroDto, ModelState);

            var esValido = TryValidateModel(libroDto);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDto, libroDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Autor()
            {
                Id = id
            });

            await context.SaveChangesAsync();
            return Ok();

        }

    }
}
