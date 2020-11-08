using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisController : ControllerBase
    {
        private readonly AppDbContext context;

        public PaisController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Pais
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IEnumerable<Pais> Get()
        {
            return context.Pais.ToList();
        }

        // GET: api/Pais/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public Pais Get(int id)
        {
            var pais = context.Pais.FirstOrDefault(p => p.pais_Id_Pais == id);
            return pais;
        }

        // POST: api/Pais
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pais/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public void Delete(int id)
        {
        }
    }
}
