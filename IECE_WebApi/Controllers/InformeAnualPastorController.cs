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
                var meses = new Dictionary<int, string>
                {
                    {0, "Desconocido"},
                    {1, "Enero"},
                    {2, "Febrero"},
                    {3, "Marzo"},
                    {4, "Abril"},
                    {5, "Mayo"},
                    {6, "Junio"},
                    {7, "Julio"},
                    {8, "Agosto"},
                    {9, "Septiembre"},
                    {10, "Octubre"},
                    {11, "Noviembre"},
                    {12, "Diciembre"}
                };
                return Ok(_context.Informe.Select(s => new
                {
                    IdInforme = s.IdInforme,
                    Anio = s.Anio,
                    Mes = meses[s.Mes],
                }).ToList());
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
                var meses = new Dictionary<int, string>
                {
                    {0, "Desconocido"},
                    {1, "Enero"},
                    {2, "Febrero"},
                    {3, "Marzo"},
                    {4, "Abril"},
                    {5, "Mayo"},
                    {6, "Junio"},
                    {7, "Julio"},
                    {8, "Agosto"},
                    {9, "Septiembre"},
                    {10, "Octubre"},
                    {11, "Noviembre"},
                    {12, "Diciembre"}
                };
                var informe = _context.Informe
                    .Where(w => w.IdInforme == id)
                    .Select(s => new
                    {
                        IdInforme = s.IdInforme,
                        IdTipoUsuario = s.IdTipoUsuario,
                        Mes = meses[s.Mes].ToUpper(),
                        Anio = s.Anio,
                        IdDistrito = s.IdDistrito,
                        IdSector = s.IdSector,
                        LugarReunion = s.LugarReunion,
                        Status = s.Status,
                    })
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
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Informe data)
        {
            try
            {

                List<Informe> informes = _context.Informe
                    .Where(s => s.Anio == data.Anio)
                    .ToList();

                bool informeExiste = informes.Any(a => a.Mes == data.Mes);
                if (informeExiste)
                {
                    return BadRequest($"Ya existe un informe para el mes {data.Mes}");
                }

                Informe informe = new Informe
                {
                    IdTipoUsuario = data.IdTipoUsuario,
                    Mes = data.Mes,
                    Anio = data.Anio,
                    IdDistrito = data.IdDistrito,
                    IdSector = data.IdSector,
                    LugarReunion = data.LugarReunion,
                    FechaReunion = data.FechaReunion,
                    Status = data.Status,
                    Usu_Id_Usuario = data.Usu_Id_Usuario,
                    FechaRegistro = DateTime.Now
                };
                _context.Informe.Add(informe);
                _context.SaveChanges();
                return Ok(informe);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
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
