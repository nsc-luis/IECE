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
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        // GET: api/HogarDomicilio/GetByDistrito/5
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByDistrito(int dis_Id_Distrito)
        {
            // HogarDomicilio hogardomicilio = new HogarDomicilio();
            try
            {
                var domicilios = (from hd in context.HogarDomicilio
                    join dis in context.Distrito
                    on hd.dis_Id_Distrito equals dis.dis_Id_Distrito
                    join sec in context.Sector
                    on hd.sec_Id_Sector equals sec.sec_Id_Sector
                    join pais in context.Pais
                    on hd.pais_Id_Pais equals pais.pais_Id_Pais
                    join est in context.Estado
                    on hd.est_Id_Estado equals est.est_Id_Estado
                    where hd.dis_Id_Distrito == dis_Id_Distrito
                    select new
                    {
                        hd_Calle = hd.hd_Calle,
                        hd_Numero_Exterior = hd.hd_Numero_Exterior,
                        hd_Numero_Interior = hd.hd_Numero_Interior,
                        hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                        hd_Subdivision = hd.hd_Subdivision,
                        hd_Localidad = hd.hd_Localidad,
                        hd_Municipio_Ciudad = hd.hd_Municipio_Ciudad,
                        est_Nombre = est.est_Nombre,
                        pais_Nombre_Corto = pais.pais_Nombre_Corto,
                        hd_Telefono = hd.hd_Telefono,
                        dis_Numero = dis.dis_Numero,
                        dis_Alias = dis.dis_Alias,
                        sec_Alias = sec.sec_Alias
                    }).ToList();
                return Ok(
                    new {
                        status = "success",
                        data = domicilios
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

        // GET: api/HogarDomicilio/GetBySector/5
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBySector(int sec_Id_Sector)
        {
            // HogarDomicilio hogardomicilio = new HogarDomicilio();
            try
            {
                var domicilios = (from hd in context.HogarDomicilio
                                  join dis in context.Distrito
                                  on hd.dis_Id_Distrito equals dis.dis_Id_Distrito
                                  join sec in context.Sector
                                  on hd.sec_Id_Sector equals sec.sec_Id_Sector
                                  join pais in context.Pais
                                  on hd.pais_Id_Pais equals pais.pais_Id_Pais
                                  join est in context.Estado
                                  on hd.est_Id_Estado equals est.est_Id_Estado
                                  where hd.sec_Id_Sector == sec_Id_Sector
                                  select new
                                  {
                                      hd_Calle = hd.hd_Calle,
                                      hd_Numero_Exterior = hd.hd_Numero_Exterior,
                                      hd_Numero_Interior = hd.hd_Numero_Interior,
                                      hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                                      hd_Subdivision = hd.hd_Subdivision,
                                      hd_Localidad = hd.hd_Localidad,
                                      hd_Municipio_Ciudad = hd.hd_Municipio_Ciudad,
                                      est_Nombre = est.est_Nombre,
                                      pais_Nombre_Corto = pais.pais_Nombre_Corto,
                                      hd_Telefono = hd.hd_Telefono,
                                      dis_Id_Distrito = dis.dis_Id_Distrito,
                                      dis_Numero = dis.dis_Numero,
                                      dis_Alias = dis.dis_Alias,
                                      sec_Id_Sector = sec.sec_Id_Sector,
                                      sec_Alias = sec.sec_Alias,
                                      sec_Numero = sec.sec_Numero
                                  }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        domicilios = domicilios
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

        // POST: api/HogarDomicilio
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] HogarDomicilio hogardomicilio)
        {
            try
            {
                context.HogarDomicilio.Add(hogardomicilio);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = true,
                        nvoHogarDomicilio = hogardomicilio.hd_Id_Hogar
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest
                (
                    new
                    {
                        status = false,
                        message = ex.Message
                    }
                );
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
