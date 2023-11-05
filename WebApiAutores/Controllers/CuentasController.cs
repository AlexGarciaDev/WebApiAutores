using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs.Authentication;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IDataProtector dataProtector;
        private readonly IServiceHash serviceHash;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtection, IServiceHash serviceHash)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.dataProtector = dataProtection.CreateProtector("TextoUnicoQueSirveComoLlave");
            this.serviceHash = serviceHash;
        }

        [HttpGet("hash/{PlaneText}")]
        public IActionResult BuilderHash(string PlaneText)
        {
            serviceHash.BuilderHash(PlaneText);
            var hash1 = serviceHash.getHash();

            serviceHash.BuilderHash(PlaneText);
            var hash2 = serviceHash.getHash();

            return Ok(new
            {
                PlaneText= PlaneText,
                hash1 = hash1,
                hash2 = hash2
            });
        }

        [HttpGet("encrypt")]
        public IActionResult Get()
        {
            var textoPlano = "Marlon Garcia";
            var textoCifrado = dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado = textoDesencriptado
            });
        }

        [HttpGet("timeEncrypted")]
        public IActionResult encryptTime()
        {
            var dataProtectorByTime = dataProtector.ToTimeLimitedDataProtector();
            var textoPlano = "Marlon Garcia";
            var textoCifrado = dataProtectorByTime.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            var textoDesencriptado = "";

            Thread.Sleep(6000);

            try
            {
                textoDesencriptado = dataProtectorByTime.Unprotect(textoCifrado);
            }
            catch(Exception ex)
            {
                textoDesencriptado+= ex.Message;
            }

            

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado = textoDesencriptado
            });
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
