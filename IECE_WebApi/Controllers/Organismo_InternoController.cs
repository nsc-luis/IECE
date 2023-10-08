using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Organismo_InternoController : ControllerBase
    {
        private readonly AppDbContext context;

        public Organismo_InternoController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        // Objeto de personas del organismo interno
        public class infoPersonasDelOI
        {
            public virtual Organismo_Interno oi { get; set; }
            public Persona presidente { get; set; }
            public Persona vicePresidente { get; set; }
            public Persona secretario { get; set; }
            public Persona subSecretario { get; set; }
            public Persona tesorero { get; set; }
            public Persona subTesorero { get; set; }
        }

        public class AsignacionDeCargo
        {
            public string cargo { get; set; }
            public int per_Id_Persona { get; set; }
        }

        // GET: api/<Organismo_InternoController>
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult Get()
        {
            try
            {
                var organismosInternos = context.Organismo_Interno.ToList();

                return Ok(new
                {
                    status = "success",
                    organismosInternos
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // GET: api/GetBySector/9
        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBySector(int sec_Id_Sector)
        {
            try
            {
                // Instancia objeto de los organimos internos con detalle
                List<infoPersonasDelOI> organismosInternos = new List<infoPersonasDelOI>();

                var oi = (from orgInt in context.Organismo_Interno
                          where orgInt.org_Id_Sector == sec_Id_Sector
                          select orgInt).ToList();
                foreach (var orgInt in oi)
                {
                    organismosInternos.Add(new infoPersonasDelOI
                    {
                        oi = orgInt,
                        presidente = orgInt.org_Presidente != null ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Presidente) : null,
                        vicePresidente = orgInt.org_Presidente != null ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Vice_Presidente) : null,
                        secretario = orgInt.org_Presidente != null ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Secretario) : null,
                        subSecretario = orgInt.org_Presidente != null ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Sub_Secretario) : null,
                        tesorero = orgInt.org_Presidente != null ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Tesorero) : null,
                        subTesorero = orgInt.org_Presidente != null ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Sub_Tesorero) : null
                    });
                }

                return Ok(new
                {
                    status = "success",
                    organismosInternos
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex
                });
            }
        }

        // GET: api/GetMujeresBySector/9
        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMujeresBySector(int sec_Id_Sector)
        {
            try
            {
                var mujeres = (from p in context.Persona
                               where p.per_Activo
                               && p.per_Categoria == "ADULTO_MUJER"
                               && p.per_Vivo
                               && p.sec_Id_Sector == sec_Id_Sector
                               select new
                               {
                                   p.per_Id_Persona,
                                   p.per_Nombre,
                                   p.per_Apellido_Paterno,
                                   p.per_Apellido_Materno,
                                   p.per_Apellido_Casada
                               }).ToList();
                return Ok(new { status = "success", mujeres });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // GET: api/GetJovenesBySector/9
        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult GetJovenesBySector(int sec_Id_Sector)
        {
            try
            {
                var jovenes = (from p in context.Persona
                               where p.per_Activo
                               && p.per_Categoria.Contains("JOVEN")
                               && p.per_Vivo
                               && p.sec_Id_Sector == sec_Id_Sector
                               select new
                               {
                                   p.per_Id_Persona,
                                   p.per_Nombre,
                                   p.per_Apellido_Paterno,
                                   p.per_Apellido_Materno
                               }).ToList();
                return Ok(new { status = "success", jovenes });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // GET: api/GetNinosBySector/9
        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult GetNinosBySector(int sec_Id_Sector)
        {
            try
            {
                var ninos = (from p in context.Persona
                             where p.per_Activo
                             && p.per_Categoria.Contains("NIÑ")
                             && p.per_Vivo
                             && p.sec_Id_Sector == sec_Id_Sector
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Apellido_Casada
                             }).ToList();
                return Ok(new { status = "success", ninos });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // GET api/<Organismo_InternoController>/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            try
            {
                var orgInt = context.Organismo_Interno.FirstOrDefault(oi => oi.org_Id == id);

                return Ok(new
                {
                    status = "success",
                    orgInt
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // POST api/<Organismo_InternoController>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Organismo_Interno organismoInterno)
        {
            try
            {
                organismoInterno.org_Fecha_Captura = fechayhora;

                context.Organismo_Interno.Add(organismoInterno);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // PUT api/<Organismo_InternoController>/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put(int id, [FromBody] AsignacionDeCargo asignacion)
        {
            try
            {
                var organismoInterno = context.Organismo_Interno.FirstOrDefault(o => o.org_Id == id);
                organismoInterno.org_Fecha_Captura = fechayhora;
                switch(asignacion.cargo)
                {
                    case "presidente":
                        organismoInterno.org_Presidente = asignacion.per_Id_Persona;
                        break;
                    case "vicePresidente":
                        organismoInterno.org_Vice_Presidente = asignacion.per_Id_Persona;
                        break;
                    case "secretario":
                        organismoInterno.org_Secretario = asignacion.per_Id_Persona;
                        break;
                    case "subSecretario":
                        organismoInterno.org_Sub_Secretario = asignacion.per_Id_Persona;
                        break;
                    case "tesorero":
                        organismoInterno.org_Tesorero = asignacion.per_Id_Persona;
                        break;
                    case "subTesorero":
                        organismoInterno.org_Sub_Tesorero = asignacion.per_Id_Persona;
                        break;
                }
                context.Organismo_Interno.Update(organismoInterno);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // DELETE api/<Organismo_InternoController>/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var organismoInterno = context.Organismo_Interno.FirstOrDefault(oi => oi.org_Id == id);
                context.Organismo_Interno.Remove(organismoInterno);
                context.SaveChanges();
                
                return Ok(new
                {
                    status = "success",
                    organismoInterno
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }
    }
}
