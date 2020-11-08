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
    public class SectorController : ControllerBase
    {

        private readonly AppDbContext context;

        public SectorController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Sector
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Sector>> Get()
        {
            Sector sector = new Sector();
            try
            {
                return Ok(context.Sector.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Sector/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Sector sector = new Sector();
            try
            {
                sector = context.Sector.FirstOrDefault(sec => sec.sec_Id_Sector == id);
                return Ok(sector);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Sector
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Sector/5
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
