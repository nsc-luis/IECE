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
                                 sec_Tipo_Sector = sec.sec_Tipo_Sector
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
