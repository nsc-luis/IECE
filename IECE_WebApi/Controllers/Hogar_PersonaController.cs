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
    public class Hogar_PersonaController : ControllerBase
    {
        private readonly AppDbContext context;

        public Hogar_PersonaController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Hogar_Persona
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Hogar_Persona>> Get()
        {
            try
            {
                return Ok(context.Hogar_Persona.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Hogar_Persona/GetHogarByPersona/{per_Id_Persona}
        [Route("[action]/{per_Id_Persona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetHogarByPersona(int per_Id_Persona)
        {
            Hogar_Persona hogar_persona = new Hogar_Persona();
            try
            {
                hogar_persona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == per_Id_Persona);
                return Ok(hogar_persona);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Hogar_Persona/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {
            Hogar_Persona hogar_persona = new Hogar_Persona();
            try
            {
                hogar_persona = context.Hogar_Persona.FirstOrDefault(hp => hp.hp_Id_Hogar_Persona == id);
                return Ok(hogar_persona);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Hogar_Domicilio/GetListaHogares
        [Route("[action]")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetListaHogares()
        {
            try
            {
                var query = (from hp in context.Hogar_Persona
                             join hd in context.HogarDomicilio
                             on hp.hd_Id_Hogar equals hd.hd_Id_Hogar
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

        // GET: api/Hogar_Domicilio/GetDatosHogarDomicilio
        [Route("[action]/{hd_Id_Hogar}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetDatosHogarDomicilio(int hd_Id_Hogar)
        {
            try
            {
                var query = (from hp in context.Hogar_Persona
                             join hd in context.HogarDomicilio
                             on hp.hd_Id_Hogar equals hd.hd_Id_Hogar
                             join e in context.Estado
                             on hd.est_Id_Estado equals e.est_Id_Estado
                             join pais in context.Pais
                             on hd.pais_Id_Pais equals pais.pais_Id_Pais
                             where hp.hd_Id_Hogar == hd_Id_Hogar
                             && hp.hp_Jerarquia == 1
                             select new
                             {
                                 hd_Id_Hogar = hp.hd_Id_Hogar,
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

        // GET: api/Hogar_Persona/GetMiembros/2
        [Route("[action]/{hd_Id_Hogar}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMiembros(int hd_Id_Hogar)
        {
            try
            {
                var query = (from hp in context.Hogar_Persona
                             join p in context.Persona
                             on hp.per_Id_Persona equals p.per_Id_Persona
                             where hp.hd_Id_Hogar == hd_Id_Hogar
                             orderby (hp.hp_Jerarquia)
                             select new
                             {
                                 hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                 hd_Id_Hogar = hp.hd_Id_Hogar,
                                 hp_Jerarquia = hp.hp_Jerarquia,
                                 per_Id_Persona = p.per_Id_Persona,
                                 per_Nombre = p.per_Nombre,
                                 per_Apellido_Paterno = p.per_Apellido_Paterno,
                                 per_Apellido_Materno = p.per_Apellido_Materno
                             }).ToList();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Hogar_Persona
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Hogar_Persona hogar_persona)
        {
            try
            {
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        where hp.hd_Id_Hogar == hogar_persona.hd_Id_Hogar
                                        orderby (hp.hp_Jerarquia)
                                        select new
                                        {
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                            hd_Id_Hogar = hp.hd_Id_Hogar,
                                            hp_Jerarquia = hp.hp_Jerarquia,
                                            per_Id_Persona = hp.per_Id_Persona
                                        }).ToList();


                if (miembrosDelHogar.Count() > 0)
                {
                    foreach (var miembro in miembrosDelHogar)
                    {
                        if (miembro.hp_Jerarquia == hogar_persona.hp_Jerarquia)
                        {
                            context.Hogar_Persona.Add(hogar_persona);
                            context.SaveChanges();

                            var registro = new Hogar_Persona
                            {
                                hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                                hd_Id_Hogar = miembro.hd_Id_Hogar,
                                per_Id_Persona = miembro.per_Id_Persona,
                                hp_Jerarquia = miembro.hp_Jerarquia + 1
                            };
                            context.Entry(registro).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        else if (miembro.hp_Jerarquia > hogar_persona.hp_Jerarquia)
                        {
                            var registro = new Hogar_Persona
                            {
                                hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                                hd_Id_Hogar = miembro.hd_Id_Hogar,
                                per_Id_Persona = miembro.per_Id_Persona,
                                hp_Jerarquia = miembro.hp_Jerarquia + 1
                            };
                            context.Entry(registro).State = EntityState.Modified;
                            context.SaveChanges();
                        }// else
                        //{
                        //    context.Hogar_Persona.Add(hogar_persona);
                        //    context.SaveChanges();
                        //}
                    }
                }
                else
                {
                    context.Hogar_Persona.Add(hogar_persona);
                    context.SaveChanges();
                }
                return Ok(
                    new
                    {
                        status = "success",
                        hogar_persona
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            //try
            //{
            //    context.Hogar_Persona.Add(hogar_persona);
            //    context.SaveChanges();
            //    return Ok
            //    (
            //        new
            //        {
            //            status = "success",
            //            hogar_persona
            //        }
            //    );
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex);
            //}
        }

        // PUT: api/Hogar_Persona/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Hogar_Persona hogar_persona)
        {
            if (hogar_persona.hp_Id_Hogar_Persona == id)
            {
                context.Entry(hogar_persona).State = EntityState.Modified;
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
            Hogar_Persona hogar_persona = new Hogar_Persona();
            hogar_persona = context.Hogar_Persona.FirstOrDefault(hp => hp.hp_Id_Hogar_Persona == id);
            if (hogar_persona != null)
            {
                context.Hogar_Persona.Remove(hogar_persona);
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
