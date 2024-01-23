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

        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult VisitanteBySector(int sec_Id_Sector)
        {
            try
            {
                var visitantes = context.Visitante.Where(v => v.sec_Id_Sector == sec_Id_Sector && v.vp_Activo == true).ToList();
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
            catch (Exception ex) {
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

                if (contador.Count < 1) { visitante.vp_Numero_Lista = 1; }
                if (contador.Count != visitante.vp_Numero_Lista)
                {
                    for (int i = visitante.vp_Numero_Lista; i <= contador.Count; i++)
                    {

                    }
                }

                visitante.vp_Activo = true;
                visitante.Fecha_Registro = fechayhora;
                visitante.vp_Numero_Lista = contador.Count > 0 ? contador.LastOrDefault().vp_Numero_Lista + 1 : 1;
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

        [HttpPut]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] Visitante visitante)
        {
            try
            {
                context.Entry(visitante).State = EntityState.Modified;
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
