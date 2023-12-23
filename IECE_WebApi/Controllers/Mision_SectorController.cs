using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            public int ms_Numero { get; set; }
            public int usu_Id_Usuario { get; set; }
        }

        public class misionesSectores
        {
            public Sector sectores { get; set; }
            public List<Mision_Sector> misiones { get; set; }
        }

        [HttpGet("{sec_Id_Sector}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int sec_Id_Sector)
        {
            try
            {
                var misiones = (from m in context.Mision_Sector
                                where m.sec_Id_Sector == sec_Id_Sector && m.ms_Activo == true
                                orderby m.ms_Numero
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

        [Route("[action]/{dto}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMisionesSectores(int dto)
        {
            try
            {
                List<misionesSectores> sectoresYMisiones = new List<misionesSectores>();

                var query = (from sec in context.Sector
                             join dis in context.Distrito
                             on sec.dis_Id_Distrito equals dis.dis_Id_Distrito
                             where sec.dis_Id_Distrito == dto && sec.sec_Tipo_Sector == "SECTOR"
                             orderby sec.sec_Id_Sector ascending
                             select sec).ToList();

                foreach (var sector in query) {

                    var misiones = (from m in context.Mision_Sector
                                    where m.sec_Id_Sector == sector.sec_Id_Sector && m.ms_Activo == true
                                    orderby m.ms_Numero
                                    select m).ToList();


                    sectoresYMisiones.Add(new misionesSectores {sectores = sector, misiones = misiones });    
                }

                return Ok(new
                {
                    status = "success",
                    misiones = sectoresYMisiones
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
                var misiones = context.Mision_Sector
                    .Where(m => m.sec_Id_Sector == info.sec_Id_Sector && m.ms_Activo == true)
                    .OrderBy(m => m.ms_Numero).ToList();                
                var ultimoRegistro = misiones.LastOrDefault();
                Mision_Sector mision = new Mision_Sector
                {
                    ms_Numero = misiones.Count == 0 ? 1 : ultimoRegistro.ms_Numero + 1,
                    ms_Alias = info.ms_Alias,
                    ms_Activo = true,
                    sec_Id_Sector = info.sec_Id_Sector,
                    ms_Fecha_Captura = fechayhora,
                    usu_Id_Usuario = info.usu_Id_Usuario
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
                var numeroOriginal = mision.ms_Numero;

                // Obtener las misiones activas restantes con números mayores o iguales al de la misión editada
                var misionesAfectadas = context.Mision_Sector
                    .Where(m => m.sec_Id_Sector == mision.sec_Id_Sector && m.ms_Activo == true && m.ms_Id != mision.ms_Id)
                    .OrderBy(m => m.ms_Numero)
                    .ToList();

                //Se edita el Registro de interes
                if (ms.ms_Numero <= misionesAfectadas.Count + 1) //Si el numero que quiere asignar no es superior al numero de misiones activas
                {
                    mision.ms_Numero = ms.ms_Numero;
                }
                mision.ms_Alias = ms.ms_Alias;
                context.Mision_Sector.Update(mision);
                context.SaveChanges();

                // Ajusta los números de las otras misiones
                if (ms.ms_Numero <= misionesAfectadas.Count + 1 && misionesAfectadas.Count >= 1)
                {

                    foreach (var mission in misionesAfectadas)
                    {
                        if (mission.ms_Numero < numeroOriginal && mission.ms_Numero >= mision.ms_Numero)
                        {
                            mission.ms_Numero += 1; // Aumenta el numero en 1
                        }else if (mission.ms_Numero > numeroOriginal && mission.ms_Numero <= mision.ms_Numero)
                        {
                            mission.ms_Numero -= 1; // Disminuye el numero en 1
                        }
                    }
                    context.SaveChanges();
                }





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

                // Obtener las misiones activas restantes con números mayores o iguales al de la misión inactivada
                var misionesAfectadas = context.Mision_Sector
                    .Where(m => m.sec_Id_Sector ==mision.sec_Id_Sector && m.ms_Activo ==true && m.ms_Numero >= mision.ms_Numero)
                    .ToList();

                // Si la misión eliminada no es la última, ajustar los números de las misiones afectadas
                if (misionesAfectadas.Count >= 1)
                {
                    // Obtener el número más bajo de las misiones afectadas
                    int minNumero = misionesAfectadas.Min(m => m.ms_Numero);

                    foreach (var mission in misionesAfectadas)
                    {
                        mission.ms_Numero -= 1; // Ajustar el número
                    }
                    context.SaveChanges();
                }







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
