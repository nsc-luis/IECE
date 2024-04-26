using System;
using System.Collections.Generic;
using System.Linq;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
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



        public class Presentacion_Nino_Visitante: Presentacion_Nino
        {

            public string nombreNinoVisitante { get; set; }

        }

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
                // GUARDA DATOS EN LA TABLA DE PRESENTACION
                presentacion.Fecha_Registro = fechayhora;
                presentacion.usu_Id_Usuario = mu_pem_Id_Pastor;
                context.Presentacion_Nino.Add(presentacion);
                context.SaveChanges();

                // GUARDA DATOS EN EL REGISTRO HISTORICO
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                hte.RegistroHistorico(
                    presentacion.per_Id_Persona,
                    sec_Id_Sector,
                    23203,
                    presentacion.pdn_Ministro_Oficiante,
                    presentacion.pdn_Fecha_Presentacion.Value,
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

        // POST: api/Presentacion_Nino
        [HttpPost("[action]/{sec_Id_Sector}/{mu_pem_Id_Pastor}")]
        [EnableCors("AllowOrigin")]
        public IActionResult PresentacionNinoVisitante([FromBody] Presentacion_Nino_Visitante presentacion, int sec_Id_Sector, int mu_pem_Id_Pastor)
        {
            try
            {
                //Registra al Niño de Visitantes con estatus Inactivo, Muerto y Fecha de nacimiento 01/01/0001 para que no se contabilise en Membresía
                //Sólo es para contabilizar la presentación, pues se requiere un id_persona.
                Persona nino = new Persona();
                nino.per_Bautizado = false;
                nino.per_Activo = false;
                nino.per_En_Comunion = false;
                nino.per_Vivo = false;
                nino.per_Visibilidad_Abierta = false;
                nino.sec_Id_Sector = sec_Id_Sector;
                nino.per_Categoria = "NIÑO"; //Motivo a que este registro es irrelevante, no importa si es Niño o Niña.
                nino.per_Nombre = presentacion.nombreNinoVisitante;
                nino.per_Apellido_Paterno = "(HIJO DE VISITANTES)";
                nino.per_Fecha_Nacimiento = new DateTime(0001,01,01); //Motivo a que este registro es irrelevante, la fecha de Nac. es inventada.
                nino.per_RFC_Sin_Homo = "NIVIM01010001";
                nino.per_Nombre_Completo = ManejoDeApostrofes.QuitarApostrofe2(presentacion.nombreNinoVisitante);
                nino.pro_Id_Profesion_Oficio1 = 1;
                nino.pro_Id_Profesion_Oficio2 = 1;
                nino.per_Estado_Civil = "SOLTERO";
                nino.Fecha_Registro = fechayhora;
                nino.usu_Id_Usuario = mu_pem_Id_Pastor;
                nino.idFoto = 1;
                context.Persona.Add(nino);
                context.SaveChanges();

                //ASIGNA A ESTE NIÑO A UN HOGAR FICTISIO PARA NO AFECTAR LA FUNCIONALIDAD DE LA APP.
                Hogar_Persona PH = new Hogar_Persona
                {
                    //hd_Id_Hogar = 18701, //Id de Hogar Ficticio en Base de Datos Local.
                    hd_Id_Hogar = 21570, //Id de Hogar Ficticio en Base de Datos Servidor Oficial.
                    per_Id_Persona = nino.per_Id_Persona,
                    hp_Jerarquia = 1,
                    usu_Id_Usuario = mu_pem_Id_Pastor,
                    Fecha_Registro = fechayhora
                };
                context.Hogar_Persona.Add(PH);
                context.SaveChanges();


                // GUARDA DATOS EN LA TABLA DE PRESENTACION
                presentacion.per_Id_Persona = nino.per_Id_Persona;
                presentacion.Fecha_Registro = fechayhora;
                presentacion.usu_Id_Usuario = mu_pem_Id_Pastor;
                context.Presentacion_Nino.Add(presentacion);
                context.SaveChanges();

                // GUARDA DATOS EN EL REGISTRO HISTORICO
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                hte.RegistroHistorico(
                    presentacion.per_Id_Persona,
                    sec_Id_Sector,
                    23203,
                    presentacion.pdn_Ministro_Oficiante,
                    presentacion.pdn_Fecha_Presentacion.Value,
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
                presentacion.usu_Id_Usuario = presentacion.usu_Id_Usuario;
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
                    //ELIMINA REGISTRO HISTORICO DE LA PRESENTACION
                    Historial_Transacciones_Estadisticas registroHistorico = new Historial_Transacciones_Estadisticas();
                    registroHistorico = context.Historial_Transacciones_Estadisticas.FirstOrDefault(rh => rh.per_Persona_Id == presentacion.per_Id_Persona && rh.ct_Codigo_Transaccion == 23203);
                    context.Historial_Transacciones_Estadisticas.Remove(registroHistorico);
                    context.SaveChanges();

                    //ELIMINA PRESENTACION DEL NIÑO
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
