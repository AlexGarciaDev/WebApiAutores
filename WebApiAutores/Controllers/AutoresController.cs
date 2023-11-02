using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filters;
using WebApiAutores.services;
using WebApiAutores.Services.ImplementServices;

namespace WebApiAutores.Controllers
{
    [Route("api/autores")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("configuraciones")]
        public ActionResult<String> GetCongiguration()
        {
            string result = configuration["APELLIDO"];

            return result;
        }

        [HttpGet]
        [ServiceFilter(typeof(MethodFilterAction))]
        public async Task<ActionResult<List<AutorFindDto>>>  Get()
        {
            var Autor =  await context.Autores.ToListAsync();

            return mapper.Map<List<AutorFindDto>>(Autor);
        }

        [HttpGet("{id:int}",Name ="getAutor")]
        public async Task<ActionResult<AutorDtoConLibros>> Get(int id)
        {
            var autor = await context.Autores.
                Include(autorBD =>autorBD.AutoresLibros).
                ThenInclude(autoLibroDB => autoLibroDB.Libro).
                FirstOrDefaultAsync(x=>x.Id==id);

            if (autor==null)
            {
                return NotFound();
            }

            return mapper.Map<AutorDtoConLibros>(autor);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorCreateDto>>> Get(String nombre)
        {
            var autor = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            if (autor == null)
            {
                return NotFound();
            }



            return mapper.Map<List<AutorCreateDto>>(autor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreateDto autorDto)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre== autorDto.Nombre);

            if (existeAutor)
            {
                return BadRequest("El autor que estas creando ya existe");
            }

            var Autor = mapper.Map<Autor>(autorDto);

            context.Add(Autor);
            await context.SaveChangesAsync();

            var autorResponse = mapper.Map<AutorFindDto>(Autor);

            return CreatedAtRoute("getAutor",new
            {
                id = Autor.Id
            }, autorResponse);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(AutorCreateDto autor, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id== id);

            if (!existe) {
                return NotFound();
            }

            var Autor = mapper.Map<Autor>(autor);
            Autor.Id= id;

            context.Update(Autor);
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Autor()
            {
                Id = id
            }  );

            await context.SaveChangesAsync();
            return Ok();

        }
    }
}
