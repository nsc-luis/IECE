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
            public Sector sector { get; set; }
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
                        sector = context.Sector.FirstOrDefault(sec => sec.sec_Id_Sector == orgInt.org_Id_Sector),
                        presidente = orgInt.org_Presidente != null || orgInt.org_Presidente != 0? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Presidente) : null,
                        vicePresidente = orgInt.org_Vice_Presidente != null || orgInt.org_Vice_Presidente !=0? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Vice_Presidente) : null,
                        secretario = orgInt.org_Secretario != null || orgInt.org_Secretario !=0? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Secretario):null,
                        subSecretario = orgInt.org_Sub_Secretario != null || orgInt.org_Sub_Secretario !=0? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Sub_Secretario) : null,
                        tesorero = orgInt.org_Tesorero != null || orgInt.org_Tesorero !=0? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Tesorero) : null,
                        subTesorero = orgInt.org_Sub_Tesorero != null || orgInt.org_Sub_Tesorero !=0? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Sub_Tesorero) : null
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

        // GET: api/GetBySector/9
        [HttpGet]
        [Route("[action]/{dis_Id_Distrito}")]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByDistrito(int dis_Id_Distrito)
        {
            try
            {
                // Instancia objeto de los organimos internos con detalle
                List<infoPersonasDelOI> organismosInternos = new List<infoPersonasDelOI>();

                var oi = (from orgInt in context.Organismo_Interno
                          join sec in context.Sector on orgInt.org_Id_Sector equals sec.sec_Id_Sector
                          join dis in context.Distrito on sec.dis_Id_Distrito equals dis.dis_Id_Distrito
                          where dis.dis_Id_Distrito ==dis_Id_Distrito && sec.sec_Tipo_Sector=="SECTOR"
                          orderby sec.sec_Id_Sector
                          select orgInt).ToList();

                foreach (var orgInt in oi)
                {
                    organismosInternos.Add(new infoPersonasDelOI
                    {
                        oi = orgInt,
                        sector = context.Sector.FirstOrDefault(sec=>sec.sec_Id_Sector == orgInt.org_Id_Sector),
                        presidente = orgInt.org_Presidente != null || orgInt.org_Presidente != 0 ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Presidente) : null,
                        vicePresidente = orgInt.org_Vice_Presidente != null || orgInt.org_Vice_Presidente != 0 ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Vice_Presidente) : null,
                        secretario = orgInt.org_Secretario != null || orgInt.org_Secretario != 0 ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Secretario) : null,
                        subSecretario = orgInt.org_Sub_Secretario != null || orgInt.org_Sub_Secretario != 0 ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Sub_Secretario) : null,
                        tesorero = orgInt.org_Tesorero != null || orgInt.org_Tesorero != 0 ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Tesorero) : null,
                        subTesorero = orgInt.org_Sub_Tesorero != null || orgInt.org_Sub_Tesorero != 0 ? context.Persona.FirstOrDefault(p => p.per_Id_Persona == orgInt.org_Sub_Tesorero) : null
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
                                   p.per_Apellido_Casada,
                                   p.per_Nombre_Completo
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
                                   p.per_Apellido_Materno,
                                   p.per_Nombre_Completo
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
                                 p.per_Apellido_Casada,
                                 p.per_Nombre_Completo
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





        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult QuitarEspaciosEnBlanco()
        {
            try
            {
                // Obtiene listado de personas
                var orgInt = (from oi in context.Organismo_Interno select oi).ToList();

                // A cada persona le los acentos en nombre y apellidos y
                // guarda el nombre completo en el campo per_Nombre_Completo
                foreach (var oi in orgInt)
                {
                    // Genera nombre completo
                    oi.org_Categoria = oi.org_Categoria.Trim();
                    oi.org_Nombre = oi.org_Nombre.Trim();
                    oi.org_Tipo_Organismo = oi.org_Tipo_Organismo.Trim();

                    if (oi.org_Nombre == "ELIZABETH" || oi.org_Nombre == "ELIZABET" || oi.org_Nombre == "ELISABETH")
                    {
                        oi.org_Nombre = "ELISABET";
                    }

                    if (oi.org_Nombre == "REYNA ESTHER" || oi.org_Nombre == "REINA ESTER" )
                    {
                        oi.org_Nombre = "REINA ESTHER";
                    }

                    if (oi.org_Nombre == "NOHEMI" || oi.org_Nombre == "NOHEMÍ")
                    {
                        oi.org_Nombre = "NOEMI";
                    }

                    if (oi.org_Nombre == "LOYDA" )
                    {
                        oi.org_Nombre = "LOIDA";
                    }

                    if (oi.org_Nombre == "JOCABETH")
                    {
                        oi.org_Nombre = "JOCABED";
                    }

                    if (oi.org_Nombre == "ROMANTI-EZER" || oi.org_Nombre == "ROMANTI-EZER")
                    {
                        oi.org_Nombre = "ROMAMTI-EZER";
                    }

                    if (oi.org_Categoria == "em")
                    {
                        oi.org_Categoria = "FEMENIL";
                    }

                    if (oi.org_Nombre == "SAREPTHA DE SIDON")
                    {
                        oi.org_Nombre = "SAREPTA DE SIDÓN";
                    }

                    if (oi.org_Nombre == "OBED EDOM")
                    {
                        oi.org_Nombre = "OBED-EDOM";
                    }

                    if (oi.org_Nombre == "ROSA DE SARON")
                    {
                        oi.org_Nombre = "ROSA DE SARÓN";
                    }

                    if (oi.org_Nombre == "PRINCIPES DE SION" || oi.org_Nombre == "PRÍNCIPES DE SION" || oi.org_Nombre == "PRINCIPES DE SIÓN")
                    {
                        oi.org_Nombre = "PRÍNCIPES DE SIÓN";
                    }

                    if (oi.org_Nombre == "CAMINANTES DE EMMAUS")
                    {
                        oi.org_Nombre = "CAMINANTES DE EMMAÚS";
                    }

                    if (oi.org_Nombre == "DISCIPULOS DE EMMAUN")
                    {
                        oi.org_Nombre = "DISCÍPULOS DE EMMAÚS";
                    }

                    if (oi.org_Nombre == "MARIA DE NAZARET" || oi.org_Nombre == "MARIA DE NAZARETH" || oi.org_Nombre == "MARÍA DE NAZARETH")
                    {
                        oi.org_Nombre = "MARÍA DE NAZARET";
                    }

                    if (oi.org_Nombre == "MARIA MAGDALENA" || oi.org_Nombre == "MRÍA MAGDALENA")
                    {
                        oi.org_Nombre = "MARÍA MAGDALENA";
                    }

                    if (oi.org_Nombre == "REY JOSIAS" || oi.org_Nombre == "REY JOSIAS")
                    {
                        oi.org_Nombre = "REY JOSÍAS";
                    }

                    if (oi.org_Nombre == "MONTE DE SION" )
                    {
                        oi.org_Nombre = "MONTE DE SIÓN";
                    }

                    if (oi.org_Nombre == "PRINCIPE JONATAN" || oi.org_Nombre == "PRÍNCIPE JONATAN" || oi.org_Nombre == "PRINCIPE JONATHAN")
                    {
                        oi.org_Nombre = "PRÍNCIPE JONATHÁN";
                    }

                    if (oi.org_Nombre == "SION DEL GRAN REY")
                    {
                        oi.org_Nombre = "SIÓN DEL GRAN REY";
                    }

                    if (oi.org_Nombre == "CAUDILLO MOISES")
                    {
                        oi.org_Nombre = "CAUDILLO MOISÉS";
                    }

                    if (oi.org_Nombre == "CAUDILLO JOSUE")
                    {
                        oi.org_Nombre = "CAUDILLO JOSUÉ";
                    }

                    if (oi.org_Nombre == "LEGISLADOR MOISES")
                    {
                        oi.org_Nombre = "LEGISLADOR MOISÉS";
                    }

                    if (oi.org_Nombre == "NIÑO MOISES")
                    {
                        oi.org_Nombre = "NIÑO MOISÉS";
                    }

                    if (oi.org_Nombre == "HIJAS DE SION")
                    {
                        oi.org_Nombre = "HIJAS DE SIÓN";
                    }

                    if (oi.org_Nombre == "REY SALOMON")
                    {
                        oi.org_Nombre = "REY SALOMÓN";
                    }

                    if (oi.org_Nombre == "HEREDAD DE JEHOVA")
                    {
                        oi.org_Nombre = "HEREDAD DE JEHOVÁ";
                    }

                    if (oi.org_Nombre == "DEBORA")
                    {
                        oi.org_Nombre = "DÉBORA";
                    }

                    if (oi.org_Nombre == "LOS JUSTOS RECABITAS")
                    {
                        oi.org_Nombre = "LOS JUSTOS RECHABITAS";
                    }

                    if (oi.org_Nombre == "PRIS" || oi.org_Nombre == "PRICSILA")
                    {
                        oi.org_Nombre = "PRISCILA";
                    }

                    if (oi.org_Nombre == "ANA LA PROFETIZA")
                    {
                        oi.org_Nombre = "ANA LA PROFETISA";
                    }

                    if (oi.org_Nombre == "DAMARIS")
                    {
                        oi.org_Nombre = "DÁMARIS";
                    }

                    if (oi.org_Nombre == "SALOME")
                    {
                        oi.org_Nombre = "SALOMÉ";
                    }

                    if (oi.org_Nombre == "GENESIS")
                    {
                        oi.org_Nombre = "GÉNESIS";
                    }

                    // Guarda cambios
                    context.Organismo_Interno.Update(oi);
                    context.SaveChanges();
                }

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
    }
}
