using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.ClasesGenerales;
using IECE_WebApi.Contexts;
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

        // METODO PARA ALTA DE REGISTRO HISTORICO
        private IActionResult RegistroHistorico(
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
                nvoRegistro.Usu_Usuario_Id = 1;
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
                             per_Nombre = p.per_Nombre,
                             per_Apellido_Paterno = p.per_Apellido_Paterno,
                             per_ApellidoMaterno = p.per_Apellido_Materno,
                             per_Fecha_Nacimiento = p.per_Fecha_Nacimiento,
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
            List<PersonaDomicilioMiembros> query = new List<PersonaDomicilioMiembros>();
            var query1 = (from p in context.Persona
                          join s in context.Sector
                          on p.sec_Id_Sector equals s.sec_Id_Sector
                          where p.sec_Id_Sector == sec_Id_Sector
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
                              p.per_foto,
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
                                  hd_Id_Hogar = hp.hd_Id_Hogar,
                                  per_Id_Persona = p.per_Id_Persona,
                                  per_Nombre = p.per_Nombre,
                                  per_Apellido_Paterno = p.per_Apellido_Paterno,
                                  per_Apellido_Materno = p.per_Apellido_Materno,
                                  hd_Calle = hd.hd_Calle,
                                  hd_Numero_Exterior = hd.hd_Numero_Exterior,
                                  hd_Numero_Interior = hd.hd_Numero_Interior,
                                  hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                                  hd_Subdivision = hd.hd_Subdivision,
                                  hd_Localidad = hd.hd_Localidad,
                                  hd_Municipio_Ciudad = hd.hd_Municipio_Ciudad,
                                  est_Nombre = e.est_Nombre,
                                  pais_Nombre_Corto = pais.pais_Nombre_Corto,
                                  hd_Telefono = hd.hd_Telefono
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
                                  per_Apellido_Materno = p.per_Apellido_Materno
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

        // GET: api/Persona/GetBautizadosBySector/sec_Id_Sector
        [Route("[action]/{sec_Id_Sector}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetBautizadosBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == true
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
                             && !(from hte in context.Historial_Transacciones_Estadisticas
                                  where hte.ct_Codigo_Transaccion == 11103 
                                  || hte.ct_Codigo_Transaccion == 11102
                                  select hte.per_Persona_Id).Contains(p.per_Id_Persona)
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
                             && !(from hte in context.Historial_Transacciones_Estadisticas
                                  where hte.ct_Codigo_Transaccion == 11101
                                  || (hte.ct_Codigo_Transaccion == 11102 || hte.ct_Codigo_Transaccion == 11103)
                                  select hte.per_Persona_Id).Contains(p.per_Id_Persona)
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

       // GET: api/Persona/GetNoBautizadosDefuncionAlejamientoBySector/sec_Id_Sector
       [Route("[action]/{sec_Id_Sector}")]
       [HttpGet]
       [EnableCors("AllowOrigin")]
        public IActionResult GetNoBautizadosDefuncionAlejamientoBySector(int sec_Id_Sector)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector
                             on p.sec_Id_Sector equals s.sec_Id_Sector
                             where p.sec_Id_Sector == sec_Id_Sector
                             && p.per_Bautizado == false
                             && p.per_Vivo == true
                             && !(from hte in context.Historial_Transacciones_Estadisticas
                                  where hte.ct_Codigo_Transaccion == 12101 || hte.ct_Codigo_Transaccion == 12102
                                  select hte.per_Persona_Id).Contains(p.per_Id_Persona)
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
                                  p.per_foto,
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
                                      hd_Id_Hogar = hp.hd_Id_Hogar,
                                      per_Id_Persona = p.per_Id_Persona,
                                      per_Nombre = p.per_Nombre,
                                      per_Apellido_Paterno = p.per_Apellido_Paterno,
                                      per_Apellido_Materno = p.per_Apellido_Materno,
                                      hd_Calle = hd.hd_Calle,
                                      hd_Numero_Exterior = hd.hd_Numero_Exterior,
                                      hd_Numero_Interior = hd.hd_Numero_Interior,
                                      hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                                      hd_Subdivision = hd.hd_Subdivision,
                                      hd_Localidad = hd.hd_Localidad,
                                      hd_Municipio_Ciudad = hd.hd_Municipio_Ciudad,
                                      est_Nombre = e.est_Nombre,
                                      pais_Nombre_Corto = pais.pais_Nombre_Corto,
                                      hd_Telefono = hd.hd_Telefono
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
                                      per_Apellido_Materno = p.per_Apellido_Materno
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
                                 where p.sec_Id_Sector == sec_Id_Sector
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
                             && (p.per_Bautizado == bautizado && p.per_Activo)
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
        [Route("[action]/{per_Id_Persona}/{tipoExcomunion}/{delitoExomunion}/{fechaExcomunion}")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaBautizadoExcomunion(
            int per_Id_Persona, 
            int tipoExcomunion, 
            string delitoExomunion, 
            DateTime fechaExcomunion)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where p.per_Id_Persona == per_Id_Persona
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Alias,
                                 s.sec_Id_Sector
                             }).ToList();
                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                hte.ct_Codigo_Transaccion = tipoExcomunion;
                hte.dis_Distrito_Alias = query[0].dis_Alias;
                hte.dis_Distrito_Id = query[0].dis_Id_Distrito;
                hte.hte_Cancelado = false;
                hte.hte_Comentario = delitoExomunion;
                hte.hte_Fecha_Transaccion = fechaExcomunion;
                hte.per_Persona_Id = per_Id_Persona;
                hte.sec_Sector_Alias = query[0].sec_Alias;
                hte.sec_Sector_Id = query[0].sec_Id_Sector;
                hte.Usu_Usuario_Id = 1;

                var query2 = (from p in context.Persona
                              where p.per_Id_Persona == per_Id_Persona
                              select p).FirstOrDefault();
                query2.per_En_Comunion = false;
                context.SaveChanges();

                context.Historial_Transacciones_Estadisticas.Add(hte);
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

        // POST: api/Persona/BajaBautizadoDefuncion/per_Id_Persona
        [HttpPost]
        [Route("[action]/{per_Id_Persona}/{comentarioDefuncion}/{fechaDefuncion}")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaBautizadoDefuncion(
            int per_Id_Persona,
            string comentarioDefuncion,
            DateTime fechaDefuncion)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where p.per_Id_Persona == per_Id_Persona
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Alias,
                                 s.sec_Id_Sector
                             }).ToList();
                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                hte.ct_Codigo_Transaccion = 11101;
                hte.dis_Distrito_Alias = query[0].dis_Alias;
                hte.dis_Distrito_Id = query[0].dis_Id_Distrito;
                hte.hte_Cancelado = false;
                hte.hte_Comentario = comentarioDefuncion;
                hte.hte_Fecha_Transaccion = fechaDefuncion;
                hte.per_Persona_Id = per_Id_Persona;
                hte.sec_Sector_Alias = query[0].sec_Alias;
                hte.sec_Sector_Id = query[0].sec_Id_Sector;
                hte.Usu_Usuario_Id = 1;

                context.Historial_Transacciones_Estadisticas.Add(hte);
                context.SaveChanges();

                var query2 = (from p in context.Persona
                              where p.per_Id_Persona == per_Id_Persona
                              select p).FirstOrDefault();
                query2.per_Vivo = false;
                query2.per_Activo = false;
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

        // POST: api/Persona/BajaNoBautizadoDefuncionAlejamiento/per_Id_Persona
        [HttpPost]
        [Route("[action]/{per_Id_Persona}/{comentario}/{codigoTransaccion}/{fechaTransaccion}")]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaNoBautizadoDefuncionAlejamiento(
            int per_Id_Persona,
            string comentario,
            int codigoTransaccion,
            DateTime fechaTransaccion)
        {
            try
            {
                var query = (from p in context.Persona
                             join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                             join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where p.per_Id_Persona == per_Id_Persona
                             select new
                             {
                                 d.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Alias,
                                 s.sec_Id_Sector
                             }).ToList();
                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                hte.ct_Codigo_Transaccion = codigoTransaccion;
                hte.dis_Distrito_Alias = query[0].dis_Alias;
                hte.dis_Distrito_Id = query[0].dis_Id_Distrito;
                hte.hte_Cancelado = false;
                hte.hte_Comentario = comentario;
                hte.hte_Fecha_Transaccion = fechaTransaccion;
                hte.per_Persona_Id = per_Id_Persona;
                hte.sec_Sector_Alias = query[0].sec_Alias;
                hte.sec_Sector_Id = query[0].sec_Id_Sector;
                hte.Usu_Usuario_Id = 1;

                context.Historial_Transacciones_Estadisticas.Add(hte);
                context.SaveChanges();

                var query2 = (from p in context.Persona
                              where p.per_Id_Persona == per_Id_Persona
                              select p).FirstOrDefault();
                query2.per_Vivo = codigoTransaccion == 12101 ? false : true;
                query2.per_Activo = false;
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

        // POST: api/Persona/AddPersonaHogar
        [Route("[action]/{jerarquia}/{hdId}")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaHogar([FromBody] Persona persona, int jerarquia, int hdId)
        {
            try
            {
                persona.Fecha_Registro = fechayhora;
                persona.usu_Id_Usuario = 1;
                context.Persona.Add(persona);
                context.SaveChanges();

                Hogar_Persona hpModel = new Hogar_Persona();
                var miembrosDelHogar = (from hp in context.Hogar_Persona
                                        where hp.hd_Id_Hogar == hdId
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
                    if (miembro.hp_Jerarquia == jerarquia)
                    {
                        hpModel.per_Id_Persona = persona.per_Id_Persona;
                        hpModel.hp_Jerarquia = jerarquia;
                        hpModel.hd_Id_Hogar = hdId;
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
                    if (miembro.hp_Jerarquia > jerarquia)
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

                if (jerarquia > miembrosDelHogar.Count())
                {
                    hpModel.per_Id_Persona = persona.per_Id_Persona;
                    hpModel.hp_Jerarquia = jerarquia;
                    hpModel.hd_Id_Hogar = hdId;
                    hpModel.Fecha_Registro = fechayhora;
                    hpModel.usu_Id_Usuario = 1;
                    context.Hogar_Persona.Add(hpModel);
                    context.SaveChanges();
                }

                return Ok
                (
                    new
                    {
                        status = "success",
                        persona = persona,
                        hogar_persona = hpModel
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

        // POST: api/Persona/AddPersonaDomicilioHogar
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult AddPersonaDomicilioHogar([FromBody] PersonaDomicilio pd)
        {
            try
            {
                Persona p = new Persona();
                p = pd.PersonaEntity;
                p.usu_Id_Usuario = 1;
                p.Fecha_Registro = fechayhora;
                context.Persona.Add(p);
                context.SaveChanges();

                HogarDomicilio hd = new HogarDomicilio();
                hd = pd.HogarDomicilioEntity;
                hd.usu_Id_Usuario = 1;
                hd.Fecha_Registro = fechayhora;
                context.HogarDomicilio.Add(hd);
                context.SaveChanges();

                Hogar_Persona hp = new Hogar_Persona();
                hp.hp_Jerarquia = 1;
                hp.per_Id_Persona = p.per_Id_Persona;
                hp.hd_Id_Hogar = hd.hd_Id_Hogar;
                hp.Fecha_Registro = fechayhora;
                hp.usu_Id_Usuario = 1;
                context.Hogar_Persona.Add(hp);
                context.SaveChanges();

                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                int ct_Codigo_Transaccion = 0;
                DateTime hte_Fecha_Transaccion = DateTime.Now;
                if (p.per_Bautizado)
                {
                    ct_Codigo_Transaccion = 11201;
                    hte_Fecha_Transaccion = p.per_Fecha_Bautismo;
                }
                else
                {
                    ct_Codigo_Transaccion = 12201;
                    hte_Fecha_Transaccion = fechayhora;
                }
                RegistroHistorico(p.per_Id_Persona, p.sec_Id_Sector, ct_Codigo_Transaccion, "", hte_Fecha_Transaccion, p.usu_Id_Usuario);

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
                        message = ex.Message
                    }
                );
            }
        }

        // PUT: api/Persona/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put([FromBody] PersonaComentarioHTE objeto, int id)
        {
            try
            {
                Persona persona = new Persona();
                persona = objeto.PersonaEntity;
                persona.Fecha_Registro = fechayhora;
                persona.usu_Id_Usuario = 1;
                Historial_Transacciones_Estadisticas hte = new Historial_Transacciones_Estadisticas();
                int ct_Codigo_Transaccion = persona.per_Bautizado ? 11201 : 12201;
                DateTime hte_Fecha_Transaccion = DateTime.Now;
                RegistroHistorico(persona.per_Id_Persona, persona.sec_Id_Sector, ct_Codigo_Transaccion, objeto.ComentarioHTE, hte_Fecha_Transaccion, persona.usu_Id_Usuario);

                // MODIFICACION DE REGISTRO DE PERSONA
                context.Entry(persona).State = EntityState.Modified;
                context.SaveChanges();
                return Ok(
                    new
                    {
                        status = "success",
                        mensaje = "Datos guardados satisfactoriamente."
                    });
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = ex
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
