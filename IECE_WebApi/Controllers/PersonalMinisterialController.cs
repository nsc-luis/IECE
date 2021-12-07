using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonalMinisterialController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonalMinisterialController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Personal_Ministerial
        // METODO PARA LISTAR PERSONAL MINISTERIAL
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult Get()
        {
            try
            {
                var query = (from pem in context.Personal_Ministerial
                             select new
                             {
                                 pem_Id_Ministro = pem.pem_Id_Ministro,
                                 pem_Activo = pem.pem_Activo,
                                 per_Id_Miembro = pem.per_Id_Miembro,
                                 pem_Nombre = pem.pem_Nombre,
                                 sec_Id_Congregacion = pem.sec_Id_Congregacion,
                                 pem_En_Permiso = pem.pem_En_Permiso,
                                 pem_emailIECE = pem.pem_emailIECE,
                                 pem_email_Personal = pem.pem_email_Personal,
                                 pem_Grado_Ministerial = pem.pem_Grado_Ministerial
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        personalMinisterial = query
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

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER LOS DISTRITOS/SECTORES CON ACCESO SEGUN ID DEL MINISTRO
        [Route("[action]/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetAlcancePastoralByMinistro(int idMinistro)
        {
            var query = (from d in context.Distrito
                         where d.pem_Id_Obispo == idMinistro && d.dis_Activo == true
                         orderby d.dis_Numero ascending, d.dis_Tipo_Distrito ascending
                         select new
                         {
                             d.dis_Id_Distrito,
                             d.dis_Alias,
                             d.dis_Tipo_Distrito,
                             d.dis_Numero
                         }).ToList();

            if (query.Count > 0)
            {
                return Ok(new
                {
                    status = "success",
                    obispo = true,
                    datos = query
                });
            }
            else if (query.Count == 0)
            {
                var query2 = (from s in context.Sector
                              join d in context.Distrito
                              on s.dis_Id_Distrito equals d.dis_Id_Distrito
                              where s.pem_Id_Pastor == idMinistro && s.sec_Tipo_Sector == "SECTOR"
                              orderby d.dis_Numero ascending, s.sec_Numero ascending
                              select new
                              {
                                  d.dis_Id_Distrito,
                                  d.dis_Alias,
                                  d.dis_Tipo_Distrito,
                                  d.dis_Numero,
                                  s.sec_Alias,
                                  s.sec_Id_Sector,
                                  s.sec_Tipo_Sector
                              }).ToList();
                if (query2.Count > 0)
                {
                    return Ok(new
                    {
                        status = "success",
                        obispo = false,
                        datos = query2
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No se encontraron registros para el ID del Ministro."
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = "No se encontraron registros para el ID del Ministro."
                });
            }
        }

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER INFO DEL SECTOR SEGUN ID DEL MINISTRO
        [Route("[action]/{sec_Id_Congregacion}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMinistrosBySector(int sec_Id_Congregacion)
        {
            try
            {
                var query = (from pem in context.Personal_Ministerial
                             where pem.sec_Id_Congregacion == sec_Id_Congregacion
                             && pem.pem_Grado_Ministerial == "ANCIANO"
                             && pem.pem_Activo == true
                             orderby pem.pem_Nombre ascending
                             select new
                             {
                                 pem_Id_Ministro = pem.pem_Id_Ministro,
                                 pem_Activo = pem.pem_Activo,
                                 per_Id_Miembro = pem.per_Id_Miembro,
                                 pem_Nombre = pem.pem_Nombre,
                                 sec_Id_Congregacion = pem.sec_Id_Congregacion,
                                 pem_En_Permiso = pem.pem_En_Permiso,
                                 pem_emailIECE = pem.pem_emailIECE,
                                 pem_email_Personal = pem.pem_email_Personal,
                                 pem_Grado_Ministerial = pem.pem_Grado_Ministerial
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        personalMinisterial = query
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

        // GET: api/GetSectoresByMinistro/idMinistro
        [Route("[action]/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSectoresByMinistro(int idMinistro)
        {
            try
            {
                var query = (from s in context.Sector
                              join d in context.Distrito
                              on s.dis_Id_Distrito equals d.dis_Id_Distrito
                              where s.pem_Id_Pastor == idMinistro && s.sec_Tipo_Sector == "SECTOR"
                              orderby d.dis_Numero ascending, s.sec_Numero ascending
                              select new
                              {
                                  d.dis_Id_Distrito,
                                  d.dis_Alias,
                                  d.dis_Tipo_Distrito,
                                  d.dis_Numero,
                                  s.sec_Alias,
                                  s.sec_Id_Sector,
                                  s.sec_Tipo_Sector
                              }).ToList();
                return Ok(new
                {
                    status = "success",
                    sectores = query
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

        // GET: api/GetSectoresByDistritoMinistro/idDistrito/idMinistro
        [Route("[action]/{idDistrito}/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSectoresByDistritoMinistro(int idDistrito, int idMinistro)
        {
            try
            {
                var query = (from s in context.Sector
                             join d in context.Distrito
                             on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where s.dis_Id_Distrito == idDistrito && s.pem_Id_Pastor == idMinistro
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 d.dis_Tipo_Distrito,
                                 d.dis_Numero,
                                 s.sec_Alias,
                                 s.sec_Id_Sector,
                                 s.sec_Tipo_Sector
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    sectores = query
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

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER INFO DEL MINISTRO POR SU ID
        [HttpGet("{pem_Id_Ministro}")]
        [EnableCors("AllowOrigin")]
        public Personal_Ministerial Get(int pem_Id_Ministro)
        {
            var ministro = context.Personal_Ministerial.FirstOrDefault(pem => pem.pem_Id_Ministro == pem_Id_Ministro);
            return ministro;
        }
    }
}