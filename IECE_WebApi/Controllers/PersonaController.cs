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
    public class PersonaController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonaController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Persona
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Persona>> Get()
        {
            try
            {
                return Ok(context.Persona.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Persona/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Persona persona = new Persona();
            try
            {
                persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
                return Ok(persona);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Persona/GetProfesion1/idPersona
        [Route("[action]/{idPersona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetProfesion1(int idPersona)
        {
            var query = (from per in context.Persona
                         join pro in context.Profesion_Oficio
                         on per.pro_Id_Profesion_Oficio1 equals pro.pro_Id_Profesion_Oficio
                         where per.per_Id_Persona == idPersona
                         select new
                         {
                             pro_Definicion_Profesion_Oficio = pro.pro_Definicion_Profesion_Oficio,
                             pro_Desc_Profesion_Oficio = pro.pro_Desc_Profesion_Oficio
                         }).ToList();
            return Ok(query);
        }

        // GET: api/Persona/GetProfesion2/idPersona
        [Route("[action]/{idPersona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetProfesion2(int idPersona)
        {
            var query = (from per in context.Persona
                         join pro in context.Profesion_Oficio
                         on per.pro_Id_Profesion_Oficio2 equals pro.pro_Id_Profesion_Oficio
                         where per.per_Id_Persona == idPersona
                         select new
                         {
                             pro_Definicion_Profesion_Oficio = pro.pro_Definicion_Profesion_Oficio,
                             pro_Desc_Profesion_Oficio = pro.pro_Desc_Profesion_Oficio
                         }).ToList();
            return Ok(query);
        }

        // GET: api/Persona/GetByRFCSinHomo/str
        [Route("[action]/{RFCSinHomo}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByRFCSinHomo(string RFCSinHomo)
        {
            // var persona = context.Persona.FirstOrDefault(per => per.per_RFC_Sin_Homo == RFCSinHomo);

            var query = (from p in context.Persona
                         join s in context.Sector
                         on p.sec_Id_Sector equals s.sec_Id_Sector
                         join d in context.Distrito
                         on s.dis_Id_Distrito equals d.dis_Id_Distrito
                         where p.per_RFC_Sin_Homo == RFCSinHomo
                         select new
                         {
                             per_Nombre = p.per_Nombre,
                             per_Apellido_Paterno = p.per_Apellido_Paterno,
                             per_ApellidoMaterno = p.per_Apellido_Materno,
                             per_Fecha_Nacimiento = p.per_Fecha_Nacimiento,
                             dis_Numero = d.dis_Numero,
                             dis_Localidad = d.dis_Localidad,
                             sec_Numero = s.sec_Numero,
                             sec_Localidad = s.sec_Localidad
                         }).ToList();

            if (query.Count > 0)
            {
                return Ok(
                    new object[] {
                        new {status = true, persona = query}
                    }
                );
            }
            else
            {
                return Ok(
                    new object[] {
                        new {status = false, mensaje = "Persona no encontrada"}
                    }
                );
            }
        }

        // POST: api/Persona
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Persona persona)
        {
            try
            {
                context.Persona.Add(persona);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = "success",
                        nvaPersona = persona.per_Id_Persona
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Persona/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put([FromBody] Persona persona, int id)
        {
            if (persona.per_Id_Persona == id)
            {
                context.Entry(persona).State = EntityState.Modified;
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
        public IActionResult Delete(int id)
        {
            // CONSULTA HOGAR A PARTIR DEL ID DE LA PERSONA
            Hogar_Persona hogar_persona = new Hogar_Persona();
            hogar_persona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == id);

            // CONSULTA LOS MIEMBROS DEL HOGAR AL QUE PERTENESE LA PERSONA
            var miembrosDelHogar = (from hp in context.Hogar_Persona
                                    join p in context.Persona
                                    on hp.per_Id_Persona equals p.per_Id_Persona
                                    where hp.hd_Id_Hogar == hogar_persona.hd_Id_Hogar
                                    orderby hp.hp_Jerarquia
                                    select new
                                    {
                                        hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                        hd_Id_Hogar = hp.hd_Id_Hogar,
                                        per_Id_Persona = hp.per_Id_Persona,
                                        hp_Jerarquia = hp.hp_Jerarquia
                                    }).ToList();
            for (int i = miembrosDelHogar.Count(); i > hogar_persona.hp_Jerarquia; i--)
            {
                // ACTUALIZA LA JERARQUIA DE LOS MIEMBROS RESTANTES DENTRO DEL HOGAR
                if (miembrosDelHogar[i - 1].hp_Jerarquia > hogar_persona.hp_Jerarquia)
                {
                    var registro = new Hogar_Persona
                    {
                        hp_Id_Hogar_Persona = miembrosDelHogar[i - 1].hp_Id_Hogar_Persona,
                        hd_Id_Hogar = miembrosDelHogar[i - 1].hd_Id_Hogar,
                        per_Id_Persona = miembrosDelHogar[i - 1].per_Id_Persona,
                        hp_Jerarquia = i - 1
                    };
                    context.Entry(registro).State = EntityState.Modified;
                    context.SaveChanges();
                }

            }

            // ELIMINA REGISTRO DE LA PERSONA EN LA TABLA HOGAR_PERSONA
            if (hogar_persona != null)
            {
                context.Hogar_Persona.Remove(hogar_persona);
                context.SaveChanges();
            }

            // ELIMINA EL DOMICILIO SI LA PERSONA QUE SE ELIMINA ES LA ULTIMA DEL HOGAR
            if(miembrosDelHogar.Count() == 1)
            {
                HogarDomicilio hd = new HogarDomicilio();
                hd = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == miembrosDelHogar[0].hd_Id_Hogar);
                if (hd != null)
                {
                    context.HogarDomicilio.Remove(hd);
                    context.SaveChanges();
                }
            }

            // ELIMINA LA PERSONA POR SU ID Y RETORNA EL RESULTADO
            Persona persona = new Persona();
            persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
            if (persona != null)
            {
                context.Persona.Remove(persona);
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
