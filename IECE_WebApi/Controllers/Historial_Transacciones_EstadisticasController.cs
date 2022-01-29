using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
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
    public class Historial_Transacciones_EstadisticasController : ControllerBase
    {
        private readonly AppDbContext context;

        public Historial_Transacciones_EstadisticasController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;
        // GET: api/Historial_Transacciones_Estadisticas
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Historial_Transacciones_Estadisticas/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int per_Id_Persona)
        {
            try
            {
                var query = (from hte in context.Historial_Transacciones_Estadisticas
                             where hte.per_Persona_Id == per_Id_Persona
                             select hte).ToList();
                return Ok(new
                {
                    status = "success",
                    info = query
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

        // POST: api/Historial_Transacciones_Estadisticas
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Historial_Transacciones_Estadisticas/5
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
