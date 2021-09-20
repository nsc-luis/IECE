using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly DateTime fechayhora = DateTime.UtcNow;
        private readonly IConfiguration _configuration;
        private readonly string superSecreto = "9453bbb3-6aa9-4d3d-826d-85cf343ce59f";

        public object JwTRegistredClaimName { get; private set; }

        public UsuarioController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._configuration = configuration;
            this.context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Usuario>> Get()
        {
            try
            {
                return Ok(context.Usuario.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // METODO QUE CONSTRUYE EL TOKEN
        private IActionResult BuildToken(UserInfo usuario)
        {
            var infoSesion = (from u in context.Usuario
                         join mu in context.Ministro_Usuario
                         on u.Id equals mu.mu_aspNetUsers_Id
                         join pem in context.Personal_Ministerial
                         on mu.mu_pem_Id_Pastor equals pem.pem_Id_Ministro
                         join s in context.Sector
                         on pem.pem_Id_Ministro equals s.pem_Id_Pastor
                         join d in context.Distrito
                         on s.dis_Id_Distrito equals d.dis_Id_Distrito
                         where u.Email == usuario.Email
                         select new
                         {
                             Id = u.Id,
                             Email = u.Email,
                             PasswordHash = u.PasswordHash,
                             mu_pem_Id_Pastor = mu.mu_pem_Id_Pastor,
                             mu_permiso = mu.mu_permiso,
                             pem_Nombre = pem.pem_Nombre,
                             pem_Grado_Ministerial = pem.pem_Grado_Ministerial,
                             pem_Foto_Ministro = pem.pem_Foto_Ministro,
                             pem_email_Personal = pem.pem_email_Personal,
                             pem_Cel1 = pem.pem_Cel1,
                             pem_Cel2 = pem.pem_Cel2,
                             dis_Id_Distrito = d.dis_Id_Distrito,
                             dis_Tipo_Distrito = d.dis_Tipo_Distrito,
                             dis_Numero = d.dis_Numero,
                             dis_Alias = d.dis_Alias,
                             sec_Id_Sector = s.sec_Id_Sector,
                             sec_Tipo_Sector = s.sec_Tipo_Sector,
                             sec_Numero = s.sec_Numero,
                             sec_Alias = s.sec_Alias
                         }).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Llave_super_secreta"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("E38shaGPv6xDgUHN4BffduCQx5fXSMRhyEY2r5tD"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(2);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "iece-tpr.ddns.net",
                audience: "iece",
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return Ok(new
            {
                status = "success",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiration,
                message = "Credenciales correctas! Cargando aplicacion...",
                infoSesion = infoSesion
            });
        }

        [HttpPost]
        [Route("Create")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CrearUsuario([FromBody] UserInfo usuario)
        {
            if (usuario.superSecreto != superSecreto)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = "Error!. La clave 'super secreto' es incorrecta, el proceso se ha cancelado."
                    });
            }
            if (ModelState.IsValid)
            {
                var user = new Usuario { UserName = usuario.Email, Email = usuario.Email };
                var result = await _userManager.CreateAsync(user, usuario.Password);
                if (result.Succeeded)
                {
                    // BuildToken(usuario);
                    return Ok(
                        new
                        {
                            status = "success",
                            mensaje = "Usuario creado satisfactoriamente.",
                            nvoUsuario = user
                        });
                }
                else
                {
                    return Ok(
                        new
                        {
                            status = "error",
                            mensaje = result.Errors
                        });
                }
            }
            else
            {
                return Ok(ModelState);
            }
        }

        [HttpPost]
        [Route("Login")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Login([FromBody] UserInfo usuario)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return BuildToken(usuario);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: Credenciales incorrectas! Vuelva a intentar.");
                    return Ok(new
                    {
                        status = "error",
                        message = "Error: Credenciales incorrectas! Vuelva a intentar."
                    });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ingrese los campos requeridos.");
                return Ok(new
                {
                    status = "error",
                    message = "Ingrese los campos requeridos."
                });
            }
        }

        // GET: api/Usuario/5
        //[HttpGet("{id}")]
        //[EnableCors("AllowOrigin")]
        //public IActionResult Get(int id)
        //{
        //    Usuario usuario = new Usuario();
        //    try
        //    {
        //        usuario = context.Usuario.FirstOrDefault(usu => usu.usu_Id_Usuario == id);
        //        return Ok(usuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // POST: api/Usuario
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Usuario usuario)
        {
            try
            {
                context.Usuario.Add(usuario);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
