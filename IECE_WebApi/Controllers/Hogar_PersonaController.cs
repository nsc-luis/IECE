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

        // POST: api/Hogar_Persona
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] Hogar_Persona hogar_persona)
        {
            try
            {
                context.Hogar_Persona.Add(hogar_persona);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
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
