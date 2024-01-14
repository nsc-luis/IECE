using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            public virtual Nota nota { get; set; }
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
                visitante.Vp_Activo = true;
                visitante.Fecha_Registro = fechayhora;
                context.Visitante.Add(visitante);
                context.SaveChanges();

                Nota nota = vn.nota;
                nota.Fecha_Registro = fechayhora;
                nota.Vp_Id_Visitante = visitante.Vp_Id_Visitante;
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

        [HttpPut("{Vp_Id_Visitante}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] Visitante v, int Vp_Id_Visitante)
        {
            try
            {
                var visitante = context.Visitante.FirstOrDefault(vp => vp.Vp_Id_Visitante == Vp_Id_Visitante);
                visitante.Vp_Direccion = v.Vp_Direccion;
                visitante.Vp_Numero_Lista = v.Vp_Numero_Lista;
                visitante.Vp_Tipo_Visitante = v.Vp_Tipo_Visitante;
                visitante.Vp_Nombre = v.Vp_Nombre;
                visitante.Vp_Telefono_Contacto = v.Vp_Telefono_Contacto;
                context.Visitante.Update(visitante);
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
