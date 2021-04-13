using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class FotoController : ControllerBase
    {
        //[HttpPost]
        //[EnableCors("AllowOrigin")]
        //public IActionResult Post([FromForm] Foto foto)
        //{
        //    try
        //    {
        //        var filePath = "C:\\Users\\Propietario\\Desktop\\images\\" + foto.foto.FileName;
        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            foto.foto.CopyTo(stream);
        //        }
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}
    }
}