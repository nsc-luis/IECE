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
    public class HogarController : ControllerBase
    {
        private readonly AppDbContext context;

        // GET: api/Hogar
        [HttpGet]
        public IEnumerable<Hogar> Get()
        {
            return context.Hogar.ToList();
        }

        // GET: api/Hogar/5
        [HttpGet("{id}")]
        public Hogar Get(int id)
        {
            var hogar = context.Hogar.FirstOrDefault(hog => hog.hog_Id_Hogar == id);
            return hogar;
        }

        // POST: api/Hogar
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Hogar/5
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
