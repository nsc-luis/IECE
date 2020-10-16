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
    public class DomicilioController : ControllerBase
    {
        private readonly AppDbContext context;

        public DomicilioController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Domicilio
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Domicilio>> Get()
        {
            Domicilio domicilio = new Domicilio();
            try
            {
                return Ok(context.Domicilio.ToList());
            }
            catch (Exception ex)
            {
                /* return BadRequest(
                    new object[] {
                        new {error = "Error: Aun no hay dingun domicilio registrado."}
                    }); */
                return BadRequest(ex.Message);
            }
            //return context.Domicilio.ToList();
        }

        // GET: api/Domicilio/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {
            Domicilio domicilio = new Domicilio();
            try
            {
                domicilio = context.Domicilio.FirstOrDefault(dom => dom.dom_Id_Domicilio == id);
                return Ok(domicilio);
            }
            catch (Exception ex)
            {
                /*return BadRequest(
                    new object[] {
                        new {error = "Error: no se encontro el indice del domicilio"}
                    }); */
                return BadRequest(ex.Message);
            }
            
        }

        // GET: api/Domicilio/GetByIdHogar/5
        [Route("[action]/{idHogar}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetByIdHogar(int idHogar)
        {
            Domicilio domicilio = new Domicilio();
            try
            {
                domicilio = context.Domicilio.FirstOrDefault(dom => dom.hog_Id_Hogar == idHogar);
                return Ok(domicilio);
            }
            catch (Exception ex)
            {
                /*return BadRequest(
                    new object[] {
                        new {error = "Error: no se encontro el indice del domicilio"}
                    }); */
                return BadRequest(ex.Message);
            }

        }

        // POST: api/Domicilio
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] Domicilio domicilio)
        {
            try
            {
                context.Domicilio.Add(domicilio);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Domicilio/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Domicilio domicilio)
        {
            if (domicilio.dom_Id_Domicilio == id)
            {
                context.Entry(domicilio).State = EntityState.Modified;
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
            var domicilio = context.Domicilio.FirstOrDefault(dom => dom.dom_Id_Domicilio == id);
            if (domicilio != null)
            {
                context.Domicilio.Remove(domicilio);
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
