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
    public class Comision_LocalController : ControllerBase
    {
        private readonly AppDbContext context;

        public Comision_LocalController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/ComisionLocal
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comision_Local>>> GetComisionLocal()
        {
            try
            {
                var comisionesDisponibles = await context.Comision_Local.ToListAsync();

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

        // GET: api/Comision_Local/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comision_Local>> GetComisionLocal(int id)
        {
            var comision = context.Comision_Local.FirstOrDefault(com => com.ComisionLocal_Id  == id);

            var comisionLocal = await context.Comision_Local.FindAsync(id);

            if (comisionLocal == null)
            {
                return NotFound();
            }

            return comisionLocal;
        }

        // PUT: api/ComisionLocal/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComisionLocal(int id, Comision_Local comisionLocal)
        {
            if (id != comisionLocal.ComisionLocal_Id)
            {
                return BadRequest();
            }

            context.Entry(comisionLocal).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComisionLocalExists(id))
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

        // POST: api/ComisionLocal
        [HttpPost]
        public async Task<ActionResult<Comision_Local>> PostComisionLocal(Comision_Local comisionLocal)
        {
            context.Comision_Local.Add(comisionLocal);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetComisionLocal", new { id = comisionLocal.ComisionLocal_Id }, comisionLocal);
        }

        // DELETE: api/ComisionLocal/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comision_Local>> DeleteComisionLocal(int id)
        {
            var comisionLocal = await context.Comision_Local.FindAsync(id);
            if (comisionLocal == null)
            {
                return NotFound();
            }

            context.Comision_Local.Remove(comisionLocal);
            await context.SaveChangesAsync();

            return comisionLocal;
        }

        private bool ComisionLocalExists(int id)
        {
            return context.Comision_Local.Any(e => e.ComisionLocal_Id == id);
        }
    }
}
