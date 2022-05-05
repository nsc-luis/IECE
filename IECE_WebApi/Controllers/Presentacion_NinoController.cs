using System;
using System.Collections.Generic;
using System.Linq;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Presentacion_NinoController : ControllerBase
    {
        private readonly AppDbContext context;

        public Presentacion_NinoController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/Presentacion_Nino/GetBySector/5
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetBySector(int idSector)
        {
            try
            {
                var presentaciones = (from pdn in context.Presentacion_Nino
                                      join sec in context.Sector
                                      on pdn.sec_Id_Sector equals sec.sec_Id_Sector
                                      join p in context.Persona
                                      on pdn.per_Id_Persona equals p.per_Id_Persona
                                      where pdn.sec_Id_Sector == idSector
                                      select new
                                      {
                                          pdn.pdn_Id_Presentacion,
                                          pdn.per_Id_Persona,
                                          p.per_Nombre,
                                          p.per_Apellido_Paterno,
                                          p.per_Apellido_Materno,
                                          pdn.pdn_Ministro_Oficiante,
                                          pdn.pdn_Fecha_Presentacion,
                                          pdn.sec_Id_Sector
                                      }).ToList();
                return Ok(new
                {
                    status = "success",
                    presentaciones = presentaciones
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

        // GET: api/Presentacion_Nino/5
        [HttpGet("{id}", Name = "Get")]
        [EnableCors("AllowOrigin")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Presentacion_Nino
        [HttpPost("{sec_Id_Sector}/{mu_pem_Id_Pastor}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Presentacion_Nino presentacion, int sec_Id_Sector, int mu_pem_Id_Pastor)
        {
            try
            {
                presentacion.Fecha_Registro = fechayhora;
                context.Presentacion_Nino.Add(presentacion);
                context.SaveChanges();

                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                hte.RegistroHistorico(
                    presentacion.per_Id_Persona,
                    sec_Id_Sector,
                    23203,
                    "Nueva presentacion",
                    fechayhora,
                    mu_pem_Id_Pastor
                );

                return Ok(
                    new
                    {
                        status = "success",
                        presentacion = presentacion
                    });
            }
            catch (Exception e)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = e.Message
                    });
            }
        }

        // PUT: api/Presentacion_Nino/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put([FromBody] Presentacion_Nino presentacion, int id)
        {
            try
            {
                presentacion.Fecha_Registro = fechayhora;
                presentacion.usu_Id_Usuario = 1;
                context.Entry(presentacion).State = EntityState.Modified;
                context.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    mensaje = "Datos guardados satisfactoriamente."
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

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Delete(int id)
        {
            try
            {
                Presentacion_Nino presentacion = new Presentacion_Nino();
                presentacion = context.Presentacion_Nino.FirstOrDefault(p => p.pdn_Id_Presentacion == id);
                if (presentacion != null)
                {
                    context.Presentacion_Nino.Remove(presentacion);
                    context.SaveChanges();
                }
                return Ok(new
                {
                    status = "success",
                    mensaje = "El registro fue borrado correctamente"
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
