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

        private class ListaMatrimoniosLegalizaciones
        {
            public int mat_Id_MatrimonioLegalizacion { get; set; }
            public string mat_Tipo_Enlace { get; set; }
            public string mat_NombreConyugeHombre { get; set; }
            public string mat_NombreConyugeMujer { get; set; }
            public int sec_Numero { get; set; }
            public string sec_Alias { get; set; }
        }

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
                return Ok(new
                {
                    status = "success",
                    matrimonioLegalizacion = matLegal
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

        // GET: api/Matrimonio_Legalizacion/GetBySector/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetBySector(int idSector)
        {
            List<ListaMatrimoniosLegalizaciones> listaMatrimoniosLegalizaciones = new List<ListaMatrimoniosLegalizaciones>();
            try
            {
                var matLeg = (from mat in context.Matrimonio_Legalizacion
                              join sec in context.Sector
                              on mat.sec_Id_Sector equals sec.sec_Id_Sector
                              where mat.sec_Id_Sector == idSector
                              select new
                              {
                                  mat.mat_Id_MatrimonioLegalizacion,
                                  mat.mat_Tipo_Enlace,
                                  mat.per_Id_Persona_Hombre,
                                  mat.per_Id_Persona_Mujer,
                                  mat.mat_Nombre_Contrayente_Hombre_Foraneo,
                                  mat.mat_Nombre_Contrayente_Mujer_Foraneo,
                                  sec.sec_Numero,
                                  sec.sec_Alias
                              }).ToList();
                foreach (var mt in matLeg)
                {
                    // listaMatrimoniosLegalizaciones = new ListaMatrimoniosLegalizaciones
                    string NombreConyugeHombre = "";
                    string NombreConyugeMujer = "";

                    if (mt.per_Id_Persona_Hombre != null)
                    {
                        var qHombre = (from per in context.Persona
                                       where per.per_Id_Persona == mt.per_Id_Persona_Hombre
                                       select new
                                       {
                                           per.per_Nombre,
                                           per.per_Apellido_Paterno,
                                           per.per_Apellido_Materno
                                       }).ToList();
                        foreach (var qh in qHombre)
                        {
                            NombreConyugeHombre = qh.per_Nombre + " " + qh.per_Apellido_Paterno + " " + qh.per_Apellido_Materno;
                        }

                    }
                    else
                    {
                        NombreConyugeHombre = mt.mat_Nombre_Contrayente_Hombre_Foraneo;
                    }
                    if (mt.per_Id_Persona_Mujer != null)
                    {
                        var qMujer = (from per in context.Persona
                                      where per.per_Id_Persona == mt.per_Id_Persona_Mujer
                                      select new
                                      {
                                          per.per_Nombre,
                                          per.per_Apellido_Paterno,
                                          per.per_Apellido_Materno
                                      }).ToList();
                        foreach (var qm in qMujer)
                        {
                            NombreConyugeMujer = qm.per_Nombre + " " + qm.per_Apellido_Paterno + " " + qm.per_Apellido_Materno;
                        }
                    }
                    else
                    {
                        NombreConyugeMujer = mt.mat_Nombre_Contrayente_Mujer_Foraneo;
                    }
                    listaMatrimoniosLegalizaciones.Add(new ListaMatrimoniosLegalizaciones
                    {
                        mat_Id_MatrimonioLegalizacion = mt.mat_Id_MatrimonioLegalizacion,
                        mat_Tipo_Enlace = mt.mat_Tipo_Enlace,
                        mat_NombreConyugeHombre = NombreConyugeHombre,
                        mat_NombreConyugeMujer = NombreConyugeMujer,
                        sec_Numero = mt.sec_Numero,
                        sec_Alias = mt.sec_Alias
                    });
                }
                return Ok(new
                {
                    status = "success",
                    matrimoniosLegalizaciones = listaMatrimoniosLegalizaciones
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
                             where (p.per_Categoria == "JOVEN_MUJER" || p.per_Categoria == "ADULTO_MUJER")
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
                             && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Hombre).Contains(p.per_Id_Persona)
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
                             where (p.per_Categoria == "JOVEN_HOMBRE" || p.per_Categoria == "ADULTO_HOMBRE")
                             && p.sec_Id_Sector == idSector
                             && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Hombre).Contains(p.per_Id_Persona)
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
                context.Matrimonio_Legalizacion.Add(matLegal);
                context.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    nvoMatLegal = matLegal.mat_Id_MatrimonioLegalizacion
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    message = ex.Message
                });
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
