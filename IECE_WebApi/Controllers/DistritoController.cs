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
    public class DistritoController : ControllerBase
    {
        private readonly AppDbContext context;
        public DistritoController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Distrito
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Distrito>> Get()
        {
            Distrito distrito = new Distrito();
            try
            {
                return Ok(context.Distrito.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Distrito/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Distrito distrito = new Distrito();
            try
            {
                distrito = context.Distrito.FirstOrDefault(dis => dis.dis_Id_Distrito == id);
                return Ok(distrito);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Distrito
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Distrito/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
