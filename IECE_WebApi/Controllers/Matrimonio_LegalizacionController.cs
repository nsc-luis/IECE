using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Matrimonio_LegalizacionController : ControllerBase
    {
        private readonly AppDbContext context;

        public Matrimonio_LegalizacionController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/Matrimonio_Legalizacion
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Matrimonio_LegalizacionController>> Get()
        {
            try
            {
                return Ok(context.Matrimonio_Legalizacion.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Matrimonio_Legalizacion/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {
            Matrimonio_Legalizacion matLegal = new Matrimonio_Legalizacion();
            try
            {
                matLegal = context.Matrimonio_Legalizacion.FirstOrDefault(mat => mat.mat_Id_MatrimonioLegalizacion == id);
                return Ok(matLegal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: /api/Matrimonio_Legalizacion/GetMujeresPorSectorParaLegalizacion/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetMujeresPorSectorParaLegalizacion(int idSector)
        {
            try
            {
                var query = (from p in context.Persona
                         where p.per_Categoria == "ADULTO_MUJER"
                         && p.sec_Id_Sector == idSector
                         && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
                         select new
                         {
                             p.per_Id_Persona,
                             p.per_Nombre,
                             p.per_Apellido_Paterno,
                             p.per_Apellido_Materno,
                             p.per_Categoria,
                             p.sec_Id_Sector
                         }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        mujeresParaLegalizacion = query
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        messange = ex.Message
                    }
                );
            }
        }

        // GET: /api/Matrimonio_Legalizacion/GetMujeresPorSectorParaMatrimonio/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetMujeresPorSectorParaMatrimonio(int idSector)
        {
            try
            {
                var query = (from p in context.Persona
                             where p.per_Categoria == "JOVEN_MUJER"
                             && p.sec_Id_Sector == idSector
                             && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Categoria,
                                 p.sec_Id_Sector
                             }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        mujeresParaMatrimonio = query
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        messange = ex.Message
                    }
                );
            }
        }
        
        // GET: /api/Matrimonio_Legalizacion/GetHombresPorSectorParaLegalizacion/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetHombresPorSectorParaLegalizacion(int idSector)
        {
            try
            {
                var query = (from p in context.Persona
                             where p.per_Categoria == "ADULTO_HOMBRE"
                             && p.sec_Id_Sector == idSector
                             && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Categoria,
                                 p.sec_Id_Sector
                             }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        hombresParaLegalizacion = query
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        messange = ex.Message
                    }
                );
            }
        }

        // GET: /api/Matrimonio_Legalizacion/GetHombresPorSectorParaMatrimonio/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetHombresPorSectorParaMatrimonio(int idSector)
        {
            try
            {
                var query = (from p in context.Persona
                             where p.per_Categoria == "JOVEN_HOMBRE"
                             && p.sec_Id_Sector == idSector
                             && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Categoria,
                                 p.sec_Id_Sector
                             }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        hombresParaMatrimonio = query
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        messange = ex.Message
                    }
                );
            }
        }

        // POST: api/Matrimonio_Legalizacion
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] Matrimonio_Legalizacion matLegal)
        {
            try
            {
                matLegal.Fecha_Registro = fechayhora;
                matLegal.usu_Id_Usuario = 1;
                context.Matrimonio_Legalizacion.Add(matLegal);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = true,
                        nvoMatLegal = matLegal.mat_Id_MatrimonioLegalizacion
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest
                (
                    new
                    {
                        status = false,
                        message = ex.Message
                    }
                );
            }
        }

        // PUT: api/Matrimonio_Legalizacion/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Matrimonio_Legalizacion matLegal)
        {
            try
            {
                matLegal.Fecha_Registro = fechayhora;
                matLegal.usu_Id_Usuario = 1;
                context.Entry(matLegal).State = EntityState.Modified;
                context.SaveChanges();
                return Ok(
                    new
                    {
                        status = "success",
                        mensaje = "Datos guardados satisfactoriamente."
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

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Delete(int id)
        {
            // ELIMINA REGISTRO DEL MATRIMONIO/LEGALIZACION
            Matrimonio_Legalizacion matLegal = new Matrimonio_Legalizacion();
            matLegal = context.Matrimonio_Legalizacion.FirstOrDefault(mat => mat.mat_Id_MatrimonioLegalizacion == id);
            if (matLegal != null)
            {
                context.Matrimonio_Legalizacion.Remove(matLegal);
                context.SaveChanges();
                return Ok(
                        new
                        {
                            status = "success",
                            message = "Se eliminó el registro de la persona con todos sus datos."
                        }
                    );
            }
            else
            {
                return Ok(
                    new
                    {
                        status = "error",
                        message = "No se encontro registro o no pudo eliminarse."
                    }
                );
            }
        }
    }
}
