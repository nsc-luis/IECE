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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public IActionResult Get()
        {
            try
            {
                var query = (from sec in context.Sector
                             select new
                             {
                                 sec_Id_Sector = sec.sec_Id_Sector,
                                 sec_Alias = sec.sec_Alias,
                                 // sec_Numero = sec.sec_Numero,
                                 sec_Tipo_Sector = sec.sec_Tipo_Sector,
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        sectores = query
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }

        // GET: api/Sector/GetSectoresByDistrito/dis_Id_Distrito
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSectoresByDistrito(int dis_Id_Distrito)
        {
            try
            {
                var query = (from sec in context.Sector
                             join dis in context.Distrito
                             on sec.dis_Id_Distrito equals dis.dis_Id_Distrito
                             where sec.dis_Id_Distrito == dis_Id_Distrito
                             orderby sec.sec_Id_Sector ascending
                             select new
                             {
                                 sec_Id_Sector = sec.sec_Id_Sector,
                                 sec_Alias = sec.sec_Alias,
                                 sec_Tipo_Sector = sec.sec_Tipo_Sector,
                                 sec.sec_Numero,
                                 pem_Id_Pastor = sec.pem_Id_Pastor != null ? 0 : sec.pem_Id_Pastor
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        sectores = query
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        status = false,
                        mensaje = ex.Message
                    }
                );
            }
        }

        // GET: /api/Sector/GetPastorBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPastorBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from s in context.Sector
                         join pem in context.Personal_Ministerial
                         on s.pem_Id_Pastor equals pem.pem_Id_Ministro
                         where s.sec_Id_Sector == sec_Id_Sector
                         select new
                         {
                             pem_Id_Pastor = s.pem_Id_Pastor,
                             pem_Id_Ministro = pem.pem_Id_Ministro,
                             pem_Nombre = pem.pem_Nombre,
                             pem_emailIECE = pem.pem_emailIECE,
                             pem_email_Personal= pem.pem_email_Personal,
                             pem_Grado_Ministerial = pem.pem_Grado_Ministerial
                         }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        ministros = query
                    });
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = ex.Message
                    });
            }
        }

        // GET: api/Sector/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            // Sector sector = new Sector();
            try
            {
                var sector = (from s in context.Sector
                              join d in context.Distrito
                              on s.dis_Id_Distrito equals d.dis_Id_Distrito
                              where s.sec_Id_Sector == id
                              select new
                              {
                                  sec_Id_Sector = s.sec_Id_Sector,
                                  sec_Tipo_Sector = s.sec_Tipo_Sector,
                                  sec_Numero = s.sec_Numero,
                                  sec_Alias = s.sec_Alias,
                                  dis_Id_Distrito = s.dis_Id_Distrito,
                                  dis_Alias = d.dis_Alias,
                                  dis_Tipo_Distrito = d.dis_Tipo_Distrito,
                                  dis_Numero = d.dis_Numero,
                                  dis_Area = d.dis_Area
                              }).ToList();
                // sector = context.Sector.FirstOrDefault(sec => sec.sec_Id_Sector == id);
                return Ok(
                    new {
                        status = "success",
                        sector = sector
                    });
            }
            catch (Exception ex)
            {
                return Ok(
                    new {
                        status = "error",
                        mensaje = ex.Message
                    });
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
