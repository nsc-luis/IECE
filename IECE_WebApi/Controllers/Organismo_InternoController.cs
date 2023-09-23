using DocumentFormat.OpenXml.Office2010.Excel;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Organismo_InternoController : ControllerBase
    {
        private readonly AppDbContext context;

        public Organismo_InternoController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/<Organismo_InternoController>
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult Get()
        {
            try
            {
                var organismosInternos = context.Organismo_Interno.ToList();
                return Ok(new
                {
                    status = "success",
                    organismosInternos
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // GET api/<Organismo_InternoController>/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            try
            {
                var organismoInterno = context.Organismo_Interno.FirstOrDefault(oi => oi.org_Id == id);
                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // POST api/<Organismo_InternoController>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Organismo_Interno organismoInterno)
        {
            try
            {
                context.Organismo_Interno.Add(organismoInterno);
                context.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // PUT api/<Organismo_InternoController>/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put(int id, [FromBody] Organismo_Interno organismoInterno)
        {
            try
            {
                organismoInterno.org_Id = id;
                organismoInterno.org_Fecha_Captura = fechayhora;
                context.Organismo_Interno.Update(organismoInterno);
                context.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // DELETE api/<Organismo_InternoController>/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var organismoInterno = context.Organismo_Interno.FirstOrDefault(oi => oi.org_Id == id);
                context.Organismo_Interno.Remove(organismoInterno);
                context.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }
    }
}
