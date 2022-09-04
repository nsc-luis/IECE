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
    public class SolicitudNuevaProfesionController : ControllerBase
    {
        private readonly AppDbContext context;

        public SolicitudNuevaProfesionController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        // POST: api/SolicitudNuevaProfesion
        [Route("[action]/{nvaProfesion}/{usu_Id_Usuario}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult RegistroDeNvaSolicitud(string nvaProfesion, int usu_Id_Usuario)
        {
            try
            {
                var solicitud = new SolicitudNuevaProfesion
                {
                    descNvaProfesion = nvaProfesion,
                    solicitudAtendida = false,
                    usu_Id_Usuario = usu_Id_Usuario,
                    fechaSolicitud = fechayhora
                };
                context.SolicitudNuevaProfesion.Add(solicitud);
                context.SaveChanges();

                SendMailController sendMail = new SendMailController(context);
                sendMail.EnviarSolicitudNvaProfesion(nvaProfesion, usu_Id_Usuario);

                return Ok(new
                    {
                        status = "success",
                        solicitud = solicitud
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(new
                    {
                        status = "error",
                        message = ex.Message
                    }
                );
            }
        }
    }
}