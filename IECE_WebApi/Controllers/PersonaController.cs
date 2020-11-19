﻿using System;
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
        // public IEnumerable<Persona> Get()
        public ActionResult<IEnumerable<Persona>> Get()
        {
            Persona persona = new Persona();
            // return context.Persona.ToList();
            try
            {
                // persona = context.Persona.ToList();
                return Ok(context.Persona.ToList());
            }
            catch (Exception ex)
            {
                /* return BadRequest(
                    new object[] {
                        new {error = "Error: no se encontro ninguna persona en la lista"}
                    }); */
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Persona/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            //var persona = context.Persona.FirstOrDefault(per => per.per_Id_Miembro == id);
            //return persona;
            Persona persona = new Persona();
            try
            {
                persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
                return Ok(persona);
            } catch (Exception ex)
            {
                /* return BadRequest(
                    new object[] {
                        new {error = "Error: no se encontro el indice de persona"}
                    }); */
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
        public ActionResult Put(int id, [FromBody] Persona persona)
        {
            if(persona.per_Id_Persona == id)
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
            var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
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
