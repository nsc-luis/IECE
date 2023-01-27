using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
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

        public class datosParaCambio
        {
            public string cadenaDeValidacion { get; set; }
            public string nvaContrasena { get; set; }
        }

        private static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        string REMITENTE = "soporte@iece.mx";
        string EMAILPASSWORD = Environment.GetEnvironmentVariable("emailPass");//"s0p0rt3$";
        string SMTPSERVER = "mail.iece.mx";
        int PUERTO = 587;
        bool ENCRIPTACION = false;
        bool FORMATO = true;

        // POST api/<controller>
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult GenerarValidacionParaCambio(string correo)
        {
            try
            {
                DateTime caducidad = DateTime.Now.AddHours(24);
                var emailIECE = context.Personal_Ministerial.FirstOrDefault(ep => ep.pem_emailIECE == correo);
                var emailPersonal = context.Personal_Ministerial.FirstOrDefault(ep => ep.pem_email_Personal == correo);

                bool ministroActivo = false;

                if (emailIECE != null)
                {
                    if (emailIECE.pem_emailIECE == correo)
                    {
                        ministroActivo = emailIECE.pem_Activo;
                    }
                }

                if (emailPersonal != null)
                {
                    if (emailPersonal.pem_email_Personal == correo)
                    {
                        ministroActivo = emailPersonal.pem_Activo;
                    }
                }

                var validaCorreo = (from u in context.Usuario
                                    where u.Email == correo
                                    select u).ToList();

                if (validaCorreo.Count > 0 && ministroActivo)
                {
                    var datosParaCambio = new Valida_Cambio_Contrasena
                    {
                        vcc_Cadena = RandomString(100),
                        vcc_Correo = correo,
                        vcc_Caducidad = caducidad
                    };
                    context.Valida_Cambio_Contrasena.Add(datosParaCambio);
                    context.SaveChanges();

                    datos datosEnvioCorreo = new datos
                    {
                        smtpServer = SMTPSERVER,
                        puerto = PUERTO,
                        remitente = REMITENTE,
                        password = EMAILPASSWORD,
                        encriptacion = ENCRIPTACION,
                        formato = FORMATO,
                        //destinatario = "nsc_luis@nscco.com.mx",
                        //destinatario = "nsc_luis@nscco.com.mx;jacinto_molina@yahoo.com",
                        destinatario = $"soporte@iece.mx;{correo}",
                        asunto = "IECE WebApp, Solicitud de cambio de contraseña.",
                        //mensaje = "http://localhost:3000/ValidaCambioDeContrasena?cadenaDeValidacion=" + datosParaCambio.vcc_Cadena
                        mensaje = "<html><body>" +
                        "Para proceder con el cambio de su contraseña para el acceso a la Aplicación " +
                        "Web de Control de Membresía de la IECE, acceda al siguiente enlace: " +
                        "<br /><br /><br />" +
                        "http://iece-tpr.ddns.net:81/ValidaCambioDeContrasena?cadenaDeValidacion=" + datosParaCambio.vcc_Cadena +
                        "<br /><br /><br />" +
                        "Cualquier problema con el cambio de su contraseña comuniquese al " +
                        "Departamento de Soporte Técnico al siguiente correo: soporte@iece.mx " +
                        "<br /><br /> Dios le bendiga!" +
                        "</body></html>"
                    };

                    EnviaCorreoDeRestablecimiento(datosEnvioCorreo);

                    return Ok(new
                    {
                        status = "success"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Error: El correo ingresado NO esta registrado ó el usuario/ministro NO esta activo."
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

        // GET api/ValidaCambioDeContrasena?cadenaDeValidacion=[cadena]
        [HttpGet]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult ValidaCambioDeContrasena(string cadenaDeValidacion)
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
                    return Ok(new
                    {
                        status = "success",
                        datosParaCambio = validaDatos
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Error: La cadena de validación es incorrecta o ya caducó."
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

        // POST api/CambiarContrasena/cadenaDeValidacion/nvaContrasena
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult CambiarContrasena([FromBody] datosParaCambio datos)
        {
            try
            {
                var validaDatos = (from vcc in context.Valida_Cambio_Contrasena
                                   where vcc.vcc_Cadena == datos.cadenaDeValidacion
                                   select vcc).ToList();

                DateTime fechahora = DateTime.Now;

                if (validaDatos.Count > 0
                    && fechahora < validaDatos[0].vcc_Caducidad)
                {
                    var usuario = context.Usuario.FirstOrDefault(u => u.Email == validaDatos[0].vcc_Correo);
                    usuario.PasswordHash = PasswordHasher.GenerateIdentityV3Hash(datos.nvaContrasena);
                    context.Usuario.Update(usuario);
                    context.SaveChanges();

                    return Ok(new
                    {
                        status = "success",
                        mensaje = "Contraseña cambiada satisfactoriamente!."
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "Error: La cadena de validación es incorrecta o ya caducó."
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
        [EnableCors("AllowOrigin")]
        public IActionResult EnviaCorreoDeRestablecimiento([FromBody]datos objeto)
        {
            var destinatarios = objeto.destinatario.Split(';');
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = objeto.smtpServer;
                smtp.Port = objeto.puerto;
                smtp.EnableSsl = objeto.encriptacion;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
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

        // POST /api/EnviarSolicitudNvaProfesion/{descProf}/{usu_Id_Usuario}
        [HttpPost]
        [Route("[action]/{usu_Id_Usuario}/{per_Id_Persona}/{descNvaProf=}")]
        [EnableCors("AllowOrign")]
        public IActionResult EnviarSolicitudNvaProfesion(int usu_Id_Usuario, int per_Id_Persona, string descNvaProf)
        {
            try
            {
                var persona = context.Persona.FirstOrDefault(p => p.per_Id_Persona == per_Id_Persona);
                var ministro = context.Personal_Ministerial.FirstOrDefault(m => m.pem_Id_Ministro == usu_Id_Usuario);
                datos datosEnvioCorreo = new datos
                {
                    smtpServer = SMTPSERVER,
                    puerto = PUERTO,
                    remitente = REMITENTE,
                    password = EMAILPASSWORD,
                    encriptacion = ENCRIPTACION,
                    formato = FORMATO,
                    //destinatario = "nsc_luis@nscco.com.mx",
                    //destinatario = "nsc_luis@nscco.com.mx;jacinto_molina@yahoo.com",
                    destinatario = "soporte@iece.mx",
                    asunto = "IECE WebApp. Solicitud de nueva profesion.",
                    mensaje = "<html><body>Paz de Dios. <br />" +
                        $"El ministro <strong>{ministro.pem_Nombre}</strong> a ingresado/editador a la persona " +
                        $"<strong>{persona.per_Nombre} {persona.per_Apellido_Paterno} (per_Id_Persona: {persona.per_Id_Persona})</strong> con " +
                        "una profesion nueva, la cual es: " +
                        $"<ul><li><strong>{descNvaProf}</strong></li></ul>" +
                        "Se añade registro de la solicitud a la base de datos para seguimiento." +
                        "<br />Dios bendiga!" +
                        "</body></html>"
                };
                SmtpClient smtp = new SmtpClient();
                smtp.Host = datosEnvioCorreo.smtpServer;
                smtp.Port = datosEnvioCorreo.puerto;
                smtp.EnableSsl = datosEnvioCorreo.encriptacion;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(datosEnvioCorreo.remitente, datosEnvioCorreo.password);
                MailMessage message = new MailMessage();
                message.From = new MailAddress(datosEnvioCorreo.remitente);
                message.To.Add(new MailAddress(datosEnvioCorreo.destinatario));
                message.Bcc.Add(new MailAddress("nsc_luis@nscco.com.mx"));
                //message.To.Add(new MailAddress("jacinto_molina@yahoo.com"));
                // message.ReplyToList.Add(new MailAddress(objeto.remitente));
                message.Subject = datosEnvioCorreo.asunto;
                message.IsBodyHtml = datosEnvioCorreo.formato;
                message.Body = datosEnvioCorreo.mensaje;
                smtp.Send(message);
                return Ok(new
                {
                    status = "success",
                    mensaje = message
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "errro",
                    mensaje = ex.Message
                });
            }
        }

        // POST /api/EnviarSolicitudNvaProfesion/{descProf}/{usu_Id_Usuario}
        [HttpPost]
        [Route("[action]/{pais_Id_Pais}/{usu_Id_Usuario}/{nvoEstado=}")]
        [EnableCors("AllowOrign")]
        public IActionResult EnviarSolicitudNvoEstado(
            int pais_Id_Pais,
            int usu_Id_Usuario,
            int per_Id_Persona,
            string nvoEstado)
        {

            try
            {
                if (nvoEstado != "" || nvoEstado != null)
                {
                    var pais = context.Pais.FirstOrDefault(p => p.pais_Id_Pais == pais_Id_Pais);
                    var ministro = context.Personal_Ministerial.FirstOrDefault(m => m.pem_Id_Ministro == usu_Id_Usuario);
                    var persona = context.Persona.FirstOrDefault(p => p.per_Id_Persona == per_Id_Persona);
                    datos datosEnvioCorreo = new datos
                    {
                        smtpServer = SMTPSERVER,
                        puerto = PUERTO,
                        remitente = REMITENTE,
                        password = EMAILPASSWORD,
                        encriptacion = ENCRIPTACION,
                        formato = FORMATO,
                        //destinatario = "nsc_luis@nscco.com.mx",
                        //destinatario = "nsc_luis@nscco.com.mx;jacinto_molina@yahoo.com",
                        destinatario = "soporte@iece.mx",
                        asunto = "IECE WebApp. Solicitud de nuevo estado.",
                        mensaje = "<html><body>Paz de Dios. <br />" +
                            $"El ministro <strong>{ministro.pem_Nombre}</strong> a ingresado un nuevo estado " +
                            $"para el pais {pais.pais_Nombre}, la cual es: " +
                            $"<ul><li><strong>{nvoEstado}</strong></li>" +
                            $"<li>Para la persona: <strong>{persona.per_Nombre} {persona.per_Apellido_Paterno}</strong></li></ul>" +
                            "Se añade registro de la solicitud a la base de datos para seguimiento." +
                            "<br />Dios bendiga!" +
                            "</body></html>"
                    };
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = datosEnvioCorreo.smtpServer;
                    smtp.Port = datosEnvioCorreo.puerto;
                    smtp.EnableSsl = datosEnvioCorreo.encriptacion;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(datosEnvioCorreo.remitente, datosEnvioCorreo.password);
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(datosEnvioCorreo.remitente);
                    message.To.Add(new MailAddress(datosEnvioCorreo.destinatario));
                    message.Bcc.Add(new MailAddress("nsc_luis@nscco.com.mx"));
                    //message.To.Add(new MailAddress("jacinto_molina@yahoo.com"));
                    //message.ReplyToList.Add(new MailAddress(objeto.remitente));
                    message.Subject = datosEnvioCorreo.asunto;
                    message.IsBodyHtml = datosEnvioCorreo.formato;
                    message.Body = datosEnvioCorreo.mensaje;
                    smtp.Send(message);
                    return Ok(new
                    {
                        status = "success",
                        mensaje = message
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No se ha ingresado ningun estado nuevo."
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "errro",
                    mensaje = ex.Message
                });
            }
        }
    }
}