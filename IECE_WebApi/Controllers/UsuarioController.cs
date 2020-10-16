using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext context;

        public UsuarioController(AppDbContext context)
        {
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

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Usuario usuario = new Usuario(); 
            try
            {
                usuario = context.Usuario.FirstOrDefault(usu => usu.usu_Id_Usuario == id);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Usuario usuario)
        {
            if(usuario.usu_Id_Usuario == id)
            {
                context.Entry(usuario).State = EntityState.Modified;
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Delete(int id)
        {
            Usuario usuario = new Usuario();
            usuario = context.Usuario.FirstOrDefault(usu => usu.usu_Id_Usuario == id);
            if (usuario != null)
            {
                context.Usuario.Remove(usuario);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
