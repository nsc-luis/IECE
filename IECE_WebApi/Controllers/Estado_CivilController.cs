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
    public class Estado_CivilController : ControllerBase
    {
        private readonly AppDbContext context;

        public Estado_CivilController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Estado_Civil
        [HttpGet]
        public ActionResult<IEnumerable<Estado_Civil>> Get()
        {
            Estado_Civil estado_civil = new Estado_Civil();
            try
            {
                return Ok(context.Estado_Civil.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Estado_Civil/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Estado_Civil estado_civil = new Estado_Civil();
            try
            {
                estado_civil = context.Estado_Civil.FirstOrDefault(eci => eci.eci_Id_Relacion_Civil == id);
                return Ok(estado_civil);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        // POST: api/Estado_Civil
        [HttpPost]
        public ActionResult Post([FromBody] Estado_Civil estado_civil)
        {
            try
            {
                context.Estado_Civil.Add(estado_civil);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Estado_Civil/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Estado_Civil estado_civil)
        {
            if(estado_civil.eci_Id_Relacion_Civil == id)
            {
                context.Entry(estado_civil).State = EntityState.Modified;
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
            Estado_Civil estado_civil = new Estado_Civil();
            estado_civil = context.Estado_Civil.FirstOrDefault(eci => eci.eci_Id_Relacion_Civil == id);
            if (estado_civil != null)
            {
                context.Estado_Civil.Remove(estado_civil);
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
