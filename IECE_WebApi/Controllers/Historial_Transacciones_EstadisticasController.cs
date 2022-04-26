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
        [HttpGet("{per_Id_Persona}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int per_Id_Persona)
        {
            try
            {
                var query = (from hte in context.Historial_Transacciones_Estadisticas
                             join sec in context.Sector on hte.sec_Sector_Id equals sec.sec_Id_Sector
                             join dis in context.Distrito on hte.dis_Distrito_Id equals dis.dis_Id_Distrito
                             join cte in context.Codigo_Transacciones_Estadisticas on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                             where hte.per_Persona_Id == per_Id_Persona
                             select new
                             {
                                 hte.hte_Id_Transaccion,
                                 hte.hte_Fecha_Transaccion,
                                 hte.hte_Comentario,
                                 cte.ct_Categoria,
                                 cte.ct_Tipo,   
                                 cte.ct_Subtipo,
                                 sec.sec_Id_Sector,
                                 sec.sec_Alias,
                                 dis.dis_Tipo_Distrito,
                                 dis.dis_Numero,
                                 dis.dis_Alias
                             }).ToList();
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
