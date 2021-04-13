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

        private IActionResult BuildToken(UserInfo usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Llave_super_secreta"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "iece-tpr.ddns.net",
                audience: "iece",
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiration
            });
        }

        [HttpPost]
        [Route("Create")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CrearUsuario([FromBody] UserInfo usuario)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuario { UserName = usuario.Email, Email = usuario.Email };
                var result = await _userManager.CreateAsync(user, usuario.Password);
                if (result.Succeeded)
                {
                    return BuildToken(usuario);
                }
                else
                {
                    return BadRequest("User or password invalid");
                }
            }
            else
            {
                return BadRequest(ModelState);
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
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
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

        // PUT: api/Usuario/5
        //[HttpPut("{id}")]
        //[EnableCors("AllowOrigin")]
        //public ActionResult Put(int id, [FromBody] Usuario usuario)
        //{
        //    if (usuario.usu_Id_Usuario == id)
        //    {
        //        context.Entry(usuario).State = EntityState.Modified;
        //        context.SaveChanges();
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //[EnableCors("AllowOrigin")]
        //public IActionResult Delete(int id)
        //{
        //    Usuario usuario = new Usuario();
        //    usuario = context.Usuario.FirstOrDefault(usu => usu.usu_Id_Usuario == id);
        //    if (usuario != null)
        //    {
        //        context.Usuario.Remove(usuario);
        //        context.SaveChanges();
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}
    }
}
