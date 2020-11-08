using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Cors;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Profesion_OficioController : ControllerBase
    {
        private readonly AppDbContext context;

        public Profesion_OficioController(AppDbContext context)
        {
            this.context = context;
        }
        // GET: api/Profesion_Oficio
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Profesion_Oficio>> Get()
        {
            Profesion_Oficio profesion_oficio = new Profesion_Oficio();
            try
            {
                return Ok(context.Profesion_Oficio.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Profesion_Oficio/5
        /*[HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Profesion_Oficio profesion_oficio = new Profesion_Oficio();
            try
            {
                profesion_oficio = context.Profesion_Oficio.FirstOrDefault(pro => pro.pro_Id_Profesion_Oficio == id);
                return Ok(profesion_oficio);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Profesion_Oficio
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Profesion_Oficio/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }*/
    }
}
