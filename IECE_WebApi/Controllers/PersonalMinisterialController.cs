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
using System.Globalization;
using System.Text;
using IECE_WebApi.Helpers;
using System.IO;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonalMinisterialController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonalMinisterialController(AppDbContext context)
        {
            this.context = context;
        }

        public class infoNvoAuxiliar
        {
            public int per_Id_Persona { get; set; }
            //public string comentario { get; set; }
            //public DateTime fecha { get; set; }
        }

        public class infoNvoSecretario
        {
            public int pem_Id_Ministro { get; set; }
            public int sec_Id_Sector { get; set; }
            public int idUsuario { get; set; }
            //public string comentario { get; set; }
            //public DateTime fecha { get; set; }
        }

        public class infoNvoSecretarioDto
        {
            public int pem_Id_Ministro { get; set; }
            public int dis_Id_Distrito { get; set; }
            public int idUsuario { get; set; }
            public string comentario { get; set; }
            public DateTime fecha { get; set; }
        }

        private DateTime fechayhora = DateTime.UtcNow;

        public class PersonaAVincular
        {
            public int id_Persona { get; set; }
            public int id_Ministro { get; set; }
            public string nombre_Persona { get; set; }
            public string nombre_Elemento { get; set; }
            public int sec_Id_Sector { get; set; }
            public int usu_Id_Usuario { get; set; }
            public DateTime fechaTransaccion { get; set; }
        }

        public class AuxiliarBaja
        {
            public int id_Ministro { get; set; }
            public string nombre_Elemento { get; set; }
            public int sec_Id_Sector { get; set; }
            public int usu_Id_Usuario { get; set; }
            public string causaDeBaja { get; set; }
            public DateTime fechaTransaccion { get; set; }
        }

        public class PersonalAdministrativo
        {
            public string cargo { get; set; }
            public Personal_Ministerial datosPersonalMinisterial { get; set; }
        }

        public class infoNuevaAsignacion
        {
            public int pem_Id_Ministro { get; set; }
            public int dis_Id_Distrito { get; set; }
            public int sec_Id_Sector { get; set; }
            public int idUsuario { get; set; }
            public string puesto { get; set; }
        }

        public class personalMinisterialConFoto : Personal_Ministerial
        {
            public byte[] Imagen { get; set; }
            public string MIMEType { get; set; }

        }

        public class personalMinisterialConFotoDto : Personal_Ministerial
        {
            public int per_Id_Persona { get; set; }

            public int sec_Id_Sector { get; set; }

            public string sector { get; set; }
            public bool per_Activo { get; set; }
            public bool per_En_Comunion { get; set; }
            public bool per_Vivo { get; set; }
            public string per_Nombre { get; set; }
            public string per_Apellido_Paterno { get; set; }
            public string per_Apellido_Materno { get; set; }
            public string per_Apellido_Casada { get; set; }
            public string apellidoPrincipal { get; set; }
            public bool per_Bautizado { get; set; }
            public byte[] imagen { get; set; }
            public string MIMEType { get; set; }

        }


        // GET: api/Personal_Ministerial
        // METODO PARA LISTAR PERSONAL MINISTERIAL
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult Get()
        {
            try
            {
                var query = (from pem in context.Personal_Ministerial
                             select new
                             {
                                 pem_Id_Ministro = pem.pem_Id_Ministro,
                                 pem_Activo = pem.pem_Activo,
                                 per_Id_Miembro = pem.per_Id_Miembro,
                                 pem_Nombre = pem.pem_Nombre,
                                 sec_Id_Congregacion = pem.sec_Id_Congregacion,
                                 pem_En_Permiso = pem.pem_En_Permiso,
                                 pem_emailIECE = pem.pem_emailIECE,
                                 pem_email_Personal = pem.pem_email_Personal,
                                 pem_Grado_Ministerial = pem.pem_Grado_Ministerial
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        personalMinisterial = query
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER LOS DISTRITOS/SECTORES CON ACCESO SEGUN ID DEL MINISTRO
        [Route("[action]/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetAlcancePastoralByMinistro(int idMinistro)
        {
            var query = (from d in context.Distrito
                         where d.pem_Id_Obispo == idMinistro && d.dis_Activo == true
                         orderby d.dis_Numero ascending, d.dis_Tipo_Distrito ascending
                         select new
                         {
                             d.dis_Id_Distrito,
                             d.dis_Alias,
                             d.dis_Tipo_Distrito,
                             d.dis_Numero
                         }).ToList();

            if (query.Count > 0)
            {
                return Ok(new
                {
                    status = "success",
                    obispo = true,
                    datos = query
                });
            }
            else if (query.Count == 0)
            {
                var query2 = (from s in context.Sector
                              join d in context.Distrito
                              on s.dis_Id_Distrito equals d.dis_Id_Distrito
                              where s.pem_Id_Pastor == idMinistro && s.sec_Tipo_Sector == "SECTOR"
                              orderby d.dis_Numero ascending, s.sec_Numero ascending
                              select new
                              {
                                  d.dis_Id_Distrito,
                                  d.dis_Alias,
                                  d.dis_Tipo_Distrito,
                                  d.dis_Numero,
                                  s.sec_Alias,
                                  s.sec_Id_Sector,
                                  s.sec_Tipo_Sector
                              }).ToList();
                if (query2.Count > 0)
                {
                    return Ok(new
                    {
                        status = "success",
                        obispo = false,
                        datos = query2
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No se encontraron registros para el ID del Ministro."
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = "No se encontraron registros para el ID del Ministro."
                });
            }
        }

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER INFO DEL SECTOR SEGUN ID DEL MINISTRO
        [Route("[action]/{sec_Id_Congregacion}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMinistrosBySector(int sec_Id_Congregacion)
        {
            try
            {
                var query = (from pem in context.Personal_Ministerial
                             where pem.sec_Id_Congregacion == sec_Id_Congregacion
                             && pem.pem_Grado_Ministerial == "ANCIANO"
                             && pem.pem_Activo == true
                             orderby pem.pem_Nombre ascending
                             select new
                             {
                                 pem_Id_Ministro = pem.pem_Id_Ministro,
                                 pem_Activo = pem.pem_Activo,
                                 per_Id_Miembro = pem.per_Id_Miembro,
                                 pem_Nombre = pem.pem_Nombre,
                                 sec_Id_Congregacion = pem.sec_Id_Congregacion,
                                 pem_En_Permiso = pem.pem_En_Permiso,
                                 pem_emailIECE = pem.pem_emailIECE,
                                 pem_email_Personal = pem.pem_email_Personal,
                                 pem_Grado_Ministerial = pem.pem_Grado_Ministerial
                             }).ToList();
                return Ok(
                    new
                    {
                        status = true,
                        personalMinisterial = query
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }

        // GET: api/GetMinistrosAncianoActivoByDistrito/idDistrito
        [Route("[action]/{idDistrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetMinistrosAncianoActivoByDistrito(int idDistrito)
        {
            try
            {
                var query = (from pem in context.Personal_Ministerial
                              where pem.pem_Activo == true && pem.pem_Grado_Ministerial == "ANCIANO"
                              && (from s in context.Sector where s.dis_Id_Distrito == idDistrito select s.sec_Id_Sector).AsQueryable().Contains(pem.sec_Id_Congregacion)
                              select new
                              {
                                  pem.pem_Id_Ministro,
                                  pem.pem_Nombre
                              }).ToList();
                return Ok(new
                {
                    status = "success",
                    ministros = query
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

        // GET: api/GetSectoresByMinistro/idMinistro
        [Route("[action]/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSectoresByMinistro(int idMinistro)
        {
            try
            {
                var query = (from s in context.Sector
                             join d in context.Distrito
                             on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where s.pem_Id_Pastor == idMinistro && s.sec_Tipo_Sector == "SECTOR"
                             orderby d.dis_Numero ascending, s.sec_Numero ascending
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 d.dis_Tipo_Distrito,
                                 d.dis_Numero,
                                 d.pem_Id_Obispo,
                                 s.sec_Alias,
                                 s.sec_Id_Sector,
                                 s.sec_Tipo_Sector
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    sectores = query
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

        // GET: api/GetSectoresByDistritoMinistro/idDistrito/idMinistro
        [Route("[action]/{idDistrito}/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSectoresByDistritoMinistro(int idDistrito, int idMinistro)
        {
            try
            {
                var consultaObispo = context.Distrito.FirstOrDefault(d => d.pem_Id_Obispo == idMinistro && d.dis_Id_Distrito == idDistrito);
                bool obispo = consultaObispo == null ? false : true;

                var query = (from s in context.Sector
                             join d in context.Distrito
                             on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where s.dis_Id_Distrito == idDistrito && s.pem_Id_Pastor == idMinistro
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 d.dis_Tipo_Distrito,
                                 d.dis_Numero,
                                 s.sec_Alias,
                                 s.sec_Id_Sector,
                                 s.sec_Tipo_Sector
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    sectores = query,
                    obispo
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

        // GET api/GetSecretarioBySector/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSecretarioBySector(int idSector)
        {
            try
            {
                var query = (from s in context.Sector
                             join pem in context.Personal_Ministerial on s.pem_Id_Secretario equals pem.pem_Id_Ministro
                             where s.sec_Id_Sector == idSector
                             select pem).ToList();

                    return Ok(new
                {
                    status = "success",
                    infoSecretario = query
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

        // GET api/GetSecretarioBySector/idSector
        [Route("[action]/{idSector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetTesoreroBySector(int idSector)
        {
            try
            {
                var query = (from s in context.Sector
                             join pem in context.Personal_Ministerial on s.pem_Id_Tesorero equals pem.pem_Id_Ministro
                             where s.sec_Id_Sector == idSector
                             select pem).ToList();
                return Ok(new
                {
                    status = "success",
                    infoTesorero = query
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

        // GET api/GetSecretarioByDistrito/idDistrito
        [Route("[action]/{idDistrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetSecretarioByDistrito(int idDistrito)
        {
            try
            {
                var query = (from d in context.Distrito
                             join pem in context.Personal_Ministerial on d.pem_Id_Secretario equals pem.pem_Id_Ministro
                             where d.dis_Id_Distrito == idDistrito
                             select pem).ToList();
                return Ok(new
                {
                    status = "success",
                    infoSecretario = query
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


        // GET api/GetTesoreroByDistrito/idDistrito
        [Route("[action]/{idDistrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetTesoreroByDistrito(int idDistrito)
        {
            try
            {
                var query = (from d in context.Distrito
                             join pem in context.Personal_Ministerial on d.pem_Id_Tesorero equals pem.pem_Id_Ministro
                             where d.dis_Id_Distrito == idDistrito
                             select pem).ToList();
                return Ok(new
                {
                    status = "success",
                    infoTesorero = query
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

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER INFO DEL MINISTRO POR SU ID
        [HttpGet("{pem_Id_Ministro}")]
        [EnableCors("AllowOrigin")]
        public Personal_Ministerial Get(int pem_Id_Ministro)
        {
            var ministro = context.Personal_Ministerial.FirstOrDefault(pem => pem.pem_Id_Ministro == pem_Id_Ministro);
            return ministro;
        }

        // GET: api/Personal_Ministerial
        [Route("[action]/{idMinistro}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetDistritosByMinistro(int idMinistro)
        {
            try
            {
                List<Distrito> distritos = new List<Distrito>();
                var d1 = (from d in context.Distrito
                             where d.pem_Id_Obispo == idMinistro && d.dis_Activo==true
                             select d).ToList();

                var d2 = (from s in context.Sector
                          join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                          where s.pem_Id_Pastor == idMinistro
                          select d).ToList();

                distritos.AddRange(d1);
                distritos.AddRange(d2);

                return Ok(new
                {
                    status = "success",
                    distritos = distritos.Distinct()
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

        // GET: /api/Sector/GetObispoByDistrito/idDistrito
        [Route("[action]/{idDistrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetObispoByDistrito(int idDistrito)
        {
            try
            {
                var query = (from d in context.Distrito
                             join pem in context.Personal_Ministerial
                             on d.pem_Id_Obispo equals pem.pem_Id_Ministro
                             where d.dis_Id_Distrito == idDistrito
                             select pem).ToList();
                return Ok(
                    new
                    {
                        status = "success",
                        ministros = query
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

        public static string RemoveAccents(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        // GET: api/Personal_Ministerial
        // METODO PARA OBTENER INFO DEL SECTOR SEGUN ID DEL MINISTRO

        // GET: api/Personal_Ministerial/GetPersonaCuyoIdPersonaNoEstaEnPersonalMinisterialBySector
        // METODO PARA OBTENER LISTA DE PERSONAS VARONES DEL SECTOR QUE AUN NO ESTAN REGISTRADOS COMO PERSONAL MINISTERIAL
        [Route("[action]/{sec_Id_Congregacion}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonaCuyoIdPersonaNoEstaEnPersonalMinisterialBySector(int sec_Id_Congregacion)
        {
            try
            {
                //Obtiene a todos los varones del Sector
                var PersonaCuyoIdPersonaNoEstaEnPersonaMinisterial = new List<object>();
                var dto = context.Sector.Where(p => p.sec_Id_Sector == sec_Id_Congregacion).Select(p => p.dis_Id_Distrito).FirstOrDefault();

                    var query =(from per in context.Persona
                                  where per.per_Activo == true && (per.per_Categoria == "ADULTO_HOMBRE" || per.per_Categoria == "JOVEN_HOMBRE")
                                  && per.sec_Id_Sector == sec_Id_Congregacion
                                  orderby per.per_Nombre
                                 select per).ToList();


                foreach (var person in query)
                {
                    var query3 = (from pem in context.Personal_Ministerial
                                  where pem.per_Id_Miembro == person.per_Id_Persona
                                  select new
                                  {
                                      pem.per_Id_Miembro,
                                      person.per_Id_Persona,
                                      person.per_Nombre,
                                      person.per_Apellido_Paterno,
                                      person.per_Apellido_Materno,
                                      person.per_Fecha_Nacimiento,

                                  }
               ).FirstOrDefault();

                    var personaEnLista = query3;
                    if (personaEnLista==null)
                    {
                       PersonaCuyoIdPersonaNoEstaEnPersonaMinisterial.Add(person);
                    }
                    
                }

                return Ok(
                new
                {
                    status = true,
                    personas= PersonaCuyoIdPersonaNoEstaEnPersonaMinisterial
                });

            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }


        // GET: api/Personal_Ministerial/GetPersonalMinisterialSinIdMiembroByDistrito
        // METODO PARA OBTENER LISTA DE PERSONAL MINISTERIAL DEL DISTRITO QUE NO TENGA REGISTRADO Id_Miembro
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalMinisterialSinIdMiembroByDistrito(int dis_Id_Distrito)
        {
            try
            {
                
                var query = (from pem in context.Personal_Ministerial
                             join s in context.Sector on pem.sec_Id_Congregacion equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where d.dis_Id_Distrito == dis_Id_Distrito
                             && pem.pem_Activo == true && (pem.per_Id_Miembro == null || pem.per_Id_Miembro == 0)
                             orderby pem.pem_Nombre ascending
                             select new
                             {
                                 pem.pem_Id_Ministro,
                                 pem.pem_Activo,
                                 pem.per_Id_Miembro,
                                 pem.pem_Nombre,
                                 pem.sec_Id_Congregacion,
                                 pem.pem_En_Permiso,
                                 pem.pem_emailIECE,
                                 pem.pem_email_Personal,
                                 pem.pem_Grado_Ministerial,
                                 pem.pem_Fecha_Nacimiento
                             }).ToList();

                            return Ok(
                        new
                        {
                            status = true,
                            personalSinVincularConPersona = query,
                        }
                      );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }

        // POST: api/PersonalMinisterial/AddPersonalMinisterial
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonalMinisterial([FromBody] PersonaAVincular vnm)
        {
            try
            {
                // DEFINE OBJETO PERSONA
                Persona persona = new Persona();
                persona = (from per in context.Persona
                           where per.per_Id_Persona == vnm.id_Persona
                           select per).FirstOrDefault();

                Personal_Ministerial elementoMinisterial =new Personal_Ministerial();
                Registro_Transacciones RegistroHistorico = new Registro_Transacciones();


                // VINCULA UNA PERSONA 
                if (vnm.id_Ministro != 0) // Si no es Zero, significa que ya existe el Ministro, solo va a vincular con Id_Miembro. 
                {
                    //Obtiene el Elemento del Personal Ministerial que desea Editar
                    elementoMinisterial = (from min in context.Personal_Ministerial
                                           where min.pem_Id_Ministro == vnm.id_Ministro
                                           select min).FirstOrDefault();

                    //Modifica el campo per_Id_miembro y otros campos que pueden ser dinámicos y los graba.
                    elementoMinisterial.per_Id_Miembro = vnm.id_Persona;
                    elementoMinisterial.pem_Fecha_Nacimiento = persona.per_Fecha_Nacimiento;
                    elementoMinisterial.sec_Id_Congregacion = persona.sec_Id_Sector;
                    elementoMinisterial.pem_Cel1 = persona.per_Telefono_Movil;
                    elementoMinisterial.pem_email_Personal = persona.per_Email_Personal;
                    elementoMinisterial.usu_Id_Usuario = vnm.usu_Id_Usuario;

                    context.SaveChanges();
                }
                else //si es Zero, se trata de Una Alta de Nuevo Personal Ministerial
                {
                    elementoMinisterial.pem_Activo = true;
                    elementoMinisterial.per_Id_Miembro = vnm.id_Persona;
                    elementoMinisterial.pem_Nombre = vnm.nombre_Persona;
                    var nombreCompleto = ManejoDeApostrofes.QuitarApostrofe2(vnm.nombre_Persona);
                    elementoMinisterial.pem_Nombre_Sin_Acentos = nombreCompleto;
                    elementoMinisterial.pem_Fecha_Nacimiento = persona.per_Fecha_Nacimiento;
                    elementoMinisterial.sec_Id_Congregacion = persona.sec_Id_Sector;
                    elementoMinisterial.pem_Grado_Ministerial = "AUXILIAR";
                    elementoMinisterial.pem_Dedicado = false;
                    elementoMinisterial.pem_En_Permiso = false;
                    elementoMinisterial.pem_Tipo_Jubilacion = "";
                    elementoMinisterial.pem_Registrado_Gobernacion = false;
                    elementoMinisterial.pem_Necesita_Credencial = false;
                    elementoMinisterial.pem_Cel1 = persona.per_Telefono_Movil;
                    elementoMinisterial.pem_email_Personal = persona.per_Email_Personal;
                    elementoMinisterial.Fecha_Registro = fechayhora;
                    elementoMinisterial.usu_Id_Usuario = vnm.usu_Id_Usuario;

                    context.Personal_Ministerial.Add(elementoMinisterial);
                    context.SaveChanges();

                    //Se crea el Registro Histórico de la Alta de Auxiliar .
                    Registro_TransaccionesController rt = new Registro_TransaccionesController(context);
                    rt.RegistroHistorico(
                     elementoMinisterial.pem_Id_Ministro,
                     elementoMinisterial.sec_Id_Congregacion,
                     "ALTA DE NUEVO PERSONAL MINISTERIAL",
                     null,
                     "AUXILIAR",
                     vnm.fechaTransaccion,
                     vnm.usu_Id_Usuario,
                     vnm.usu_Id_Usuario
                    );

                    //Se envía email al Obispo y a soporte de la Alta de Auxiliar que está gestionando el Pastor.
                    SendMailController smc = new SendMailController(context);
                    smc.AltaDeAuxiliar(elementoMinisterial.pem_Id_Ministro, vnm.usu_Id_Usuario);
                }                             

                return Ok
                (
                    new
                    {
                        status = "success"
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok
                (
                    new
                    {
                        status = "error",
                        message = ex.Message
                    }
                );
            }
        }

        // GET: api/Personal_Ministerial
        // Personal Ministerial por Sector, vinculado con un Id_Miembro
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalMinisterialByDistrito(int dis_Id_Distrito)
        {
            try
            {
                //Traerá sólo al Personal Ministerial que tenga vinculado un ID de Persona.
                var PersonalMinisterial = (from PM in context.Personal_Ministerial
                                           join P in context.Persona on PM.per_Id_Miembro equals P.per_Id_Persona
                                           join S in context.Sector on PM.sec_Id_Congregacion equals S.sec_Id_Sector
                                           join D in context.Distrito on S.dis_Id_Distrito equals D.dis_Id_Distrito
                                           where D.dis_Id_Distrito == dis_Id_Distrito && PM.pem_Activo == true
                                           orderby S.sec_Numero, PM.pem_Nombre
                                           select new 
                                           { 
                                           pem_Id_Ministro = PM.pem_Id_Ministro,
                                           pem_Nombre = PM.pem_Nombre, 
                                           pem_Grado_Ministerial = PM.pem_Grado_Ministerial,
                                           per_Id_Persona = P.per_Id_Persona,
                                           per_Activo=   P.per_Activo,
                                           per_En_Comunion = P.per_En_Comunion,
                                           per_Vivo= P.per_Vivo,
                                           sec_Id_Sector = P.sec_Id_Sector,
                                           sec_Tipo_Sector = S.sec_Tipo_Sector,
                                           sec_Numero =S.sec_Numero,
                                           sec_Alias = S.sec_Alias,
                                           per_Nombre = P.per_Nombre,
                                           per_Apellido_Paterno = P.per_Apellido_Paterno,
                                           per_Apellido_Materno = P.per_Apellido_Materno,
                                           per_Apellido_Casada=P.per_Apellido_Casada,
                                           apellidoPrincipal = (P.per_Apellido_Casada == "" || P.per_Apellido_Casada == null) ? P.per_Apellido_Paterno : (P.per_Apellido_Casada + "* " + P.per_Apellido_Paterno),
                                           per_Bautizado = P.per_Bautizado,
                                           pem_Foto_Ministro = (PM.pem_Foto_Ministro != null) ? PM.pem_Foto_Ministro.Replace("\\\\192.168.0.11", "c:\\DoctosCompartidos") : "c:\\DoctosCompartidos\\FotosMinisterial\\SinFoto.jpg",
                                           }).ToList();

                var personasConImagenes = new List<personalMinisterialConFotoDto>();

                foreach (var persona in PersonalMinisterial)
                {
                    var imagenPath = persona.pem_Foto_Ministro; // Ruta de la imagen en el servidor
                    var mimeType = "image/jpg"; // Establece el tipo MIME adecuado

                    if (System.IO.File.Exists(imagenPath))
                    {
                        // Lee los datos binarios de la imagen
                        var imagenBytes = System.IO.File.ReadAllBytes(imagenPath);

                        personasConImagenes.Add(new personalMinisterialConFotoDto
                        {
                            pem_Id_Ministro = persona.pem_Id_Ministro,
                            pem_Nombre = persona.pem_Nombre,
                            pem_Grado_Ministerial = persona.pem_Grado_Ministerial,
                            per_Id_Persona = persona.per_Id_Persona,
                            per_Activo = persona.per_Activo,
                            per_En_Comunion = persona.per_En_Comunion,
                            per_Vivo = persona.per_Vivo,
                            sec_Id_Sector = persona.sec_Id_Sector,
                            sector= persona.sec_Tipo_Sector + " " + persona.sec_Numero,
                            per_Nombre = persona.per_Nombre,
                            per_Apellido_Paterno = persona.per_Apellido_Paterno,
                            per_Apellido_Materno = persona.per_Apellido_Materno,
                            per_Apellido_Casada = persona.per_Apellido_Casada,
                            apellidoPrincipal = persona.apellidoPrincipal,
                            per_Bautizado = persona.per_Bautizado,
                            pem_Foto_Ministro = persona.pem_Foto_Ministro,
                            imagen = imagenBytes,
                            MIMEType = mimeType
                        });
                    }
                }




                return Ok(new
                {
                    status = "success",
                    administrativo = personasConImagenes
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



        // GET: api/Personal_Ministerial
        // Personal Ministerial por Sector, vinculado con un Id_Miembro
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalMinisterialBySector(int sec_Id_Sector)
                        
        {
            try
            {
                var PersonalMinisterial = (from PM in context.Personal_Ministerial
                                           join P in context.Persona on PM.per_Id_Miembro equals P.per_Id_Persona
                                           join S in context.Sector on PM.sec_Id_Congregacion equals S.sec_Id_Sector
                                           join D in context.Distrito on S.dis_Id_Distrito equals D.dis_Id_Distrito
                                           where P.sec_Id_Sector == sec_Id_Sector && PM.pem_Activo == true
                                           orderby S.sec_Numero, PM.pem_Grado_Ministerial,PM.pem_Nombre
                                           select new
                                           {
                                              pem_Id_Ministro = PM.pem_Id_Ministro,
                                              pem_Nombre = PM.pem_Nombre,
                                              pem_Grado_Ministerial= PM.pem_Grado_Ministerial,
                                              pem_Foto_Ministro = (PM.pem_Foto_Ministro != null) ? PM.pem_Foto_Ministro.Replace("\\\\192.168.0.11", "c:\\DoctosCompartidos"): "c:\\DoctosCompartidos\\FotosMinisterial\\SinFoto.jpg",
                                           }).ToList();

                var personasConImagenes = new List<personalMinisterialConFoto>();

                foreach (var persona in PersonalMinisterial)
                {
                    var imagenPath = persona.pem_Foto_Ministro; // Ruta de la imagen en el servidor
                    var mimeType = "image/jpg"; // Establece el tipo MIME adecuado

                    if (System.IO.File.Exists(imagenPath))
                    {
                        // Lee los datos binarios de la imagen
                        var imagenBytes = System.IO.File.ReadAllBytes(imagenPath);

                        personasConImagenes.Add(new personalMinisterialConFoto
                        {
                            pem_Id_Ministro = persona.pem_Id_Ministro,
                            pem_Nombre = persona.pem_Nombre,
                            pem_Grado_Ministerial = persona.pem_Grado_Ministerial,
                            Imagen = imagenBytes,
                            MIMEType = mimeType
                        });
                    }
                }


                return Ok(new
                {
                    status = "success",
                    administrativo = personasConImagenes
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

        // GET: api/Personal_Ministerial
        // Personal Ministerial por Sector, vinculado con un Id_Miembro
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalMinisterialBySector2(int sec_Id_Sector)

        {

            try
            {
                var PersonalMinisterial = (from PM in context.Personal_Ministerial
                                           join P in context.Persona on PM.per_Id_Miembro equals P.per_Id_Persona
                                           join S in context.Sector on P.sec_Id_Sector equals S.sec_Id_Sector
                                           join D in context.Distrito on S.dis_Id_Distrito equals D.dis_Id_Distrito
                                           where P.sec_Id_Sector == sec_Id_Sector && PM.pem_Activo == true
                                           select new
                                           {
                                               pem_Id_Ministro = PM.pem_Id_Ministro,
                                               pem_Nombre = PM.pem_Nombre,
                                               pem_Grado_Ministerial = PM.pem_Grado_Ministerial,
                                               pem_Foto_Ministro = (PM.pem_Foto_Ministro != null) ? PM.pem_Foto_Ministro.Replace("\\\\192.168.0.11", "c:\\DoctosCompartidos") : "c:\\DoctosCompartidos\\FotosMinisterial\\SinFoto.jpg",
                                           }).ToList();


                return Ok(new
                {
                    status = "success",
                    administrativo = PersonalMinisterial
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

        // GET: api/Personal_Ministerial
        // AUXILIARES POR SECTOR
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetAuxiliaresBySector2(int sec_Id_Sector)
        {
            try
            {
                var auxiliares = (from a in context.Personal_Ministerial
                                  where a.sec_Id_Congregacion == sec_Id_Sector
                                  && a.pem_Activo && (a.pem_Grado_Ministerial == "AUXILIAR" || a.pem_Grado_Ministerial == "DIÁCONO A PRUEBA")
                                  select a).ToList();
                return Ok(new
                {
                    status = "success",
                    auxiliares = auxiliares
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

        

        // GET: api/Personal_Ministerial/GetAuxiliaresBySector
        // METODO PARA OBTENER LISTA DE AUXILIARES DE UN SECTOR
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetAuxiliaresBySector(int sec_Id_Sector)
        {
            try
            {

                var query = (from pem in context.Personal_Ministerial
                             join s in context.Sector on pem.sec_Id_Congregacion equals s.sec_Id_Sector
                             where s.sec_Id_Sector == sec_Id_Sector
                             && pem.pem_Activo == true 
                             && pem.pem_Grado_Ministerial == "AUXILIAR"

                             orderby pem.pem_Nombre ascending
                             select new
                             {
                                 pem.pem_Id_Ministro,
                                 pem.pem_Activo,
                                 pem.per_Id_Miembro,
                                 pem.pem_Nombre,
                                 pem.sec_Id_Congregacion,
                                 pem.pem_En_Permiso,
                                 pem.pem_emailIECE,
                                 pem.pem_email_Personal,
                                 pem.pem_Grado_Ministerial,
                                 pem.pem_Fecha_Nacimiento
                             }).ToList();

                return Ok(
            new
            {
                status = true,
                personalMinisterial = query,
            }
          );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }


        // GET: api/PersonalMinisterial/GetPersonalAdministrativoBySector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalAdministrativoSecundarioBySector(int sec_Id_Sector)
        {
            try
            {
                List<PersonalAdministrativo> administrativoSecundario = new List<PersonalAdministrativo>();


                PersonalAdministrativo pastor = new PersonalAdministrativo();
                Personal_Ministerial pastor1 = (from pm in context.Personal_Ministerial
                                                    join s in context.Sector on pm.pem_Id_Ministro equals s.pem_Id_Pastor
                                                    where s.sec_Id_Sector == sec_Id_Sector
                                                    && pm.pem_Activo
                                                    select pm).FirstOrDefault();

                pastor.cargo = "PASTOR";
                pastor.datosPersonalMinisterial = pastor1;
                administrativoSecundario.Add(pastor);


                PersonalAdministrativo secretario = new PersonalAdministrativo();
                Personal_Ministerial secretario1 = (from pm in context.Personal_Ministerial
                                                    join s in context.Sector on pm.pem_Id_Ministro equals s.pem_Id_Secretario
                                                    where s.sec_Id_Sector == sec_Id_Sector
                                                    && pm.pem_Activo
                                                    select pm).FirstOrDefault();

               

                secretario.cargo = "SECRETARIO";
                secretario.datosPersonalMinisterial = secretario1;
                administrativoSecundario.Add(secretario);

                PersonalAdministrativo subsecretario = new PersonalAdministrativo();
                Personal_Ministerial subSecretario1 = (from pm in context.Personal_Ministerial
                                                   join s in context.Sector on pm.pem_Id_Ministro equals s.pem_Id_SubSecretario
                                                   where s.sec_Id_Sector == sec_Id_Sector
                                                   && pm.pem_Activo
                                                   select pm).FirstOrDefault();
                subsecretario.cargo = "SUBSECRETARIO";
                subsecretario.datosPersonalMinisterial = subSecretario1;
                administrativoSecundario.Add(subsecretario);

                PersonalAdministrativo tesorero = new PersonalAdministrativo ();
                Personal_Ministerial tesorero1 = (from pm in context.Personal_Ministerial
                                                      join s in context.Sector on pm.pem_Id_Ministro equals s.pem_Id_Tesorero
                                                      where s.sec_Id_Sector == sec_Id_Sector
                                                      && pm.pem_Activo
                                                      select pm).FirstOrDefault();
                tesorero.cargo = "TESORERO";
                tesorero.datosPersonalMinisterial = tesorero1;
                administrativoSecundario.Add(tesorero);

                PersonalAdministrativo subtesorero = new PersonalAdministrativo();
                Personal_Ministerial subTesorero1 = (from pm in context.Personal_Ministerial
                                                      join s in context.Sector on pm.pem_Id_Ministro equals s.pem_Id_SubTesorero
                                                      where s.sec_Id_Sector == sec_Id_Sector
                                                      && pm.pem_Activo
                                                      select pm).FirstOrDefault();
                subtesorero.cargo = "SUBTESORERO";
                subtesorero.datosPersonalMinisterial = subTesorero1;
                administrativoSecundario.Add(subtesorero);


                return Ok(new
                {
                    status = "success",
                    administrativo = administrativoSecundario
                }); ;
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


        // GET: api/PersonalMinisterial/GetPersonalAdministrativoSecundarioByDistrito
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonalAdministrativoSecundarioByDistrito(int dis_Id_Distrito)
        {
            try
            {
                List<PersonalAdministrativo> administrativoSecundario = new List<PersonalAdministrativo>();


                PersonalAdministrativo obispo =  new PersonalAdministrativo();
                Personal_Ministerial obispo1 = (from pm in context.Personal_Ministerial
                                                join s in context.Sector on pm.sec_Id_Congregacion equals s.sec_Id_Sector
                                                join d in context.Distrito on pm.pem_Id_Ministro equals d.pem_Id_Obispo
                                                where d.dis_Id_Distrito == dis_Id_Distrito
                                                && pm.pem_Activo
                                                select pm).FirstOrDefault();

                obispo.cargo = "OBISPO";
                obispo.datosPersonalMinisterial = obispo1;
                administrativoSecundario.Add(obispo);


                PersonalAdministrativo obispoSuplente = new PersonalAdministrativo();
                Personal_Ministerial obispoSuplente1 = (from pm in context.Personal_Ministerial
                                                join s in context.Sector on pm.sec_Id_Congregacion equals s.sec_Id_Sector
                                                join d in context.Distrito on pm.pem_Id_Ministro equals d.pem_Id_Obispo_Suplente
                                                where d.dis_Id_Distrito == dis_Id_Distrito
                                                && pm.pem_Activo
                                                select pm).FirstOrDefault();

                obispoSuplente.cargo = "OBISPO SUPLENTE";
                obispoSuplente.datosPersonalMinisterial = obispoSuplente1;
                administrativoSecundario.Add(obispoSuplente);

                PersonalAdministrativo secreatario = new PersonalAdministrativo();
                Personal_Ministerial secreatario1 = (from pm in context.Personal_Ministerial
                                                        join s in context.Sector on pm.sec_Id_Congregacion equals s.sec_Id_Sector
                                                        join d in context.Distrito on pm.pem_Id_Ministro equals d.pem_Id_Secretario
                                                        where d.dis_Id_Distrito == dis_Id_Distrito
                                                        && pm.pem_Activo
                                                        select pm).FirstOrDefault();

                secreatario.cargo = "SECRETARIO";
                secreatario.datosPersonalMinisterial = secreatario1;
                administrativoSecundario.Add(secreatario);


                PersonalAdministrativo subsecreatario = new PersonalAdministrativo();
                Personal_Ministerial subsecreatario1 = (from pm in context.Personal_Ministerial
                                                     join s in context.Sector on pm.sec_Id_Congregacion equals s.sec_Id_Sector
                                                     join d in context.Distrito on pm.pem_Id_Ministro equals d.pem_Id_Sub_Secretario
                                                     where d.dis_Id_Distrito == dis_Id_Distrito
                                                     && pm.pem_Activo
                                                     select pm).FirstOrDefault();

                subsecreatario.cargo = "SUBSECRETARIO";
                subsecreatario.datosPersonalMinisterial = subsecreatario1;
                administrativoSecundario.Add(subsecreatario);


                PersonalAdministrativo tesorero = new PersonalAdministrativo();
                Personal_Ministerial tesorero1 = (from pm in context.Personal_Ministerial
                                                        join s in context.Sector on pm.sec_Id_Congregacion equals s.sec_Id_Sector
                                                        join d in context.Distrito on pm.pem_Id_Ministro equals d.pem_Id_Tesorero
                                                        where d.dis_Id_Distrito == dis_Id_Distrito
                                                        && pm.pem_Activo
                                                        select pm).FirstOrDefault();

                tesorero.cargo = "TESORERO";
                tesorero.datosPersonalMinisterial = tesorero1;
                administrativoSecundario.Add(tesorero);

                PersonalAdministrativo subtesorero = new PersonalAdministrativo();
                Personal_Ministerial subtesorero1 = (from pm in context.Personal_Ministerial
                                                  join s in context.Sector on pm.sec_Id_Congregacion equals s.sec_Id_Sector
                                                  join d in context.Distrito on pm.pem_Id_Ministro equals d.pem_Id_Sub_Tesorero
                                                  where d.dis_Id_Distrito == dis_Id_Distrito
                                                  && pm.pem_Activo
                                                  select pm).FirstOrDefault();

                subtesorero.cargo = "SUBTESORERO";
                subtesorero.datosPersonalMinisterial = subtesorero1;
                administrativoSecundario.Add(subtesorero);


                return Ok(new
                {
                    status = "success",
                    administrativo = administrativoSecundario
                }); ;
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


        // POST: api/Personal_Ministerial
        // ALTA DE AUXILIAR EN EL SECTOR
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AltaAuxiliarEnSector([FromBody] infoNvoAuxiliar info)
        {
            try
            {
                var per = context.Persona.FirstOrDefault(p => p.per_Id_Persona == info.per_Id_Persona);
                Personal_Ministerial pem = new Personal_Ministerial
                {
                    per_Id_Miembro = per.per_Id_Persona,
                    pem_Nombre = per.per_Nombre + " " + per.per_Apellido_Paterno + (per.per_Apellido_Materno != "" ? " " + per.per_Apellido_Materno : ""),
                    sec_Id_Congregacion = per.sec_Id_Sector,
                    pem_Activo = true,
                    pem_Fecha_Nacimiento = per.per_Fecha_Nacimiento,
                    pem_Grado_Ministerial = "AUXILIAR",
                    pem_Cel1 = per.per_Telefono_Movil != null ? per.per_Telefono_Movil : null,
                    pem_email_Personal = per.per_Email_Personal != null ? per.per_Email_Personal : null,
                };

                context.Personal_Ministerial.Add(pem);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    nuevoAuxiliar = pem
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

        // POST: api/Personal_Ministerial
        // ESTABLECE SECRETARIO DEL SECTOR
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult SetSecretarioDelSector([FromBody] infoNvoSecretario info)
        {
            try
            {
                var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == info.sec_Id_Sector);
                sector.pem_Id_Secretario = info.pem_Id_Ministro;
                context.Sector.Update(sector);
                context.SaveChanges();

                SendMailController smc = new SendMailController(context);
                smc.CambioDeSecretario(info.pem_Id_Ministro, info.idUsuario);

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

        // POST: api/Personal_Ministerial
        // ESTABLECE TESORERO DEL SECTOR
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult SetTesoreroDelSector([FromBody] infoNvoSecretario info)
        {
            try
            {
                var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == info.sec_Id_Sector);
                sector.pem_Id_Tesorero = info.pem_Id_Ministro;
                context.Sector.Update(sector);
                context.SaveChanges();

                SendMailController smc = new SendMailController(context);
                smc.CambioDeTesorero(info.pem_Id_Ministro, info.idUsuario);

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

        // POST api/SetPersonalAdministrativoBySector
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult SetPersonalAdministrativoBySector(infoNuevaAsignacion info)
        {
            try
            {
                var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == info.sec_Id_Sector);
                switch (info.puesto)
                {
                    case "SECRETARIO":
                        sector.pem_Id_Secretario = info.pem_Id_Ministro;
                        break;
                    case "SUBSECRETARIO":
                        sector.pem_Id_SubSecretario = info.pem_Id_Ministro;
                        break;
                    case "TESORERO":
                        sector.pem_Id_Tesorero = info.pem_Id_Ministro;
                        break;
                    case "SUBTESORERO":
                        sector.pem_Id_SubTesorero = info.pem_Id_Ministro;
                        break;
                }
                context.Sector.Update(sector);
                context.SaveChanges();

                SendMailController smc = new SendMailController(context);
                smc.CambioDePersonalAdministrativo(info.pem_Id_Ministro, info.puesto, info.idUsuario);

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

        // POST api/SetPersonalAdministrativoByDistrito
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult SetPersonalAdministrativoByDistrito(infoNuevaAsignacion info)
        {
            try
            {
                var distrito = context.Distrito.FirstOrDefault(d => d.dis_Id_Distrito == info.dis_Id_Distrito);
                switch (info.puesto)
                {
                    case "SECRETARIO":
                        distrito.pem_Id_Secretario = info.pem_Id_Ministro;
                        break;
                    case "SUBSECRETARIO":
                        distrito.pem_Id_Sub_Secretario = info.pem_Id_Ministro;
                        break;
                    case "TESORERO":
                        distrito.pem_Id_Tesorero = info.pem_Id_Ministro;
                        break;
                    case "SUBTESORERO":
                        distrito.pem_Id_Sub_Tesorero = info.pem_Id_Ministro;
                        break;
                }
                context.Distrito.Update(distrito);
                context.SaveChanges();

                //SendMailController smc = new SendMailController(context);
                //smc.CambioDePersonalAdministrativo(info.pem_Id_Ministro, info.puesto, info.idUsuario);

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

        //POST: api/Personal_Ministerial/removerAsignacionDeAdministracion/{idSector}/{puesto}
        [Route("[action]/{idSector}/{puesto}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult RemoverAsignacionDeAdministracion(int idSector, string puesto)
        {
            try
            {
                var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == idSector);
                switch (puesto)
                {
                    case "SECRETARIO":
                        sector.pem_Id_Secretario = null;
                        break;
                    case "SUBSECRETARIO":
                        sector.pem_Id_SubSecretario = null;
                        break;
                    case "TESORERO":
                        sector.pem_Id_Tesorero = null;
                        break;
                    case "SUBTESORERO":
                        sector.pem_Id_SubTesorero = null;
                        break;
                }

                context.Sector.Update(sector);
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


        //POST: api/Personal_Ministerial/RemoverAsignacionDeAdministracionDistrital/{idDistrito}/{puesto}
        [Route("[action]/{idDistrito}/{puesto}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult RemoverAsignacionDeAdministracionDistrital(int idDistrito, string puesto)
        {
            try
            {
                var distrito = context.Distrito.FirstOrDefault(d => d.dis_Id_Distrito == idDistrito);
                switch (puesto)
                {
                    case "SECRETARIO":
                        distrito.pem_Id_Secretario = null;
                        break;
                    case "SUBSECRETARIO":
                        distrito.pem_Id_Sub_Secretario = null;
                        break;
                    case "TESORERO":
                        distrito.pem_Id_Tesorero = null;
                        break;
                    case "SUBTESORERO":
                        distrito.pem_Id_Sub_Tesorero = null;
                        break;
                }

                context.Distrito.Update(distrito);
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

        // POST: api/Personal_Ministerial
        // BAJA DE AUXILIAR EN EL SECTOR
        [HttpPost]
        [EnableCors("AllowOrigin")]
        [Route("[action]/{pem_Id_Ministro}")]
        public IActionResult BajaAuxiliarEnSector(int pem_Id_Ministro)
        {
            try
            {
                var pem = context.Personal_Ministerial.FirstOrDefault(p => p.pem_Id_Ministro == pem_Id_Ministro);
                if (pem.per_Id_Miembro == null || pem.per_Id_Miembro == 0)
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No se puede hacer baja de auxiliar que NO TIENE UN REGISTRO como miembro de la iglesia."
                    });
                }
                else
                {
                    context.Entry(pem).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    context.SaveChanges();

                    return Ok(new
                    {
                        status = "success"
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


        // POST: api/PersonalMinisterial/BajaDeAuxiliar
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaDeAuxiliar([FromBody] AuxiliarBaja ab)
        {
            try
            {
                Personal_Ministerial elementoMinisterial = new Personal_Ministerial();
                Registro_Transacciones RegistroHistorico = new Registro_Transacciones();


                // VINCULA UNA PERSONA 
                if (ab.id_Ministro != 0) // Se asegura que viene un ministro registrado con un Numero de id_Ministro. 
                {
                    //Obtiene el Elemento del Personal Ministerial que desea dar de Baja
                    elementoMinisterial = (from min in context.Personal_Ministerial
                                           where min.pem_Id_Ministro == ab.id_Ministro
                                           select min).FirstOrDefault();

                    if (ab.causaDeBaja != "CAMBIO DE DOMICILIO") //Si la Baja NO es por cambio de Domicilio, se dara de Baja de la Tabla Personal Ministerial.
                    {
                        //Inactiva al Auxiliar y lo graba.
                        elementoMinisterial.pem_Activo = false;
                        context.SaveChanges();

                        //Registra la Transacción Ministerial
                        Registro_TransaccionesController rt = new Registro_TransaccionesController(context);
                        rt.RegistroHistorico(
                         elementoMinisterial.pem_Id_Ministro,
                         elementoMinisterial.sec_Id_Congregacion,
                         "BAJA DE PERSONAL MINISTERIAL",
                         ab.causaDeBaja,
                         "",
                         ab.fechaTransaccion,
                         ab.usu_Id_Usuario,
                         ab.usu_Id_Usuario
                        );

                        //Se envía email al Obispo y a soporte de la Baja de Auxiliar que está gestionando el Pastor.
                        SendMailController smc = new SendMailController(context);
                        smc.BajaDeAuxiliar(elementoMinisterial.pem_Id_Ministro, ab.usu_Id_Usuario);
                    } else 
                    //Si la Baja es por cambio de Domicilio, sólo cambiará el campo "Id_Sector" a Null. Esperando a que otro Sector lo Vincule.
                    {
                        //Inactiva al Auxiliar y lo graba.
                        //elementoMinisterial.sec_Id_Congregacion = 0; Habíamos pensado en dejar la congregación en Cero, pero crea efectos en algunos reportes.
                        context.SaveChanges();
                    }
                    
                }

                return Ok
                (
                    new
                    {
                        status = "success"
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok
                (
                    new
                    {
                        status = "error",
                        message = ex.Message
                    }
                );
            }
        }

        // POST: api/Personal_Ministerial
        // ESTABLECE SECRETARIO DEL SECTOR
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult SetSecretarioDelDistrito([FromBody] infoNvoSecretarioDto info)
        {
            try
            {
                var distrito = context.Distrito.FirstOrDefault(d => d.dis_Id_Distrito == info.dis_Id_Distrito);
                distrito.pem_Id_Secretario = info.pem_Id_Ministro;
                context.Distrito.Update(distrito);
                context.SaveChanges();

                //Nota: No se grabará el registro historico porque desde Secretaría General lo harán cuando llegue el acta del Cambio de Administración por reglamento.
                /*{Registro_TransaccionesController registroTransacciones = new Registro_TransaccionesController(context);
                registroTransacciones.RegistroHistorico(info.pem_Id_Ministro, 0, "DESIGNACIÓN DE CARGO ADMINISTRATIVO", "SECRETARIO", info.comentario, info.fecha, 0, info.idUsuario);*/


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

        // POST: api/Personal_Ministerial
        // ESTABLECE TESORERO DEL SECTOR
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult SetTesoreroDelDistrito([FromBody] infoNvoSecretarioDto info)
        {
            try
            {
                var distrito = context.Distrito.FirstOrDefault(d => d.dis_Id_Distrito == info.dis_Id_Distrito);
                distrito.pem_Id_Tesorero = info.pem_Id_Ministro;
                context.Distrito.Update(distrito);
                context.SaveChanges();

                //Falta crear el registro de Transacción de designación de Tesoero de Distrito

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

        


        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult GenerarNombreCompleto()
        {
            try
            {
                // Obtiene listado de personas
                var personas = (from p in context.Personal_Ministerial select p).ToList();

                // A cada persona le los acentos en nombre y apellidos y
                // guarda el nombre completo en el campo per_Nombre_Completo
                foreach (var p in personas)
                {
                    var nombre = ManejoDeApostrofes.QuitarApostrofe2(p.pem_Nombre);
                    
                    // Genera nombre completo
                    p.pem_Nombre_Sin_Acentos = $"{nombre}";

                    // Guarda cambios
                    context.Personal_Ministerial.Update(p);
                    context.SaveChanges();
                }

                return Ok(new
                {
                    status = "success",
                    personas
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