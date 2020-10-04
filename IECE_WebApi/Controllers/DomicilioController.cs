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
    public class DomicilioController : ControllerBase
    {
        private readonly AppDbContext context;

        // GET: api/Domicilio
        [HttpGet]
        public IEnumerable<Domicilio> Get()
        {
            return context.Domicilio.ToList();
        }

        // GET: api/Domicilio/5
        [HttpGet("{id}")]
        public Domicilio Get(int id)
        {
            var domicilio = context.Domicilio.FirstOrDefault(dom => dom.dom_Id_Domicilio == id);
            return domicilio;
        }

        // POST: api/Domicilio
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Domicilio/5
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
