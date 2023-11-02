using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [Route("api/libros/{libroId:int}/comentarios")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioFindDto>>> Get(int libroId)
        {
            var exiteLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!exiteLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(comentariosDB =>comentariosDB.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioFindDto>>(comentarios);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId ,[FromBody] ComentarioCreateDto comentarioDto)
        {
            var claims = HttpContext.User.Claims.Where(claim => claim.Type=="email").FirstOrDefault();

            var email = claims.Value;

            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var exiteLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!exiteLibro)
            {
                return NotFound();
            }

            var Comentario = mapper.Map<Comentarios>(comentarioDto);

            Comentario.LibroId= libroId;
            Comentario.UsuarioId= usuarioId;
            context.Add(Comentario);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(int libroId, int id, [FromBody] ComentarioCreateDto comentarioDto)
        {
            var exiteLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!exiteLibro)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(x => x.Id == id);

            if (!existeComentario)
            {
                return NotFound();
            }

            var Comentario = mapper.Map<Comentarios>(comentarioDto);
            Comentario.Id = id;
            Comentario.LibroId = libroId;

            context.Update(Comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
