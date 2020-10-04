using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoController : ControllerBase
    {
        private readonly AppDbContext context;

        public EstadoController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Estado
        [HttpGet]
        public IEnumerable<Estado> Get()
        {
            return context.Estado.ToList();
        }

        // GET: api/Estado/5
        [HttpGet("{id}")]
        public Estado Get(int id)
        {
            var estado = context.Estado.FirstOrDefault(e => e.est_Id_Estado == id);
            return estado;
        }

        // POST: api/Estado
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Estado/5
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
