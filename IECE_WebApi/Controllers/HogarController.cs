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
    public class HogarController : ControllerBase
    {
        private readonly AppDbContext context;

        public HogarController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Hogar
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Hogar>> Get()
        {
            try
            {
                return Ok(context.Hogar.ToList());
            }
            catch (Exception ex)
            {
                /* turn BadRequest(
                        new object []
                        {
                            new { error = "Error: aun no hay domicilios guardados"}
                        }
                    ); */
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Hogar/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Hogar hogar = new Hogar();
            try
            {
                hogar = context.Hogar.FirstOrDefault(hog => hog.hog_Id_Hogar == id);
                return Ok(hogar);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Hogar
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] Hogar hogar)
        {
            try
            {
                context.Hogar.Add(hogar);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Hogar/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Hogar hogar)
        {
            if(hogar.hog_Id_Hogar == id)
            {
                context.Entry(hogar).State = EntityState.Modified;
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
            Hogar hogar = new Hogar();
            hogar = context.Hogar.FirstOrDefault(hog => hog.hog_Id_Hogar == id);
            if (hogar != null)
            {
                context.Hogar.Remove(hogar);
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
