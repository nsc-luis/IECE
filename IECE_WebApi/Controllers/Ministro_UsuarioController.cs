using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Ministro_UsuarioController : ControllerBase
    {
        private readonly AppDbContext context;

        public Ministro_UsuarioController(AppDbContext context)
        {
            this.context = context;
        }

        // POST: api/Ministro_Usuario
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Ministro_Usuario ministro_usuario)
        {
            try
            {
                context.Ministro_Usuario.Add(ministro_usuario);
                context.SaveChanges();
                return Ok(
                    new
                    {
                        status = "success",
                        registro = ministro_usuario
                    });
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = ex.Message
                    });
            }
        }
    }
}