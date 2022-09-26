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

        public class MatrimonioLegalizacionDomicilio
        {
            public virtual Matrimonio_Legalizacion matLegalEntity { get; set; }
            public virtual HogarDomicilio HogarDomicilioEntity { get; set; }
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

                    if (mt.per_Id_Persona_Hombre != 0)
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
                    if (mt.per_Id_Persona_Mujer != 0)
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
                             && (p.sec_Id_Sector == idSector && p.per_Activo == true)
                             // && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
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
                             && p.sec_Id_Sector == idSector && (p.per_Activo == true && !p.per_Estado_Civil.Contains("CASAD"))
                             // && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
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
                             && (p.sec_Id_Sector == idSector && p.per_Activo == true)
                             // && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Hombre).Contains(p.per_Id_Persona)
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
                             && p.sec_Id_Sector == idSector && (p.per_Activo == true && !p.per_Estado_Civil.Contains("CASAD"))
                             // && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Hombre).Contains(p.per_Id_Persona)
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

        // POST: api/Matrimonio_Legalizacion/AltaMatriminioLegalizacion/true/
        [Route("[action]/{boolNvoDomicilio}/{nvoEstado}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult AltaMatriminioLegalizacion([FromBody] MatrimonioLegalizacionDomicilio matLegalDom, bool boolNvoDomicilio, string nvoEstado = "")
        {
            try
            {
                Matrimonio_Legalizacion matLegal = matLegalDom.matLegalEntity;
                HogarDomicilio dom = matLegalDom.HogarDomicilioEntity;
                PersonaController pc = new PersonaController(context);
                int idNvoEstado = 0;

                var estados = (from e in context.Estado
                               where e.pais_Id_Pais == dom.pais_Id_Pais
                               select e).ToList();

                if (estados.Count < 1 && nvoEstado != "")
                {
                    var p = context.Pais.FirstOrDefault(pais => pais.pais_Id_Pais == dom.pais_Id_Pais);
                    var est = new Estado
                    {
                        est_Nombre_Corto = nvoEstado.Substring(0, 3),
                        est_Nombre = nvoEstado,
                        pais_Id_Pais = dom.pais_Id_Pais,
                        est_Pais = p.pais_Nombre_Corto
                    };
                    context.Estado.Add(est);
                    context.SaveChanges();
                    idNvoEstado = est.est_Id_Estado;
                }

                int ct = matLegal.mat_Tipo_Enlace == "MATRIMONIO" ? 21001 : 21102;
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                // VALIDACION INICIAL
                if (matLegal.mat_Nombre_Contrayente_Hombre_Foraneo == null
                    && matLegal.mat_Nombre_Contrayente_Mujer_Foraneo == null)
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Uno de los 2 miembros del enlace matrimonial/legalización debe pertenecer al sector local."
                    });
                }
                else
                {
                    var perHombre = (from p1 in context.Persona
                                     where p1.per_Id_Persona == matLegal.per_Id_Persona_Hombre
                                     select p1).ToList();

                    // GUARDA ESTATUS Y REGISTRO HISTORICO DEL HOMBRE
                    foreach (Persona p in perHombre)
                    {
                        p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                        p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                        p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                        p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                        p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                        p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                        p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                        p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                        p.per_Estado_Civil = "CASADO(A)";
                    }
                    context.SaveChanges();
                    hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, 11201, "Enlace matrimonial", matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

                    // GUARDA ESTATUS Y REGISTRO HISTORICO DE LA MUJER
                    var perMujer = (from p2 in context.Persona
                                    where p2.per_Id_Persona == matLegal.per_Id_Persona_Mujer
                                    select p2).ToList();
                    foreach (Persona p in perMujer)
                    {
                        p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                        p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                        p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                        p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                        p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                        p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                        p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                        p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                        p.per_Estado_Civil = "CASADO(A)";
                    }
                    context.SaveChanges();
                    hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, 11201, "Enlace matrimonial", matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

                    // GUARDA REGISTRO EN TABLA MATRIMONIO_LEGALIZACION
                    matLegal.Fecha_Registro = matLegal.mat_Fecha_Boda_Eclesiastica;
                    context.Matrimonio_Legalizacion.Add(matLegal);
                    context.SaveChanges();

                    // GUARDA REGISTRO HISTORICO DE MATRIMONIO_LEGALIZACION
                    if (matLegal.per_Id_Persona_Hombre == 0 && matLegal.per_Id_Persona_Mujer > 0)
                    {
                        hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, ct, "", matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }
                    else if (matLegal.per_Id_Persona_Mujer == 0 && matLegal.per_Id_Persona_Hombre > 0)
                    {
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct, "", matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }
                    else
                    {
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct, "", matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }

                    // AGREGAR NUEVO HOGAR
                    if (boolNvoDomicilio)
                    {
                        // AGREGANDO HOGAR
                        if (estados.Count < 1 && nvoEstado != "")
                        {
                            dom.est_Id_Estado = idNvoEstado;
                        }

                        context.HogarDomicilio.Add(dom);
                        context.SaveChanges();

                        // AGREGAR REGISTRO HISTORICO DEL NUEVO HOGAR
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, 31001, "", fechayhora, dom.usu_Id_Usuario);

                        // AGREGANDO PERSONAS AL NUEVO HOGAR
                        var hph = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perHombre[0].per_Id_Persona);
                        hph.hp_Jerarquia = 1;
                        hph.hd_Id_Hogar = dom.hd_Id_Hogar;
                        context.Hogar_Persona.Update(hph);
                        context.SaveChanges();

                        // RESTRUCTURA JERARQUIAS DEL HOGAR ANTERIOR DEL HOMBRE
                        pc.RestructuraJerarquiasBaja(perHombre[0].per_Id_Persona);

                        var hpm = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perMujer[0].per_Id_Persona);
                        hpm.hp_Jerarquia = 2;
                        hpm.hd_Id_Hogar = dom.hd_Id_Hogar;
                        context.Hogar_Persona.Update(hpm);
                        context.SaveChanges();

                        // RESTRUCTURA JERARQUIAS DEL HOGAR ANTERIOR LA MUJER
                        pc.RestructuraJerarquiasBaja(perMujer[0].per_Id_Persona);
                    }

                    return Ok(new
                    {
                        status = "success",
                        nvoMatLegal = matLegal.mat_Id_MatrimonioLegalizacion
                    });
                }

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
        public ActionResult Put([FromBody] Matrimonio_Legalizacion matLegal, int id)
        {
            try
            {
                var perHombre = (from p1 in context.Persona
                                 where p1.per_Id_Persona == matLegal.per_Id_Persona_Hombre
                                 select p1).ToList();
                foreach (Persona p in perHombre)
                {
                    p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                    p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                    p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                    p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                    p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                    p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                    p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                    p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                }
                context.SaveChanges();

                var perMujer = (from p2 in context.Persona
                                where p2.per_Id_Persona == matLegal.per_Id_Persona_Mujer
                                select p2).ToList();
                foreach (Persona p in perMujer)
                {
                    p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                    p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                    p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                    p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                    p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                    p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                    p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                    p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                }
                context.SaveChanges();

                matLegal.Fecha_Registro = fechayhora;
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
