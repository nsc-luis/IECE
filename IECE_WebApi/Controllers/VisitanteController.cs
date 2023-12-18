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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            public string N_Nota { get; set; }
            public DateTime N_Fecha_Nota { get; set; }
        }

        [HttpGet]
        [Route("[action]/{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult VisitanteBySector(int sec_Id_Sector)
        {
            try
            {
                var visitantes = context.Visitante.Where(v => v.sec_Id_Sector == sec_Id_Sector).ToList();
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
                                orderby vp.Vp_Numero_Lista ascending
                                select vp).ToList();

                if (contador.Count < 1) { visitante.Vp_Numero_Lista = 1; }
                if (contador.Count != visitante.Vp_Numero_Lista)
                {
                    for (int i = visitante.Vp_Numero_Lista; i <= contador.Count; i++)
                    {

                    }
                }

                visitante.Vp_Activo = true;
                visitante.Fecha_Registro = fechayhora;
                visitante.Vp_Numero_Lista = contador.Count > 0 ? contador.LastOrDefault().Vp_Numero_Lista + 1 : 1;
                context.Visitante.Add(visitante);
                context.SaveChanges();

                Nota nota = new Nota()
                {
                    Vp_Id_Visitante = visitante.Vp_Id_Visitante,
                    N_Nota = vn.N_Nota,
                    N_Fecha_Nota = vn.N_Fecha_Nota,
                    Fecha_Registro = fechayhora,
                    Usu_Id_Usuario = visitante.Usu_Id_Usuario
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
        [Route("[action]/{Vp_Id_Visitante}")]
        public IActionResult BajaDeVisitante(int Vp_Id_Visitante)
        {
            try
            {
                var visitante = context.Visitante.FirstOrDefault(vp => vp.Vp_Id_Visitante == Vp_Id_Visitante);
                visitante.Vp_Activo = false;
                context.Visitante.Update(visitante);
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
