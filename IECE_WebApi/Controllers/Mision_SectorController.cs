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
    public class Mision_SectorController : ControllerBase
    {
        private readonly AppDbContext context;

        public Mision_SectorController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        public class infoMision
        {
            public string ms_Alias { get; set; }
            public int sec_Id_Sector { get; set; }
            public int Usu_Usuario { get; set; }
        }


        [HttpGet("{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int sec_Id_Sector)
        {
            try
            {
                var misiones = (from m in context.Mision_Sector
                                where m.sec_Id_Sector == sec_Id_Sector
                                select m).ToList();
                return Ok(new
                {
                    status = "success",
                    misiones
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

        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] infoMision info)
        {
            try
            {
                var misiones = context.Mision_Sector.OrderBy(m => m.ms_Numero).ToList();
                var ultimoRegistro = misiones.LastOrDefault();
                Mision_Sector mision = new Mision_Sector
                {
                    ms_Numero = misiones.Count == 0 ? 1 : ultimoRegistro.ms_Numero + 1,
                    ms_Alias = info.ms_Alias,
                    ms_Activo = true,
                    sec_Id_Sector = info.sec_Id_Sector,
                    ms_Fecha_Captura = fechayhora,
                    Usu_Usuario = info.Usu_Usuario
                };
                context.Mision_Sector.Add(mision);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    mision
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

        [HttpPut("{ms_Id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] Mision_Sector ms, int ms_Id)
        {
            try
            {
                var mision = context.Mision_Sector.FirstOrDefault(m => m.ms_Id == ms_Id);
                mision.ms_Alias = ms.ms_Alias;
                context.Mision_Sector.Update(mision);
                context.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    mision
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

        [HttpPost]
        [EnableCors("AllowOrigin")]
        [Route("[action]/{ms_Id}")]
        public IActionResult BajaDeMision(int ms_Id)
        {
            try
            {
                var mision = context.Mision_Sector.FirstOrDefault(m => m.ms_Id == ms_Id);
                mision.ms_Activo = false;
                context.Mision_Sector.Update(mision);
                context.SaveChanges();
                return Ok(new
                {
                    status = "success"
                });
            }
            catch(Exception ex)
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
