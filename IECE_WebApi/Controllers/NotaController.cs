using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotaController : ControllerBase
    {
        private readonly AppDbContext context;

        public NotaController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        [HttpGet("{idVisitante}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int idVisitante)
        {
            try
            {
                var notas = (from n in context.Nota
                             where n.vp_Id_Visitante == idVisitante
                             orderby n.n_Fecha_Nota
                             select n).ToList();
                return Ok(new
                {
                    status = "success",
                    notas
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

        [HttpPost()]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Nota nota)
        {
            try
            {
                Nota nvaNota = nota;
                nvaNota.Fecha_Registro = fechayhora;
                context.Nota.Add(nvaNota);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
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

        [HttpPut("{n_Id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] Nota nota, int n_Id)
        {
            try
            {
                var editaNota = context.Nota.FirstOrDefault(n => n.n_Id == n_Id);
                editaNota.n_Fecha_Nota = nota.n_Fecha_Nota;
                editaNota.n_Nota = nota.n_Nota;
                editaNota.Fecha_Registro = fechayhora;

                context.Nota.Update(editaNota);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    editaNota
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

        [HttpDelete("{n_Id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Delete(int n_Id)
        {
            try
            {
                var borrarNota = context.Nota.FirstOrDefault(n => n.n_Id == n_Id);
                context.Nota.Remove(borrarNota);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    borrarNota
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
    }
}
