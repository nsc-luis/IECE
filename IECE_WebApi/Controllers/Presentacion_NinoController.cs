using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Presentacion_NinoController : ControllerBase
    {
        private readonly AppDbContext context;

        public Presentacion_NinoController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/Presentacion_Nino/GetBySector/5
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetBySector(int idSector)
        {
            try
            {
                var presentaciones = (from pdn in context.Presentacion_Nino
                                      join sec in context.Sector
                                      on pdn.sec_Id_Sector equals sec.sec_Id_Sector
                                      join p in context.Persona
                                      on pdn.per_Id_Persona equals p.per_Id_Persona
                                      where pdn.sec_Id_Sector == idSector
                                      select new
                                      {
                                          pdn.pdn_Id_Presentacion,
                                          pdn.per_Id_Persona,
                                          p.per_Nombre,
                                          p.per_Apellido_Paterno,
                                          p.per_Apellido_Materno,
                                          pdn.pdn_Ministro_Oficiante,
                                          pdn.pdn_Fecha_Presentacion,
                                          pdn.sec_Id_Sector
                                      }).ToList();
                return Ok(new
                {
                    status = "success",
                    presentaciones = presentaciones
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

        // GET: api/Presentacion_Nino/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Presentacion_Nino
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Presentacion_Nino/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
