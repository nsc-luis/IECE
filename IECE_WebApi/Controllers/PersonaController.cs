using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
using IECE_WebApi.Models;
using IECE_WebApi.Repositorios;
using ImageMagick;
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
    public class PersonaController : ControllerBase
    {
        private readonly AppDbContext context;

        public PersonaController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        private class PersonaDomicilioMiembros
        {
            public object persona { get; set; }
            public object hogar { get; set; }
            public object domicilio { get; set; }
            public object miembros { get; set; }
            
        }

        // MODELO PRIVADO PARA EL RESUMEN DE LA MEMBRESIA
        private class ResumenDeMembresia
        {
            public int hb { get; set; }
            public int mb { get; set; }
            public int jhb { get; set; }
            public int jmb { get; set; }
            public int jhnb { get; set; }
            public int jmnb { get; set; }
            public int ninos { get; set; }
            public int ninas { get; set; }
            public int totalBautizados { get; set; }
            public int totalNoBautizados { get; set; }
            public int totalDeMiembros { get; set; }
        }

        // MODELO PARA CREAR EL ARREGLO DE LA LISTA DE CATEGORIAS
        private class FiltroCategorias
        {
            public string dato { get; set; }
            public string categoria { get; set; }
            public bool bautizado { get; set; }
            public int valor { get; set; }
        }

        // MODELO FORMULARIO BAJA DE PERSONA PARA CAMBIO DE DOMICILIO
        public class ModeloBajaPersonaCambioDomicilio
        {
            public int idPersona { get; set; }
            public string tipoDestino { get; set; }
            public DateTime fechaTransaccion { get; set; }
            public int idUsuario { get; set; }
            public bool bajaPorBajaDePadres { get; set; }
        }

        // MODELO FORMULARIO BAJA DE NO BAUTIZADO ALEJAMIENTO/DEFUNCION
        public class bnbad
        {
            public int personaSeleccionada { get; set; }
            public string comentario { get; set; }
            public DateTime fechaTransaccion { get; set; }
            public int idUsuario { get; set; }
        }

        // LISTA DE CATEGORIAS, UTILIZA EL MODELO: FiltroCategorias
        FiltroCategorias[] listaCategorias = new FiltroCategorias[]
        {
            new FiltroCategorias{dato = "hb", categoria = "ADULTO_HOMBRE", bautizado = true, valor = 0 },
            new FiltroCategorias{dato = "mb", categoria = "ADULTO_MUJER", bautizado = true, valor = 0 },
            new FiltroCategorias{dato = "jhb", categoria = "JOVEN_HOMBRE", bautizado = true, valor = 0 },
            new FiltroCategorias{dato = "jmb", categoria = "JOVEN_MUJER", bautizado = true, valor = 0 },
            new FiltroCategorias{dato = "jhnb", categoria = "JOVEN_HOMBRE", bautizado = false, valor = 0 },
            new FiltroCategorias{dato = "jmnb", categoria = "JOVEN_MUJER", bautizado = false, valor = 0 },
            new FiltroCategorias{dato = "ninos", categoria = "NIÑO", bautizado = false, valor = 0 },
            new FiltroCategorias{dato = "ninas", categoria = "NIÑA", bautizado = false, valor = 0 },
        };

        // METODO PRIVADO PARA CALCULAR EL RESUMEN DE MEMBRESIA POR SECTOR
        private IActionResult CalculaResumenPorSector(int sec_Id_Sector)
        {
            foreach (FiltroCategorias categoria in listaCategorias)
            {
                categoria.valor = (from p in context.Persona
                                   where p.sec_Id_Sector == sec_Id_Sector &&
                                   (p.per_Categoria == categoria.categoria && p.per_Bautizado == categoria.bautizado)
                                   && p.per_Activo == true
                                   select new { p.per_Id_Persona }).Count();
            }

            int totalBautizados = 0;
            int totalNoBautizados = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i < 4) { totalBautizados += listaCategorias[i].valor; }
                else { totalNoBautizados += listaCategorias[i].valor; }
            }
            int totalDeMiembros = totalBautizados + totalNoBautizados;

            ResumenDeMembresia resumen = new ResumenDeMembresia();
            resumen.totalDeMiembros = totalDeMiembros;
            resumen.hb = listaCategorias[0].valor;
            resumen.mb = listaCategorias[1].valor;
            resumen.jhb = listaCategorias[2].valor;
            resumen.jmb = listaCategorias[3].valor;
            resumen.totalBautizados = totalBautizados;
            resumen.jhnb = listaCategorias[4].valor;
            resumen.jmnb = listaCategorias[5].valor;
            resumen.ninos = listaCategorias[6].valor;
            resumen.ninas = listaCategorias[7].valor;
            resumen.totalNoBautizados = totalNoBautizados;
            return Ok(resumen);
        }

        private int AltaDeProfesion(int idMinistro, int idPersona, string nombreProfesion)
        {
            // REGISTRA NUEVA SOLICITUD EN BASE DE DATOS
            var solicitud = new SolicitudNuevaProfesion
            {
                descNvaProfesion = nombreProfesion,
                solicitudAtendida = false,
                usu_Id_Usuario = idMinistro,
                per_Id_Persona = idPersona,
                fechaSolicitud = fechayhora
            };
            context.SolicitudNuevaProfesion.Add(solicitud);
            context.SaveChanges();

            // REGISTRA NUEVA PROFESION EN BASE DE DATOS
            Profesion_Oficio pro = new Profesion_Oficio
            {
                pro_Categoria = "OTRO",
                pro_Sub_Categoria = solicitud.descNvaProfesion,
                usu_Id_Usuario = idMinistro,
                Fecha_Registro = fechayhora
            };
            context.Profesion_Oficio.Add(pro);
            context.SaveChanges();

            // ENVIA CORREO DE NUEVA SOLICITUD
            SendMailController sendMail = new SendMailController(context);
            sendMail.EnviarSolicitudNvaProfesion(idMinistro, idPersona, nombreProfesion);

            // RETORNA ID DE LA NUEVA PROFESION
            return pro.pro_Id_Profesion_Oficio;
        }

        // METODO PRIVADO PARA RECALCULAR JERARQUIAS PARA BAJAS
        [HttpPost]
        [Route("[action]/{idPersona}")]
        [EnableCors("AllowOrigin")]
        public IActionResult RestructuraJerarquiasBaja(int idPersona)
        {
            try
            {
                // OBTINE INFORMACION DEL HOGAR PARA CONSULTAR LOS MIEMBROS DEL HOGAR
                var objhp = (from hp in context.Hogar_Persona
                             where hp.per_Id_Persona == idPersona
                             select hp).FirstOrDefault();

                var jerarquiaMaxima = 0;

                // CONSULTA LOS MIEMBROS DEL HOGAR
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        where hp.hd_Id_Hogar == objhp.hd_Id_Hogar
                                        orderby (hp.hp_Jerarquia)
                                        select new
                                        {
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                            hd_Id_Hogar = hp.hd_Id_Hogar,
                                            hp_Jerarquia = hp.hp_Jerarquia,
                                            per_Id_Persona = hp.per_Id_Persona
                                        }).ToList();

                // DISMINUYE EN 1 LA JERARQUIA DE TODOS LOS MIEMBROS DEL HOGAR (jerarquia - 1)
                foreach (var miembro in miembrosDelHogar)
                {
                    if (miembro.hp_Jerarquia > jerarquiaMaxima)
                    { //Si la jerarquía del integrante es mayor que la que se consideraba jerarquiaMaxima, toma como valor máximo esta jerarquía mayor.
                        jerarquiaMaxima = miembro.hp_Jerarquia;
                    }

                    if (miembro.per_Id_Persona != idPersona
                        && miembro.hp_Jerarquia >= objhp.hp_Jerarquia)
                    {
                        var registro = new Hogar_Persona
                        {
                            hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                            hd_Id_Hogar = miembro.hd_Id_Hogar,
                            per_Id_Persona = miembro.per_Id_Persona,
                            hp_Jerarquia = miembro.hp_Jerarquia - 1, //Todos los integrantes del hogar se les recorre la Jerarquía
                            usu_Id_Usuario = 1,
                            Fecha_Registro = fechayhora
                        };
                        context.Hogar_Persona.Update(registro);
                        context.SaveChanges();
                    }
                }

                // ESTABLECE A LA PERSONA QUE SE DA DE BAJA COMO EL ULTIMO EN EL HOGAR
                objhp.hp_Jerarquia = jerarquiaMaxima;
                context.SaveChanges();

                // ASEGURA JERARQUIAS CORRECTAS
                AseguraJerarquias(objhp.hd_Id_Hogar);

                return Ok(new
                {
                    status = "success",
                    objeto = objhp
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

        // METODO PRIVADO PARA RECALCULAR JERARQUIAS PARA ALTAS
        [HttpPost]
        [Route("[action]/{idPersona}/{nvaJerarquia}")]
        [EnableCors("AllowOrigin")]
        public IActionResult RestructuraJerarquiasAlta(int idPersona, int nvaJerarquia)
        {
            try
            {
                // CONSULTA EL HOGAR AL QUE PERTENECE LA PERSONA
                var objhp = (from hp in context.Hogar_Persona
                             where hp.per_Id_Persona == idPersona
                             select hp).ToList();

                // OBTIENE LOS MIEMBROS DEL HOGAR CONSULTADO
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        where hp.hd_Id_Hogar == objhp[0].hd_Id_Hogar
                                        orderby hp.hp_Jerarquia
                                        select new
                                        {
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                            hd_Id_Hogar = hp.hd_Id_Hogar,
                                            hp_Jerarquia = hp.hp_Jerarquia,
                                            per_Id_Persona = hp.per_Id_Persona
                                        }).ToList();

                // SI LA JERARQUIA SELECCIONADA ES 1 AUMENTA ESTABLECE (JERARQUIA + 1) 
                // EN TODOS LOS MIEMBROS DEL HOGAR
                if (nvaJerarquia == 1)
                {
                    foreach (var miembro in miembrosDelHogar)
                    {
                        if (miembro.per_Id_Persona != idPersona)
                        {
                            var registro = new Hogar_Persona
                            {
                                hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                                hd_Id_Hogar = miembro.hd_Id_Hogar,
                                per_Id_Persona = miembro.per_Id_Persona,
                                hp_Jerarquia = miembro.hp_Jerarquia + 1,
                                usu_Id_Usuario = 1,
                                Fecha_Registro = fechayhora
                            };
                            context.Hogar_Persona.Update(registro);
                            context.SaveChanges();
                        }
                    }
                }
                else // SI NuevaJERARQUIA ES MAYOR DE 1, ESTABLECE (JERARQUIA + 1) A LOS MIEMBROS CON JERARQUIA IGUAL O MAYOR A LA SELECCIONADA
                {
                    foreach (var miembro in miembrosDelHogar)
                    {
                        if (miembro.hp_Jerarquia >= nvaJerarquia
                            && miembro.per_Id_Persona != idPersona)
                        {
                            var registro = new Hogar_Persona
                            {
                                hp_Id_Hogar_Persona = miembro.hp_Id_Hogar_Persona,
                                hd_Id_Hogar = miembro.hd_Id_Hogar,
                                per_Id_Persona = miembro.per_Id_Persona,
                                hp_Jerarquia = miembro.hp_Jerarquia + 1,
                                usu_Id_Usuario = 1,
                                Fecha_Registro = fechayhora
                            };
                            context.Hogar_Persona.Update(registro);
                            context.SaveChanges();
                        }
                    }
                }

                // ESTABLECE A LA PERSONA EN LA JERARQUIA SELECIONADA
                objhp[0].hp_Jerarquia = nvaJerarquia;
                context.Hogar_Persona.Update(objhp[0]);
                context.SaveChanges();

                // ASEGURA JERARQUIAS
                AseguraJerarquias(objhp[0].hd_Id_Hogar);
                return Ok(new
                {
                    status = "success",
                    objeto = objhp
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

        // ASEGURA JERARQUIAS
        private void AseguraJerarquias(int hd_Id_Hogar)
        {
            // OBTIENE LOS MIEMBROS DEL HOGAR CONSULTADO
            List<Hogar_Persona> miembrosDelHogar = (from hp in context.Hogar_Persona
                                                    where hp.hd_Id_Hogar == hd_Id_Hogar
                                                    orderby hp.hp_Jerarquia
                                                    select hp).ToList();
            int i = 1;
            foreach (Hogar_Persona m in miembrosDelHogar)
            {
                m.hp_Jerarquia = i;
                context.Hogar_Persona.Update(m);
                context.SaveChanges();
                i = i + 1;
            }
        }

        // GET: api/Persona
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Persona>> Get()
        {
            try
            {
                return Ok(context.Persona.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Persona/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int id)
        {
            Persona persona = new Persona();
            try
            {
                persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
                return Ok(persona);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Persona/GetProfesion1/idPersona
        [Route("[action]/{idPersona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetProfesion1(int idPersona)
        {
            var query = (from per in context.Persona
                         join pro in context.Profesion_Oficio
                         on per.pro_Id_Profesion_Oficio1 equals pro.pro_Id_Profesion_Oficio
                         where per.per_Id_Persona == idPersona
                         select new
                         {
                             pro_Sub_Categoria = pro.pro_Sub_Categoria,
                             pro_Categoria = pro.pro_Categoria
                         }).ToList();
            return Ok(query);
        }

        // GET: api/Persona/GetProfesion2/idPersona
        [Route("[action]/{idPersona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetProfesion2(int idPersona)
        {
            var query = (from per in context.Persona
                         join pro in context.Profesion_Oficio
                         on per.pro_Id_Profesion_Oficio2 equals pro.pro_Id_Profesion_Oficio
                         where per.per_Id_Persona == idPersona
                         select new
                         {
                             pro_Sub_Categoria = pro.pro_Sub_Categoria,
                             pro_Categoria = pro.pro_Categoria
                         }).ToList();
            return Ok(query);
        }

        // GET: api/Persona/GetListaNinosBySector/227
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetListaNinosBySector(int sec_Id_Sector)
        {
            try
            {
                //SELECT per_Id_Persona,per_Nombre,per_Apellido_Paterno,
                //  per_Apellido_Materno,per_Categoria 
                //FROM Persona 
                //WHERE per_Categoria LIKE 'NIÑ%' 
                //  AND per_Id_Persona NOT IN (SELECT per_Id_Persona FROM Presentacion_Nino)
                var query = (from p in context.Persona
                             where (p.per_Categoria.Contains("NIÑ") && p.sec_Id_Sector == sec_Id_Sector)
                             && p.per_Activo
                             && !(from pdn in context.Presentacion_Nino select pdn.per_Id_Persona).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Categoria
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    listaDeNinos = query
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

        // GET: api/Persona/GetByRFCSinHomo/str
        [Route("[action]/{RFCSinHomo}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByRFCSinHomo(string RFCSinHomo)
        {
            // var persona = context.Persona.FirstOrDefault(per => per.per_RFC_Sin_Homo == RFCSinHomo);

            var query = (from p in context.Persona
                         join s in context.Sector
                         on p.sec_Id_Sector equals s.sec_Id_Sector
                         join d in context.Distrito
                         on s.dis_Id_Distrito equals d.dis_Id_Distrito
                         where p.per_RFC_Sin_Homo == RFCSinHomo
                         select new
                         {
                             p.per_Id_Persona,
                             per_Nombre = p.per_Nombre,
                             per_Apellido_Paterno = p.per_Apellido_Paterno,
                             per_ApellidoMaterno = p.per_Apellido_Materno,
                             per_Fecha_Nacimiento = p.per_Fecha_Nacimiento,
                             p.per_Bautizado,
                             p.per_En_Comunion,
                             p.per_Vivo,
                             p.per_Categoria,
                             dis_Numero = d.dis_Numero,
                             dis_Tipo_Distrito = d.dis_Tipo_Distrito,
                             sec_Alias = s.sec_Alias
                         }).ToList();

            if (query.Count > 0)
            {
                return Ok(
                     new { status = true, persona = query }
                );
            }
            else
            {
                return Ok(
                    new { status = false, mensaje = "Persona no encontrada" }
                );
            }
        }

        // GET: api/Persona/GetBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBySector(int sec_Id_Sector)
        {
            var hogares = new Hogares(context);

            //Provee una lista de las personas Activas o Inactivas que pertenecen a un Sector
            List<PersonaDomicilioMiembros> query = new List<PersonaDomicilioMiembros>();
            var query1 = (from p in context.Persona
                          join s in context.Sector
                          on p.sec_Id_Sector equals s.sec_Id_Sector
                          where p.sec_Id_Sector == sec_Id_Sector
                          orderby p.per_Nombre
                          select new
                          {
                              p.per_Id_Persona,
                              p.per_Activo,
                              p.per_En_Comunion,
                              p.per_Vivo,
                              p.per_Visibilidad_Abierta,
                              p.sec_Id_Sector,
                              p.per_Categoria,
                              p.per_Nombre,
                              p.per_Apellido_Paterno,
                              p.per_Apellido_Materno,
                              p.per_Nombre_Completo,
                              p.per_Fecha_Nacimiento,
                              edad = (fechayhora - p.per_Fecha_Nacimiento).Days / 365,
                              p.per_RFC_Sin_Homo,
                              p.per_Nombre_Padre,
                              p.per_Nombre_Madre,
                              p.per_Nombre_Abuelo_Paterno,
                              p.per_Nombre_Abuela_Paterna,
                              p.per_Nombre_Abuelo_Materno,
                              p.per_Nombre_Abuela_Materna,
                              p.pro_Id_Profesion_Oficio1,
                              p.pro_Id_Profesion_Oficio2,
                              p.per_Apellido_Casada,
                              apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada ==null) ? p.per_Apellido_Paterno :(p.per_Apellido_Casada +"* " + p.per_Apellido_Paterno),
                              p.per_Telefono_Movil,
                              p.per_Email_Personal,
                              p.idFoto,
                              p.per_Bautizado,
                              p.per_Lugar_Bautismo,
                              p.per_Fecha_Bautismo,
                              p.per_Ministro_Que_Bautizo,
                              p.per_Fecha_Recibio_Espiritu_Santo,
                              p.per_Bajo_Imposicion_De_Manos,
                              p.per_Cargos_Desempenados,
                              p.per_Cambios_De_Domicilio,
                              p.per_Estado_Civil,
                              p.per_Nombre_Conyuge,
                              p.per_Fecha_Boda_Civil,
                              p.per_Num_Acta_Boda_Civil,
                              p.per_Libro_Acta_Boda_Civil,
                              p.per_Oficialia_Boda_Civil,
                              p.per_Registro_Civil,
                              p.per_Fecha_Boda_Eclesiastica,
                              p.per_Lugar_Boda_Eclesiastica,
                              p.per_Cantidad_Hijos,
                              p.per_Nombre_Hijos,
                              p.per_Nacionalidad,
                              p.per_Lugar_De_Nacimiento,
                              s.sec_Alias,
                              s.sec_Numero,
                              ProfesionOficio1 = (from p2 in context.Persona
                                                  join pro in context.Profesion_Oficio
                                                  on p2.pro_Id_Profesion_Oficio1 equals pro.pro_Id_Profesion_Oficio
                                                  where p2.per_Id_Persona == p.per_Id_Persona
                                                  select new { pro.pro_Sub_Categoria, pro.pro_Categoria }).ToList(),
                              ProfesionOficio2 = (from p2 in context.Persona
                                                  join pro in context.Profesion_Oficio
                                                  on p2.pro_Id_Profesion_Oficio2 equals pro.pro_Id_Profesion_Oficio
                                                  where p2.per_Id_Persona == p.per_Id_Persona
                                                  select new { pro.pro_Sub_Categoria, pro.pro_Categoria }).ToList()
                          }).ToList();
            foreach (var persona in query1)
            {
                var query2 = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == persona.per_Id_Persona);
                var query3 = (from hp in context.Hogar_Persona
                              join hd in context.HogarDomicilio
                              on hp.hd_Id_Hogar equals hd.hd_Id_Hogar
                              join e in context.Estado
                              on hd.est_Id_Estado equals e.est_Id_Estado
                              join pais in context.Pais
                              on hd.pais_Id_Pais equals pais.pais_Id_Pais
                              join p in context.Persona
                              on hp.per_Id_Persona equals p.per_Id_Persona
                              where hp.hd_Id_Hogar == query2.hd_Id_Hogar
                              && hp.hp_Jerarquia == 1
                              select new
                              {
                                  hp.hd_Id_Hogar,
                                  p.per_Id_Persona,
                                  p.per_Nombre,
                                  p.per_Apellido_Paterno,
                                  p.per_Apellido_Materno,
                                  p.per_Apellido_Casada,
                                  apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada +"* " + p.per_Apellido_Paterno),
                                  hd.hd_Calle,
                                  hd.hd_Numero_Exterior,
                                  hd.hd_Numero_Interior,
                                  hd.hd_Tipo_Subdivision,
                                  hd.hd_Subdivision,
                                  hd.hd_Localidad,
                                  hd.hd_Municipio_Ciudad,
                                  e.est_Nombre,
                                  pais.pais_Nombre_Corto,
                                  hd.hd_Telefono,
                                  hd.hd_Activo,
                                  hd.hd_CP,
                                  direccion = hogares.getDireccion(hd.hd_Calle, hd.hd_Numero_Exterior, hd.hd_Numero_Interior, hd.hd_Tipo_Subdivision, hd.hd_Subdivision, hd.hd_Localidad, hd.hd_Municipio_Ciudad, e.est_Nombre, pais.pais_Nombre_Corto, hd.hd_CP)
            }).ToList();
                var query4 = (from hp in context.Hogar_Persona
                              join p in context.Persona
                              on hp.per_Id_Persona equals p.per_Id_Persona
                              where hp.hd_Id_Hogar == query2.hd_Id_Hogar
                              && p.per_Vivo // && p.per_Activo
                              orderby (hp.hp_Jerarquia)
                              select new
                              {
                                  hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                  hd_Id_Hogar = hp.hd_Id_Hogar,
                                  hp_Jerarquia = hp.hp_Jerarquia,
                                  per_Id_Persona = p.per_Id_Persona,
                                  per_Nombre = p.per_Nombre,
                                  per_Apellido_Paterno = p.per_Apellido_Paterno,
                                  per_Apellido_Materno = p.per_Apellido_Materno,
                                  per_Apellido_Casada = p.per_Apellido_Casada,
                                  apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                  p.per_Bautizado,
                                  p.per_Fecha_Nacimiento,
                                  p.per_Telefono_Movil,
                                  p.per_Activo
                              }).ToList();
                query.Add(new PersonaDomicilioMiembros
                {
                    persona = persona,
                    hogar = query2,
                    domicilio = query3,
                    miembros = query4                    
                });
            }
            return Ok(query);
        }

        // GET: api/Persona/GetBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetVivoNoActivoBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             where p.sec_Id_Sector == sec_Id_Sector
                             && (p.per_Vivo == true && p.per_Activo == false)
                             orderby p.per_Nombre
                             select p).ToList();

                return Ok(new
                {
                    status = "success",
                    personas = query
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




        // GET: api/Persona/GetBautizadosBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBautizadosBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             //&& p.per_Bautizado == true
                             && p.per_En_Comunion == true
                             && p.per_Activo == true
                             select p).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetBautizadosComunionBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBautizadosComunionBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == true
                             && p.per_En_Comunion == true
                             && p.per_Vivo == true
                             //&& !(from hte in context.Historial_Transacciones_Estadisticas
                             //     where (hte.ct_Codigo_Transaccion == 11103
                             //     || hte.ct_Codigo_Transaccion == 11102)
                             //     select hte.per_Persona_Id).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Apellido_Casada,
                                 apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetBautizadosComunionVivoBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBautizadosComunionVivoBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == true
                             && p.per_En_Comunion == true
                             && p.per_Vivo == true
                             //&& !(from hte in context.Historial_Transacciones_Estadisticas
                             //     where hte.ct_Codigo_Transaccion == 11101
                             //     || (hte.ct_Codigo_Transaccion == 11102 || hte.ct_Codigo_Transaccion == 11103)
                             //     select hte.per_Persona_Id).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Apellido_Casada,
                                 apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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



        // GET: api/Persona/GetBautizadosComunionVivoBySector/dis_Id_Distrito
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBautizadosComunionVivoByDistrito(int dis_Id_Distrito)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito                             
                             where d.dis_Id_Distrito == dis_Id_Distrito
                             && p.per_Bautizado == true
                             && p.per_En_Comunion == true
                             && p.per_Vivo == true
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Apellido_Casada,
                                 apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetNoBautizadosDefuncionBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetNoBautizadosDefuncionBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == false
                             && p.per_Vivo == true
                             && p.per_Activo
                             //&& !(from hte in context.Historial_Transacciones_Estadisticas
                             //     where hte.ct_Codigo_Transaccion == 12101
                             //     select hte.per_Persona_Id).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetNoBautizadosAlejamientoBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetNoBautizadosAlejamientoBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == false
                             && p.per_Vivo == true
                             && p.per_Activo
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetBautizadosBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetNoBautizadosBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == false
                             && p.per_Vivo == true
                             && p.per_Activo == true
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetByDistrito/dis_Id_Distrito
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetByDistrito(int dis_Id_Distrito)
        {
            try
            {
                List<PersonaDomicilioMiembros> query = new List<PersonaDomicilioMiembros>();
                var query1 = (from p in context.Persona
                              join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                              join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                              where d.dis_Id_Distrito == dis_Id_Distrito
                              select new
                              {
                                  p.per_Id_Persona,
                                  p.per_Activo,
                                  p.per_En_Comunion,
                                  p.per_Vivo,
                                  p.per_Visibilidad_Abierta,
                                  p.sec_Id_Sector,
                                  p.per_Categoria,
                                  p.per_Nombre,
                                  p.per_Apellido_Paterno,
                                  p.per_Apellido_Materno,
                                  p.per_Fecha_Nacimiento,
                                  edad = (fechayhora - p.per_Fecha_Nacimiento).Days / 365,
                                  p.per_RFC_Sin_Homo,
                                  p.per_Nombre_Padre,
                                  p.per_Nombre_Madre,
                                  p.per_Nombre_Abuelo_Paterno,
                                  p.per_Nombre_Abuela_Paterna,
                                  p.per_Nombre_Abuelo_Materno,
                                  p.per_Nombre_Abuela_Materna,
                                  p.pro_Id_Profesion_Oficio1,
                                  p.pro_Id_Profesion_Oficio2,
                                  p.per_Telefono_Movil,
                                  p.per_Email_Personal,
                                  p.idFoto,
                                  p.per_Bautizado,
                                  p.per_Lugar_Bautismo,
                                  p.per_Fecha_Bautismo,
                                  p.per_Ministro_Que_Bautizo,
                                  p.per_Fecha_Recibio_Espiritu_Santo,
                                  p.per_Bajo_Imposicion_De_Manos,
                                  p.per_Cargos_Desempenados,
                                  p.per_Cambios_De_Domicilio,
                                  p.per_Estado_Civil,
                                  p.per_Nombre_Conyuge,
                                  p.per_Fecha_Boda_Civil,
                                  p.per_Num_Acta_Boda_Civil,
                                  p.per_Libro_Acta_Boda_Civil,
                                  p.per_Oficialia_Boda_Civil,
                                  p.per_Registro_Civil,
                                  p.per_Fecha_Boda_Eclesiastica,
                                  p.per_Lugar_Boda_Eclesiastica,
                                  p.per_Apellido_Casada,
                                  apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                  p.per_Cantidad_Hijos,
                                  p.per_Nombre_Hijos,
                                  p.per_Nacionalidad,
                                  p.per_Lugar_De_Nacimiento,
                                  s.sec_Alias,
                                  s.sec_Numero,
                                  ProfesionOficio1 = (from p2 in context.Persona
                                                      join pro in context.Profesion_Oficio
                                                      on p2.pro_Id_Profesion_Oficio1 equals pro.pro_Id_Profesion_Oficio
                                                      where p2.per_Id_Persona == p.per_Id_Persona
                                                      select new { pro.pro_Sub_Categoria, pro.pro_Categoria }).ToList(),
                                  ProfesionOficio2 = (from p2 in context.Persona
                                                      join pro in context.Profesion_Oficio
                                                      on p2.pro_Id_Profesion_Oficio2 equals pro.pro_Id_Profesion_Oficio
                                                      where p2.per_Id_Persona == p.per_Id_Persona
                                                      select new { pro.pro_Sub_Categoria, pro.pro_Categoria }).ToList()
                              }).ToList();
                foreach (var persona in query1)
                {
                    var query2 = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == persona.per_Id_Persona);
                    var query3 = (from hp in context.Hogar_Persona
                                  join hd in context.HogarDomicilio
                                  on hp.hd_Id_Hogar equals hd.hd_Id_Hogar
                                  join e in context.Estado
                                  on hd.est_Id_Estado equals e.est_Id_Estado
                                  join pais in context.Pais
                                  on hd.pais_Id_Pais equals pais.pais_Id_Pais
                                  join p in context.Persona
                                  on hp.per_Id_Persona equals p.per_Id_Persona
                                  where hp.hd_Id_Hogar == query2.hd_Id_Hogar
                                  && hp.hp_Jerarquia == 1
                                  select new
                                  {
                                      hd_Id_Hogar = hp.hd_Id_Hogar,
                                      per_Id_Persona = p.per_Id_Persona,
                                      per_Nombre = p.per_Nombre,
                                      per_Apellido_Paterno = p.per_Apellido_Paterno,
                                      per_Apellido_Materno = p.per_Apellido_Materno,
                                      p.per_Apellido_Casada,
                                      apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                      hd_Calle = hd.hd_Calle,
                                      hd_Numero_Exterior = hd.hd_Numero_Exterior,
                                      hd_Numero_Interior = hd.hd_Numero_Interior,
                                      hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                                      hd_Subdivision = hd.hd_Subdivision,
                                      hd_Localidad = hd.hd_Localidad,
                                      hd_Municipio_Ciudad = hd.hd_Municipio_Ciudad,
                                      est_Nombre = e.est_Nombre,
                                      pais_Nombre_Corto = pais.pais_Nombre_Corto,
                                      hd_Telefono = hd.hd_Telefono,
                                      hd.hd_Activo
                                  }).ToList();
                    var query4 = (from hp in context.Hogar_Persona
                                  join p in context.Persona
                                  on hp.per_Id_Persona equals p.per_Id_Persona
                                  where hp.hd_Id_Hogar == query2.hd_Id_Hogar
                                  orderby (hp.hp_Jerarquia)
                                  select new
                                  {
                                      hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                      hd_Id_Hogar = hp.hd_Id_Hogar,
                                      hp_Jerarquia = hp.hp_Jerarquia,
                                      per_Id_Persona = p.per_Id_Persona,
                                      per_Nombre = p.per_Nombre,
                                      per_Apellido_Paterno = p.per_Apellido_Paterno,
                                      per_Apellido_Materno = p.per_Apellido_Materno,
                                      per_Apellido_Casada = p.per_Apellido_Casada,
                                      apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                      p.per_Bautizado,
                                      p.per_Fecha_Nacimiento,
                                      p.per_Telefono_Movil,
                                      p.per_Activo
                                  }).ToList();
                    query.Add(new PersonaDomicilioMiembros
                    {
                        persona = persona,
                        hogar = query2,
                        domicilio = query3,
                        miembros = query4
                    });
                }
                return Ok(query);
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

        // GET: api/Persona/GetPersonaRestitucion/227/true
        [Route("[action]/{sec_Id_Sector}/{bautizado}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonaRestitucion(int sec_Id_Sector, bool bautizado)
        {
            try
            {
                // Declarando variables de personas
                var personas = new List<object>();

                // Poblando variables con personas del sector y con visibilidad abierta
                var delSector = (from p in context.Persona
                                 join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                                 join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                 join hp in context.Hogar_Persona on p.per_Id_Persona equals hp.per_Id_Persona
                                 where (p.sec_Id_Sector == sec_Id_Sector && p.per_Bautizado == bautizado)
                                 && !p.per_En_Comunion
                                 && !p.per_Activo
                                 && p.per_Vivo
                                 select new
                                 {
                                     p.per_Id_Persona,
                                     p.per_Activo,
                                     p.per_En_Comunion,
                                     p.per_Vivo,
                                     p.per_Visibilidad_Abierta,
                                     p.sec_Id_Sector,
                                     p.per_Nombre,
                                     p.per_Apellido_Paterno,
                                     p.per_Apellido_Materno,
                                     p.per_Apellido_Casada,
                                     apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                     p.per_Bautizado,
                                     p.per_Categoria,
                                     s.sec_Numero,
                                     s.sec_Tipo_Sector,
                                     s.sec_Alias,
                                     d.dis_Id_Distrito,
                                     d.dis_Alias,
                                     hp.hd_Id_Hogar
                                 }).ToList();

                var otroSector = (from p in context.Persona
                                  join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                                  join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                  where (p.sec_Id_Sector != sec_Id_Sector && p.per_Visibilidad_Abierta)
                                  && (!p.per_En_Comunion && p.per_Bautizado == bautizado)
                                  select new
                                  {
                                      p.per_Id_Persona,
                                      p.per_Activo,
                                      p.per_En_Comunion,
                                      p.per_Vivo,
                                      p.per_Visibilidad_Abierta,
                                      p.sec_Id_Sector,
                                      p.per_Nombre,
                                      p.per_Apellido_Paterno,
                                      p.per_Apellido_Materno,
                                      p.per_Bautizado,
                                      s.sec_Numero,
                                      s.sec_Tipo_Sector,
                                      s.sec_Alias,
                                      d.dis_Id_Distrito,
                                      d.dis_Alias
                                  }).ToList();

                foreach (var p in delSector) { personas.Add(p); }
                foreach (var p in otroSector) { personas.Add(p); }

                // Retornando resultados
                return Ok(new
                {
                    status = "success",
                    personas = personas
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

        // GET: api/Persona/GetPersonaCambioDomicilio/227/false
        [Route("[action]/{sec_Id_Sector}/{bautizado}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonaCambioDomicilio(int sec_Id_Sector, bool bautizado)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where (p.sec_Id_Sector != sec_Id_Sector && p.per_Visibilidad_Abierta)
                             && (p.per_Bautizado == bautizado && !p.per_Activo)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Bautizado,
                                 s.sec_Numero,
                                 s.sec_Tipo_Sector,
                                 s.sec_Alias,
                                 d.dis_Id_Distrito,
                                 d.dis_Alias
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetResumenMembresiaBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetResumenMembresiaBySector(int sec_Id_Sector)
        {
            try
            {
                var resumen = CalculaResumenPorSector(sec_Id_Sector);
                return Ok(new
                {
                    status = "success",
                    resumen = resumen
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

        // GET: api/Persona/GetResumenMembresiaByDistrito/dis_Id_Distrito
        [Route("[action]/{dis_Id_Distrito}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetResumenMembresiaByDistrito(int dis_Id_Distrito)
        {
            try
            {
                ResumenDeMembresia resumen = new ResumenDeMembresia();
                resumen.totalBautizados = 0;
                resumen.hb = 0;
                resumen.mb = 0;
                resumen.jhb = 0;
                resumen.jmb = 0;
                resumen.totalNoBautizados = 0;
                resumen.jhnb = 0;
                resumen.jmnb = 0;
                resumen.ninos = 0;
                resumen.ninas = 0;
                resumen.totalDeMiembros = 0;

                var sectores = (from sec in context.Sector
                                join dis in context.Distrito
                                on sec.dis_Id_Distrito equals dis.dis_Id_Distrito
                                where sec.dis_Id_Distrito == dis_Id_Distrito
                                orderby sec.sec_Id_Sector ascending
                                select new { sec.sec_Id_Sector }).ToList();

                foreach (var sector in sectores)
                {
                    foreach (FiltroCategorias categoria in listaCategorias)
                    {
                        categoria.valor = (from p in context.Persona
                                           where p.sec_Id_Sector == sector.sec_Id_Sector &&
                                           (p.per_Categoria == categoria.categoria && p.per_Bautizado == categoria.bautizado)
                                           && p.per_Activo == true
                                           select new { p.per_Id_Persona }).Count();
                    }

                    int totalBautizados = 0;
                    int totalNoBautizados = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (i < 4) { totalBautizados += listaCategorias[i].valor; }
                        else { totalNoBautizados += listaCategorias[i].valor; }
                    }
                    int totalDeMiembros = totalBautizados + totalNoBautizados;

                    resumen.totalDeMiembros += totalDeMiembros;
                    resumen.hb += listaCategorias[0].valor;
                    resumen.mb += listaCategorias[1].valor;
                    resumen.jhb += listaCategorias[2].valor;
                    resumen.jmb += listaCategorias[3].valor;
                    resumen.totalBautizados += totalBautizados;
                    resumen.jhnb += listaCategorias[4].valor;
                    resumen.jmnb += listaCategorias[5].valor;
                    resumen.ninos += listaCategorias[6].valor;
                    resumen.ninas += listaCategorias[7].valor;
                    resumen.totalNoBautizados += totalNoBautizados;
                }
                return Ok(new
                {
                    status = "success",
                    resumen = resumen
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

        // GET: api/Persona/GetPersonaCambioDomicilioReactivacionRestitucion
        [Route("[action]")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonaCambioDomicilioReactivacionRestitucion([FromBody] PersonaParaCambioDomicilioReactivacionRestitucion filtro)
        {
            var query = new object();
            try
            {
                if (filtro.sectorPersona == filtro.sectorUsuario)
                {
                    query = (from p in context.Persona
                             where p.per_Bautizado == filtro.bautizado &&
                             p.per_Activo == filtro.activo &&
                             p.per_Visibilidad_Abierta == filtro.visibiliadaAbierta &&
                             p.sec_Id_Sector == filtro.sectorPersona
                             select p).ToList();
                }
                else
                {
                    query = (from p in context.Persona
                             where p.per_Bautizado == filtro.bautizado &&
                             p.per_Activo == filtro.activo &&
                             p.per_Visibilidad_Abierta == filtro.visibiliadaAbierta &&
                             p.sec_Id_Sector != filtro.sectorPersona
                             select p).ToList();
                }

                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // GET: api/Persona/GetPersonasVisibilidadAbierta
        [Route("[action]/{per_Bautizado}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonasVisibilidadAbierta(bool per_Bautizado)
        {
            try
            {
                var personas = (from p in context.Persona
                                join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                                join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                where p.per_Bautizado == per_Bautizado && p.per_Visibilidad_Abierta
                                select new
                                {
                                    p.per_Id_Persona,
                                    p.per_Nombre,
                                    p.per_Apellido_Paterno,
                                    p.per_Apellido_Materno,
                                    p.per_Apellido_Casada,
                                    apellidoPrincipal = (p.per_Apellido_Casada == "" || p.per_Apellido_Casada == null) ? p.per_Apellido_Paterno : (p.per_Apellido_Casada + "* " + p.per_Apellido_Paterno),
                                    p.per_Activo,
                                    p.per_En_Comunion,
                                    p.sec_Id_Sector,
                                    s.sec_Numero,
                                    s.sec_Tipo_Sector,
                                    s.sec_Alias,
                                    d.dis_Id_Distrito,
                                    d.dis_Tipo_Distrito,
                                    d.dis_Numero,
                                    d.dis_Alias
                                }).ToList();
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

        // GET: api/Persona/GetPersonasParaAuxiliarBySector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetPersonasParaAuxiliarBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == true
                             && p.per_En_Comunion == true
                             && p.per_Vivo == true
                             && (p.per_Categoria == "ADULTO_HOMBRE" || p.per_Categoria == "JOVEN_HOMBRE")
                             && !(from pem in context.Personal_Ministerial
                                  where pem.sec_Id_Congregacion == sec_Id_Sector 
                                  && pem.pem_Grado_Ministerial == "AUXILIAR" 
                                  && pem.per_Id_Miembro != null
                                  select pem.per_Id_Miembro).Contains(p.per_Id_Persona)
                             select new
                             {
                                 p.per_Id_Persona,
                                 p.per_Activo,
                                 p.per_En_Comunion,
                                 p.per_Vivo,
                                 p.per_Visibilidad_Abierta,
                                 p.sec_Id_Sector,
                                 p.per_Nombre,
                                 p.per_Apellido_Paterno,
                                 p.per_Apellido_Materno,
                                 p.per_Bautizado
                             }).ToList();
                return Ok(new
                {
                    status = "success",
                    personas = query
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

        // POST: api/Persona
        [HttpPost]
        [Route("[action]/{per_Id_Persona}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Persona persona)
        {
            try
            {
                persona.Fecha_Registro = fechayhora;
                persona.usu_Id_Usuario = 1;
                context.Persona.Add(persona);
                context.SaveChanges();
                return Ok
                (
                    new
                    {
                        status = true,
                        nvaPersona = persona.per_Id_Persona
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

        // POST: api/Persona/BajaBautizadoExcomunion/per_Id_Persona
        [HttpPost]
        [Route("[action]/{per_Id_Persona}/{tipoExcomunion}/{delitoExomunion}/{fechaExcomunion}/{usu_Id_Usuario}")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaBautizadoExcomunion(
            int per_Id_Persona,
            int tipoExcomunion,
            string delitoExomunion,
            DateTime fechaExcomunion,
            int usu_Id_Usuario)
        {
            try
            {
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                // CONSULTA EL HOGAR AL QUE PERTENECE LA PERSONA
                var objhp = (from hp1 in context.Hogar_Persona
                             where hp1.per_Id_Persona == per_Id_Persona
                             select hp1).ToList();

                // OBTIENE LOS MIEMBROS DEL HOGAR CONSULTADO
                var miembrosDelHogar = (from per in context.Persona
                                        join hp in context.Hogar_Persona on per.per_Id_Persona equals hp.per_Id_Persona
                                        where hp.hd_Id_Hogar == objhp[0].hd_Id_Hogar && per.per_Activo == true
                                        select new
                                        {
                                            per_Id_Persona = hp.per_Id_Persona,
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona
                                        }).ToList();

                // OBTIENE DATOS DE LA PERSONA
                var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == per_Id_Persona);

                // CUENTA LAS PERSONAS BAUTIZADAS
                int bautizados = 0;
                foreach (var p0 in miembrosDelHogar)
                {
                    var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p0.per_Id_Persona);
                    if (persona.per_Bautizado && persona.per_Vivo && persona.per_En_Comunion)
                    {
                        bautizados = bautizados + 1;
                    }
                }

                //// SE ESTABLECE LA JERARQUIA DE LA PERSONA A ULTIMO EN EL HOGAR
                //var hp2 = context.Hogar_Persona.FirstOrDefault(h => h.per_Id_Persona == per_Id_Persona);
                //hp2.hp_Jerarquia = 99;
                //context.Hogar_Persona.Update(hp2);
                //context.SaveChanges();

                // CAMBIO DE ESTATUS DE LA PERSONA
                p.per_En_Comunion = false;
                p.per_Activo = false;
                context.Persona.Update(p);
                context.SaveChanges();

                // SE REGISTRA HISTORIAL ESTADISTICO DE LA PERSONA
                hte.RegistroHistorico(per_Id_Persona, p.sec_Id_Sector, tipoExcomunion, delitoExomunion, fechaExcomunion, usu_Id_Usuario);

                if (bautizados == 1) //Si era el Último Bautizado, se debe de dar de Baja el Hogar
                {
                    // SE ESTABLECE LA BAJA DEL DOMICILIO ANTERIOR DEBIDO A QUE NO HAY PERSONAS BAUTIZADAS
                    var hdx = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == objhp[0].hd_Id_Hogar);
                    hdx.hd_Activo = false;
                    context.HogarDomicilio.Update(hdx);
                    context.SaveChanges();

                    // SE GENERA REGISTRO DE BAJA DE DOMICILIO
                    hte.RegistroHistorico(
                        p.per_Id_Persona,
                        p.sec_Id_Sector,
                        31102,
                        $"{p.per_Nombre} {p.per_Apellido_Paterno} {p.per_Apellido_Materno}",
                        fechaExcomunion,
                        usu_Id_Usuario);

                    foreach (var p1 in miembrosDelHogar) // Si era el ultimo bautizado sigunifica que si hay más son No Bautizados en el Hogar, se deben dar de Baja por 'BAJA DE PADRES'
                    {
                        if (p1.per_Id_Persona != per_Id_Persona)
                        {
                            // SE INACTIVAN LAS PERSONAS DEL DOMICILIO ANTERIOR PORQUE YA NO HAY PERSONAS BAUTIZADAS
                            var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p1.per_Id_Persona);
                            persona.per_Activo = false;
                            context.Persona.Update(persona);
                            context.SaveChanges();

                            // SE GENERA REGISTRO DE BAJA POR PADRES
                            hte.RegistroHistorico(persona.per_Id_Persona, persona.sec_Id_Sector, 12106, "POR BAJA DE PADRES", fechaExcomunion, usu_Id_Usuario);
                        }
                    }
                }
                else
                {
                    // Restructura las Jerarquías y las arregla para que sean consecutivas dejando a la persona dada de baja al final en la jerarquía del Hogar.
                    RestructuraJerarquiasBaja(per_Id_Persona);
                }

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

        // POST: api/Persona/BajaBautizadoDefuncion
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaBautizadoDefuncion([FromBody] bnbad bnbad)
        {
            try
            {
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                // CONSULTA EL HOGAR AL QUE PERTENECE LA PERSONA, esto porque despues debe revisar si el Hogar se debe dar de baja
                var objhp = (from hp1 in context.Hogar_Persona
                             where hp1.per_Id_Persona == bnbad.personaSeleccionada
                             select hp1).ToList();

                // OBTIENE LOS MIEMBROS DEL HOGAR CONSULTADO que aun estén Activos
                var miembrosDelHogar = (from per in context.Persona
                                        join hp in context.Hogar_Persona on per.per_Id_Persona equals hp.per_Id_Persona
                                        where hp.hd_Id_Hogar == objhp[0].hd_Id_Hogar && per.per_Activo == true
                                        select new
                                        {
                                            per_Id_Persona = hp.per_Id_Persona,
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona
                                        }).ToList();

                // DATOS DE LA PERSONA
                var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == bnbad.personaSeleccionada);

                // CUENTA LAS PERSONAS BAUTIZADAS que esten EN COMUNION para saber si despues debe dar de baja el Hogar si éste que falleció era el último bautizado en el hogar
                int bautizados = 0;
                foreach (var p0 in miembrosDelHogar)
                {
                    var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p0.per_Id_Persona);
                    if (persona.per_Bautizado && persona.per_Vivo && persona.per_En_Comunion)
                    {
                        bautizados = bautizados + 1;
                    }
                }

                // CAMBIO DE ESTATUS DE LA PERSONA para marcarla como fallecida
                p.per_Activo = false;
                p.per_Vivo = false;
                context.Persona.Update(p);
                context.SaveChanges();

                // SE REGISTRA HISTORIAL ESTADISTICO DE LA defunción de la persona
                hte.RegistroHistorico(bnbad.personaSeleccionada, p.sec_Id_Sector, 11101, "", bnbad.fechaTransaccion, bnbad.idUsuario);


                //Si solo hay 1 bautizado significa que es el último bautizado en el hogar y se debe dar de baja el hogar y reacomodar a los integrantes y sus Jerarquias
                if (bautizados == 1)
                {
                    // SE ESTABLECE LA BAJA DEL DOMICILIO ANTERIOR DEBIDO A QUE NO HAY PERSONAS BAUTIZADAS
                    var hdx = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == objhp[0].hd_Id_Hogar);
                    hdx.hd_Activo = false;
                    context.HogarDomicilio.Update(hdx);
                    context.SaveChanges();

                    // SE GENERA REGISTRO DE BAJA DE DOMICILIO
                    hte.RegistroHistorico(
                        p.per_Id_Persona,
                        p.sec_Id_Sector,
                        31102,
                        $"{p.per_Nombre} {p.per_Apellido_Paterno} {p.per_Apellido_Materno}",
                        bnbad.fechaTransaccion,
                        bnbad.idUsuario);

                    foreach (var p1 in miembrosDelHogar) // SE INACTIVAN LAS PERSONAS NB (QUE NO SON LA PERSONA SELECCIONADA) DEL DOMICILIO ANTERIOR PORQUE YA NO HAY PERSONAS BAUTIZADAS
                    {
                        if (p1.per_Id_Persona != bnbad.personaSeleccionada)
                        {
                            var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p1.per_Id_Persona);
                            persona.per_Activo = false;
                            context.Persona.Update(persona);
                            context.SaveChanges();

                            // SE GENERA REGISTRO DE BAJA POR PADRES
                            hte.RegistroHistorico(persona.per_Id_Persona, persona.sec_Id_Sector, 12106, "POR BAJA DE PADRES", bnbad.fechaTransaccion, bnbad.idUsuario);
                        }
                    }
                }
                else //Si No era el último Bautizado en el Hogar
                {
                    // Restructura las Jerarquías y las arregla para que sean consecutivas dejando a la persona dada de baja al final en la jerarquía del Hogar.
                    RestructuraJerarquiasBaja(bnbad.personaSeleccionada);
                }

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

        // POST: api/Persona/BajaNoBautizadoDefuncion/per_Id_Persona
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaNoBautizadoDefuncion([FromBody] bnbad bnbad)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where p.per_Id_Persona == bnbad.personaSeleccionada
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Alias,
                                 s.sec_Id_Sector
                             }).ToList();
                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                hte.ct_Codigo_Transaccion = 12101;
                hte.dis_Distrito_Alias = query[0].dis_Alias;
                hte.dis_Distrito_Id = query[0].dis_Id_Distrito;
                hte.hte_Cancelado = false;
                hte.hte_Comentario = bnbad.comentario;
                hte.hte_Fecha_Transaccion = bnbad.fechaTransaccion;
                hte.per_Persona_Id = bnbad.personaSeleccionada;
                hte.sec_Sector_Alias = query[0].sec_Alias;
                hte.sec_Sector_Id = query[0].sec_Id_Sector;
                hte.Usu_Usuario_Id = bnbad.idUsuario;

                context.Historial_Transacciones_Estadisticas.Add(hte);
                context.SaveChanges();

                var query2 = (from p in context.Persona
                              where p.per_Id_Persona == bnbad.personaSeleccionada
                              select p).FirstOrDefault();
                query2.per_Vivo = false;
                query2.per_Activo = false;
                context.SaveChanges();

                RestructuraJerarquiasBaja(query2.per_Id_Persona);

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

        // POST: api/Persona/BajaNoBautizadoAlejamiento/per_Id_Persona
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaNoBautizadoAlejamiento([FromBody] bnbad bnbad)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where p.per_Id_Persona == bnbad.personaSeleccionada
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Alias,
                                 s.sec_Id_Sector
                             }).ToList();
                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                hte.ct_Codigo_Transaccion = 12102;
                hte.dis_Distrito_Alias = query[0].dis_Alias;
                hte.dis_Distrito_Id = query[0].dis_Id_Distrito;
                hte.hte_Cancelado = false;
                hte.hte_Comentario = bnbad.comentario != null ? bnbad.comentario : "";
                hte.hte_Fecha_Transaccion = bnbad.fechaTransaccion;
                hte.per_Persona_Id = bnbad.personaSeleccionada;
                hte.sec_Sector_Alias = query[0].sec_Alias;
                hte.sec_Sector_Id = query[0].sec_Id_Sector;
                hte.Usu_Usuario_Id = bnbad.idUsuario;

                context.Historial_Transacciones_Estadisticas.Add(hte);
                context.SaveChanges();

                var query2 = (from p in context.Persona
                              where p.per_Id_Persona == bnbad.personaSeleccionada
                              select p).FirstOrDefault();
                //query2.per_Vivo = true;
                query2.per_Activo = false;
                context.SaveChanges();

                RestructuraJerarquiasBaja(query2.per_Id_Persona);

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

        // METODO BAJA PERSONA POR CAMBIO DE DOMICILIO
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaPersonaCambioDomicilio([FromBody] ModeloBajaPersonaCambioDomicilio mbpcd)
        {
            try
            {
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                // CONSULTA EL HOGAR AL QUE PERTENECE LA PERSONA
                var objhp = (from hp1 in context.Hogar_Persona
                             where hp1.per_Id_Persona == mbpcd.idPersona
                             select hp1).ToList();

                // OBTIENE LOS MIEMBROS DEL HOGAR CONSULTADO
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        join per in context.Persona on hp.per_Id_Persona equals per.per_Id_Persona
                                        where hp.hd_Id_Hogar == objhp[0].hd_Id_Hogar && per.per_Activo == true
                                        select new
                                        {
                                            per_Id_Persona = hp.per_Id_Persona,
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                            per_Activo = per.per_Activo

                                        }).ToList();

                // DATOS DE LA PERSONA
                var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == mbpcd.idPersona);

                // CUENTA LAS PERSONAS BAUTIZADAS
                int bautizados = 0;
                foreach (var p0 in miembrosDelHogar)
                {
                    var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p0.per_Id_Persona);
                    if (persona.per_Bautizado && persona.per_Vivo && persona.per_Activo && persona.per_En_Comunion)
                    {
                        bautizados = bautizados + 1;
                    }
                }

                // Lógica para determinar el Código de Transacción
                int codigoTransaccion = 0;

                if (p.per_Bautizado) //Si es BAUTIZADO
                {
                    codigoTransaccion = mbpcd.tipoDestino == "INTERNO" ? 11104 : 11105;
                }
                else //Si es NO BAUTIZADO
                {
                    codigoTransaccion = mbpcd.tipoDestino == "INTERNO" ? 12103 : 12104;
                }

                // SE CAMBIA ESTATUS DE LA VISIBILIDAD DE LA PERSONA Y A ESTATUS INACTIVO PARA AMBOS CASOS BAUTIZADOS Y NO BAUTIZADOS
                p.per_Activo = false;
                p.per_Visibilidad_Abierta = true;
                context.Persona.Update(p);
                context.SaveChanges();

                // AGREGA REGISTRO HISTORICO DEL CAMBIO DE ESTATUS DE LA PERSONA
                hte.RegistroHistorico(p.per_Id_Persona, p.sec_Id_Sector, codigoTransaccion, "", mbpcd.fechaTransaccion, mbpcd.idUsuario);

                //// SE ESTABLECE LA JERARQUIA DE LA PERSONA A ULTIMO EN EL HOGAR
                //var hp2 = context.Hogar_Persona.FirstOrDefault(h => h.per_Id_Persona == mbpcd.idPersona);
                //hp2.hp_Jerarquia = 99;
                //context.Hogar_Persona.Update(hp2);
                //context.SaveChanges();

                //// ASEGURA JERARQUIAS CORRECTAS
                //AseguraJerarquias(hp2.hd_Id_Hogar);



                //Si es BAUTIZADO revisa si debe dar de Baja el Hogar y otros integrantes No Bautizados
                if (p.per_Bautizado)
                {
                    if (bautizados == 1) //Si es el último bautizad,o debe de darse de baja el Hogar
                    {
                        // SE ESTABLECE LA BAJA DEL DOMICILIO ANTERIOR DEBIDO A QUE NO HAY PERSONAS BAUTIZADAS
                        var hdx = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == objhp[0].hd_Id_Hogar);
                        hdx.hd_Activo = false;
                        context.HogarDomicilio.Update(hdx);
                        context.SaveChanges();

                        // SE GENERA REGISTRO DE BAJA DE DOMICILIO
                        hte.RegistroHistorico(
                            p.per_Id_Persona,
                            p.sec_Id_Sector,
                            31102,
                            $"{p.per_Nombre} {p.per_Apellido_Paterno} {p.per_Apellido_Materno}",
                            mbpcd.fechaTransaccion,
                            mbpcd.idUsuario);


                        foreach (var p1 in miembrosDelHogar) //Si es el último bautizado debe también dar de baja a los dependientes No Bautizados Activos.
                        {
                            if (p1.per_Id_Persona != mbpcd.idPersona && p1.per_Activo == true)
                            {
                                // SE INACTIVAN LAS PERSONAS DEL DOMICILIO ANTERIOR PORQUE YA NO HAY PERSONAS BAUTIZADAS
                                var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p1.per_Id_Persona);
                                persona.per_Activo = false;
                                if (mbpcd.bajaPorBajaDePadres == true) { persona.per_Visibilidad_Abierta = false; } else { persona.per_Visibilidad_Abierta = true; }
                                context.Persona.Update(persona);
                                context.SaveChanges();

                                //// SE ESTABLECE LA JERARQUIA DE LA PERSONA A ULTIMO EN EL HOGAR
                                //var hp1 = context.Hogar_Persona.FirstOrDefault(h => h.per_Id_Persona == p1.per_Id_Persona);
                                //hp1.hp_Jerarquia = 99;
                                //context.Hogar_Persona.Update(hp1);
                                //context.SaveChanges();
                                // ASEGURA JERARQUIAS CORRECTAS
                                AseguraJerarquias(objhp[0].hd_Id_Hogar);

                                // SE GENERA REGISTRO DE BAJA POR CAMBIO DE DOMICILIO O POR BAJA DE PADRES SEGUN SE HAYA SELECCIONADO EN EL FRONT END
                                int codTranBajaHijos = mbpcd.tipoDestino == "INTERNO" ? 12103 : 12104;
                                hte.RegistroHistorico(
                                    persona.per_Id_Persona,
                                    persona.sec_Id_Sector,
                                    mbpcd.bajaPorBajaDePadres == true ? 12106 : codTranBajaHijos, // define baja por Baja de Padres o baja por Cambio de Domicilio interno o externo
                                    "",
                                    mbpcd.fechaTransaccion,
                                    mbpcd.idUsuario
                                );
                            }
                        }
                    }
                    else
                    {
                        // Restructura las Jerarquías y las arregla para que sean consecutivas dejando a la persona dada de baja al final en la jerarquía del Hogar.
                        RestructuraJerarquiasBaja(mbpcd.idPersona);
                    }
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

        // POST: api/Persona/AddPersonaHogar
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaHogar([FromBody] PersonaHogarExistente phe)
        {
            try
            {
                // DEFINE OBJETO PERSONA
                Persona persona = new Persona();
                persona = phe.PersonaEntity;
                var nombreCompleto = persona.per_Nombre + " " + persona.per_Apellido_Paterno + " " + (persona.per_Apellido_Materno == "" ? "" : persona.per_Apellido_Materno);
                nombreCompleto = ManejoDeApostrofes.QuitarApostrofe2(nombreCompleto);
                persona.per_Nombre_Completo = nombreCompleto;

                // ALTA DE PERSONA
                persona.Fecha_Registro = fechayhora;
                context.Persona.Add(persona);
                context.SaveChanges();

                // ALTA DE PERSONA EN HOGAR
                Hogar_Persona hp = new Hogar_Persona
                {
                    hp_Jerarquia = 0,
                    per_Id_Persona = persona.per_Id_Persona,
                    hd_Id_Hogar = phe.hdId,
                    Fecha_Registro = fechayhora,
                    usu_Id_Usuario = persona.usu_Id_Usuario
                };
                context.Hogar_Persona.Add(hp);
                context.SaveChanges();
                RestructuraJerarquiasAlta(persona.per_Id_Persona, phe.jerarquia);

                // GENERA HISTORIAL DE PERSONA
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                int ct_Codigo_Transaccion = 0;
                DateTime hte_Fecha_Transaccion = DateTime.Now;
                DateTime Fecha_Lanzamiento_App = new DateTime(2023,6,01);

                if (persona.per_Bautizado)
                {
                    ct_Codigo_Transaccion = 11001;
                    hte.RegistroHistorico(
                        persona.per_Id_Persona,
                        (persona.per_Fecha_Bautismo < Fecha_Lanzamiento_App && phe.idSectorBautismo != 0) ? phe.idSectorBautismo : persona.sec_Id_Sector,
                        ct_Codigo_Transaccion,
                        "",
                        persona.per_Fecha_Bautismo,
                        persona.usu_Id_Usuario);
                }
                else
                {
                    ct_Codigo_Transaccion = 12001;
                    hte.RegistroHistorico(
                        persona.per_Id_Persona,
                        persona.sec_Id_Sector,
                        ct_Codigo_Transaccion,
                        "",
                        phe.FechaTransaccionHistorica == null ? persona.per_Fecha_Nacimiento : phe.FechaTransaccionHistorica,
                        persona.usu_Id_Usuario);
                }


                // ALTA DE NUEVAS PROFESIONES
                if (phe.idOficio1 == 1 && phe.nvaProfesionOficio1 != "")
                {
                    var idNvaProf1 = AltaDeProfesion(persona.usu_Id_Usuario, persona.per_Id_Persona, phe.nvaProfesionOficio1);
                    persona.pro_Id_Profesion_Oficio1 = idNvaProf1;
                }
                else
                {
                    persona.pro_Id_Profesion_Oficio1 = phe.idOficio1;
                }


                if (phe.idOficio2 == 1 && phe.nvaProfesionOficio2 != "")
                {
                    var idNvaProf2 = AltaDeProfesion(persona.usu_Id_Usuario, persona.per_Id_Persona, phe.nvaProfesionOficio2);
                    persona.pro_Id_Profesion_Oficio2 = idNvaProf2;
                }
                else
                {
                    persona.pro_Id_Profesion_Oficio2 = phe.idOficio2;
                }

                // SE GRABA EL NUEVO OFICIO EN LA PERSONA DE INFO DE LA PERSONA
                context.Persona.Update(persona);
                context.SaveChanges();

                // GENERA REGISTRO Y CORREO DE NUEVA PROFESION
                SolicitudNuevaProfesionController snpc = new SolicitudNuevaProfesionController(context);
                if (persona.pro_Id_Profesion_Oficio1 == 1 && phe.nvaProfesionOficio1 != "")
                {
                    snpc.RegistroDeNvaSolicitud(persona.usu_Id_Usuario, persona.per_Id_Persona, phe.nvaProfesionOficio1);
                }
                if (persona.pro_Id_Profesion_Oficio2 == 1 && phe.nvaProfesionOficio2 != "")
                {
                    snpc.RegistroDeNvaSolicitud(persona.usu_Id_Usuario, persona.per_Id_Persona, phe.nvaProfesionOficio2);
                }

                return Ok
                (
                    new
                    {
                        status = "success",
                        persona = persona,
                        //hogar_persona = hpModel
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

        // POST: api/Persona/AddPersonaDomicilioHogar/prueba
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaDomicilioHogar([FromBody] PersonaDomicilio pd)
        {
            try
            {
                // ALTA DE LA PERSONA
                Persona p = new Persona();
                p = pd.PersonaEntity;

                var nombreCompleto = p.per_Nombre + " " + p.per_Apellido_Paterno + " " + (p.per_Apellido_Materno == "" ? "" : p.per_Apellido_Materno);
                nombreCompleto = ManejoDeApostrofes.QuitarApostrofe2(nombreCompleto);
                p.per_Nombre_Completo = nombreCompleto;

                p.Fecha_Registro = fechayhora;
                context.Persona.Add(p);
                context.SaveChanges();

                // ALTA DE NUEVAS PROFESIONES
                if (pd.idOficio1 == 1 && pd.nvaProfesionOficio1 != "")
                {
                    var idNvaProf1 = AltaDeProfesion(p.usu_Id_Usuario, p.per_Id_Persona, pd.nvaProfesionOficio1);
                    p.pro_Id_Profesion_Oficio1 = idNvaProf1;
                }
                else
                {
                    p.pro_Id_Profesion_Oficio1 = pd.idOficio1;
                }
                if (pd.idOficio2 == 1 && pd.nvaProfesionOficio2 != "")
                {
                    var idNvaProf2 = AltaDeProfesion(p.usu_Id_Usuario, p.per_Id_Persona, pd.nvaProfesionOficio2);
                    p.pro_Id_Profesion_Oficio2 = idNvaProf2;
                }
                else
                {
                    p.pro_Id_Profesion_Oficio2 = pd.idOficio2;
                }

                // SE GRABA EL NUEVO OFICIO EN LA PERSONA DE INFO DE LA PERSONA
                context.Persona.Update(p);
                context.SaveChanges();

                // ALTA DE DOMICILIO
                HogarDomicilio hd = new HogarDomicilio();
                hd = pd.HogarDomicilioEntity; //Los Datos del Domicilio los pone en la variable 'hd'
                int idNvoEstado = 0;
                var estados = (from e in context.Estado     //Trae los Estados/Provincias del País que trae el 'domicilio'
                               where e.pais_Id_Pais == hd.pais_Id_Pais
                               select e).ToList();

                //Si el campo nvoEstado trae un nuevo Estado, lo agrega a la Tabla 'Estado' y envía el email de solicitud del Nuevo Estado a Soporte Técnico.
                if (pd.nvoEstado != "")
                {
                    var pais = context.Pais.FirstOrDefault(pais2 => pais2.pais_Id_Pais == hd.pais_Id_Pais);
                    var est = new Estado
                    {
                        est_Nombre_Corto = pd.nvoEstado.Substring(0, 3),
                        est_Nombre = pd.nvoEstado,
                        pais_Id_Pais = hd.pais_Id_Pais,
                        est_Pais = pais.pais_Nombre_Corto,
                    };
                    context.Estado.Add(est);
                    context.SaveChanges();
                    idNvoEstado = est.est_Id_Estado;

                    SendMailController sendMail = new SendMailController(context);
                    sendMail.EnviarSolicitudNvoEstado(pais.pais_Id_Pais, p.usu_Id_Usuario, p.per_Id_Persona, pd.nvoEstado);
                }

                //Graba el Nuevo Domicilio con el Estado/Provincia Seleccionado o con el Recien Creado
                hd.Fecha_Registro = fechayhora;
                hd.est_Id_Estado = pd.nvoEstado != "" ? idNvoEstado : hd.est_Id_Estado;
                hd.usu_Id_Usuario = p.usu_Id_Usuario;
                context.HogarDomicilio.Add(hd);
                context.SaveChanges();

                // ALTA DEL HOGAR-PERSONA
                Hogar_Persona hp = new Hogar_Persona();
                hp.hp_Jerarquia = 1;
                hp.per_Id_Persona = p.per_Id_Persona;
                hp.hd_Id_Hogar = hd.hd_Id_Hogar;
                hp.Fecha_Registro = fechayhora;
                hp.usu_Id_Usuario = p.usu_Id_Usuario;
                context.Hogar_Persona.Add(hp);
                context.SaveChanges();

                // REGISTRO HISTORICO DE LA ALTA DE LA PERSONA
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                int ct_Codigo_Transaccion = 0;
                DateTime? hte_Fecha_Transaccion = DateTime.Now;
                DateTime Fecha_Lanzamiento_App = new DateTime(2023, 6, 01);
                int idSector = 0;
                if (p.per_Bautizado) //Si la Alta es un Bautismo
                {
                    ct_Codigo_Transaccion = 11001;
                    hte_Fecha_Transaccion = p.per_Fecha_Bautismo;
                    //Selecciona el Sector en base a si Eligió un Sector ya registrado en BBDD y si a era fecha de Antes o Despues del Lanzamiento de la App.
                    idSector = (p.per_Fecha_Bautismo < Fecha_Lanzamiento_App && pd.idSectorBautismo != 0) ? pd.idSectorBautismo : p.sec_Id_Sector;
                }
                else //Si la Alta es un Nuevo Ingreso de un No Bautiado
                {
                    ct_Codigo_Transaccion = 12001;
                    idSector = p.sec_Id_Sector;
                    hte_Fecha_Transaccion = pd.FechaTransaccionHistorica == null ? p.per_Fecha_Nacimiento : pd.FechaTransaccionHistorica;
                }

                hte.RegistroHistorico(
                    p.per_Id_Persona,
                    idSector,
                    ct_Codigo_Transaccion,
                    "",
                    hte_Fecha_Transaccion,
                    p.usu_Id_Usuario
                );

                // REGISTRO HISTORICO DEL ALTA DEL HOGAR
                hte.RegistroHistorico(
                    p.per_Id_Persona,
                    p.sec_Id_Sector,
                    31001,
                    $"{p.per_Nombre} {p.per_Apellido_Paterno} {p.per_Apellido_Materno}",
                    hte_Fecha_Transaccion,
                    p.usu_Id_Usuario
                );

                // GENERA REGISTRO Y CORREO DE NUEVA PROFESION
                SolicitudNuevaProfesionController snpc = new SolicitudNuevaProfesionController(context);
                if (p.pro_Id_Profesion_Oficio1 == 1 && pd.nvaProfesionOficio1 != "")
                {
                    snpc.RegistroDeNvaSolicitud(p.usu_Id_Usuario, p.per_Id_Persona, pd.nvaProfesionOficio1);
                }
                if (p.pro_Id_Profesion_Oficio2 == 1 && pd.nvaProfesionOficio2 != "")
                {
                    snpc.RegistroDeNvaSolicitud(p.usu_Id_Usuario, p.per_Id_Persona, pd.nvaProfesionOficio2);
                }

                return Ok
                (
                    new
                    {
                        status = "success",
                        persona = p,
                        hogardomicilio = hd,
                        hogar_persona = hp
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
                        message = ex
                    }
                );
            }
        }

        // POST: api/Persona/RevinculaPersonaHogarExistente/{per_Id_Persona}/{hd_Id_Hogar}/{hp_Jerarquia}
        [Route("[action]/{per_Id_Persona}/{hd_Id_Hogar}/{hp_Jerarquia}/{usu_Id_Usuario}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult RevinculaPersonaHogarExistente(
            int per_Id_Persona,
            int hd_Id_Hogar,
            int hp_Jerarquia,
            int usu_Id_Usuario)
        {
            try
            {
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                // CONSULTA EL HOGAR AL QUE PERTENECE LA PERSONA
                var objhp = (from hp1 in context.Hogar_Persona
                             where hp1.per_Id_Persona == per_Id_Persona
                             select hp1).ToList();

                //Se guardan el Hogar Anterior en una Variable hdInicial
                int hdInicial = objhp[0].hd_Id_Hogar;

                // OBTIENE LOS MIEMBROS ACTIVOS DEL HOGAR ANTERIOR
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        join per in context.Persona on hp.per_Id_Persona equals per.per_Id_Persona
                                        where hp.hd_Id_Hogar == objhp[0].hd_Id_Hogar && per.per_Activo == true
                                        select new
                                        {
                                            per_Id_Persona = hp.per_Id_Persona,
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona
                                        }).ToList();

                // CONSULTA LOS DATOS DE LA PERSONA QUE SE ESTA REVINCULANDO
                var datosPersona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == per_Id_Persona);

                // Procede a Vincular LA PERSONA EN HOGAR EXISTENTE QUE ELIGIÓ
                objhp[0].hd_Id_Hogar = hd_Id_Hogar;
                objhp[0].hp_Jerarquia = hp_Jerarquia;
                objhp[0].usu_Id_Usuario = usu_Id_Usuario;
                objhp[0].Fecha_Registro = fechayhora;
                context.Hogar_Persona.Update(objhp[0]);
                context.SaveChanges();

                // RESTRUCTURA LAS JERARQUIAS EN EL DOMICILIO RECEPTOR
                RestructuraJerarquiasAlta(per_Id_Persona, hp_Jerarquia);

                // GENERA REGISTRO HISTORICO DE LA EDICION DE LA PERSONA
                int ct = datosPersona.per_Bautizado ? 11201 : 12201;
                hte.RegistroHistorico(per_Id_Persona, datosPersona.sec_Id_Sector, ct, "REVINCULACION A OTRO HOGAR", fechayhora, usu_Id_Usuario);

                // CUENTA LAS PERSONAS BAUTIZADAS EN EL HOGAR INICIAL
                int bautizados = 0;
                foreach (var p in miembrosDelHogar)
                {
                    var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p.per_Id_Persona);
                    if (persona.per_Bautizado)
                    {
                        bautizados = bautizados + 1;
                    }
                }

                if (bautizados == 1) //Si la persona era la última bautizada en el Hogar se debe de dar de Baja el Hogar y los NB que haya en él
                {
                    // OBTIENE LOS MIEMBROS DEL HOGAR Anterior
                    var miembrosDelHogar2 = (from hp1 in context.Hogar_Persona
                                             join per in context.Persona on hp1.per_Id_Persona equals per.per_Id_Persona
                                             where hp1.hd_Id_Hogar == hdInicial && per.per_Activo == true
                                             select new
                                             {
                                                 per_Id_Persona = hp1.per_Id_Persona,
                                                 hp_Id_Hogar_Persona = hp1.hp_Id_Hogar_Persona
                                             }).ToList();

                    foreach (var p in miembrosDelHogar2)
                    {
                        //// SE INACTIVAN LAS PERSONAS DEL DOMICILIO ANTERIOR PORQUE YA NO HAY PERSONAS BAUTIZADAS
                        //var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p.per_Id_Persona);
                        //persona.per_Activo = false;
                        //context.Persona.Update(persona);
                        //context.SaveChanges();

                        //// SE GENERA REGISTRO DE BAJA POR PADRES
                        //hte.RegistroHistorico(persona.per_Id_Persona, persona.sec_Id_Sector, 12106, "BAJA POR PADRES", fechayhora, usu_Id_Usuario);

                        // CONSULTA EL Registro HOGAR_PERSONA de cada NO Bautizado
                        var objhp2 = (from hp1 in context.Hogar_Persona
                                      where hp1.per_Id_Persona == p.per_Id_Persona
                                      select hp1).ToList();

                        // Vincula A LA PERSONA NO BAUTIZADA EN EL NUEVO DOMICILIO A DONDE SE REVINCULÓ AL PADRE?MADRE
                        objhp2[0].hd_Id_Hogar = hd_Id_Hogar;
                        objhp2[0].hp_Jerarquia = objhp2[0].hp_Jerarquia;
                        objhp2[0].usu_Id_Usuario = usu_Id_Usuario;
                        objhp2[0].Fecha_Registro = fechayhora;
                        context.Hogar_Persona.Update(objhp2[0]);
                        context.SaveChanges();

                        // GENERA REGISTRO HISTORICO DE LA EDICION DE CADA NO BAUTIZADO QUE SE REVINCULA
                        int ct2 = 12201;
                        hte.RegistroHistorico(p.per_Id_Persona, datosPersona.sec_Id_Sector, ct2, "REVINCULACION A OTRO HOGAR", fechayhora, usu_Id_Usuario);
                    }

                    // SE ESTABLECE LA BAJA DEL DOMICILIO ANTERIOR DEBIDO A QUE NO HAY PERSONAS BAUTIZADAS
                    var hd = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == hdInicial);
                    hd.hd_Activo = false;
                    context.HogarDomicilio.Update(hd);
                    context.SaveChanges();

                    // SE GENERA REGISTRO DE BAJA DE DOMICILIO 
                    hte.RegistroHistorico(
                        datosPersona.per_Id_Persona,
                        datosPersona.sec_Id_Sector,
                        31102,
                        $"{datosPersona.per_Nombre} {datosPersona.per_Apellido_Paterno} {datosPersona.per_Apellido_Materno}",
                        fechayhora,
                        usu_Id_Usuario
                    );
                }
                // RESTRUCTURA LAS JERARQUIAS EN EL DOMICILIO ANTERIOR
                AseguraJerarquias(hdInicial);

                // RESTRUCTURA LAS JERARQUIAS EN EL DOMICILIO NUEVO
                AseguraJerarquias(hd_Id_Hogar);

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

        //POST: api/Persona/RevinculaPersonaNvoHogar/{per_Id_Persona}/{usu_Id_Usuario}/{nvoEstado}
        [Route("[action]/{per_Id_Persona}/{usu_Id_Usuario}/{nvoEstado?}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult RevinculaPersonaNvoHogar([FromBody] HogarDomicilio hogarDomicilio, int per_Id_Persona, int usu_Id_Usuario, string nvoEstado = "")
        {
            try
            {
                int idNvoEstado = 0;
                var estados = (from e in context.Estado
                               where e.pais_Id_Pais == hogarDomicilio.pais_Id_Pais
                               select e).ToList();

                //Si el campo nvoEstado no viene vacío, sino que trae un nvoEstado, lo agrega a la Tabla 'Estado' y envía el email de solicitud del Nuevo Estado a Soporte Técnico.
                if (nvoEstado != "")
                {
                    var p = context.Pais.FirstOrDefault(pais => pais.pais_Id_Pais == hogarDomicilio.pais_Id_Pais);
                    var est = new Estado
                    {
                        est_Nombre_Corto = nvoEstado.Substring(0, 3),
                        est_Nombre = nvoEstado,
                        pais_Id_Pais = hogarDomicilio.pais_Id_Pais,
                        est_Pais = p.pais_Nombre_Corto
                    };
                    context.Estado.Add(est);
                    context.SaveChanges();
                    idNvoEstado = est.est_Id_Estado;

                    SendMailController sendMail = new SendMailController(context);
                    sendMail.EnviarSolicitudNvoEstado(p.pais_Id_Pais, usu_Id_Usuario, per_Id_Persona, nvoEstado);
                }

                // CONSULTA EL HOGAR_PERSONA ACTUAL AL QUE PERTENECE LA PERSONA
                var objhp = (from hp1 in context.Hogar_Persona
                             where hp1.per_Id_Persona == per_Id_Persona
                             select hp1).ToList();

                //Se guardan el Hogar Anterior
                int hdInicial = objhp[0].hd_Id_Hogar;

                // OBTIENE LOS MIEMBROS ACTIVOS DEL HOGAR CONSULTADO
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        join per in context.Persona on hp.per_Id_Persona equals per.per_Id_Persona
                                        where hp.hd_Id_Hogar == objhp[0].hd_Id_Hogar && per.per_Activo == true
                                        select new
                                        {
                                            per_Id_Persona = hp.per_Id_Persona,
                                            hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona
                                        }).ToList();

                // DATOS DE LA PERSONA QUE SE ESTA REVINCULANDO
                var datosPersona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == per_Id_Persona);

                // CONSULTA DATOS DEL DISTRITO
                var distrito = (from s in context.Sector
                                join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                where s.sec_Id_Sector == datosPersona.sec_Id_Sector
                                select d).ToList();

                // INSTANCIA UN OBJETO PARA REGISTROS HISTORICOS
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);

                // AGREGA DOMICILIO NUEVO
                var hd = new HogarDomicilio();
                hd.dis_Id_Distrito = distrito[0].dis_Id_Distrito;
                hd.est_Id_Estado = nvoEstado != "" ? idNvoEstado : hogarDomicilio.est_Id_Estado;
                hd.Fecha_Registro = fechayhora;
                hd.hd_Activo = true;
                hd.hd_Calle = hogarDomicilio.hd_Calle;
                hd.hd_Localidad = hogarDomicilio.hd_Localidad;
                hd.hd_Municipio_Ciudad = hogarDomicilio.hd_Municipio_Ciudad;
                hd.hd_Numero_Exterior = hogarDomicilio.hd_Numero_Exterior;
                hd.hd_Numero_Interior = hogarDomicilio.hd_Numero_Interior;
                hd.hd_Tipo_Subdivision = hogarDomicilio.hd_Tipo_Subdivision;
                hd.hd_Subdivision = hogarDomicilio.hd_Subdivision;
                hd.hd_Telefono = hogarDomicilio.hd_Telefono;
                hd.pais_Id_Pais = hogarDomicilio.pais_Id_Pais;
                hd.sec_Id_Sector = datosPersona.sec_Id_Sector;
                hd.hd_CP = hogarDomicilio.hd_CP;
                hd.usu_Id_Usuario = usu_Id_Usuario;
                context.HogarDomicilio.Add(hd);
                context.SaveChanges();

                // REGISTRO HISTORICO DEL NUEVO HOGAR
                hte.RegistroHistorico(
                    per_Id_Persona,
                    datosPersona.sec_Id_Sector,
                    31001,
                    (datosPersona.per_Nombre + " " + datosPersona.per_Apellido_Paterno + " " + datosPersona.per_Apellido_Materno),
                    fechayhora,
                    usu_Id_Usuario
                );

                // Vincula A LA PERSONA EN EL NUEVO DOMICILIO
                objhp[0].hd_Id_Hogar = hd.hd_Id_Hogar;
                objhp[0].hp_Jerarquia = 1;
                objhp[0].usu_Id_Usuario = usu_Id_Usuario;
                objhp[0].Fecha_Registro = fechayhora;
                context.Hogar_Persona.Update(objhp[0]);
                context.SaveChanges();

                // GENERA REGISTRO HISTORICO DE LA EDICION DE LA PERSONA
                int ct = datosPersona.per_Bautizado ? 11201 : 12201;
                hte.RegistroHistorico(per_Id_Persona, datosPersona.sec_Id_Sector, ct, "REVINCULACION A OTRO HOGAR", fechayhora, usu_Id_Usuario);

                // CUENTA LAS PERSONAS BAUTIZADAS del Hogar Anterior
                int bautizados = 0;
                foreach (var p in miembrosDelHogar)
                {
                    var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p.per_Id_Persona);
                    if (persona.per_Bautizado && persona.per_Vivo && persona.per_Activo && persona.per_En_Comunion)
                    {
                        bautizados = bautizados + 1;
                    }
                }

                if (bautizados == 1)
                { // LA PERSONA CREA UN NUEVO DOMICILIO PERO ES LA ULTIMA PERSONA BAUTIZADA EN EL DOMICILIO AL QUE PERTENECE

                    // OBTIENE LOS MIEMBROS DEL HOGAR que quedará solo
                    var miembrosDelHogar2 = (from hp1 in context.Hogar_Persona
                                             join per in context.Persona on hp1.per_Id_Persona equals per.per_Id_Persona
                                             where hp1.hd_Id_Hogar == hdInicial && per.per_Activo == true
                                             select new
                                             {
                                                 per_Id_Persona = hp1.per_Id_Persona,
                                                 hp_Id_Hogar_Persona = hp1.hp_Id_Hogar_Persona
                                             }).ToList();

                    foreach (var p in miembrosDelHogar2)
                    {
                        //// SE INACTIVAN LAS PERSONAS DEL DOMICILIO ANTERIOR PORQUE YA NO HAY PERSONAS BAUTIZADAS
                        //var persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == p.per_Id_Persona);
                        //persona.per_Activo = false;
                        //context.Persona.Update(persona);
                        //context.SaveChanges();

                        //// SE GENERA REGISTRO DE BAJA POR PADRES
                        //hte.RegistroHistorico(persona.per_Id_Persona, persona.sec_Id_Sector, 12106, "BAJA POR PADRES", fechayhora, usu_Id_Usuario);

                        // CONSULTA EL Registro HOGAR_PERSONA de cada NO Bautizado
                        var objhp2 = (from hp1 in context.Hogar_Persona
                                      where hp1.per_Id_Persona == p.per_Id_Persona
                                      select hp1).ToList();

                        // Vincula A LA PERSONA NO BAUTIZADA EN EL NUEVO DOMICILIO A DONDE SE REVINCULÓ AL PADRE?MADRE
                        objhp2[0].hd_Id_Hogar = hd.hd_Id_Hogar;
                        objhp2[0].hp_Jerarquia = objhp2[0].hp_Jerarquia;
                        objhp2[0].usu_Id_Usuario = usu_Id_Usuario;
                        objhp2[0].Fecha_Registro = fechayhora;
                        context.Hogar_Persona.Update(objhp2[0]);
                        context.SaveChanges();

                        // GENERA REGISTRO HISTORICO DE LA EDICION DE CADA NO BAUTIZADO QUE SE REVINCULA
                        int ct2 = 12201;
                        hte.RegistroHistorico(p.per_Id_Persona, datosPersona.sec_Id_Sector, ct2, "REVINCULACION A OTRO HOGAR", fechayhora, usu_Id_Usuario);

                    }

                    // SE ESTABLECE LA BAJA DEL DOMICILIO ANTERIOR DEBIDO A QUE NO HAY PERSONAS BAUTIZADAS
                    var hdx = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == hdInicial);
                    hdx.hd_Activo = false;
                    context.HogarDomicilio.Update(hdx);
                    context.SaveChanges();

                    // SE GENERA REGISTRO HISTORICO DE BAJA DE DOMICILIO
                    hte.RegistroHistorico(
                        datosPersona.per_Id_Persona,
                        datosPersona.sec_Id_Sector,
                        31102,
                        $"{datosPersona.per_Nombre} {datosPersona.per_Apellido_Paterno} {datosPersona.per_Apellido_Materno}",
                        fechayhora,
                        usu_Id_Usuario
                    );
                }

                // RESTRUCTURA LAS JERARQUIAS EN EL DOMICILIO ANTERIOR
                AseguraJerarquias(hdInicial);

                // RESTRUCTURA LAS JERARQUIAS EN EL DOMICILIO NUEVO
                AseguraJerarquias(hd.hd_Id_Hogar);

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

        // PUT: api/Persona/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] PersonaComentarioHTE objeto, int id)
        {
            try
            {
                var persona = objeto.PersonaEntity;
                // CONSULTA EL ESTADO DEL BAUTISMO DE LA PERSONA PARA SABER CONDICION PRINCIPAL
                var personaBD = context.Persona.FirstOrDefault(per => per.per_Id_Persona == persona.per_Id_Persona);
                bool registroBautismoBD = personaBD.per_Bautizado;
                context.Entry(personaBD).State = EntityState.Detached;
                DateTime Fecha_Lanzamiento_App = new DateTime(2023, 6, 01);
                int idSector = 0;

                //Consulta el Registro Historico del Bautismo, para que si la Edición altera Lugar o Fecha de Bautismo se actualicé tambien el Histórico
                var registroHistoricoDeBautismo = context.Historial_Transacciones_Estadisticas.FirstOrDefault(rh => rh.per_Persona_Id == persona.per_Id_Persona && rh.ct_Codigo_Transaccion == 11001);
                
                if (objeto.idSectorBautismo != 0) //Si se eligió un Sector Registrado
                {
                    idSector = persona.per_Fecha_Bautismo < Fecha_Lanzamiento_App ? objeto.idSectorBautismo : persona.sec_Id_Sector;
                }
                else //Si se escribió Texto libre en el input de Lugar de Bautismo
                {
                    idSector = persona.sec_Id_Sector;
                } 

                // ACTUALIZA LUGAR y/o FECHA DE BAUTISMO EN EL REGISTRO HISTORICO CORRESPONDIENTE
                // SOLO SI ES ACTUALIZACIÓN, Y EN LA EDICIÓN CAMBIO EL SECTOR O LA FECHA DE BAUTISMO
                if (registroBautismoBD && ((idSector != registroHistoricoDeBautismo.sec_Sector_Id) || (personaBD.per_Fecha_Bautismo != persona.per_Fecha_Bautismo)))
                {
                    var SectorDistrito = (from s in context.Sector
                                          join d in context.Distrito
                                          on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                          where s.sec_Id_Sector == idSector
                                          select new
                                          {
                                              sec_Id_Sector = s.sec_Id_Sector,
                                              sec_Alias = s.sec_Alias,
                                              dis_Id_Distrito = s.dis_Id_Distrito,
                                              dis_Alias = d.dis_Alias
                                          }).ToList();

                    registroHistoricoDeBautismo.dis_Distrito_Id = SectorDistrito[0].dis_Id_Distrito;
                    registroHistoricoDeBautismo.dis_Distrito_Alias = SectorDistrito[0].dis_Alias;
                    registroHistoricoDeBautismo.sec_Sector_Id = SectorDistrito[0].sec_Id_Sector;
                    registroHistoricoDeBautismo.sec_Sector_Alias = SectorDistrito[0].sec_Alias;
                    registroHistoricoDeBautismo.hte_Fecha_Transaccion = personaBD.per_Fecha_Bautismo != persona.per_Fecha_Bautismo? persona.per_Fecha_Bautismo: personaBD.per_Fecha_Bautismo;
                    context.Historial_Transacciones_Estadisticas.Update(registroHistoricoDeBautismo);
                    context.SaveChanges();
                }

                //Si cambia la Fecha de Bautismo

                // ALTA DE NUEVAS PROFESIONES
                if (objeto.idOficio1 == 1 && objeto.nvaProfesionOficio1 != "")
                {
                    var idNvaProf1 = AltaDeProfesion(persona.usu_Id_Usuario, personaBD.per_Id_Persona, objeto.nvaProfesionOficio1);
                    persona.pro_Id_Profesion_Oficio1 = idNvaProf1;
                }
                else
                {
                    persona.pro_Id_Profesion_Oficio1 = objeto.idOficio1;
                }


                if (objeto.idOficio2 == 1 && objeto.nvaProfesionOficio2 != "")
                {
                    var idNvaProf2 = AltaDeProfesion(persona.usu_Id_Usuario, personaBD.per_Id_Persona, objeto.nvaProfesionOficio2);
                    persona.pro_Id_Profesion_Oficio2 = idNvaProf2;
                }
                else
                {
                    persona.pro_Id_Profesion_Oficio2 = objeto.idOficio2;
                }

                // NUEVA INSTANCIA DEL CONTROLADOR DE Historial de transacciones estadisticas
                Historial_Transacciones_EstadisticasController hte = new Historial_Transacciones_EstadisticasController(context);
                int ct_Codigo_Transaccion = 0;
                DateTime? hte_Fecha_Transaccion = DateTime.Now;
                persona.Fecha_Registro = fechayhora;

                // CONDICION PRINCIPAL PARA PASAR A PERSONAL BAUTIZADO 
                if (persona.per_Bautizado != registroBautismoBD)
                {
                    persona.per_Bautizado = true;
                    persona.per_En_Comunion = true;
                    hte_Fecha_Transaccion = persona.per_Fecha_Bautismo;
                    // REGISTRO HISTORICO BAJA POR CAMBIO DE NO BAUTIZADO A BAUTIZADO
                    hte.RegistroHistorico(
                        persona.per_Id_Persona, 
                        persona.sec_Id_Sector, 
                        12105, 
                        objeto.ComentarioHTE, 
                        persona.per_Fecha_Bautismo,
                        persona.usu_Id_Usuario);
                    // REGISTRO HISTORICO ALTA POR CAMBIO DE NO BAUTIZADO A BAUTIZADO
                    hte.RegistroHistorico(
                        persona.per_Id_Persona, 
                        persona.sec_Id_Sector, 
                        11001, 
                        objeto.ComentarioHTE,
                        persona.per_Fecha_Bautismo, 
                        persona.usu_Id_Usuario);
                }
                else
                {
                    ct_Codigo_Transaccion = persona.per_Bautizado ? 11201 : 12201;
                    // REGISTRO HISTORIO SIMPLE DE ACTUALIZACIÓN DE DATOS PERSONALES
                    hte.RegistroHistorico(
                        persona.per_Id_Persona, 
                        persona.sec_Id_Sector, 
                        ct_Codigo_Transaccion, 
                        objeto.ComentarioHTE, 
                        hte_Fecha_Transaccion, 
                        persona.usu_Id_Usuario);
                }

                // MODIFICACION DE INFO DE LA PERSONA
                context.Entry(persona).State = EntityState.Modified;
                context.SaveChanges();

                return Ok(
                    new
                    {
                        status = "success",
                        persona
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

        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult AgregarFoto([FromForm]IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    // RECOLECTA CARACTERISTICAS DE LA IMAGEN
                    Foto foto = new Foto
                    {
                        guid = Guid.NewGuid().ToString(),
                        extension = Path.GetExtension(image.FileName),
                        mimeType = image.ContentType,
                        size = int.Parse(image.Length.ToString()),
                        path = "c:\\DoctosCompartidos\\FotosPersonal\\" // define donde guardar la imagen
                    };

                    // DEFINE EL NOMBRE DEL ARCHIVO PARA GUARDAR LA IMAGEN
                    string ImageName = foto.guid + foto.extension;

                    // GUARDAR IMAGEN EN DISCO
                    //string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", ImageName);
                    string SavePath = Path.Combine(foto.path + foto.guid + foto.extension);
                    //using (var stream = new FileStream(SavePath, FileMode.Create))
                    //{
                    //    image.CopyTo(stream);
                    //}

                    using (MagickImage oMagickImage = new MagickImage(image.OpenReadStream()))
                    {
                        if (oMagickImage.Width >= oMagickImage.Height)
                        {
                            oMagickImage.Resize(120, 0);
                        }
                        else
                        {
                            oMagickImage.Resize(0, 120);
                        }
                        oMagickImage.Write(SavePath);
                    }

                    // AGREGA REGISTRO A LA BASE DE DATOS
                    context.Foto.Add(foto);
                    context.SaveChanges();

                    return Ok(new
                    {
                        status = "success",
                        foto = foto
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "No se cargo niguna imagen"
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

        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult CambiarClavePersona()
        {
            try
            {
                var personas = context.Persona.ToList();
                int i = 0;
                string pattern = @"[^AEIOU]";
                Regex rg = new Regex(pattern);
                foreach (var per in personas)
                {
                    //char[] arrAP = per.per_Apellido_Paterno.ToCharArray();
                    //char[] arrAPV = rg.Replace(per.per_Apellido_Paterno, "").ToCharArray();
                    //string ap = arrAP[0] == arrAPV[0] ? arrAP[0].ToString() + arrAPV[1].ToString() : arrAP[0].ToString() + arrAPV[0].ToString();
                    string ap = per.per_Apellido_Paterno.Substring(0, 2);
                    string n = per.per_Nombre.Substring(0, 2);
                    string d = per.per_Fecha_Nacimiento.Day < 10 ? "0" + per.per_Fecha_Nacimiento.Day.ToString() : per.per_Fecha_Nacimiento.Day.ToString();
                    string m = per.per_Fecha_Nacimiento.Month < 10 ? "0" + per.per_Fecha_Nacimiento.Month.ToString() : per.per_Fecha_Nacimiento.Month.ToString();
                    string a = per.per_Fecha_Nacimiento.Year.ToString();
                    string genero = per.per_Categoria == "ADULTO_HOMBRE" || per.per_Categoria == "NIÑO" || per.per_Categoria == "JOVEN_HOMBRE" ? "M" : "F";
                    string nvaClave = ap + n + genero + d + m + a;

                    per.per_RFC_Sin_Homo = nvaClave;
                    context.Persona.Update(per);
                    context.SaveChanges();
                    context.Entry(per).State = EntityState.Detached;
                }
                return Ok(new
                {
                    status = "success",
                });
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
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
                var personas = (from p in context.Persona select p).ToList();

                // A cada persona le los acentos en nombre y apellidos y
                // guarda el nombre completo en el campo per_Nombre_Completo
                foreach(var p  in personas)
                {
                    var nombre = ManejoDeApostrofes.QuitarApostrofe2(p.per_Nombre);
                    var apellidoPaterno = ManejoDeApostrofes.QuitarApostrofe2(p.per_Apellido_Paterno);
                    var apellidoMaterno = p.per_Apellido_Materno != null ? ManejoDeApostrofes.QuitarApostrofe2(p.per_Apellido_Materno) : "";
                    var apellidoCasada = p.per_Apellido_Casada != null && p.per_Apellido_Casada != "" ? ManejoDeApostrofes.QuitarApostrofe2(p.per_Apellido_Casada) + "*" : "";
                    var apellidoPrincipal = (apellidoCasada != null && apellidoCasada != "") ? (apellidoCasada + " " + apellidoPaterno) : apellidoPaterno;
                    // Genera nombre completo
                    p.per_Nombre_Completo = $"{nombre} {apellidoPrincipal} {apellidoMaterno} ";

                    // Guarda cambios
                    context.Persona.Update(p);
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

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Delete(int id)
        {
            List<string> message = new List<string>();

            // CONSULTA HOGAR A PARTIR DEL ID DE LA PERSONA
            Hogar_Persona hogar_persona = new Hogar_Persona();
            hogar_persona = context.Hogar_Persona.FirstOrDefault(hp => hp.per_Id_Persona == id);

            // CONSULTA LOS MIEMBROS DEL HOGAR AL QUE PERTENESE LA PERSONA
            var miembrosDelHogar = (from hp in context.Hogar_Persona
                                    join p in context.Persona
                                    on hp.per_Id_Persona equals p.per_Id_Persona
                                    where hp.hd_Id_Hogar == hogar_persona.hd_Id_Hogar
                                    orderby hp.hp_Jerarquia
                                    select new
                                    {
                                        hp_Id_Hogar_Persona = hp.hp_Id_Hogar_Persona,
                                        hd_Id_Hogar = hp.hd_Id_Hogar,
                                        per_Id_Persona = hp.per_Id_Persona,
                                        hp_Jerarquia = hp.hp_Jerarquia
                                    }).ToList();
            for (int i = miembrosDelHogar.Count(); i > hogar_persona.hp_Jerarquia; i--)
            {
                // ACTUALIZA LA JERARQUIA DE LOS MIEMBROS RESTANTES DENTRO DEL HOGAR
                if (miembrosDelHogar[i - 1].hp_Jerarquia > hogar_persona.hp_Jerarquia)
                {
                    var registro = new Hogar_Persona
                    {
                        hp_Id_Hogar_Persona = miembrosDelHogar[i - 1].hp_Id_Hogar_Persona,
                        hd_Id_Hogar = miembrosDelHogar[i - 1].hd_Id_Hogar,
                        per_Id_Persona = miembrosDelHogar[i - 1].per_Id_Persona,
                        hp_Jerarquia = i - 1,
                        Fecha_Registro = fechayhora,
                        usu_Id_Usuario = 1
                    };
                    context.Entry(registro).State = EntityState.Modified;
                    context.SaveChanges();
                }

            }

            // ELIMINA REGISTRO DE LA PERSONA EN LA TABLA HOGAR_PERSONA
            if (hogar_persona != null)
            {
                context.Hogar_Persona.Remove(hogar_persona);
                context.SaveChanges();
                message.Add("Se elimino la jerarquia que correspondia a la persona en el hogar.");
            }

            // ELIMINA EL DOMICILIO SI LA PERSONA QUE SE ELIMINA ES LA ULTIMA DEL HOGAR
            if (miembrosDelHogar.Count() == 1)
            {
                HogarDomicilio hd = new HogarDomicilio();
                hd = context.HogarDomicilio.FirstOrDefault(d => d.hd_Id_Hogar == miembrosDelHogar[0].hd_Id_Hogar);
                if (hd != null)
                {
                    context.HogarDomicilio.Remove(hd);
                    context.SaveChanges();
                    message.Add("Se eliminó el registro del domicilio por ser la utima persona registado en el mismo.");
                }
            }

            // ELIMINA LA PERSONA POR SU ID Y RETORNA EL RESULTADO
            Persona persona = new Persona();
            persona = context.Persona.FirstOrDefault(per => per.per_Id_Persona == id);
            if (persona != null)
            {
                context.Persona.Remove(persona);
                context.SaveChanges();
                message.Add("Se eliminó el registro de la persona con todos sus datos.");
                return Ok(
                        new
                        {
                            status = "success",
                            message = message.ToArray()
                        }
                    );
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
