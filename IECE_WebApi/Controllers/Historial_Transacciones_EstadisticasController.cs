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
using static IECE_WebApi.Helpers.SubConsultas;

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

        // CLASE PARA METODOS DE CONSULTA DE HISTORIAL ESTADISTICO
        public class FechasSectorDistritoCodigo
        {
            public int idSectorDistrito { get; set; }
            public DateTime fechaInicial { get; set; }
            public DateTime fechaFinal { get; set; }
            public int codigo { get; set; }
        }

        // CLASE PARA METODOS DE CONSULTA DE HISTORIAL ESTADISTICO (2)
        public class FechasSectorDistrito
        {
            public int idSectorDistrito { get; set; }
            public DateTime fechaInicial { get; set; }
            public DateTime fechaFinal { get; set; }
        }

        // CLASE PARA BAJA POR CAMBIO DE DOMICILIO
        public class objBajaCambioDomicilio
        {
            public int idPersona { get; set; }
            public bool bautizado { get; set; }
            public DateTime fecha { get; set; }
            public bool CambioOtroDistrito { get; set; }
            public int idUsuario { get; set; }
        }

        // CLASE PARA RESTITUCION/ACTIVACION
        public class altaReactivacionRestitucion_HogarActual
        {
            public int idPersona { get; set; }
            public string comentario { get; set; }
            public DateTime fecha { get; set; }
            public int idMinistro { get; set; }
            public int jerarquia { get; set; }
        }

        public class AltaCamDomReactRest_HogarExistente
        {
            public int idPersona { get; set; }
            public int sec_Id_Sector { get; set; }
            public string comentario { get; set; }
            public DateTime fecha { get; set; }
            public int idMinistro { get; set; }
            public int idDomicilio { get; set; }
            public int jerarquia { get; set; }
            public int ct_Codigo_Transaccion { get; set;}
        }

        // METODO PARA ALTA DE REGISTRO HISTORICO
        [HttpPost]
        [Route("[action]/{per_Id_Persona}/{sec_Id_Sector}/{ct_Codigo_Transaccion}/{hte_Comentario}/{hte_Fecha_Transaccion=}/{Usu_Usuario_Id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult RegistroHistorico(
            int per_Id_Persona,
            int sec_Id_Sector,
            int ct_Codigo_Transaccion,
            string hte_Comentario,
            DateTime? hte_Fecha_Transaccion,
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
                nvoRegistro.hte_Id_Transaccion = 0;
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

                // ALTA DE REGISTRO PARA HISTORICO
                context.Historial_Transacciones_Estadisticas.Add(nvoRegistro);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    registro = nvoRegistro
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
            try
            {
                //Consulta los datos actuales de la Persona en proceso de Alta por Cambio de Domicilio
                var persona = (from per in context.Persona
                               where per.per_Id_Persona == altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.per_Id_Persona
                               select per).ToList();

                Persona p = persona[0];

                switch (altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion)
                {
                    case 11002: // Restitución Bautizado
                        p.per_En_Comunion = true;
                        p.per_Visibilidad_Abierta = false;
                        p.per_Activo = true;
                        p.Fecha_Registro = fechayhora;
                        p.sec_Id_Sector = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector;
                        p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                        context.Entry(p).State = EntityState.Modified;
                        //context.Persona.Add(p);
                        context.SaveChanges();

                        RegistroHistorico(
                            persona[0].per_Id_Persona,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Comentario,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Fecha_Transaccion,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                        );
                        break;
                    default: // Cambio de domicilio bautizado y no bautizado.
                        int codigoTransaccion = 0;
                        string comentario = "";

                        if (persona[0].per_Bautizado)//Si ES BAUTIZADO
                        {
                            //codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 11004 ? 11004 : 11005;
                            //comentario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 11003 ? "Cambio de domicilio interno" : "Cambio de domicilio externo";
                            codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion;
                            comentario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Comentario;
                        }
                        else //SI ES NO BAUTIZADO
                        {
                            //codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 12002 ? 12002 : 12003;
                            //comentario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion == 12002 ? "Cambio de domicilio interno" : "Cambio de domicilio externo";
                            codigoTransaccion = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion;
                            comentario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Comentario;
                        }
                        p.per_Visibilidad_Abierta = false;
                        p.Fecha_Registro = fechayhora;
                        p.per_Activo = true;
                        p.sec_Id_Sector = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector;
                        p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                        // context.Persona.Add(p);
                        context.Entry(p).State = EntityState.Modified;
                        context.SaveChanges();

                        RegistroHistorico(
                            persona[0].per_Id_Persona,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                            codigoTransaccion,
                            comentario,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Fecha_Transaccion,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                        );
                        break;
                    case 12004: // Reactivacion No Bautizado
                        p.per_Activo = true;
                        p.per_Visibilidad_Abierta = false;
                        p.Fecha_Registro = fechayhora;
                        p.sec_Id_Sector = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector;
                        p.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                        // context.Persona.Add(p);
                        context.Entry(p).State = EntityState.Modified;
                        context.SaveChanges();

                        RegistroHistorico(
                            p.per_Id_Persona,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.ct_Codigo_Transaccion,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Comentario,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Fecha_Transaccion,
                            altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                        );
                        break;
                }

                // ALTA DEL NUEVO HOGAR
                HogarDomicilio hd = new HogarDomicilio();
                hd = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.HD;
                hd.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                hd.Fecha_Registro = fechayhora;
                hd.hd_Activo = true;
                context.HogarDomicilio.Add(hd);
                context.SaveChanges();

                // REGISTRO HISTORICO DEL NUEVO HOGAR
                RegistroHistorico(
                    persona[0].per_Id_Persona,
                    altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.sec_Id_Sector,
                    31001,
                    (p.per_Nombre + " " + p.per_Apellido_Paterno + " " + p.per_Apellido_Materno),
                    altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Fecha_Transaccion,
                    altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                );

                // VINCULA LA PERSONA CON EL HOGAR EN LA TABLA PERSONA-HOGAR
                var hp = context.Hogar_Persona.FirstOrDefault(h => h.per_Id_Persona == persona[0].per_Id_Persona);
                hp.hp_Jerarquia = 1;
                hp.hd_Id_Hogar = hd.hd_Id_Hogar;
                hp.Fecha_Registro = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Fecha_Transaccion;
                hp.usu_Id_Usuario = altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id;
                context.Hogar_Persona.Update(hp);
                context.SaveChanges();

                // VERIFICA SI LA PERSONA PERTENECÍA AL PERSONAL MINISTERIAL PARA CAMBIAR SU CONGREGACIÓN EN LA TABLA PERSONAL MINISTERIAL, esta parte es Temporar ya que desaparecerá el campo "Id_Congregacion" cuando ya esté toda la membresía registrada.
                var pm = context.Personal_Ministerial.FirstOrDefault(pem => pem.per_Id_Miembro == p.per_Id_Persona && pem.pem_Activo == true);

                if (pm != null) //Si encuentra que la Persona Cambiada de Domicilio pertenece al Personal Ministerial, le cambia el Sector/Congregació
                {
                    pm.sec_Id_Congregacion = p.sec_Id_Sector;
                    context.Personal_Ministerial.Update(pm);
                    context.SaveChanges();

                    //Registra la Transacción Ministerial
                    Registro_TransaccionesController rt = new Registro_TransaccionesController(context);
                    rt.RegistroHistorico(
                     pm.pem_Id_Ministro,
                     pm.sec_Id_Congregacion,
                     "EDICIÓN GENERAL DE DATOS DE PERSONAL MINISTERIAL",
                     "CAMBIO DE SECTOR",
                     "",
                     altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.hte_Fecha_Transaccion,
                    altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id,
                     altaCambioDomicilioRestitucionReactivacion_nuevoDomicilio.Usu_Usuario_Id
                     );
                }

                return Ok(new
                {
                    status = "success"
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
        // Parametros segun modelo AltaCambioDomicilioReactivacionRestitucion_HogarExistente
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult AltaCambioDomicilioReactivacionRestitucion_HogarExistente([FromBody] AltaCamDomReactRest_HogarExistente acdrr_he)
        {
            try
            {
                PersonaController pc = new PersonaController(context);
                // OBTENER DATOS DE LA PERSONA
                var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == acdrr_he.idPersona);

                // OBTENER DATOS DEL HOGAR
                var h = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == acdrr_he.idPersona);

                // CUENTA PERSONAS BAUTIZADAS, VIVAS, EN COMUNION Y ACTIVAS DENTRO DEL HOGAR
                int contador = 0;
                var miembros = (from hp in context.Hogar_Persona
                                join per in context.Persona on hp.per_Id_Persona equals per.per_Id_Persona
                                where hp.hd_Id_Hogar == acdrr_he.idDomicilio
                                && per.per_Bautizado == true
                                && per.per_Activo == true
                                && per.per_En_Comunion == true
                                && per.per_Vivo == true
                                select new
                                {
                                    per.per_Id_Persona,
                                    per.per_Bautizado,
                                    hp.hp_Jerarquia
                                }).ToList();
                foreach (var m in miembros)
                {
                    contador = m.per_Bautizado == true ? contador + 1 : contador + 0;
                }

                // inicializa codigo de transaccion
                int ct = acdrr_he.ct_Codigo_Transaccion;

                switch (acdrr_he.ct_Codigo_Transaccion)
                {
                    case 11002: // Restitución
                        
                            if (p.per_Bautizado == true) //Si es Bautizado se trata de una
                            {

                                

                                // si se Vinculará a un Hogar Diferente al que pretenecía
                                if (acdrr_he.idDomicilio != h.hd_Id_Hogar)
                                {
                                    // baja en el hogar anterior
                                    pc.RestructuraJerarquiasBaja(acdrr_he.idPersona);

                                    // alta en el nuevo
                                    h.hd_Id_Hogar = acdrr_he.idDomicilio;
                                    h.hp_Jerarquia = acdrr_he.jerarquia;
                                    h.usu_Id_Usuario = acdrr_he.idMinistro;
                                    context.Hogar_Persona.Update(h);
                                    context.SaveChanges();
                                    // y restructura de jerarquias
                                    pc.RestructuraJerarquiasAlta(acdrr_he.idPersona, acdrr_he.jerarquia);

                                    // Aactualiza estatus de la persona
                                    p.per_En_Comunion = true;
                                    p.per_Activo = true;
                                    p.per_Visibilidad_Abierta = false;
                                    p.sec_Id_Sector = acdrr_he.sec_Id_Sector;
                                    p.usu_Id_Usuario = acdrr_he.idMinistro;
                                    context.Persona.Update(p);
                                    context.SaveChanges();

                                    // se genera registro historico de la persona
                                    RegistroHistorico(acdrr_he.idPersona, p.sec_Id_Sector, ct, acdrr_he.comentario, acdrr_he.fecha, acdrr_he.idMinistro);
                                }
                                else // la persona se restituyó en EL MISMO HOGAR
                                {
                                        h.hp_Jerarquia = acdrr_he.jerarquia;
                                        h.usu_Id_Usuario = acdrr_he.idMinistro;
                                        context.Hogar_Persona.Update(h);
                                        context.SaveChanges();

                                        // y restructura de jerarquias
                                        pc.RestructuraJerarquiasAlta(acdrr_he.idPersona, acdrr_he.jerarquia);

                                        // Aactualiza estatus de la persona
                                        p.per_En_Comunion = true;
                                        p.per_Activo = true;
                                        p.per_Visibilidad_Abierta = false;
                                        p.sec_Id_Sector = acdrr_he.sec_Id_Sector;
                                        p.usu_Id_Usuario = acdrr_he.idMinistro;
                                        context.Persona.Update(p);
                                        context.SaveChanges();

                                        // se genera registro historico de la persona
                                        RegistroHistorico(acdrr_he.idPersona, p.sec_Id_Sector, ct, acdrr_he.comentario, acdrr_he.fecha, acdrr_he.idMinistro);
                                }
                            }
                         break;

                     case 12004: //Reactivación

                            // la persona no es bautizada y se reactiva en el mismo hogar
                            if (h.hd_Id_Hogar == acdrr_he.idDomicilio)
                            {
                                h.hp_Jerarquia = acdrr_he.jerarquia;
                                h.usu_Id_Usuario = acdrr_he.idMinistro;
                                context.Hogar_Persona.Update(h);
                                context.SaveChanges();

                                // restructura jerarquias
                                pc.RestructuraJerarquiasAlta(acdrr_he.idPersona, acdrr_he.jerarquia);

                                p.per_Activo = true;
                                p.per_Visibilidad_Abierta = false;
                                p.sec_Id_Sector = acdrr_he.sec_Id_Sector;
                                p.usu_Id_Usuario = acdrr_he.idMinistro;
                                context.Persona.Update(p);
                                context.SaveChanges();

                                // se genera registro historico de la persona
                                RegistroHistorico(acdrr_he.idPersona, p.sec_Id_Sector, ct, acdrr_he.comentario, acdrr_he.fecha, acdrr_he.idMinistro);
                            }
                        else  // la persona no es bautizada y se reactiva en un Hogar Diferente
                        {
                                // baja del hogar anterior
                                pc.RestructuraJerarquiasBaja(acdrr_he.idPersona);

                                // alta en el hogar nuevo
                                h.hd_Id_Hogar = acdrr_he.idDomicilio;
                                h.hp_Jerarquia = acdrr_he.jerarquia;
                                h.usu_Id_Usuario = acdrr_he.idMinistro;
                                context.Hogar_Persona.Update(h);
                                context.SaveChanges();
                                // restructura jerarquias en el nuevo hogar
                                pc.RestructuraJerarquiasAlta(acdrr_he.idPersona, acdrr_he.jerarquia);


                                p.per_Activo = true;
                                p.per_Visibilidad_Abierta = false;
                                p.sec_Id_Sector = acdrr_he.sec_Id_Sector;
                                p.usu_Id_Usuario = acdrr_he.idMinistro;
                                context.Persona.Update(p);
                                context.SaveChanges();
                                // genera registro historico de la persona
                                RegistroHistorico(acdrr_he.idPersona, p.sec_Id_Sector, ct, acdrr_he.comentario, acdrr_he.fecha, acdrr_he.idMinistro);
                            }
                            break;


                    default: // Cambio de Domicilio

                            // baja en el hogar anterior
                            pc.RestructuraJerarquiasBaja(acdrr_he.idPersona);

                            // alta en el nuevo 
                            h.hd_Id_Hogar = acdrr_he.idDomicilio;
                            h.hp_Jerarquia = acdrr_he.jerarquia;
                            h.usu_Id_Usuario = acdrr_he.idMinistro;
                            context.Hogar_Persona.Update(h);
                            context.SaveChanges();

                            // y restructura de jerarquias
                            pc.RestructuraJerarquiasAlta(acdrr_he.idPersona, acdrr_he.jerarquia);

                            // actualiza estatus de la persona
                            p.per_Activo = true;
                            p.per_Visibilidad_Abierta = false;
                            p.sec_Id_Sector = acdrr_he.sec_Id_Sector;
                            p.usu_Id_Usuario = acdrr_he.idMinistro;
                            context.Persona.Update(p);
                            context.SaveChanges();

                            // se genera registro historico de la persona
                            RegistroHistorico(acdrr_he.idPersona, p.sec_Id_Sector, ct, acdrr_he.comentario, acdrr_he.fecha, acdrr_he.idMinistro);
                        break;
                }

                // VERIFICA SI LA PERSONA PERTENECÍA AL PERSONAL MINISTERIAL PARA CAMBIAR SU CONGREGACIÓN EN LA TABLA PERSONAL MINISTERIAL, esta parte es Temporar ya que desaparecerá el campo "Id_Congregacion" cuando ya esté toda la membresía registrada.
                var pm = context.Personal_Ministerial.FirstOrDefault(pem => pem.per_Id_Miembro == p.per_Id_Persona && pem.pem_Activo == true);

                if (pm != null) //Si encuentra que la Persona Cambiada de Domicilio pertenece al Personal Ministerial, le cambia el Sector/Congregació
                {
                        pm.sec_Id_Congregacion = p.sec_Id_Sector;
                        context.Personal_Ministerial.Update(pm);
                        context.SaveChanges();
                }

                return Ok(new
                {
                    status = "success"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex
                });
            }
        }

        // METODO PARA COMPONENTES DE VICTOR
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult AltaReactivacionRestitucion_HogarActual([FromBody] altaReactivacionRestitucion_HogarActual arrha)
        {
            try
            {
                // OBTENER DATOS DE LA PERSONA
                PersonaController pc = new PersonaController(context);
                var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == arrha.idPersona);

                // OBTENER DATOS DEL HOGAR
                var h = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == arrha.idPersona);

                // CUENTA PERSONAS BAUTIZADAS, VIVAS, EN COMUNION Y ACTIVAS DENTRO DEL HOGAR
                int contador = 0;
                var miembros = (from hp in context.Hogar_Persona
                                join per in context.Persona on hp.per_Id_Persona equals per.per_Id_Persona
                                where hp.hd_Id_Hogar == h.hd_Id_Hogar
                                && per.per_Bautizado == true
                                && per.per_Activo == true
                                && per.per_En_Comunion == true
                                && per.per_Vivo == true
                                select new
                                {
                                    per.per_Id_Persona,
                                    per.per_Bautizado,
                                    hp.hp_Jerarquia
                                }).ToList();
                foreach (var m in miembros)
                {
                    contador = m.per_Bautizado == true ? contador + 1 : contador + 0;
                }

                // CONDICION PRINCIPAL PARA TIPO DE MIEMBRO BAUTIZADO O NO BAUTIZADO
                if (p.per_Bautizado == true)
                {

                    // Esenario 1: en el hogar hay varias personas bautizadas activas, No ocupa dar de Alta el Hogar. solo a la Persona
                    p.per_Activo = true;
                    p.per_En_Comunion = true;
                    context.Persona.Update(p);
                    context.SaveChanges();

                    // Restructura de jerarquias
                    pc.RestructuraJerarquiasAlta(arrha.idPersona, arrha.jerarquia);

                    //  Genera registro historico
                    RegistroHistorico(arrha.idPersona, p.sec_Id_Sector, 11002, arrha.comentario, arrha.fecha, arrha.idMinistro);

                    if (contador == 0)
                    {
                        // Esenario 2: en el hogar hay varias personas no bautizadas o excomulgadas osea que se adicionalmente se ocupa dar de Alta el Hogar.
                        // ACTIVACION DE HOGAR
                        var d = context.HogarDomicilio.FirstOrDefault(dom => dom.hd_Id_Hogar == h.hd_Id_Hogar);
                        d.hd_Activo = true;
                        context.HogarDomicilio.Update(d);
                        context.SaveChanges();

                        // REGISTRO HISTORICO DEL HOGAR
                        RegistroHistorico(arrha.idPersona, p.sec_Id_Sector, 31001, (p.per_Nombre + " " + p.per_Apellido_Paterno + " " +p.per_Apellido_Materno), arrha.fecha, arrha.idMinistro);

                        // OBTENER MIEMBROS DEL HOGAR
                        var mh = (from hp in context.Hogar_Persona
                                  join per in context.Persona on hp.per_Id_Persona equals per.per_Id_Persona
                                  where hp.hd_Id_Hogar == h.hd_Id_Hogar && per.per_Vivo == true
                                  select new
                                  {
                                      per.per_Id_Persona,
                                      per.per_Bautizado
                                  }).ToList();

                        //// cambia estatus de personas no bautizadas a activas (ct: 12201)
                        //foreach (var m in mh)
                        //{
                        //    if (!m.per_Bautizado)
                        //    {
                        //        var miembro = context.Persona.FirstOrDefault(miem => miem.per_Id_Persona == m.per_Id_Persona);
                        //        miembro.per_Activo = true;
                        //        context.Persona.Update(miembro);
                        //        context.SaveChanges();

                        //        // GENERA REGISTRO HISTORICO DE LA PERSONA REACTIVADA
                        //        RegistroHistorico(m.per_Id_Persona, miembro.sec_Id_Sector, 12201, "", arrha.fecha, arrha.idMinistro);
                        //    }
                        //}
                        //// RESTRUCTURA JERARQUIAS
                        //pc.RestructuraJerarquiasAlta(arrha.idPersona, 1);

                        return Ok(new
                        {
                            status = "success"
                        });
                    }
                    return Ok(new
                    {
                        status = "success"
                    });
                }
                else if (!p.per_Bautizado)
                {
                    // Esenario 3: en el hogar hay varias personas bautizadas activas
                    p.per_Activo = true;
                    p.per_En_Comunion = false;
                    context.Persona.Update(p);
                    context.SaveChanges();

                    // Restructura de jerarquias
                    pc.RestructuraJerarquiasAlta(arrha.idPersona, arrha.jerarquia);

                    //  Genera registro historico
                    RegistroHistorico(arrha.idPersona, p.sec_Id_Sector, 12004, arrha.comentario, arrha.fecha, arrha.idMinistro);

                    return Ok(new
                    {
                        status = "success"
                    });
                }
                else
                {
                    // Esenario 4: la persona no es bautizada y no hay mas miembros bautizados en el hogar
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Error: No se puede reactivar a la persona en el mismo hogar ya debe haber al menos un miembro bautizado, en comunion, vivo y activo en el hogar."
                    });
                }
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
        // Consulta tabla de Historial por Fechas, Sector y Codigo
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult HistorialPorFechaSectorCodigo([FromBody] FechasSectorDistritoCodigo fsdc)
        {
            try
            {
                var query = (from hte in context.Historial_Transacciones_Estadisticas
                             join cte in context.Codigo_Transacciones_Estadisticas
                             on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                             where hte.sec_Sector_Id == fsdc.idSectorDistrito
                             && (hte.hte_Fecha_Transaccion >= fsdc.fechaInicial && hte.hte_Fecha_Transaccion <= fsdc.fechaFinal)
                             && hte.ct_Codigo_Transaccion == fsdc.codigo
                             select new
                             {
                                 cte.ct_Grupo,
                                 cte.ct_Tipo,
                                 cte.ct_Subtipo
                             }).ToList();

                if (query.Count() > 0)
                {
                    var resultado = new
                    {
                        query[0].ct_Grupo,
                        query[0].ct_Tipo,
                        query[0].ct_Subtipo,
                        contador = query.Count(),
                        detalles = query
                    };

                    return Ok(new
                    {
                        status = "success",
                        datos = resultado
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No hay registros para mostrar"
                    });
                }

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
        // Consulta tabla de Historial por Fechas, Distrito y Codigo
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult HistorialPorFechaDistritoCodigo([FromBody] FechasSectorDistritoCodigo fsdc)
        {
            try
            {
                var query = (from hte in context.Historial_Transacciones_Estadisticas
                             join cte in context.Codigo_Transacciones_Estadisticas
                             on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                             join per in context.Persona on hte.per_Persona_Id equals per.per_Id_Persona
                             where hte.dis_Distrito_Id == fsdc.idSectorDistrito
                             && (hte.hte_Fecha_Transaccion >= fsdc.fechaInicial && hte.hte_Fecha_Transaccion <= fsdc.fechaFinal)
                             && fsdc.codigo == hte.ct_Codigo_Transaccion
                             select new
                             {
                                 cte.ct_Grupo,
                                 cte.ct_Tipo,
                                 cte.ct_Subtipo,
                                 per.per_Nombre,
                                 per.per_Apellido_Paterno,
                                 per.per_Apellido_Materno,
                                 hte.hte_Comentario,
                                 hte.hte_Fecha_Transaccion
                             }).ToList();

                if (query.Count() > 0)
                {
                    var resultado = new
                    {
                        query[0].ct_Grupo,
                        query[0].ct_Tipo,
                        query[0].ct_Subtipo,
                        contador = query.Count(),
                        detalles = query
                    };

                    return Ok(new
                    {
                        status = "success",
                        datos = resultado
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No hay registros para mostrar"
                    });
                }

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
        // Consulta tabla de Historial por Fechas y Distrito
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult HistorialPorFechaDistrito([FromBody] FechasSectorDistrito fsd)
        {
            try
            {
                var query = (from hte in context.Historial_Transacciones_Estadisticas
                             join cte in context.Codigo_Transacciones_Estadisticas
                             on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                             join per in context.Persona on hte.per_Persona_Id equals per.per_Id_Persona
                             where hte.dis_Distrito_Id == fsd.idSectorDistrito
                             && (hte.hte_Fecha_Transaccion >= fsd.fechaInicial && hte.hte_Fecha_Transaccion <= fsd.fechaFinal)
                             select new
                             {
                                 hte.ct_Codigo_Transaccion,
                                 cte.ct_Grupo,
                                 cte.ct_Tipo,
                                 cte.ct_Subtipo,
                                 per.per_Nombre,
                                 per.per_Apellido_Paterno,
                                 per.per_Apellido_Materno,
                                 per.per_Apellido_Casada,
                                 apellidoPrincipal = (per.per_Apellido_Casada == "" || per.per_Apellido_Casada == null) ? per.per_Apellido_Paterno : (per.per_Apellido_Casada + "* " + per.per_Apellido_Paterno),
                                 hte.hte_Comentario,
                                 hte.sec_Sector_Alias,
                                 hte.dis_Distrito_Alias,
                                 hte.hte_Fecha_Transaccion
                             }).ToList();


                return Ok(new
                {
                    status = "success",
                    datos = query
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
        // Consulta tabla de Historial por Fechas y Sector
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult HistorialPorFechaSector([FromBody] FechasSectorDistrito fsd)
        {
            try
            {
                SubConsultas subConsultas = new SubConsultas(context);
                List<HistorialPorFechaSector> query = subConsultas.SubHistorialPorFechaSector(fsd);

                return Ok(new
                {
                    status = "success",
                    datos = query
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

        // BAJA POR CAMBIO DE DOMICILIO (bautizado/no bautizado)
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaPorCambioDeDomicilio([FromBody] objBajaCambioDomicilio bcd)
        {
            try
            {
                var per = context.Persona.FirstOrDefault(p => p.per_Id_Persona == bcd.idPersona);
                per.per_Visibilidad_Abierta = true;
                per.per_Activo = false;
                context.Persona.Update(per);
                context.SaveChanges();

                int tipoDeCambio = 0;
                if (per.per_Bautizado) { tipoDeCambio = bcd.CambioOtroDistrito ? 11004 : 11003; }
                else { tipoDeCambio = bcd.CambioOtroDistrito ? 12004 : 12003; }

                PersonaController personaController = new PersonaController(context);
                personaController.RestructuraJerarquiasBaja(per.per_Id_Persona);

                RegistroHistorico(per.per_Id_Persona, per.sec_Id_Sector, tipoDeCambio, "BAJA POR CAMBIO DE DOMICILIO", bcd.fecha, bcd.idUsuario);

                return Ok(new
                {
                    status = "seccess"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    error = ex.Message
                });
            }
        }

        // METODO PARA COMPONENTES DE VICTOR
        // Cambiar estatus de visibilidad
        [HttpPost]
        [Route("[action]/{idPersona}/{idUsuario}")]
        [EnableCors("AllowOrigin")]
        public IActionResult CambiarVisibilidad(int idPersona, int idUsuario)
        {
            try
            {
                // CAMBIA ESTATUS DE LA PERSONA
                var persona = context.Persona.FirstOrDefault(p => p.per_Id_Persona == idPersona);
                persona.per_Visibilidad_Abierta = true;
                context.Persona.Update(persona);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success"
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
                             orderby hte.hte_Fecha_Transaccion ascending
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
    }
}
