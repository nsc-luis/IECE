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
                    join sub in (from hp in context.Hogar_Persona
                                join p in context.Persona
                                on hp.hp_Id_Hogar_Persona equals p.per_Id_Persona
                                where hp.hp_Jerarquia == 1
                                select new
                                {
                                    hp.hp_Id_Hogar_Persona,
                                    hp.hp_Jerarquia,
                                    hp.hd_Id_Hogar,
                                    p.per_Id_Persona,
                                    p.per_Nombre,
                                    p.per_Apellido_Paterno,
                                    p.per_Apellido_Materno
                                }) on hd.hd_Id_Hogar equals sub.hd_Id_Hogar
                    where hd.dis_Id_Distrito == dis_Id_Distrito
                    select new
                    {
                        hd.hd_Id_Hogar,
                        hd.hd_Calle,
                        hd.hd_Numero_Exterior,
                        hd.hd_Numero_Interior,
                        hd.hd_Tipo_Subdivision,
                        hd.hd_Subdivision,
                        hd.hd_Localidad,
                        hd.hd_Municipio_Ciudad,
                        est.est_Nombre,
                        pais.pais_Nombre_Corto,
                        hd.hd_Telefono,
                        dis.dis_Numero,
                        dis.dis_Alias,
                        sec.sec_Alias,
                        sub.hp_Id_Hogar_Persona,
                        sub.hp_Jerarquia,
                        sub.per_Id_Persona,
                        sub.per_Nombre,
                        sub.per_Apellido_Paterno,
                        sub.per_Apellido_Materno
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
                                  join dis in context.Distrito on hd.dis_Id_Distrito equals dis.dis_Id_Distrito
                                  join sec in context.Sector on hd.sec_Id_Sector equals sec.sec_Id_Sector
                                  join pais in context.Pais on hd.pais_Id_Pais equals pais.pais_Id_Pais
                                  join est in context.Estado on hd.est_Id_Estado equals est.est_Id_Estado
                                  join sub in (from hp in context.Hogar_Persona
                                        join p in context.Persona
                                        on hp.hp_Id_Hogar_Persona equals p.per_Id_Persona
                                        where hp.hp_Jerarquia == 1
                                        select new {
                                            hp.hp_Id_Hogar_Persona,
                                            hp.hp_Jerarquia,
                                            hp.hd_Id_Hogar, 
	                                        p.per_Id_Persona,
                                            p.per_Nombre,
                                            p.per_Apellido_Paterno,
                                            p.per_Apellido_Materno,
                                            p.per_Fecha_Nacimiento,
                                            p.per_Bautizado,
                                            p.per_Telefono_Movil
                                        }) on hd.hd_Id_Hogar equals sub.hd_Id_Hogar
                                  where hd.sec_Id_Sector == sec_Id_Sector
                                  select new
                                  {
                                      hd.hd_Id_Hogar,
                                      hd.hd_Calle,
                                      hd.hd_Numero_Exterior,
                                      hd.hd_Numero_Interior,
                                      hd.hd_Tipo_Subdivision,
                                      hd.hd_Subdivision,
                                      hd.hd_Localidad,
                                      hd.hd_Municipio_Ciudad,
                                      est.est_Id_Estado,
                                      est.est_Nombre,
                                      pais.pais_Id_Pais,
                                      pais.pais_Nombre_Corto,
                                      hd.hd_Telefono,
                                      dis.dis_Id_Distrito,
                                      dis.dis_Numero,
                                      dis.dis_Alias,
                                      sec.sec_Id_Sector,
                                      sec.sec_Alias,
                                      sec.sec_Numero,
                                      sub.hp_Id_Hogar_Persona,
                                      sub.hp_Jerarquia,
                                      sub.per_Id_Persona,
                                      sub.per_Nombre,
                                      sub.per_Apellido_Paterno,
                                      sub.per_Apellido_Materno,
                                      sub.per_Fecha_Nacimiento,
                                      sub.per_Bautizado,
                                      sub.per_Telefono_Movil
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
            try {
                context.Entry(hogardomicilio).State = EntityState.Modified;
                context.SaveChanges();
                return Ok(
                    new
                    {
                        status = "success",
                        domicilio = hogardomicilio
                    });
            }
            catch (Exception ex) {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = ex.Message
                    });
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
