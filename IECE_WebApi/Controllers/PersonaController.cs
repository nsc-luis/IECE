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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonaController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonaController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

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
                             pro_Sub_Categoria = pro.pro_Sub_Categoria,
                             pro_Categoria = pro.pro_Categoria
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
                             pro_Sub_Categoria = pro.pro_Sub_Categoria,
                             pro_Categoria = pro.pro_Categoria
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
                             dis_Tipo_Distrito = d.dis_Tipo_Distrito,
                             sec_Alias = s.sec_Alias
                         }).ToList();

            if (query.Count > 0)
            {
                return Ok(
                     new { status = true, persona = query }
                );
            }
            else
            {
                return Ok(
                    new { status = false, mensaje = "Persona no encontrada" }
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
                persona.Fecha_Registro = fechayhora;
                persona.usu_Id_Usuario = 1;
                context.Persona.Add(persona);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = true,
                        nvaPersona = persona.per_Id_Persona
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

        // POST: api/Persona/AddPersonaHogar
        [Route("[action]/{jerarquia}/{hdId}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaHogar([FromBody] Persona persona, int jerarquia, int hdId)
        {
            try
            {
                persona.Fecha_Registro = fechayhora;
                persona.usu_Id_Usuario = 1;
                context.Persona.Add(persona);
                context.SaveChanges();

                Hogar_Persona hpModel = new Hogar_Persona();
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        where hp.hd_Id_Hogar == hdId
                                        orderby (hp.hp_Jerarquia)
                                        select new
                                        {
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                            hd_Id_Hogar = hp.hd_Id_Hogar,
                                            hp_Jerarquia = hp.hp_Jerarquia,
                                            per_Id_Persona = hp.per_Id_Persona
                                        }).ToList();



                foreach (var miembro in miembrosDelHogar)
                {
                    if (miembro.hp_Jerarquia == jerarquia)
                    {
                        hpModel.per_Id_Persona = persona.per_Id_Persona;
                        hpModel.hp_Jerarquia = jerarquia;
                        hpModel.hd_Id_Hogar = hdId;
                        hpModel.Fecha_Registro = fechayhora;
                        hpModel.usu_Id_Usuario = 1;
                        context.Hogar_Persona.Add(hpModel);
                        context.SaveChanges();

                        var registro = new Hogar_Persona
                        {
                            hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                            hd_Id_Hogar = miembro.hd_Id_Hogar,
                            per_Id_Persona = miembro.per_Id_Persona,
                            hp_Jerarquia = miembro.hp_Jerarquia + 1,
                            Fecha_Registro = fechayhora,
                            usu_Id_Usuario = 1
                        };
                        context.Entry(registro).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    if (miembro.hp_Jerarquia > jerarquia)
                    {
                        var registro = new Hogar_Persona
                        {
                            hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                            hd_Id_Hogar = miembro.hd_Id_Hogar,
                            per_Id_Persona = miembro.per_Id_Persona,
                            hp_Jerarquia = miembro.hp_Jerarquia + 1,
                            Fecha_Registro = fechayhora,
                            usu_Id_Usuario = 1
                        };
                        context.Entry(registro).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                if (jerarquia > miembrosDelHogar.Count())
                {
                    hpModel.per_Id_Persona = persona.per_Id_Persona;
                    hpModel.hp_Jerarquia = jerarquia;
                    hpModel.hd_Id_Hogar = hdId;
                    hpModel.Fecha_Registro = fechayhora;
                    hpModel.usu_Id_Usuario = 1;
                    context.Hogar_Persona.Add(hpModel);
                    context.SaveChanges();
                }

                return Ok
                (
                    new
                    {
                        status = "success",
                        persona = persona,
                        hogar_persona = hpModel
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok
                (
                    new
                    {
                        status = "error",
                        message = ex.Message
                    }
                );
            }
        }

        // POST: api/Persona/AddPersonaDomicilioHogar
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaDomicilioHogar([FromBody] PersonaDomicilio pd)
        {
            try
            {
                Persona p = new Persona();
                p = pd.PersonaEntity;
                p.usu_Id_Usuario = 1;
                p.Fecha_Registro = fechayhora;
                context.Persona.Add(p);
                context.SaveChanges();

                HogarDomicilio hd = new HogarDomicilio();
                hd = pd.HogarDomicilioEntity;
                hd.usu_Id_Usuario = 1;
                hd.Fecha_Registro = fechayhora;
                context.HogarDomicilio.Add(hd);
                context.SaveChanges();

                Hogar_Persona hp = new Hogar_Persona();
                hp.hp_Jerarquia = 1;
                hp.per_Id_Persona = p.per_Id_Persona;
                hp.hd_Id_Hogar = hd.hd_Id_Hogar;
                hp.Fecha_Registro = fechayhora;
                hp.usu_Id_Usuario = 1;
                context.Hogar_Persona.Add(hp);
                context.SaveChanges();

                return Ok
                (
                    new
                    {
                        status = "success",
                        persona = p,
                        hogardomicilio = hd,
                        hogar_persona = hp
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok
                (
                    new
                    {
                        status = "error",
                        message = ex.Message
                    }
                );
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
            List<string> message = new List<string>();

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
                        hp_Jerarquia = i - 1,
                        Fecha_Registro = fechayhora,
                        usu_Id_Usuario = 1
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
                message.Add("Se elimino la jerarquia que correspondia a la persona en el hogar.");
            }

            // ELIMINA EL DOMICILIO SI LA PERSONA QUE SE ELIMINA ES LA ULTIMA DEL HOGAR
            if (miembrosDelHogar.Count() == 1)
            {
                HogarDomicilio hd = new HogarDomicilio();
                hd = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == miembrosDelHogar[0].hd_Id_Hogar);
                if (hd != null)
                {
                    context.HogarDomicilio.Remove(hd);
                    context.SaveChanges();
                    message.Add("Se eliminó el registro del domicilio por ser la utima persona registado en el mismo.");
                }
            }

            // ELIMINA LA PERSONA POR SU ID Y RETORNA EL RESULTADO
            Persona persona = new Persona();
            persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
            if (persona != null)
            {
                context.Persona.Remove(persona);
                context.SaveChanges();
                message.Add("Se eliminó el registro de la persona con todos sus datos.");
                return Ok(
                        new
                        {
                            status = "success",
                            message = message.ToArray()
                        }
                    );
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
