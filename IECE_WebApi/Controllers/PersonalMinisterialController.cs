using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalMinisterialController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonalMinisterialController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Personal_Ministerial
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

        // GET: api/Personal_Ministerial
        [HttpGet("{pem_Id_Ministro}")]
        [EnableCors("AllowOrigin")]
        public Personal_Ministerial Get(int pem_Id_Ministro)
        {
            var ministro = context.Personal_Ministerial.FirstOrDefault(pem => pem.pem_Id_Ministro == pem_Id_Ministro);
            return ministro;
        }
        }
}