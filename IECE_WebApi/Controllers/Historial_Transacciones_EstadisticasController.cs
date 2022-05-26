using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Historial_Transacciones_EstadisticasController : ControllerBase
    {
        private readonly AppDbContext context;

        public Historial_Transacciones_EstadisticasController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        // METODO PARA ALTA DE REGISTRO HISTORICO
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult RegistroHistorico(
            int per_Id_Persona,
            int sec_Id_Sector,
            int ct_Codigo_Transaccion,
            string hte_Comentario,
            DateTime hte_Fecha_Transaccion,
            int Usu_Usuario_Id
        )
        {
            try
            {
                var query = (from s in context.Sector
                             join d in context.Distrito
                             on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where s.sec_Id_Sector == sec_Id_Sector
                             select new
                             {
                                 s.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Id_Sector,
                                 s.sec_Alias
                             }).ToList();
                Historial_Transacciones_Estadisticas nvoRegistro = new Historial_Transacciones_Estadisticas();
                nvoRegistro.hte_Cancelado = false;
                nvoRegistro.dis_Distrito_Id = query[0].dis_Id_Distrito;
                nvoRegistro.dis_Distrito_Alias = query[0].dis_Alias;
                nvoRegistro.sec_Sector_Id = query[0].sec_Id_Sector;
                nvoRegistro.sec_Sector_Alias = query[0].sec_Alias;
                nvoRegistro.ct_Codigo_Transaccion = ct_Codigo_Transaccion;
                nvoRegistro.hte_Comentario = hte_Comentario;
                nvoRegistro.hte_Fecha_Transaccion = hte_Fecha_Transaccion;
                nvoRegistro.Usu_Usuario_Id = Usu_Usuario_Id;
                nvoRegistro.per_Persona_Id = per_Id_Persona;
                nvoRegistro.ct_Codigo_Transaccion = ct_Codigo_Transaccion;

                // ALTA DE REGISTRO PARA HISTORICO
                context.Historial_Transacciones_Estadisticas.Add(nvoRegistro);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
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

        // METODO PARA COMPONENTES DE VICTOR
        // Parametros segun modelo AltaCambioDomicilioReactivacionRestitucion_NuevoDomicilio
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult AltaCambioDomicilioReactivacionRestitucion_NuevoDomicilio([FromBody] AltaCambioDomicilioRestitucionReactivacion_NuevoDomicilio altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio)
        {
            var persona = (from per in context.Persona
                           where per.per_Id_Persona == altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.per_Id_Persona
                           select per).ToList();
            HogarDomicilio hd = new HogarDomicilio();
            hd = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.HD;
            hd.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
            hd.Fecha_Registro = fechayhora;
            context.HogarDomicilio.Add(hd);
            context.SaveChanges();

            Hogar_Persona hp = new Hogar_Persona();
            hp.hp_Jerarquia = 1;
            hp.per_Id_Persona = persona[0].per_Id_Persona;
            hp.hd_Id_Hogar = hd.hd_Id_Hogar;
            hp.Fecha_Registro = fechayhora;
            hp.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
            context.Hogar_Persona.Add(hp);
            context.SaveChanges();

            Persona p = persona[0];
            switch (altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion)
            {
                case 11002: // Restitución Bautizado
                    p.per_En_Comunion = true;
                    p.per_Visibilidad_Abierta = false;
                    p.per_Activo = true;
                    p.Fecha_Registro = fechayhora;
                    p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                    context.Entry(p).State = EntityState.Modified;
                    //context.Persona.Add(p);
                    context.SaveChanges();

                    RegistroHistorico(
                        persona[0].per_Id_Persona,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion,
                        "Alta por restitución",
                        fechayhora,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                    );
                    break;
                default: // Cambio de domicilio bautizado y no bautizado.
                    int codigoTransaccion = 0;
                    string comentario = "";
                    if (persona[0].per_Bautizado)
                    {
                        codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 11003 ? 11003 : 11004;
                        comentario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 11003 ? "Cambio de domicilio interno" : "Cambio de domicilio externo";
                    }
                    else
                    {
                        codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 12002 ? 12002 : 12003;
                        comentario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 12002 ? "Cambio de domicilio interno" : "Cambio de domicilio externo";
                    }
                    p.per_Visibilidad_Abierta = false;
                    p.Fecha_Registro = fechayhora;
                    p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                    // context.Persona.Add(p);
                    context.Entry(p).State = EntityState.Modified;
                    context.SaveChanges();

                    RegistroHistorico(
                        persona[0].per_Id_Persona,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                        codigoTransaccion,
                        comentario,
                        fechayhora,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                    );
                    break;
                case 12004: // Reactivacion No Bautizado
                    p.per_Activo = true;
                    p.per_Visibilidad_Abierta = false;
                    p.Fecha_Registro = fechayhora;
                    p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                    // context.Persona.Add(p);
                    context.Entry(p).State = EntityState.Modified;
                    context.SaveChanges();

                    RegistroHistorico(
                        p.per_Id_Persona,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion,
                        "Alta por reactivación",
                        fechayhora,
                        altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                    );
                    break;
            }

            return Ok(new
            {
                status = "success"
            });
        }

        // METODO PARA COMPONENTES DE VICTOR
        // Parametros segun modelo AltaCambioDomicilioReactivacionRestitucion_HogarExistente
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult AltaCambioDomicilioReactivacionRestitucion_HogarExistente([FromBody] AltaCambioDomicilioRestitucionReactivacion_HogarExistente altaCambioDomicilioRestitucionReactivacion_hogarExistente)
        {
            var persona = (from per in context.Persona
                           where per.per_Id_Persona == altaCambioDomicilioRestitucionReactivacion_hogarExistente.per_Id_Persona
                           select per).ToList();

            Hogar_Persona hpModel = new Hogar_Persona();
            var miembrosDelHogar = (from hp in context.Hogar_Persona
                                    where hp.hd_Id_Hogar == altaCambioDomicilioRestitucionReactivacion_hogarExistente.hp_Id_Hogar_Persona
                                    orderby (hp.hp_Jerarquia)
                                    select new
                                    {
                                        hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                        hd_Id_Hogar = hp.hd_Id_Hogar,
                                        hp_Jerarquia = hp.hp_Jerarquia,
                                        per_Id_Persona = hp.per_Id_Persona
                                    }).ToList();



            foreach (var miembro in miembrosDelHogar)
            {
                if (miembro.hp_Jerarquia == altaCambioDomicilioRestitucionReactivacion_hogarExistente.jerarquia)
                {
                    hpModel.per_Id_Persona = persona[0].per_Id_Persona;
                    hpModel.hp_Jerarquia = altaCambioDomicilioRestitucionReactivacion_hogarExistente.jerarquia;
                    hpModel.hd_Id_Hogar = altaCambioDomicilioRestitucionReactivacion_hogarExistente.hp_Id_Hogar_Persona;
                    hpModel.Fecha_Registro = fechayhora;
                    hpModel.usu_Id_Usuario = 1;
                    context.Hogar_Persona.Add(hpModel);
                    context.SaveChanges();

                    var registro = new Hogar_Persona
                    {
                        hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                        hd_Id_Hogar = miembro.hd_Id_Hogar,
                        per_Id_Persona = miembro.per_Id_Persona,
                        hp_Jerarquia = miembro.hp_Jerarquia + 1,
                        Fecha_Registro = fechayhora,
                        usu_Id_Usuario = 1
                    };
                    context.Entry(registro).State = EntityState.Modified;
                    context.SaveChanges();
                }
                if (miembro.hp_Jerarquia > altaCambioDomicilioRestitucionReactivacion_hogarExistente.jerarquia)
                {
                    var registro = new Hogar_Persona
                    {
                        hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                        hd_Id_Hogar = miembro.hd_Id_Hogar,
                        per_Id_Persona = miembro.per_Id_Persona,
                        hp_Jerarquia = miembro.hp_Jerarquia + 1,
                        Fecha_Registro = fechayhora,
                        usu_Id_Usuario = 1
                    };
                    context.Entry(registro).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }

            if (altaCambioDomicilioRestitucionReactivacion_hogarExistente.jerarquia > miembrosDelHogar.Count())
            {
                hpModel.per_Id_Persona = persona[0].per_Id_Persona;
                hpModel.hp_Jerarquia = altaCambioDomicilioRestitucionReactivacion_hogarExistente.jerarquia;
                hpModel.hd_Id_Hogar = altaCambioDomicilioRestitucionReactivacion_hogarExistente.hp_Id_Hogar_Persona;
                hpModel.Fecha_Registro = fechayhora;
                hpModel.usu_Id_Usuario = 1;
                context.Hogar_Persona.Add(hpModel);
                context.SaveChanges();
            }

            Persona p = persona[0];
            switch (altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion)
            {
                case 11002: // Restitución Bautizado
                    p.per_En_Comunion = true;
                    p.per_Visibilidad_Abierta = false;
                    p.per_Activo = true;
                    p.Fecha_Registro = fechayhora;
                    p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_hogarExistente.Usu_Usuario_Id;
                    // context.Persona.Add(p);
                    context.Entry(p).State = EntityState.Modified;
                    context.SaveChanges();

                    RegistroHistorico(
                        p.per_Id_Persona,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.sec_Id_Sector,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion,
                        "Alta por restitución",
                        fechayhora,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.Usu_Usuario_Id
                    );
                    break;
                default: // Cambio de domicilio bautizado y no bautizado.
                    int codigoTransaccion = 0;
                    string comentario = "";
                    if (p.per_Bautizado)
                    {
                        codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion == 11003 ? 11003 : 11004;
                        comentario = altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion == 11003 ? "Cambio de domicilio interno" : "Cambio de domicilio externo";
                    }
                    else
                    {
                        codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion == 12002 ? 12002 : 12003;
                        comentario = altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion == 12002 ? "Cambio de domicilio interno" : "Cambio de domicilio externo";
                    }
                    p.per_Visibilidad_Abierta = false;
                    p.Fecha_Registro = fechayhora;
                    p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_hogarExistente.Usu_Usuario_Id;
                    // context.Persona.Add(p);
                    context.Entry(p).State = EntityState.Modified;
                    context.SaveChanges();

                    RegistroHistorico(
                        p.per_Id_Persona,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.sec_Id_Sector,
                        codigoTransaccion,
                        comentario,
                        fechayhora,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.Usu_Usuario_Id
                    );
                    break;
                case 12004: // Reactivacion No Bautizado
                    p.per_Activo = true;
                    p.per_Visibilidad_Abierta = false;
                    p.Fecha_Registro = fechayhora;
                    p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_hogarExistente.Usu_Usuario_Id;
                    // context.Persona.Add(p);
                    context.Entry(p).State = EntityState.Modified;
                    context.SaveChanges();

                    RegistroHistorico(
                        p.per_Id_Persona,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.sec_Id_Sector,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.ct_Codigo_Transaccion,
                        "Alta por reactivación",
                        fechayhora,
                        altaCambioDomicilioRestitucionReactivacion_hogarExistente.Usu_Usuario_Id
                    );
                    break;
            }

            return Ok(new
            {
                status = "success"
            });
        }

        // GET: api/Historial_Transacciones_Estadisticas
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Historial_Transacciones_Estadisticas/5
        [HttpGet("{per_Id_Persona}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int per_Id_Persona)
        {
            try
            {
                var query = (from hte in context.Historial_Transacciones_Estadisticas
                             join sec in context.Sector on hte.sec_Sector_Id equals sec.sec_Id_Sector
                             join dis in context.Distrito on hte.dis_Distrito_Id equals dis.dis_Id_Distrito
                             join cte in context.Codigo_Transacciones_Estadisticas on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                             where hte.per_Persona_Id == per_Id_Persona
                             select new
                             {
                                 hte.hte_Id_Transaccion,
                                 hte.hte_Fecha_Transaccion,
                                 hte.hte_Comentario,
                                 cte.ct_Categoria,
                                 cte.ct_Tipo,
                                 cte.ct_Subtipo,
                                 sec.sec_Id_Sector,
                                 sec.sec_Alias,
                                 dis.dis_Tipo_Distrito,
                                 dis.dis_Numero,
                                 dis.dis_Alias
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    info = query
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
        public void Delete(int id)
        {
        }
    }
}
