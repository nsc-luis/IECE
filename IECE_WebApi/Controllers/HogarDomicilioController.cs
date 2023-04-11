using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Repositorios;
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
    public class HogarDomicilioController : ControllerBase
    {

        private readonly AppDbContext context;

        public HogarDomicilioController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/HogarDomicilio
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<HogarDomicilio>> Get()
        {
            try
            {
                return Ok(context.HogarDomicilio.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/HogarDomicilio/GetByDistrito/5
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByDistrito(int dis_Id_Distrito)
        {
            // HogarDomicilio hogardomicilio = new HogarDomicilio();
            try
            {
                var domicilios = (from hd in context.HogarDomicilio
                    join dis in context.Distrito
                    on hd.dis_Id_Distrito equals dis.dis_Id_Distrito
                    join sec in context.Sector
                    on hd.sec_Id_Sector equals sec.sec_Id_Sector
                    join pais in context.Pais
                    on hd.pais_Id_Pais equals pais.pais_Id_Pais
                    join est in context.Estado
                    on hd.est_Id_Estado equals est.est_Id_Estado
                    join sub in (from hp in context.Hogar_Persona
                                join p in context.Persona
                                on hp.hp_Id_Hogar_Persona equals p.per_Id_Persona
                                where hp.hp_Jerarquia == 1
                                select new
                                {
                                    hp.hp_Id_Hogar_Persona,
                                    hp.hp_Jerarquia,
                                    hp.hd_Id_Hogar,
                                    p.per_Id_Persona,
                                    p.per_Nombre,
                                    p.per_Activo,
                                    p.per_Apellido_Paterno,
                                    p.per_Apellido_Materno
                                }) on hd.hd_Id_Hogar equals sub.hd_Id_Hogar
                    where hd.dis_Id_Distrito == dis_Id_Distrito
                    select new
                    {
                        hd.hd_Id_Hogar,
                        hd.hd_Calle,
                        hd.hd_Numero_Exterior,
                        hd.hd_Numero_Interior,
                        hd.hd_Tipo_Subdivision,
                        hd.hd_Subdivision,
                        hd.hd_Localidad,
                        hd.hd_Municipio_Ciudad,
                        hd.pais_Id_Pais,
                        hd.est_Id_Estado,
                        hd.hd_Activo,
                        hd.dis_Id_Distrito,
                        hd.sec_Id_Sector,
                        est.est_Nombre,
                        pais.pais_Nombre_Corto,
                        hd.hd_Telefono,
                        hd.hd_CP,
                        dis.dis_Numero,
                        dis.dis_Alias,
                        sec.sec_Alias,
                        sub.hp_Id_Hogar_Persona,
                        sub.hp_Jerarquia,
                        sub.per_Id_Persona,
                        sub.per_Nombre,
                        sub.per_Apellido_Paterno,
                        sub.per_Apellido_Materno
                    }).ToList();
                return Ok(
                    new {
                        status = "success",
                        data = domicilios
                    });
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = ex.Message
                    });
            }
        }

        // GET: api/HogarDomicilio/GetBySector/5
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBySector(int sec_Id_Sector)
        {
            // HogarDomicilio hogardomicilio = new HogarDomicilio();
            try
            {
                var domicilios = (from hd in context.HogarDomicilio
                                  join dis in context.Distrito on hd.dis_Id_Distrito equals dis.dis_Id_Distrito
                                  join sec in context.Sector on hd.sec_Id_Sector equals sec.sec_Id_Sector
                                  join pais in context.Pais on hd.pais_Id_Pais equals pais.pais_Id_Pais
                                  join est in context.Estado on hd.est_Id_Estado equals est.est_Id_Estado
                                  join sub in (from hp in context.Hogar_Persona
                                        join p in context.Persona on hp.per_Id_Persona equals p.per_Id_Persona
                                        join d in context.HogarDomicilio on hp.hd_Id_Hogar equals d.hd_Id_Hogar
                                        where hp.hp_Jerarquia == 1 && d.hd_Activo == true
                                        select new {
                                            hp.hp_Id_Hogar_Persona,
                                            hp.hp_Jerarquia,
                                            hp.hd_Id_Hogar, 
	                                        p.per_Id_Persona,
                                            p.per_Activo,
                                            p.per_Nombre,
                                            p.per_Apellido_Paterno,
                                            p.per_Apellido_Materno,
                                            p.per_Fecha_Nacimiento,
                                            p.per_Bautizado,
                                            p.per_Telefono_Movil
                                        }) on hd.hd_Id_Hogar equals sub.hd_Id_Hogar
                                  where hd.sec_Id_Sector == sec_Id_Sector
                                  select new
                                  {
                                      hd.hd_Id_Hogar,
                                      hd.hd_Calle,
                                      hd.hd_Numero_Exterior,
                                      hd.hd_Numero_Interior,
                                      hd.hd_Tipo_Subdivision,
                                      hd.hd_Subdivision,
                                      hd.hd_Localidad,
                                      hd.hd_Municipio_Ciudad,
                                      est.est_Id_Estado,
                                      est.est_Nombre,
                                      pais.pais_Id_Pais,
                                      pais.pais_Nombre_Corto,
                                      hd.hd_Telefono,
                                      hd.hd_CP,
                                      hd.hd_Activo,
                                      dis.dis_Id_Distrito,
                                      dis.dis_Numero,
                                      dis.dis_Alias,
                                      sec.sec_Id_Sector,
                                      sec.sec_Alias,
                                      sec.sec_Numero,
                                      sub.hp_Id_Hogar_Persona,
                                      sub.hp_Jerarquia,
                                      sub.per_Id_Persona,
                                      sub.per_Nombre,
                                      sub.per_Apellido_Paterno,
                                      sub.per_Apellido_Materno,
                                      sub.per_Fecha_Nacimiento,
                                      sub.per_Bautizado,
                                      sub.per_Telefono_Movil
                                  }).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        domicilios = domicilios
                    });
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = ex.Message
                    });
            }
        }

        // GET: api/HogarDomicilio/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {


            try
            {
                //var hogardomicilio = (from hd in context.HogarDomicilio
                //                  join pais in context.Pais on hd.pais_Id_Pais equals pais.pais_Id_Pais
                //                  join est in context.Estado on hd.est_Id_Estado equals est.est_Id_Estado
                //                  where hd.hd_Id_Hogar == id
                //                  select new
                //                  {
                //                      hd.hd_Activo,
                //                      hd.hd_Calle,
                //                      hd.hd_Id_Hogar,
                //                      hd.hd_Localidad,
                //                      hd.hd_Municipio_Ciudad,
                //                      hd.hd_Numero_Exterior,
                //                      hd.hd_Numero_Interior,
                //                      hd.hd_Subdivision,
                //                      hd.hd_Telefono,
                //                      hd.hd_Tipo_Subdivision,
                //                      pais.pais_Nombre,
                //                      pais.pais_Nombre_Corto,
                //                      est.est_Nombre,
                //                      est.est_Nombre_Corto
                //                  }).ToList();
                //var noExterior = hogardomicilio[0].hd_Numero_Exterior == null || hogardomicilio[0].hd_Numero_Exterior == "" ? "S/N" : hogardomicilio[0].hd_Numero_Exterior;
                //var noInterior = hogardomicilio[0].hd_Numero_Interior == null || hogardomicilio[0].hd_Numero_Interior == "" ? "" : " int. " + hogardomicilio[0].hd_Numero_Interior;
                //var tipoAsentamiento = hogardomicilio[0].hd_Tipo_Subdivision == null || hogardomicilio[0].hd_Tipo_Subdivision == "" ? "" : hogardomicilio[0].hd_Tipo_Subdivision;
                //var asentamiento = hogardomicilio[0].hd_Subdivision == null || hogardomicilio[0].hd_Subdivision == "" ? "" : $"{ tipoAsentamiento} {hogardomicilio[0].hd_Subdivision}";
                //var localidad = hogardomicilio[0].hd_Localidad == null || hogardomicilio[0].hd_Localidad == "" ? "" : $"{hogardomicilio[0].hd_Localidad},";
                //var direccion = "";
                //if (hogardomicilio[0].pais_Nombre_Corto == "USA" || hogardomicilio[0].pais_Nombre_Corto == "CAN")
                //{
                //    direccion = $"{noExterior} {noInterior} {hogardomicilio[0].hd_Calle}, {asentamiento} {localidad} {hogardomicilio[0].hd_Municipio_Ciudad}, {hogardomicilio[0].est_Nombre}, {hogardomicilio[0].pais_Nombre_Corto}.";
                //}
                //else
                //{
                //    direccion = $"{hogardomicilio[0].hd_Calle} {noExterior}{noInterior}, {asentamiento}, {localidad} {hogardomicilio[0].hd_Municipio_Ciudad}, {hogardomicilio[0].est_Nombre}, {hogardomicilio[0].pais_Nombre_Corto}.";
                //}
                var hogares = new Hogares(context);
                var hogardomicilio = hogares.Address(id);
                var direccion = hogares.getDireccion(id);
                
                return Ok(new
                {
                    status = "success",
                    direccion,
                    hogardomicilio
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "success",
                    mensaje = ex.Message
                });
            }
        }

        // GET: api/HogarDomicilio/getListaHogaresBySector/IdSector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetListaHogaresBySector(int sec_Id_Sector)
        {
            try
            {
                //Instancia de clase Hogares para usar el Método que sirve para traer la Lista de Hogares y sus integrantes
                var hogares = new Hogares(context);
                var listaHogares = hogares.ListaHogaresBySector(sec_Id_Sector);

                return Ok(new
                {
                    status = true,
                    listahogares = listaHogares
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "success",
                    mensaje = ex.Message
                });
            }
        }


        // GET: api/HogarDomicilio/getListaHogaresBySector/IdSector
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetListaHogaresByDistrito(int dis_Id_Distrito)
        {
            try
            {
                //Instancia de clase Hogares para usar el Método que trae la Lista de Hogares y sus integrantes
                var hogares = new Hogares(context);
                var listaHogares = hogares.ListaHogaresByDistrito(dis_Id_Distrito);

                return Ok(new
                {
                    status = true,
                    listahogares = listaHogares
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "success",
                    mensaje = ex.Message
                });
            }
        }

        // POST: api/HogarDomicilio
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] HogarDomicilio hogardomicilio)
        {
            try
            {
                context.HogarDomicilio.Add(hogardomicilio);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = true,
                        nvoHogarDomicilio = hogardomicilio.hd_Id_Hogar
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest
                (
                    new
                    {
                        status = false,
                        message = ex.Message
                    }
                );
            }
        }

        //// GET: api/HogarDomicilio/getListaHogaresBySector/IdSector
        //[Route("[action]/{sec_Id_Sector}")]
        //[HttpGet]
        //[EnableCors("AllowOrigin")]
        //public ActionResult getListaHogaresBySector(int sec_Id_Sector)
        //{
        //    try
        //    {
        //        //Instancia de clase Hogares para usar el Método que trae la Lista de Hogares y sus integrantes
        //        var hogares = new Hogares(context);
        //        var listaHogares = hogares.ListaHogaresBySector(sec_Id_Sector);

        //        return Ok(new
        //        {
        //            status = true,
        //            listahogares = listaHogares
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = "success",
        //            mensaje = ex.Message
        //        });
        //    }
        //}

        // PUT: api/HogarDomicilio/EditaDomicilio/5
        [HttpPut("{id}/{nvoEstado?}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put([FromBody] HogarDomicilio hogardomicilio, int id, string nvoEstado="")
        {
            try {
                // CONSULTA EL TITULAR DEL HOGAR
                Hogar_Persona hp = context.Hogar_Persona.FirstOrDefault(
                h => h.hd_Id_Hogar == hogardomicilio.hd_Id_Hogar
                && h.hp_Jerarquia == 1);


                // ALTA DE DOMICILIO
                HogarDomicilio hd = new HogarDomicilio();
                hd = hogardomicilio; //Los Datos del Domicilio los pone en la variable 'hd'
                int idNvoEstado = 0;
                var estados = (from e in context.Estado     //Trae los Estados/Provincias del País que trae el 'domicilio'
                               where e.pais_Id_Pais == hd.pais_Id_Pais
                               select e).ToList();

                //Si el campo nvoEstado trae un nuevo Estado, lo agrega a la Tabla 'Estado' y envía el email de solicitud del Nuevo Estado a Soporte Técnico.
                if (nvoEstado != "")
                {
                    var pais = context.Pais.FirstOrDefault(pais2 => pais2.pais_Id_Pais == hd.pais_Id_Pais);
                    var est = new Estado
                    {
                        est_Nombre_Corto = nvoEstado.Substring(0, 3),
                        est_Nombre = nvoEstado,
                        pais_Id_Pais = hd.pais_Id_Pais,
                        est_Pais = pais.pais_Nombre_Corto,
                    };
                    context.Estado.Add(est);
                    context.SaveChanges();
                    idNvoEstado = est.est_Id_Estado;

                    SendMailController sendMail = new SendMailController(context);
                    sendMail.EnviarSolicitudNvoEstado(pais.pais_Id_Pais, hd.usu_Id_Usuario, hp.per_Id_Persona, nvoEstado);
                }

                // Guarda cambios en el domicilio con el Estado/Provincia Seleccionado o con el Recien Creado
                hd.Fecha_Registro = fechayhora;
                hd.est_Id_Estado = nvoEstado != "" ? idNvoEstado : hd.est_Id_Estado;
                hd.usu_Id_Usuario = hd.usu_Id_Usuario;
                context.Entry(hogardomicilio).State = EntityState.Modified;
                context.SaveChanges();

                // Guarda registro historico

                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                hte.RegistroHistorico(hp.per_Id_Persona, hogardomicilio.sec_Id_Sector, 31203, "EDICION DE DOMICILIO", fechayhora, hogardomicilio.usu_Id_Usuario);

                return Ok(
                    new
                    {
                        status = "success",
                        domicilio = hogardomicilio
                    });
            }
            catch (Exception ex) {
                return Ok(
                    new
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
            HogarDomicilio hogardomicilio = new HogarDomicilio();
            hogardomicilio = context.HogarDomicilio.FirstOrDefault(hd => hd.hd_Id_Hogar == id);
            if (hogardomicilio != null)
            {
                context.HogarDomicilio.Remove(hogardomicilio);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
