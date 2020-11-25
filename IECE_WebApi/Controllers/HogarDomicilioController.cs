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
    public class HogarDomicilioController : ControllerBase
    {

        private readonly AppDbContext context;

        public HogarDomicilioController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/HogarDomicilio
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<HogarDomicilio>> Get()
        {
            try
            {
                return Ok(context.HogarDomicilio.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/HogarDomicilio/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {
            HogarDomicilio hogardomicilio = new HogarDomicilio();
            try
            {
                hogardomicilio = context.HogarDomicilio.FirstOrDefault(hd => hd.hd_Id_Hogar == id);
                return Ok(hogardomicilio);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/HogarDomicilio/GetListaHogares
        [Route("[action]")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetListaHogares(int id)
        {
            try
            {
                var query = (from hp in context.Hogar_Persona
                    join hd in context.HogarDomicilio
                    on hp.hp_Id_Hogar_Persona equals hd.hd_Id_Hogar
                    join e in context.Estado
                    on hd.est_Id_Estado equals e.est_Id_Estado
                    join pais in context.Pais
                    on hd.pais_Id_Pais equals pais.pais_Id_Pais
                    join p in context.Persona
                    on hp.per_Id_Persona equals p.per_Id_Persona
                    where hp.hp_Jerarquia == 1
                    select new
                    {
                        hd_Id_Hogar = hp.hd_Id_Hogar,
                        per_Nombre = p.per_Nombre,
                        per_Apellido_Paterno = p.per_Apellido_Paterno,
                        per_Apellido_Materno = p.per_Apellido_Materno,
                        hd_Calle = hd.hd_Calle,
                        hd_Numero_Exterior = hd.hd_Numero_Exterior,
                        hd_Numero_Interior = hd.hd_Numero_Interior,
                        hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                        hd_Subdivision = hd.hd_Subdivision,
                        hd_Localidad = hd.hd_Localidad,
                        hd_Municipio_Cuidad = hd.hd_Municipio_Cuidad,
                        est_Nombre = e.est_Nombre,
                        pais_Nombre_Corto = pais.pais_Nombre_Corto,
                        hd_Telefono = hd.hd_Telefono
                    }).ToList();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/HogarDomicilio
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] HogarDomicilio hogardomicilio)
        {
            try
            {
                context.HogarDomicilio.Add(hogardomicilio);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/HogarDomicilio/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] HogarDomicilio hogardomicilio)
        {
            if (hogardomicilio.hd_Id_Hogar == id)
            {
                context.Entry(hogardomicilio).State = EntityState.Modified;
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
        public ActionResult Delete(int id)
        {
            HogarDomicilio hogardomicilio = new HogarDomicilio();
            hogardomicilio = context.HogarDomicilio.FirstOrDefault(hd => hd.hd_Id_Hogar == id);
            if (hogardomicilio != null)
            {
                context.HogarDomicilio.Remove(hogardomicilio);
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
