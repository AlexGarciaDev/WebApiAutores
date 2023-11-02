using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs.Authentication;

namespace WebApiAutores.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseAuthenticacion>> Register(Credentials CredentialsUser)
        {
            var usuario = new IdentityUser
            {
                UserName = CredentialsUser.Email,
                Email= CredentialsUser.Email
            };

            var result = await userManager.CreateAsync(usuario,CredentialsUser.Password);
            if (result.Succeeded)
            {
                return await BuildToken(CredentialsUser);
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseAuthenticacion>> Login(Credentials CredentialsUser)
        {
            var result = await signInManager.PasswordSignInAsync(CredentialsUser.Email,CredentialsUser.Password,isPersistent: false,lockoutOnFailure:false);

            if (result.Succeeded)
            {
                return await BuildToken(CredentialsUser);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }

        }

        private async Task< ResponseAuthenticacion> BuildToken(Credentials CredentialsUser)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",CredentialsUser.Email)
            };

            var usuario = await userManager.FindByEmailAsync(CredentialsUser.Email);
            var claimDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["KeyJWT"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,expires: expiration, signingCredentials:creds);

            return new ResponseAuthenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        [HttpPost("DoAdmin")]
        public async Task<ActionResult<ResponseAuthenticacion>> doAdmin(EditAdminDto editAdmin)
        {
            var usuario = await userManager.FindByEmailAsync(editAdmin.Email);

            await userManager.AddClaimAsync(usuario,new Claim("EsAdmin","1"));

            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult<ResponseAuthenticacion>> removeAdmin(EditAdminDto editAdmin)
        {
            var usuario = await userManager.FindByEmailAsync(editAdmin.Email);

            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "1"));

            return NoContent();
        }
    }
}
