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
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaisController : ControllerBase
    {
        private readonly AppDbContext context;

        public PaisController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Pais
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Pais>> Get()
        {
            try
            {
                return Ok(context.Pais.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Pais/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public Pais Get(int id)
        {
            var pais = context.Pais.FirstOrDefault(p => p.pais_Id_Pais == id);
            return pais;
        }

        // GET: api/Pais/GetEstadoByPais/Mex
        [Route("[action]/{pais_Nombre_Corto}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetEstadoByPais(string pais_Nombre_Corto)
        {
            try
            {
                var query = (from pais in context.Pais
                             join est in context.Estado
                             on pais.pais_Nombre_Corto equals est.est_Pais
                             where pais.pais_Nombre_Corto == pais_Nombre_Corto
                             orderby est.est_Nombre ascending
                             select new
                             {
                                 est_Nombre_Corto = est.est_Nombre_Corto,
                                 est_Pais = est.est_Pais,
                                 est_Nombre = est.est_Nombre
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

        // POST: api/Pais
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pais/5
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
