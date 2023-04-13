using System;
using System.Collections.Generic;
using System.Linq;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstadoController : ControllerBase
    {
        private readonly AppDbContext context;

        public EstadoController(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        // GET: api/Estado
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IEnumerable<Estado> Get()
        {
            return context.Estado.ToList();
        }

        // GET: api/Estado/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public Estado Get(int id)
        {
            var estado = context.Estado.FirstOrDefault(e => e.est_Id_Estado == id);
            return estado;
        }

        // GET: api/Estado/GetEstadoByIdPais/151
        [Route("[action]/{pais_Id_Pais}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetEstadoByIdPais(int pais_Id_Pais)
        {
            try
            {
                var query = (from est in context.Estado
                             join pais in context.Pais
                             on est.pais_Id_Pais equals pais.pais_Id_Pais
                             where est.pais_Id_Pais == pais_Id_Pais
                             orderby est.est_Nombre ascending
                             select new
                             {
                                 est_Id_Estado = est.est_Id_Estado,
                                 est_Nombre_Corto = est.est_Nombre_Corto,
                                 est_Pais = est.est_Pais,
                                 est_Nombre = est.est_Nombre,
                                 pais_Id_Pais = est.pais_Id_Pais
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        estados = query
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        status = false,
                        message = ex.Message
                    });
            }
        }

        // POST: api/Estado/SolicitudNvoEstado/{nvoEstado}/{pais_Id_Pais}/{usu_Id_Usuario}
        [Route("[action]/{nvoEstado}/{pais_Id_Pais}/{usu_Id_Usuario}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult SolicitudNvoEstado(string nvoEstado, int pais_Id_Pais, int usu_Id_Usuario)
        {
            try
            {
                //Graba los datos de la Solicitud en la Tabla 'SolicitudNvoEstado'.
                var solicitud = new SolicitudNvoEstado
                {
                    nombreNvoEstado = nvoEstado,
                    solicitudAtendida = false,
                    usu_Id_Usuario = usu_Id_Usuario,
                    fechaSolicitud = fechayhora,
                    pais_Id_Pais = pais_Id_Pais
                };
                context.SolicitudNvoEstado.Add(solicitud);
                context.SaveChanges();

                // AGREGA NUEVO ESTADO en la Tabla 'Estado'
                var pais = context.Pais.FirstOrDefault(pais2 => pais2.pais_Id_Pais == pais_Id_Pais);
                var est = new Estado
                {
                    est_Nombre_Corto = nvoEstado.Substring(0, 3),
                    est_Nombre = nvoEstado,
                    pais_Id_Pais = pais_Id_Pais,
                    est_Pais = pais.pais_Nombre_Corto
                };
                context.Estado.Add(est);
                context.SaveChanges();

                //Envía un Email a Soporte Tecnico para que ellos hagan los ajustes que crean pertinentes, como la Abreviación(Nombre Corto) del Nuevo Estado
                SendMailController sendMail = new SendMailController(context);
                sendMail.EnviarSolicitudNvoEstado(pais_Id_Pais, usu_Id_Usuario, 1, nvoEstado);

                return Ok(new
                {
                    status = "success",
                    estado = est
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

        // POST: api/Estado
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Estado/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public void Delete(int id)
        {
        }
    }
}
