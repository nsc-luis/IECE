using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMailController : ControllerBase
    {
        private readonly AppDbContext context;

        public SendMailController(AppDbContext context)
        {
            this.context = context;
        }

        public class datos
        {
            public string smtpServer { get; set; }
            public int puerto { get; set; }
            public string remitente { get; set; }
            public string password { get; set; }
            public bool encriptacion { get; set; }
            public bool formato { get; set; }
            public string destinatario { get; set; }
            public string asunto { get; set; }
            public string mensaje { get; set; }
        }

        private static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // POST api/<controller>
        [HttpPost]
        [Route("[action]")]
        public IActionResult GenerarValidacionParaCambio(string correo)
        {
            try
            {
                DateTime caducidad = DateTime.Now.AddHours(24);

                var validaCorreo = (from u in context.Usuario
                                    where u.Email == correo
                                    select u).ToList();

                if (validaCorreo.Count > 0)
                {
                    var datosParaCambio = new Valida_Cambio_Contrasena
                    {
                        vcc_Cadena = RandomString(100),
                        vcc_Correo = correo,
                        vcc_Caducidad = caducidad
                    };
                    context.Valida_Cambio_Contrasena.Add(datosParaCambio);
                    context.SaveChanges();

                    return Ok(new
                    {
                        status = "success",
                        datos = datosParaCambio
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Error: El correo ingresado no es valido."
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

        // POST api/<controller>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CambiarContrasena(string cadenaDeValidacion, string nvaContrasena)
        {
            try
            {
                var validaDatos = (from vcc in context.Valida_Cambio_Contrasena
                                   where vcc.vcc_Cadena == cadenaDeValidacion
                                   select vcc).ToList();

                DateTime fechahora = DateTime.Now;

                if (validaDatos.Count > 0
                    && fechahora < validaDatos[0].vcc_Caducidad)
                {
                    var usuario = context.Usuario.FirstOrDefault(u => u.Email == validaDatos[0].vcc_Correo);
                    usuario.PasswordHash = PasswordHasher.GenerateIdentityV3Hash(nvaContrasena);
                    context.Usuario.Update(usuario);
                    context.SaveChanges();

                    return Ok(new
                    {
                        status = "success",
                        mensaje = "Contraseña cambiada satisfactoriamente!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Error: Ocurrio un error!."
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

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]datos objeto)
        {
            var destinatarios = objeto.destinatario.Split(';');
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = objeto.smtpServer;
                smtp.Port = objeto.puerto;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = objeto.encriptacion;
                smtp.Credentials = new NetworkCredential(objeto.remitente, objeto.password);
                MailMessage message = new MailMessage();
                message.From = new MailAddress(objeto.remitente);
                foreach (string destinatario in destinatarios)
                {
                    message.To.Add(new MailAddress(destinatario));
                }
                // message.ReplyToList.Add(new MailAddress(objeto.remitente));
                message.Subject = objeto.asunto;
                message.IsBodyHtml = objeto.formato;
                message.Body = objeto.mensaje;
                smtp.Send(message);
                return Ok(
                    new
                    {
                        status = "success",
                        mensaje = "El mensaje fue enviado correctamente"
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
    }
}