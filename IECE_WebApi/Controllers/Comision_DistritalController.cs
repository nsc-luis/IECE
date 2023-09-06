using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class Comision_DistritalController : ControllerBase
    {
        private readonly AppDbContext context;

        public Comision_DistritalController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/ComisionDistrital
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comision_Distrital>>> GetComisionDistrital()
        {
            try
            {
                var comisionesDisponibles = await context.Comision_Distrital.ToListAsync();

                return Ok(new
                {
                    status = true,
                    comisionesDisponibles = comisionesDisponibles
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }








        }

        // GET: api/Comision_Distrital/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comision_Distrital>> GetComisionDistrital(int id)
        {
            var comision = context.Comision_Distrital.FirstOrDefault(com => com.ComisionDistrital_Id  == id);

            var comisionDistrital = await context.Comision_Distrital.FindAsync(id);

            if (comisionDistrital == null)
            {
                return NotFound();
            }

            return comisionDistrital;
        }

        // PUT: api/ComisionDistrital/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComisionDistrital(int id, Comision_Distrital comisionDistrital)
        {
            if (id != comisionDistrital.ComisionDistrital_Id)
            {
                return BadRequest();
            }

            context.Entry(comisionDistrital).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComisionDistritalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ComisionDistrital
        [HttpPost]
        public async Task<ActionResult<Comision_Distrital>> PostComisionDistrital(Comision_Distrital comisionDistrital)
        {
            context.Comision_Distrital.Add(comisionDistrital);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetComisionDistrital", new { id = comisionDistrital.ComisionDistrital_Id }, comisionDistrital);
        }

        // DELETE: api/ComisionDistrital/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comision_Distrital>> DeleteComisionDistrital(int id)
        {
            var comisionDistrital = await context.Comision_Distrital.FindAsync(id);
            if (comisionDistrital == null)
            {
                return NotFound();
            }

            context.Comision_Distrital.Remove(comisionDistrital);
            await context.SaveChangesAsync();

            return comisionDistrital;
        }

        private bool ComisionDistritalExists(int id)
        {
            return context.Comision_Distrital.Any(e => e.ComisionDistrital_Id == id);
        }
    }
}
