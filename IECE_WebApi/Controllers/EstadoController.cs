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
    public class EstadoController : ControllerBase
    {
        private readonly AppDbContext context;

        public EstadoController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Estado
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IEnumerable<Estado> Get()
        {
            return context.Estado.ToList();
        }

        // GET: api/Estado/5
        [HttpGet("{id}")]
        public Estado Get(int id)
        {
            var estado = context.Estado.FirstOrDefault(e => e.est_Id_Estado == id);
            return estado;
        }

        // GET: api/Estado/GetEstadoByIdPais/151
        [Route("[action]/{pais_Id_Pais}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetEstadoByIdPais(int pais_Id_Pais)
        {
            try
            {
                var query = (from est in context.Estado
                             join pais in context.Pais
                             on est.pais_Id_Pais equals pais.pais_Id_Pais
                             where est.pais_Id_Pais == pais_Id_Pais
                             orderby est.est_Nombre ascending
                             select new
                             {
                                 est_Id_Estado = est.est_Id_Estado,
                                 est_Nombre_Corto = est.est_Nombre_Corto,
                                 est_Pais = est.est_Pais,
                                 est_Nombre = est.est_Nombre,
                                 pais_Id_Pais = est.pais_Id_Pais
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        estados = query
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        status = false,
                        message = ex.Message
                    });
            }
        }

        // POST: api/Estado
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Estado/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public void Delete(int id)
        {
        }
    }
}
