using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
                                 pem_email_Personal = pem.pem_email_Personal,
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
                                  s.pem_Id_Secretario,
                                  s.pem_Id_Tesorero,
                                  dis_Id_Distrito = s.dis_Id_Distrito,
                                  dis_Alias = d.dis_Alias,
                                  dis_Tipo_Distrito = d.dis_Tipo_Distrito,
                                  dis_Numero = d.dis_Numero,
                                  dis_Area = d.dis_Area
                              }).ToList();
                // sector = context.Sector.FirstOrDefault(sec => sec.sec_Id_Sector == id);
                return Ok(
                    new
                    {
                        status = "success",
                        sector = sector
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

        // GET /api/Sector/BuscarPorTexto
        [HttpGet]
        [Route("[action]/{texto}")]
        [EnableCors("AllowOrigin")]
        public IActionResult BuscarPorTexto(string texto)
        {
            try
            {
                if (Regex.IsMatch(texto, @"^\d+$"))
                {
                    var query = (from s in context.Sector
                                 join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                 where d.dis_Numero == int.Parse(texto)
                                 select new
                                 {
                                     s.sec_Id_Sector,
                                     s.sec_Alias,
                                     d.dis_Id_Distrito,
                                     d.dis_Tipo_Distrito,
                                     d.dis_Numero,
                                     d.dis_Alias
                                 }).ToList();
                    return Ok(new
                    {
                        status = "success",
                        query
                    });
                }
                else
                {
                    var query = (from s in context.Sector
                                 join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                 where s.sec_Alias.ToUpper().Contains(texto.ToUpper())
                                 || d.dis_Alias.ToUpper().Contains(texto.ToUpper())
                                 || d.dis_Tipo_Distrito.ToUpper().Contains(texto.ToUpper())
                                 select new
                                 {
                                     s.sec_Id_Sector,
                                     s.sec_Alias,
                                     d.dis_Id_Distrito,
                                     d.dis_Tipo_Distrito,
                                     d.dis_Numero,
                                     d.dis_Alias
                                 }).ToList();
                    return Ok(new
                    {
                        status = "success",
                        query
                    });
                }
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


        // GET: api/Sector/PorTransaccionBautismo/5
        [HttpGet]
        [Route("[action]/{id_Persona}")]
        [EnableCors("AllowOrigin")]
        public IActionResult PorTransaccionBautismo(int id_Persona)
        {
            try
            {
                var persona = context.Persona.FirstOrDefault(rh => rh.per_Id_Persona == id_Persona);
                var registroHistoricoDeBautismo = context.Historial_Transacciones_Estadisticas.FirstOrDefault(rh => rh.per_Persona_Id == id_Persona && rh.ct_Codigo_Transaccion == 11001);

                //Si el Alias registrado en el Persona_Lugar_Bautismo coincide con el Alias de algun Sector. Tomará el id de esa Coincidencia como idLugarBautismo, si No, se queda en 0
                Sector sectorConCoincidenciaDeAlias = context.Sector.FirstOrDefault(sec => sec.sec_Alias == persona.per_Lugar_Bautismo);
                int idLugarBautismo = 0;          
                if (sectorConCoincidenciaDeAlias != null && registroHistoricoDeBautismo !=null) 
                {
                    Sector sector = context.Sector.FirstOrDefault(sec => sec.sec_Id_Sector == sectorConCoincidenciaDeAlias.sec_Id_Sector);
                    idLugarBautismo = sector.sec_Id_Sector;
                }
                  return Ok(
                         new
                         {
                          status = "success",
                          sector = idLugarBautismo
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


        // GET: /api/Sector/GetPersonalAdministrativoBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalAdministrativoBySector(int sec_Id_Sector)
        {
            try
            {
                //Consulta de la Tabla de PersonalMinisterial el Pastor de un Sector.
                var pastor = (from s in context.Sector
                             join pem in context.Personal_Ministerial on s.pem_Id_Pastor equals pem.pem_Id_Ministro
                             join p in context.Persona on pem.per_Id_Miembro equals p.per_Id_Persona into pastorMiembros
                             from pastorMiembro in pastorMiembros.DefaultIfEmpty()
                             where s.sec_Id_Sector == sec_Id_Sector
                             select new
                             {
                                 pem_Id_Pastor = s.pem_Id_Pastor,
                                 pem_Id_Ministro = pem.pem_Id_Ministro,
                                 pem_Nombre = pem.pem_Nombre,
                                 pem_emailIECE = pem.pem_emailIECE,
                                 pem_email_Personal = pem.pem_email_Personal,
                                 pem_Grado_Ministerial = pem.pem_Grado_Ministerial
                             }).ToList();

                //Consulta de la Tabla de Personal Ministerial al Secretario de Sector.
                var secretario = (from s in context.Sector
                                  join pem in context.Personal_Ministerial on s.pem_Id_Secretario equals pem.pem_Id_Ministro
                                  join p in context.Persona on pem.per_Id_Miembro equals p.per_Id_Persona into pastorMiembros
                                  from pm in pastorMiembros.DefaultIfEmpty()
                                  where s.sec_Id_Sector == sec_Id_Sector
                                  select new
                                  {
                                      Secretario_Id = s.pem_Id_Secretario,
                                      pem_Id_Ministro = pem.pem_Id_Ministro,
                                      pem_Nombre = pem.pem_Nombre,
                                      pem_emailIECE = pem.pem_emailIECE,
                                      pem_email_Personal = pem.pem_email_Personal,
                                      pem_Grado_Ministerial = pem.pem_Grado_Ministerial,
                                      Id_Persona = pm != null ? pm.per_Id_Persona : 0,
                                      per_Nombre = pm != null ? (pm.per_Nombre + ' ' + pm.per_Apellido_Paterno + ' ' + (pm.per_Apellido_Materno != null ? pm.per_Apellido_Materno : "")):""
                                  }).ToList();

                //Consulta de la Tabla de Personal Ministerial al Tesorero de Sector.
                var tesorero = (from s in context.Sector
                                  join pem in context.Personal_Ministerial on s.pem_Id_Tesorero equals pem.pem_Id_Ministro
                                join p in context.Persona on pem.per_Id_Miembro equals p.per_Id_Persona into pastorMiembros
                                from pm in pastorMiembros.DefaultIfEmpty()
                                where s.sec_Id_Sector == sec_Id_Sector
                                  select new
                                  {
                                      Secretario_Id = s.pem_Id_Secretario,
                                      pem_Id_Ministro = pem.pem_Id_Ministro,
                                      pem_Nombre = pem.pem_Nombre,
                                      pem_emailIECE = pem.pem_emailIECE,
                                      pem_email_Personal = pem.pem_email_Personal,
                                      pem_Grado_Ministerial = pem.pem_Grado_Ministerial,
                                      per_Id_Persona = pm.per_Id_Persona,
                                      per_Nombre = pm.per_Nombre + ' ' + pm.per_Apellido_Paterno + ' ' + (pm.per_Apellido_Materno != null ? pm.per_Apellido_Materno : "")
                                  }).ToList();

                return Ok(
                    new
                    {
                        status = "success",
                        pastor = pastor,
                        secretario=secretario,
                        tesorero = tesorero
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


    }
}
