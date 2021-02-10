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
    public class HogarDomicilioController : ControllerBase
    {

        private readonly AppDbContext context;

        public HogarDomicilioController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/HogarDomicilio
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<HogarDomicilio>> Get()
        {
            try
            {
                return Ok(context.HogarDomicilio.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/HogarDomicilio/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {
            HogarDomicilio hogardomicilio = new HogarDomicilio();
            try
            {
                hogardomicilio = context.HogarDomicilio.FirstOrDefault(hd => hd.hd_Id_Hogar == id);
                return Ok(hogardomicilio);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/HogarDomicilio
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] HogarDomicilio hogardomicilio)
        {
            try
            {
                context.HogarDomicilio.Add(hogardomicilio);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = true,
                        nvoHogarDomicilio = hogardomicilio.hd_Id_Hogar
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

        // PUT: api/HogarDomicilio/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] HogarDomicilio hogardomicilio)
        {
            if (hogardomicilio.hd_Id_Hogar == id)
            {
                context.Entry(hogardomicilio).State = EntityState.Modified;
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
            HogarDomicilio hogardomicilio = new HogarDomicilio();
            hogardomicilio = context.HogarDomicilio.FirstOrDefault(hd => hd.hd_Id_Hogar == id);
            if (hogardomicilio != null)
            {
                context.HogarDomicilio.Remove(hogardomicilio);
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
