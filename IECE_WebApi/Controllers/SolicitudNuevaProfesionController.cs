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
        [Route("[action]/{usu_Id_Usuario}/{per_Id_Persona}/{nvaProfesion=}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult RegistroDeNvaSolicitud(int usu_Id_Usuario, int per_Id_Persona, string nvaProfesion)
        {
            try
            {
                if (nvaProfesion != "" && nvaProfesion != "undefined")
                {
                    var solicitud = new SolicitudNuevaProfesion
                    {
                        descNvaProfesion = nvaProfesion,
                        solicitudAtendida = false,
                        usu_Id_Usuario = usu_Id_Usuario,
                        per_Id_Persona = per_Id_Persona,
                        fechaSolicitud = fechayhora
                    };
                    context.SolicitudNuevaProfesion.Add(solicitud);
                    context.SaveChanges();

                    SendMailController sendMail = new SendMailController(context);
                    sendMail.EnviarSolicitudNvaProfesion(usu_Id_Usuario, per_Id_Persona, nvaProfesion);

                    return Ok(new
                    {
                        status = "success",
                        solicitud = solicitud
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No se ha ingresado ninguna profesión nueva."
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
    }
}