using System;
using System.Collections.Generic;
using System.Linq;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
            public bool boolNvoDomicilio { get; set; }
            public string nvoEstado { get; set; }
            public string sectorAlias { get; set; }
            public bool viviranEnLocalidad { get; set; }
        }

        // MIEMBROS DEL MATRIMONIO / LEGALIZACION
        private void MiembrosDelMatLegal(int per_Id_Persona, int hd_Id_Hogar, int sec_Id_Sector, DateTime fecha, int usu_Id_Usuario)
        {
           // instancia historial controller
           Historial_Transacciones_EstadisticasController hc = new Historial_Transacciones_EstadisticasController(context);
           // instancia persona controller
           PersonaController pc = new PersonaController(context);

            // obtener id de hogar desde id de persona
            var hpersona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == per_Id_Persona);

            // obtener miembros del hogar
            var miembros = (from hp in context.Hogar_Persona
                            join p in context.Persona on hp.per_Id_Persona equals p.per_Id_Persona
                            where hp.hd_Id_Hogar == hpersona.hd_Id_Hogar
                            select p).ToList();

            // cuenta los bautizados del hogar viejo
            int contador = 0;
            foreach (var m in miembros)
            {
                var persona = context.Persona.FirstOrDefault(p => p.per_Id_Persona == m.per_Id_Persona);
                contador = persona.per_Bautizado ? contador + 1 : contador + 0;
            }

            // Si solo hay 1 Bautizado
            if (contador == 1)
            {
                // se actualiza registro de Todas las pesonas no bautizadas para registro en el Nuevo Hogar
                foreach (var m in miembros)
                {
                    if (m.per_Id_Persona != per_Id_Persona) {
                        var qp = context.Hogar_Persona.FirstOrDefault(p => p.per_Id_Persona == m.per_Id_Persona);
                        qp.hd_Id_Hogar = hd_Id_Hogar;
                        context.Hogar_Persona.Update(qp);
                        context.SaveChanges();

                        // se genera registro historio como cambio de domicilio
                        //hc.RegistroHistorico(qp.per_Id_Persona, sec_Id_Sector, 12103, "", fecha, usu_Id_Usuario);

                        // restructura jerarquias
                        pc.RestructuraJerarquiasAlta(qp.per_Id_Persona, 3);
                    }
                }
                // se genera registro historio de baja el hogar anterior
                var hd = context.HogarDomicilio.FirstOrDefault(h => h.hd_Id_Hogar == hpersona.hd_Id_Hogar);
                hd.hd_Activo = false;
                context.HogarDomicilio.Update(hd);
                context.SaveChanges();

                // se genera registro historico de la baja del hogar
                hc.RegistroHistorico(per_Id_Persona, sec_Id_Sector, 31102, "", fecha, usu_Id_Usuario);
            }
            else
            {
                // solo baja de la persona del hogar anterior
                pc.RestructuraJerarquiasBaja(per_Id_Persona);
            }
        }

        // ASEGURA JERARQUIAS
        private void AseguraJerarquias(int hd_Id_Hogar)
        {
            // OBTIENE LOS MIEMBROS DEL HOGAR y los ordena por Jerarquía
            List<Hogar_Persona> miembrosDelHogar = (from hp in context.Hogar_Persona
                                                    where hp.hd_Id_Hogar == hd_Id_Hogar
                                                    orderby hp.hp_Jerarquia
                                                    select hp).ToList();
            //Renumera las Jerarquías empezando en 1
            int i = 1;
            foreach (Hogar_Persona m in miembrosDelHogar)
            {
                m.hp_Jerarquia = i;
                context.Hogar_Persona.Update(m);
                context.SaveChanges();
                i = i + 1;
            }
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
                             where p.per_Categoria == "ADULTO_MUJER" && p.per_Bautizado
                             && p.sec_Id_Sector == idSector && p.per_Activo == true
                             && p.per_Estado_Civil.Contains("CASAD")
                             // && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Apellido_Casada,
                                 apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
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
                             && p.per_Activo == true && !(p.per_Estado_Civil.Contains("CASAD") || p.per_Estado_Civil.Contains("CONCUBINATO"))
                             && p.sec_Id_Sector == idSector && p.per_Bautizado 
                             // && !(from mat in context.Matrimonio_Legalizacion select mat.per_Id_Persona_Mujer).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Apellido_Casada,
                                 apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
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
                             where p.per_Categoria == "ADULTO_HOMBRE" && p.per_Bautizado
                             && p.sec_Id_Sector == idSector && p.per_Activo == true 
                             && p.per_Estado_Civil.Contains("CASAD")
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
                             where 
                             (p.per_Categoria == "JOVEN_HOMBRE" || p.per_Categoria == "ADULTO_HOMBRE")
                              && p.per_Activo == true 
                              && !(p.per_Estado_Civil.Contains("CASAD")|| p.per_Estado_Civil.Contains("CONCUB"))
                             && p.sec_Id_Sector == idSector 
                             && p.per_Bautizado 
                             //&& (p.per_Activo == true && !p.per_Estado_Civil.Contains("CASAD"))
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

        //// POST: api/Matrimonio_Legalizacion/AltaMatriminioLegalizacion/true/elNvoEstado
        //[Route("[action]")]
        //[HttpPost]
        //[EnableCors("AllowOrigin")]
        //public ActionResult AltaMatriminioLegalizacion([FromBody] MatrimonioLegalizacionDomicilio matLegalDom)

        //{
        //    Matrimonio_Legalizacion matLegal = matLegalDom.matLegalEntity;
        //    HogarDomicilio dom = matLegalDom.HogarDomicilioEntity;
        //    PersonaController pc = new PersonaController(context);
        //    int idNvoEstado = 0;
        //    try
        //    {
        //        var estados = (from e in context.Estado
        //                       where e.pais_Id_Pais == dom.pais_Id_Pais
        //                       select e).ToList();

        //        //Si se agregará un NUevo Estado en a la Tabla
        //        if (matLegalDom.nvoEstado != "" && dom.est_Id_Estado == 0)
        //            {
        //            var p = context.Pais.FirstOrDefault(pais => pais.pais_Id_Pais == dom.pais_Id_Pais);
        //            var est = new Estado
        //            {
        //                est_Nombre_Corto = matLegalDom.nvoEstado.Substring(0, 3),
        //                est_Nombre = matLegalDom.nvoEstado,
        //                pais_Id_Pais = dom.pais_Id_Pais,
        //                est_Pais = p.pais_Nombre_Corto
        //            };
        //            context.Estado.Add(est);
        //            context.SaveChanges();
        //            idNvoEstado = est.est_Id_Estado;
        //        }

        //        int ct = matLegal.mat_Tipo_Enlace == "MATRIMONIO" ? 21001 : 21102;
        //        Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);

        //        // VALIDACION INICIAL - Si los 2 campos vienen vacíos
        //        if (matLegal.mat_Nombre_Contrayente_Hombre_Foraneo == null
        //            && matLegal.mat_Nombre_Contrayente_Mujer_Foraneo == null)
        //        {
        //            return Ok(new
        //            {
        //                status = "error",
        //                mensaje = "Uno de los 2 miembros del enlace Matrimonial/Legalización debe pertenecer al Sector local."
        //            });
        //        }
        //        else
        //        {
        //            // CONSULTA Y TRAE LOS DATOS DE CADA CONYUGE
        //            var perHombre = (from p1 in context.Persona
        //                             where p1.per_Id_Persona == matLegal.per_Id_Persona_Hombre
        //                             select p1).ToList();
        //            var perMujer = (from p2 in context.Persona
        //                            where p2.per_Id_Persona == matLegal.per_Id_Persona_Mujer
        //                            select p2).ToList();

        //            var contrayenteHombre = "";
        //            var contrayenteMujer = "";

        //            // GUARDA ESTATUS Y REGISTRO HISTORICO DEL HOMBRE
        //            foreach (Persona p in perHombre)
        //            {
        //                contrayenteMujer = $"{perMujer[0].per_Nombre} {perMujer[0].per_Apellido_Paterno} {perMujer[0].per_Apellido_Materno}";
        //                p.per_Nombre_Conyuge = $"{perMujer[0].per_Nombre} {perMujer[0].per_Apellido_Paterno} {perMujer[0].per_Apellido_Materno}";
        //                p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
        //                p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
        //                p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
        //                p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
        //                p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
        //                p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
        //                p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
        //                p.per_Registro_Civil = matLegal.mat_Registro_Civil;
        //                p.per_Lugar_Boda_Eclesiastica = matLegalDom.sectorAlias;
        //                p.per_Estado_Civil = "CASADO(A)";
        //            }
        //            context.SaveChanges();
        //            hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, 11201, "POR " + matLegal.mat_Tipo_Enlace, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

        //            // GUARDA ESTATUS Y REGISTRO HISTORICO DE LA MUJER
        //            foreach (Persona p in perMujer)
        //            {
        //                contrayenteHombre = $"{perHombre[0].per_Nombre} {perHombre[0].per_Apellido_Paterno} {perHombre[0].per_Apellido_Materno}";
        //                p.per_Nombre_Conyuge = $"{perHombre[0].per_Nombre} {perHombre[0].per_Apellido_Paterno} {perHombre[0].per_Apellido_Materno}";
        //                p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
        //                p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
        //                p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
        //                p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
        //                p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
        //                p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
        //                p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
        //                p.per_Registro_Civil = matLegal.mat_Registro_Civil;
        //                p.per_Lugar_Boda_Eclesiastica = matLegalDom.sectorAlias;
        //                p.per_Estado_Civil = "CASADO(A)";
        //            }
        //            context.SaveChanges();
        //            hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, 11201, "POR " + matLegal.mat_Tipo_Enlace, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

        //            // GUARDA REGISTRO EN TABLA MATRIMONIO_LEGALIZACION
        //            matLegal.Fecha_Registro = matLegal.mat_Fecha_Boda_Eclesiastica;
        //            context.Matrimonio_Legalizacion.Add(matLegal);
        //            context.SaveChanges();


        //            // GUARDA REGISTRO HISTORICO DE MATRIMONIO_LEGALIZACION
        //            if (matLegal.per_Id_Persona_Hombre == 0 && matLegal.per_Id_Persona_Mujer > 0)
        //            {
        //                hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, ct, contrayenteMujer, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
        //            }
        //            else if (matLegal.per_Id_Persona_Mujer == 0 && matLegal.per_Id_Persona_Hombre > 0)
        //            {
        //                hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct, contrayenteHombre, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
        //            }
        //            else
        //            {
        //                hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct,contrayenteHombre + " Y " + contrayenteMujer, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
        //            }

        //            // AGREGAR NUEVO HOGAR
        //            if (matLegalDom.boolNvoDomicilio)
        //            {
        //                // AGREGANDO HOGAR
        //                if (estados.Count < 1 && matLegalDom.nvoEstado != null)
        //                {
        //                    dom.est_Id_Estado = idNvoEstado;
        //                }
        //                dom.hd_Activo = true;
        //                context.HogarDomicilio.Add(dom);
        //                context.SaveChanges();

        //                // AGREGAR REGISTRO HISTORICO DEL NUEVO HOGAR
        //                hte.RegistroHistorico(
        //                    perHombre[0].per_Id_Persona, 
        //                    perHombre[0].sec_Id_Sector, 
        //                    31001,
        //                    $"{perHombre[0].per_Nombre} {perHombre[0].per_Apellido_Paterno} {perHombre[0].per_Apellido_Materno}",
        //                    matLegal.mat_Fecha_Boda_Eclesiastica, 
        //                    dom.usu_Id_Usuario
        //                );

        //                // MANEJO DE LOS MIEMBROS DEL HOGAR DEL HOMBRE QUE SE CASA
        //                MiembrosDelMatLegal(perHombre[0].per_Id_Persona, dom.hd_Id_Hogar, matLegal.sec_Id_Sector, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

        //                // AGREGANDO HOMBRE CASADO AL NUEVO HOGAR
        //                var hph = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perHombre[0].per_Id_Persona);
        //                hph.hp_Jerarquia = 1;
        //                hph.hd_Id_Hogar = dom.hd_Id_Hogar;
        //                context.Hogar_Persona.Update(hph);
        //                context.SaveChanges();

        //                // MANEJO DE LOS MIEMBROS DEL HOGAR DE LA MUJER QUE SE CASA
        //                MiembrosDelMatLegal(perMujer[0].per_Id_Persona, dom.hd_Id_Hogar, matLegal.sec_Id_Sector, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

        //                // AGREGANDO MUJER CASADA AL NUEVO HOGAR
        //                var hpm = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perMujer[0].per_Id_Persona);
        //                hpm.hp_Jerarquia = 2;
        //                hpm.hd_Id_Hogar = dom.hd_Id_Hogar;
        //                context.Hogar_Persona.Update(hpm);
        //                context.SaveChanges();

        //                // ASEGURANDO JERARQUIAS
        //                AseguraJerarquias(dom.hd_Id_Hogar);
        //            }

        //            return Ok(new
        //            {
        //                status = "success",
        //                nvoMatLegal = matLegal.mat_Id_MatrimonioLegalizacion
        //            });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = "error",
        //            message = ex
        //        });
        //    }
        //}

        // POST: api/Matrimonio_Legalizacion/AltaMatrimonio/true/elNvoEstado
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult AltaMatrimonio([FromBody] MatrimonioLegalizacionDomicilio matLegalDom)

        {
            Matrimonio_Legalizacion matLegal = matLegalDom.matLegalEntity;
            HogarDomicilio dom = matLegalDom.HogarDomicilioEntity;
            PersonaController pc = new PersonaController(context);
            int idNvoEstado = 0;

            // Lógica para determinar si hay 1 COntrayente Foraneo o si los 2 son locales.
            bool hombreLocal = matLegal.per_Id_Persona_Hombre > 0  ? true : false;
            bool mujerLocal =  matLegal.per_Id_Persona_Mujer > 0 ? true : false;
            string contrayenteHombre = "Hombre";
            string contrayenteMujer = "Mujer";

            try
            {
                int ct = 21001;
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);

                // VALIDACION INICIAL - Si los 2 campos vienen vacíos
                if (!hombreLocal  & !mujerLocal)
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Uno de los 2 contrayentes debe pertenecer al Sector Local."
                    });
                }
                else //Si Ambos o por lo menos uno de los dos Contrayentes es de la Localidad
                {
                    // CONSULTA Y TRAE LOS DATOS DE CADA CONYUGE SEAN LOCALES O UNO FORANEO
                    var perHombre = (from p1 in context.Persona
                                     where p1.per_Id_Persona == matLegal.per_Id_Persona_Hombre
                                     select p1).ToList();
                    var perMujer = (from p2 in context.Persona
                                    where p2.per_Id_Persona == matLegal.per_Id_Persona_Mujer
                                    select p2).ToList();

                    int Id_Hogar_Mujer = 0;
                    int Id_Hogar_Hombre = 0;


                    //Determinará el Nombre de la Contrayente Mujer y su Id de Hogar Actual
                    if (perMujer.Count > 0)
                    {
                        contrayenteMujer = $"{perMujer[0].per_Nombre} {perMujer[0].per_Apellido_Paterno} {perMujer[0].per_Apellido_Materno}";
                        // obtener id de hogar desde id de persona
                        var hpersona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perMujer[0].per_Id_Persona);
                        Id_Hogar_Mujer = hpersona.hd_Id_Hogar;
                    }
                    else
                    {
                        contrayenteMujer = matLegalDom.matLegalEntity.mat_Nombre_Contrayente_Mujer_Foraneo;
                    }

                    //Determinará el Nombre del Contrayente Hombre y su Id de Hogar Actual
                    if (perHombre.Count > 0)
                    {
                        contrayenteHombre = $"{perHombre[0].per_Nombre} {perHombre[0].per_Apellido_Paterno} {perHombre[0].per_Apellido_Materno}";
                        // obtener id de hogar desde id de persona
                        var hpersona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perHombre[0].per_Id_Persona);
                        Id_Hogar_Hombre = hpersona.hd_Id_Hogar;
                    }
                    else
                    {
                        contrayenteHombre = matLegalDom.matLegalEntity.mat_Nombre_Contrayente_Hombre_Foraneo;
                    }


                    // GUARDA ESTATUS Y REGISTRO HISTORICO DEL HOMBRE
                    foreach (Persona p in perHombre)
                    {
                        p.per_Nombre_Conyuge = contrayenteMujer;
                        p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                        p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                        p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                        p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                        p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                        p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                        p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                        p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                        p.per_Lugar_Boda_Eclesiastica = matLegalDom.sectorAlias;
                        p.per_Estado_Civil = "CASADO(A)";
                        p.per_Categoria = "ADULTO_HOMBRE";

                        context.SaveChanges();
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, 11201, "POR " + matLegal.mat_Tipo_Enlace, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }


                    // GUARDA ESTATUS Y REGISTRO HISTORICO DE LA MUJER
                    foreach (Persona p in perMujer)
                    {
                        p.per_Nombre_Conyuge = contrayenteHombre;
                        p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                        p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                        p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                        p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                        p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                        p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                        p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                        p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                        p.per_Lugar_Boda_Eclesiastica = matLegalDom.sectorAlias;
                        p.per_Apellido_Casada = matLegal.mat_Apellido_Casada;
                        p.per_Estado_Civil = "CASADO(A)";
                        p.per_Categoria = "ADULTO_MUJER";


                        //Se genera el campo de per_Nombre_Completo Sin Acentos que sirve para hacer búsquedas
                        var apellidoCasada = p.per_Apellido_Casada != null && p.per_Apellido_Casada != "" ? p.per_Apellido_Casada + "*" : "";
                        var apellidoPrincipal = (apellidoCasada != "") ? (apellidoCasada + " " + p.per_Apellido_Paterno) : p.per_Apellido_Paterno;
                        var apellidoMaterno = p.per_Apellido_Materno != null ? p.per_Apellido_Materno : "";
                        var nombreCompleto = p.per_Nombre + " " + apellidoPrincipal + " " + apellidoMaterno;
                        nombreCompleto = ManejoDeApostrofes.QuitarApostrofe2(nombreCompleto);
                        p.per_Nombre_Completo = nombreCompleto;


                        context.SaveChanges();
                        hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, 11201, "POR " + matLegal.mat_Tipo_Enlace, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }


                    // GUARDA REGISTRO EN TABLA MATRIMONIO_LEGALIZACION
                    matLegal.Fecha_Registro = matLegal.mat_Fecha_Boda_Eclesiastica;
                    context.Matrimonio_Legalizacion.Add(matLegal);
                    context.SaveChanges();


                    // GUARDA REGISTRO HISTORICO DE MATRIMONIO_LEGALIZACION
                    if (matLegal.per_Id_Persona_Hombre == 0 && matLegal.per_Id_Persona_Mujer > 0)
                    {
                        hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, ct, contrayenteMujer + " Y " + contrayenteHombre, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }
                    else if (matLegal.per_Id_Persona_Mujer == 0 && matLegal.per_Id_Persona_Hombre > 0)
                    {
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct, contrayenteHombre + " Y " + contrayenteMujer, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }
                    else if (matLegal.per_Id_Persona_Mujer > 0 && matLegal.per_Id_Persona_Hombre > 0)
                    {
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct, contrayenteHombre + " Y " + contrayenteMujer, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }

                    //----------------------------------------------HASTA AQUÍ SON LOS REGISTROS COMUNES--------------------------------------------------------------------------------------------------

                    if (matLegalDom.viviranEnLocalidad) //Si La Nueva Familia se instalará en la Localidad
                    {

                        // AGREGAR NUEVO HOGAR 
                        if (matLegalDom.boolNvoDomicilio) //Si van a vincularse a un Hogar Nuevo
                        {
                            //Determina el nombre del Titular del Hogar que aparecerá en el Comentario del Registro Histórico de la Alta del Nuevo Hogar.
                            var contrayenteTitularDeHogar = "";
                            var Id_Titular_Hogar = 0;
                            if (!hombreLocal) //Si el Hombre no es Local la titular del Nuevo Hogar va ser la Mujer
                            {
                                contrayenteTitularDeHogar = matLegal.per_Id_Persona_Hombre != 0 ? contrayenteHombre : contrayenteMujer;
                                Id_Titular_Hogar = perMujer[0].per_Id_Persona;
                            }
                            else //Si ambos o por lo menos el Hombre el Local, el Hombre será el titular del Nuevo Hogar
                            {
                                contrayenteTitularDeHogar = contrayenteHombre;
                                Id_Titular_Hogar = perHombre[0].per_Id_Persona;
                            }

                            // EMPIEZA ALGORITMO PARA AGREGAR HOGAR
                            var estados = (from e in context.Estado
                                           where e.pais_Id_Pais == dom.pais_Id_Pais
                                           select e).ToList();

                            //Si aplica, se agregará un Nuevo Estado en a la Tabla
                            if (matLegalDom.nvoEstado != "" && dom.est_Id_Estado == 0)
                            {
                                var p = context.Pais.FirstOrDefault(pais => pais.pais_Id_Pais == dom.pais_Id_Pais);
                                var est = new Estado
                                {
                                    est_Nombre_Corto = matLegalDom.nvoEstado.Substring(0, 3),
                                    est_Nombre = matLegalDom.nvoEstado,
                                    pais_Id_Pais = dom.pais_Id_Pais,
                                    est_Pais = p.pais_Nombre_Corto
                                };
                                context.Estado.Add(est);
                                context.SaveChanges();
                                idNvoEstado = est.est_Id_Estado;
                            }

                            //S se dió de alta un Nuevo Estado toma el id del nuevo estado.
                            if (estados.Count < 1 && matLegalDom.nvoEstado != null)
                            {
                                dom.est_Id_Estado = idNvoEstado;
                            }

                            //Graba el Nuevo Hogar-Domicilio
                            dom.hd_Activo = true;
                            context.HogarDomicilio.Add(dom);
                            context.SaveChanges();

                            // REGISTRO HISTORICO DEL NUEVO HOGAR
                            hte.RegistroHistorico(
                                Id_Titular_Hogar,
                                matLegalDom.matLegalEntity.sec_Id_Sector,
                                31001,
                                contrayenteTitularDeHogar,
                                matLegal.mat_Fecha_Boda_Eclesiastica,
                                dom.usu_Id_Usuario
                            );
                        }


                        if (hombreLocal && (Id_Hogar_Hombre != matLegalDom.HogarDomicilioEntity.hd_Id_Hogar))
                        {
                            // REVISA SI ES EL UNICO BAUTIZADO EN EL HOGAR ACTUAL DEL HOMBRE y si es así, Asigna al Nuevo Hogar a los No Bautizados y Da de Baja el Hogar Anterior(actual)
                            MiembrosDelMatLegal(perHombre[0].per_Id_Persona, dom.hd_Id_Hogar, matLegal.sec_Id_Sector, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

                            // AGREGANDO HOMBRE CASADO AL NUEVO HOGAR
                            var hph = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perHombre[0].per_Id_Persona);
                            hph.hp_Jerarquia = 1;
                            hph.hd_Id_Hogar = dom.hd_Id_Hogar;
                            context.Hogar_Persona.Update(hph);
                            context.SaveChanges();

                            // ASEGURANDO JERARQUIAS EN EL NUEVO HOGAR
                            AseguraJerarquias(dom.hd_Id_Hogar);
                        }
                         
                        if (mujerLocal && (Id_Hogar_Mujer != matLegalDom.HogarDomicilioEntity.hd_Id_Hogar))
                        {

                            // REVISA SI ES EL UNICO BAUTIZADO EN EL HOGAR ACTUAL DE LA MUJER y si es así, Asigna al Nuevo Hogar a los No Bautizados y Da de Baja el Hogar Anterior(actual)
                            MiembrosDelMatLegal(perMujer[0].per_Id_Persona, dom.hd_Id_Hogar, matLegal.sec_Id_Sector, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);

                            // AGREGANDO MUJER CASADA AL NUEVO HOGAR
                            var hpm = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perMujer[0].per_Id_Persona);
                            hpm.hp_Jerarquia = !hombreLocal?1:2;
                            hpm.hd_Id_Hogar = dom.hd_Id_Hogar;
                            context.Hogar_Persona.Update(hpm);
                            context.SaveChanges();

                            // ASEGURANDO JERARQUIAS EN EL NUEVO HOGAR
                            AseguraJerarquias(dom.hd_Id_Hogar);
                        }
                    } //Si No pertenecerán a la Localidad, sus hogares anteriores y sus vinculaciones quedan intactas.
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
                    message = ex
                });
            }
        }


        // POST: api/Matrimonio_Legalizacion/AltaLegalizacion/true/elNvoEstado
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult AltaLegalizacion([FromBody] MatrimonioLegalizacionDomicilio matLegalDom)

        {
            Matrimonio_Legalizacion matLegal = matLegalDom.matLegalEntity;
            HogarDomicilio dom = matLegalDom.HogarDomicilioEntity;
            PersonaController pc = new PersonaController(context);

            // Lógica para determinar si hay 1 COntrayente Foraneo o si los 2 son locales.
            bool hombreLocal = matLegal.per_Id_Persona_Hombre > 0 ? true : false;
            bool mujerLocal = matLegal.per_Id_Persona_Mujer > 0 ? true : false;
            string contrayenteHombre = "Hombre";
            string contrayenteMujer = "Mujer";

            try
            {
                int ct = 21102;
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);

                // VALIDACION INICIAL - Si los 2 campos vienen vacíos
                if (!hombreLocal & !mujerLocal)
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Uno de los 2 contrayentes debe pertenecer al Sector Local."
                    });
                }
                else //Si Ambos o por lo menos uno de los dos Contrayentes es de la Localidad
                {
                    // CONSULTA Y TRAE LOS DATOS DE CADA CONYUGE SEAN LOCALES O UNO FORANEO
                    var perHombre = (from p1 in context.Persona
                                     where p1.per_Id_Persona == matLegal.per_Id_Persona_Hombre
                                     select p1).ToList();
                    var perMujer = (from p2 in context.Persona
                                    where p2.per_Id_Persona == matLegal.per_Id_Persona_Mujer
                                    select p2).ToList();

                    int Id_Hogar_Mujer = 0;
                    int Id_Hogar_Hombre = 0;


                    //Determinará el Nombre de la Contrayente Mujer y su Id de Hogar Actual
                    if (perMujer.Count > 0)
                    {
                        contrayenteMujer = $"{perMujer[0].per_Nombre} {perMujer[0].per_Apellido_Paterno} {perMujer[0].per_Apellido_Materno}";
                        // obtener id de hogar desde id de persona
                        var hpersona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perMujer[0].per_Id_Persona);
                        Id_Hogar_Mujer = hpersona.hd_Id_Hogar;
                    }
                    else
                    {
                        contrayenteMujer = matLegalDom.matLegalEntity.mat_Nombre_Contrayente_Mujer_Foraneo;
                    }

                    //Determinará el Nombre del Contrayente Hombre y su Id de Hogar Actual
                    if (perHombre.Count > 0)
                    {
                        contrayenteHombre = $"{perHombre[0].per_Nombre} {perHombre[0].per_Apellido_Paterno} {perHombre[0].per_Apellido_Materno}";
                        // obtener id de hogar desde id de persona
                        var hpersona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == perHombre[0].per_Id_Persona);
                        Id_Hogar_Hombre = hpersona.hd_Id_Hogar;
                    }
                    else
                    {
                        contrayenteHombre = matLegalDom.matLegalEntity.mat_Nombre_Contrayente_Hombre_Foraneo;
                    }


                    // GUARDA ESTATUS Y REGISTRO HISTORICO DEL HOMBRE
                    foreach (Persona p in perHombre)
                    {
                        p.per_Nombre_Conyuge = contrayenteMujer;
                        p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                        p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                        p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                        p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                        p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                        p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                        p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                        p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                        p.per_Lugar_Boda_Eclesiastica = matLegalDom.sectorAlias;                        
                        p.per_Estado_Civil = "CASADO(A)";

                        context.SaveChanges();
                        hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, 11201, "POR " + matLegal.mat_Tipo_Enlace, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }

                    // GUARDA ESTATUS Y REGISTRO HISTORICO DE LA MUJER
                    foreach (Persona p in perMujer)
                    {
                        p.per_Nombre_Conyuge = contrayenteHombre;
                        p.per_Cantidad_Hijos = matLegal.mat_Cantidad_Hijos;
                        p.per_Fecha_Boda_Civil = matLegal.mat_Fecha_Boda_Civil;
                        p.per_Fecha_Boda_Eclesiastica = matLegal.mat_Fecha_Boda_Eclesiastica;
                        p.per_Libro_Acta_Boda_Civil = matLegal.mat_Libro_Acta;
                        p.per_Nombre_Hijos = matLegal.mat_Nombre_Hijos;
                        p.per_Num_Acta_Boda_Civil = matLegal.mat_Numero_Acta;
                        p.per_Oficialia_Boda_Civil = matLegal.mat_Oficialia;
                        p.per_Registro_Civil = matLegal.mat_Registro_Civil;
                        p.per_Lugar_Boda_Eclesiastica = matLegalDom.sectorAlias;
                        p.per_Apellido_Casada = matLegal.mat_Apellido_Casada;
                        p.per_Estado_Civil = "CASADO(A)";

                        //Se genera el campo de per_Nombre_Completo Sin Acentos que sirve para hacer búsquedas
                        var apellidoCasada = p.per_Apellido_Casada != null && p.per_Apellido_Casada != "" ? p.per_Apellido_Casada + "*" : "";
                        var apellidoPrincipal = (apellidoCasada != "") ? (apellidoCasada + " " + p.per_Apellido_Paterno) : p.per_Apellido_Paterno;
                        var apellidoMaterno = p.per_Apellido_Materno != null ? p.per_Apellido_Materno : "";
                        var nombreCompleto = p.per_Nombre + " " + apellidoPrincipal + " " + apellidoMaterno;
                        nombreCompleto = ManejoDeApostrofes.QuitarApostrofe2(nombreCompleto);
                        p.per_Nombre_Completo = nombreCompleto;

                        context.SaveChanges();
                        hte.RegistroHistorico(perMujer[0].per_Id_Persona, perMujer[0].sec_Id_Sector, 11201, "POR " + matLegal.mat_Tipo_Enlace, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);
                    }


                    // GUARDA REGISTRO EN TABLA MATRIMONIO_LEGALIZACION
                    matLegal.Fecha_Registro = matLegal.mat_Fecha_Boda_Eclesiastica;
                    context.Matrimonio_Legalizacion.Add(matLegal);
                    context.SaveChanges();


                    // GUARDA REGISTRO HISTORICO DE MATRIMONIO_LEGALIZACION
                    hte.RegistroHistorico(perHombre[0].per_Id_Persona, perHombre[0].sec_Id_Sector, ct, contrayenteHombre + " Y " + contrayenteMujer, matLegal.mat_Fecha_Boda_Eclesiastica, matLegal.usu_Id_Usuario);


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
                    message = ex
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
