using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InformeAnualPastorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InformeAnualPastorController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Informe>> Get()
        {
            try
            {
                return Ok(_context.Informe.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<InformeAnualPastorController>/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult<Informe> Get(int id)
        {
            try
            {
                Informe informe = _context.Informe
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if(informe == null)
                {
                    return NotFound("El informe no se encontró");
                }
                return Ok(informe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<InformeAnualPastorController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<InformeAnualPastorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InformeAnualPastorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
