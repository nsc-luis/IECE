﻿using System;
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


namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonalMinisterialController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonalMinisterialController(AppDbContext context)
        {
            this.context = context;
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
                             where d.pem_Id_Obispo == idMinistro
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

                    Registro_TransaccionesController rt = new Registro_TransaccionesController(context);
                    rt.RegistroHistorico(
                     elementoMinisterial.pem_Id_Ministro,
                     elementoMinisterial.sec_Id_Congregacion,
                     "ALTA DE NUEVO PERSONAL MINISTERIAL",
                     null,
                     "ALTA AUXILIAR",
                     vnm.fechaTransaccion,
                     vnm.usu_Id_Usuario,
                     vnm.usu_Id_Usuario
                    );
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

    }
}