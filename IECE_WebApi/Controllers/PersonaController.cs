using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
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

        // POST: api/Persona
        [HttpPost]
        public ActionResult Post([FromBody] Persona persona)
        {
            try
            {
                context.Persona.Add(persona);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Persona/5
        [HttpPut("{id}")]
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
