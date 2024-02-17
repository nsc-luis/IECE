using DocumentFormat.OpenXml.ExtendedProperties;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VisitanteController : ControllerBase
    {
        private readonly AppDbContext context;

        public VisitanteController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        public class VisitantesYSuSector
        {
            public virtual Visitante visitante { get; set; }
            public virtual Sector sector { get; set; }
        }
        public class VisitanteNota
        {
            public virtual Visitante visitante { get; set; }
            public string n_Nota { get; set; }
            public DateTime n_Fecha_Nota { get; set; }
        }

        public class BajaVisitante
        {
            public int vp_Id_Visitante { get; set; }
            public string vp_Nombre { get; set; }
            public string n_Nota { get; set; }
            public DateTime n_Fecha_Nota { get; set; }
        }

        private int OrdenaPrioridad(int idVisitante, int idSector, int prioridad)
        {
            /*
            if == 0
                if visitantes < 1
                    se asigna 1
                else 
                    if == 1
                        se asigna 1 y los demas se incrementan en 1
                    if != 1
                        se asigna como el ultimo de prioridad
            else != 0
                if visitantes < 1
                    se asigna 1
                else visitantes > 0
                    se asigna prioridad seleccionada
                    se consulta a partir de numero asignado y se incrementan en 1
            */
            var visitantes = context.Visitante.Where(v => v.sec_Id_Sector == idSector)
                .OrderBy(v => v.vp_Numero_Lista)
                .ToList();
            int numeroDeLista = 0;
            if (prioridad == 0) //Si viene sin especificar el Numero de Lista Deseado
            {
                if (visitantes.Count == 0) //Si no hay aun registrados visitantes de este Tipo en este Sector
                {
                    numeroDeLista = 1;
                }
                else //Si ya hay visitantes de este Tipo Registrados en ese Sector
                {
                    if (prioridad == 1) //Si se quiere ponerlo como prioridad 1
                    {
                        numeroDeLista = 1;
                        foreach (var frequenter in visitantes)
                        {
                            frequenter.vp_Numero_Lista++;
                            context.Visitante.Update(frequenter);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        numeroDeLista = visitantes.Count + 1;
                    }
                }
            }
            else
            {
                if (visitantes.Count == 0)
                {
                    numeroDeLista = 1;
                }
                else if (prioridad == 1)
                {
                    numeroDeLista = 1;
                    foreach (var frequenter in visitantes)
                    {
                        frequenter.vp_Numero_Lista++;
                        context.Visitante.Update(frequenter);
                        context.SaveChanges();
                    }
                }
                else if (idVisitante == 0)
                {
                    numeroDeLista = prioridad;
                    var frequenters = context.Visitante
                        .Where(v => v.vp_Numero_Lista >= prioridad && v.sec_Id_Sector == idSector)
                        .OrderBy(v => v.vp_Numero_Lista)
                        .ToList();
                    foreach (var frequenter in frequenters)
                    {
                        frequenter.vp_Numero_Lista++;
                        context.Visitante.Update(frequenter);
                        context.SaveChanges();
                    }
                }
                else
                {
                    numeroDeLista = prioridad;
                    var visitante = context.Visitante.FirstOrDefault(v => v.vp_Id_Visitante == idVisitante);
                    
                    if (prioridad  < visitante.vp_Numero_Lista)
                    {
                        // suman 1 en el rango
                        var eVisitantes = visitantes
                        .Where(v => v.vp_Numero_Lista >= prioridad && v.vp_Numero_Lista <= visitante.vp_Numero_Lista)
                        .ToList();
                        foreach (var eVisitante in eVisitantes)
                        {
                            eVisitante.vp_Numero_Lista++;
                            context.Visitante.Update(eVisitante);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        // restan 1 en el rango
                        var eVisitantes = visitantes
                        .Where(v => v.vp_Numero_Lista >= visitante.vp_Numero_Lista && v.vp_Numero_Lista <= prioridad)
                        .ToList();
                        foreach (var eVisitante in eVisitantes)
                        {
                            eVisitante.vp_Numero_Lista--;
                            context.Visitante.Update(eVisitante);
                            context.SaveChanges();
                        }
                    }
                }
            }
            return numeroDeLista;
        }

        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult VisitantesBySector(int sec_Id_Sector)
        {


            try
            {
                //var sector = context.Sector.FirstOrDefault(sec =>sec.sec_Id_Sector == sec_Id_Sector);
                //var visitantes = context.Visitante
                //    .Where(v => v.sec_Id_Sector == sec_Id_Sector && v.vp_Activo == true)
                //    .OrderByDescending(v => v.vp_Tipo_Visitante)
                //    .ThenBy(v => v.vp_Nombre)
                //    .ToList();

                var visitantes = (from V in context.Visitante
                                           join S in context.Sector on V.sec_Id_Sector equals S.sec_Id_Sector
                                           join D in context.Distrito on S.dis_Id_Distrito equals D.dis_Id_Distrito
                                           where V.sec_Id_Sector == sec_Id_Sector && V.vp_Activo == true
                                           orderby S.sec_Tipo_Sector descending,S.sec_Numero, V.vp_Nombre
                                           select new VisitantesYSuSector
                                           {
                                            sector = S,
                                            visitante = V
                                           }).ToList();
                return Ok(new
                {
                    status = "success",
                    visitantes
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

        [HttpGet]
        [Route("[action]/{dis_Id_Distrito}")]
        [EnableCors("AllowOrigin")]
        public IActionResult VisitantesByDistrito(int dis_Id_Distrito)
        {


            try
            {
                var visitantes = (from V in context.Visitante
                                  join S in context.Sector on V.sec_Id_Sector equals S.sec_Id_Sector
                                  join D in context.Distrito on S.dis_Id_Distrito equals D.dis_Id_Distrito
                                  where S.dis_Id_Distrito == dis_Id_Distrito && V.vp_Activo == true
                                  orderby S.sec_Tipo_Sector descending, S.sec_Numero, V.vp_Nombre
                                  select new VisitantesYSuSector
                                  {
                                      sector = S,
                                      visitante = V
                                  }).ToList();

                return Ok(new
                {
                    status = "success",
                    visitantes
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

        [HttpGet("{idVisitante}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int idVisitante)
        {
            try
            {
                var visitante = context.Visitante.FirstOrDefault(vp => vp.vp_Id_Visitante == idVisitante);
                return Ok(new
                {
                    status = "success",
                    visitante
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
        public IActionResult NuevoVisitante([FromBody] VisitanteNota vn)
        {
            try
            {
                Visitante visitante = vn.visitante;
                var contador = (from vp in context.Visitante
                                where vp.sec_Id_Sector == visitante.sec_Id_Sector
                                orderby vp.vp_Numero_Lista ascending
                                select vp).ToList();

                visitante.vp_Activo = true;
                visitante.Fecha_Registro = fechayhora;
                // ordena las priordades segun seleccion
                visitante.vp_Numero_Lista = OrdenaPrioridad(visitante.vp_Id_Visitante, visitante.sec_Id_Sector, visitante.vp_Numero_Lista);

                // alta de visitante
                context.Visitante.Add(visitante);
                context.SaveChanges();

                Nota nota = new Nota()
                {
                    vp_Id_Visitante = visitante.vp_Id_Visitante,
                    n_Nota = vn.n_Nota,
                    n_Fecha_Nota = vn.n_Fecha_Nota,
                    Fecha_Registro = fechayhora,
                    usu_Id_Usuario = visitante.usu_Id_Usuario
                };

                context.Nota.Add(nota);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    visitante,
                    nota
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

        [HttpPut("{vp_Id_Visitante}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] Visitante visitante, int vp_Id_Visitante)
        {
            try
            {
                var editaVisitante = context.Visitante.FirstOrDefault(v => v.vp_Id_Visitante == vp_Id_Visitante);
                editaVisitante.vp_Tipo_Visitante = visitante.vp_Tipo_Visitante;
                editaVisitante.vp_Direccion = visitante.vp_Direccion;
                editaVisitante.vp_Nombre = visitante.vp_Nombre;
                editaVisitante.Fecha_Registro = visitante.Fecha_Registro;
                editaVisitante.vp_Telefono_Contacto = visitante.vp_Telefono_Contacto;
                editaVisitante.vp_Numero_Lista = OrdenaPrioridad(editaVisitante.vp_Id_Visitante, visitante.sec_Id_Sector, visitante.vp_Numero_Lista);
                context.Visitante.Update(editaVisitante);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    visitante
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
        [EnableCors("AllowOrigin")]
        [Route("[action]")]
        public IActionResult BajaDeVisitante([FromBody] BajaVisitante bv)
        {
            try
            {
                var visitante = context.Visitante.FirstOrDefault(vp => vp.vp_Id_Visitante == bv.vp_Id_Visitante);
                visitante.vp_Numero_Lista = OrdenaPrioridad(visitante.vp_Id_Visitante, visitante.sec_Id_Sector, context.Visitante.Count());
                visitante.vp_Activo = false;
                context.Visitante.Update(visitante);
                context.SaveChanges();

                Nota notaBaja = new Nota
                {
                    vp_Id_Visitante = bv.vp_Id_Visitante,
                    n_Fecha_Nota = bv.n_Fecha_Nota,
                    n_Nota = bv.n_Nota,
                    Fecha_Registro = fechayhora,
                    usu_Id_Usuario = visitante.usu_Id_Usuario
                };
                context.Nota.Add(notaBaja);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success"
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
    }
}
