using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class HogarController : ControllerBase
    {
        private readonly AppDbContext context;

        public HogarController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Hogar
        /// <summary>
        /// Lista todos los hogares disponibles
        /// </summary>
        /// <remarks>
        /// Ejemplo:
        ///
        ///     GET /hogar
        ///    [
        ///    {
        ///        "hog_Id_Hogar": 1,
        ///        "per_Id_Persona": 2,
        ///        "hog_Jerarquia": 1,
        ///        "hog_Relacion_Hogar_Persona": 1
        ///    },
        ///    {
        ///        "hog_Id_Hogar": 2,
        ///        "per_Id_Persona": 5,
        ///        "hog_Jerarquia": 1,
        ///        "hog_Relacion_Hogar_Persona": 2
        ///    }
        ///    ]
        ///
        /// </remarks>
        /// <returns>JSON con lista de hogares</returns>
        /// <response code="201">Retorna JSON de lista de hogares</response>
        /// <response code="400">Retorna JSON con mensaje de error</response> 
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Hogar>> Get()
        {
            try
            {
                return Ok(context.Hogar.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Hogar/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Hogar hogar = new Hogar();
            try
            {
                hogar = context.Hogar.FirstOrDefault(hog => hog.hog_Id_Hogar == id);
                return Ok(hogar);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Hogar/GetMiembros/2
        [Route("[action]/{hog_Relacion_Hogar_Persona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMiembros(int hog_Relacion_Hogar_Persona)
        {
            try
            {
                var query = (from h in context.Hogar
                    join p in context.Persona
                    on h.per_Id_Persona equals p.per_Id_Persona
                    where h.hog_Relacion_Hogar_Persona == hog_Relacion_Hogar_Persona
                    select new
                    {
                        hog_Id_Hogar = h.hog_Id_Hogar,
                        hog_Relacion_Hogar_Persona = h.hog_Relacion_Hogar_Persona,
                        hog_Jerarquia = h.hog_Jerarquia,
                        per_Id_Persona = p.per_Id_Persona,
                        per_Nombre = p.per_Nombre,
                        per_Apellido_Paterno = p.per_Apellido_Paterno,
                        per_Apellido_Materno = p.per_Apellido_Materno
                    }).ToList();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Hogar
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] Hogar hogar)
        {
            try
            {
                context.Hogar.Add(hogar);
                context.SaveChanges();
                int idHogar = hogar.hog_Id_Hogar;
                var sql = @"Update [Hogar] SET hog_Relacion_Hogar_Persona = @hog_Relacion_Hogar_Persona WHERE hog_Id_Hogar = @hog_Id_Hogar";
                context.Database.ExecuteSqlCommand(
                    sql,
                    new SqlParameter("@hog_Relacion_Hogar_Persona", idHogar),
                    new SqlParameter("@hog_Id_Hogar", idHogar)
                );
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Hogar/AddPersonaToHogar
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaToHogar([FromBody] Hogar hogar)
        {
            try
            {
                context.Hogar.Add(hogar);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Hogar/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Hogar hogar)
        {
            if(hogar.hog_Id_Hogar == id)
            {
                context.Entry(hogar).State = EntityState.Modified;
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
            Hogar hogar = new Hogar();
            hogar = context.Hogar.FirstOrDefault(hog => hog.hog_Id_Hogar == id);
            if (hogar != null)
            {
                context.Hogar.Remove(hogar);
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
