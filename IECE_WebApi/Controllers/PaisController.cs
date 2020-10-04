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
    public class PaisController : ControllerBase
    {
        private readonly AppDbContext context;

        public PaisController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Pais
        [HttpGet]
        public IEnumerable<Pais> Get()
        {
            return context.Pais.ToList();
        }

        // GET: api/Pais/5
        [HttpGet("{id}")]
        public Pais Get(int id)
        {
            var pais = context.Pais.FirstOrDefault(p => p.pais_Id_Pais == id);
            return pais;
        }

        // POST: api/Pais
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pais/5
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
