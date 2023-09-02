using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Agrupaciones_MusicalesController : ControllerBase
    {
        private readonly AppDbContext context;
        public Agrupaciones_MusicalesController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/<Agrupaciones_Musicales>
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult Get()
        {
            try
            {
                var agrupaciones = (from am in context.Agrupaciones_Musicales
                                    join per in context.Persona on am.am_Id_Persona_Director equals per.per_Id_Persona
                                    join sec in context.Sector on am.sec_Id_Sector equals sec.sec_Id_Sector
                                    join dis in context.Distrito on am.dis_Id_Distrito equals dis.dis_Id_Distrito
                                    orderby dis.dis_Numero ascending
                                    select new
                                    {
                                        am.am_Id_Agrupacion,
                                        am.am_Nombre,
                                        am.am_Fecha_Creacion,
                                        per.per_Id_Persona,
                                        per.per_Nombre,
                                        per.per_Apellido_Paterno,
                                        sec.sec_Id_Sector,
                                        sec.sec_Alias,
                                        dis.dis_Id_Distrito,
                                        dis.dis_Numero,
                                        dis.dis_Tipo_Distrito,
                                        dis.dis_Alias
                                    }).ToList();
                return Ok(new
                {
                    status = "success",
                    agrupaciones
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // GET api/<Agrupaciones_Musicales>/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Agrupaciones_Musicales>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Agrupaciones_Musicales>/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Agrupaciones_Musicales>/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public void Delete(int id)
        {
        }
    }
}
