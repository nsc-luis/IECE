using System.IO;
using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ArchivosController : Controller
    {
        [HttpGet]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult ManualDeUsuario()
        {
            try
            {

                var archivoUrl = "ManualUsuario\\MANUAL DE USUARIO DE LA APLICACIÓN WEB IECE.pdf";
                byte[] imageByteData = System.IO.File.ReadAllBytes(archivoUrl);
                var fileStream = new MemoryStream(imageByteData);
                return File(fileStream, "application/pdf", "Manual_De_Usuario_Aplicacion_IECE_Membresia.pdf");
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
