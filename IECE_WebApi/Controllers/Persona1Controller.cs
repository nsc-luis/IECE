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
    public class Persona1Controller : ControllerBase
    {
        private readonly AppDbContext context;

        public Persona1Controller(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Persona1
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Persona1>> Get()
        {
            try
            {
                return Ok(context.Persona1.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Persona1/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Persona1 persona = new Persona1();
            try
            {
                persona = context.Persona1.FirstOrDefault(per => per.per_Id_Persona == id);
                return Ok(persona);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Persona/GetByRFCSinHomo/str
        [Route("[action]/{RFCSinHomo}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByRFCSinHomo(string RFCSinHomo)
        {
            // var persona = context.Persona.FirstOrDefault(per => per.per_RFC_Sin_Homo == RFCSinHomo);

            var query = (from p in context.Persona1
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
        public IActionResult Post([FromBody] Persona1 persona)
        {
            try
            {
                context.Persona1.Add(persona);
                context.SaveChanges();
                return Ok(
                    new { status = "success", persona }
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
        public ActionResult Put(int id, [FromBody] Persona1 persona)
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
        public ActionResult Delete(int id)
        {
            var persona = context.Persona1.FirstOrDefault(per => per.per_Id_Persona == id);
            if (persona != null)
            {
                context.Persona1.Remove(persona);
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
