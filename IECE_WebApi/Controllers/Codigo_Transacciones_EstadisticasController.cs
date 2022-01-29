using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class Codigo_Transacciones_EstadisticasController : ControllerBase
    {
        private readonly AppDbContext context;

        public Codigo_Transacciones_EstadisticasController(AppDbContext context)
        {
            this.context = context;
        }

        // RETORNA TODOS LOS REGISTROS DE LA TABLA
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult Get()
        {
            try
            {
                var query = (from cte in context.Codigo_Transacciones_Estadisticas
                             select cte).ToList();

                return Ok(new
                {
                    status = "success",
                    resultado = query
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
