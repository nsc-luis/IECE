using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IECE_WebApi.Helpers;

using DocumentFormat.OpenXml.Drawing.Diagrams;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InformeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InformeController(AppDbContext context)
        {
            _context = context;
        }
        public class InformeSearchModel
        {
            public int IdTipoUsuario { get; set; }
        }
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Informe>> Get([FromQuery] InformeSearchModel sm)
        {
            try
            {
                var meses = new Dictionary<int, string>
                {
                    {0, "Desconocido"},
                    {1, "Enero"},
                    {2, "Febrero"},
                    {3, "Marzo"},
                    {4, "Abril"},
                    {5, "Mayo"},
                    {6, "Junio"},
                    {7, "Julio"},
                    {8, "Agosto"},
                    {9, "Septiembre"},
                    {10, "Octubre"},
                    {11, "Noviembre"},
                    {12, "Diciembre"}
                };
                return Ok(_context.Informe
                    .Where(w => w.IdTipoUsuario == sm.IdTipoUsuario)
                    .Select(s => new
                    {
                        IdInforme = s.IdInforme,
                        Anio = s.Anio,
                        Mes = meses[s.Mes],
                    }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<InformeAnualPastorController>/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult<Informe> Get(int id)
        {
            try
            {
                InformePastorViewModel informeVM = new InformePastorViewModel();
                var meses = new Dictionary<int, string>
                {
                    {0, "Desconocido"},
                    {1, "Enero"},
                    {2, "Febrero"},
                    {3, "Marzo"},
                    {4, "Abril"},
                    {5, "Mayo"},
                    {6, "Junio"},
                    {7, "Julio"},
                    {8, "Agosto"},
                    {9, "Septiembre"},
                    {10, "Octubre"},
                    {11, "Noviembre"},
                    {12, "Diciembre"}
                };
                Informe informe = _context.Informe
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if(informe == null)
                {
                    return NotFound("El informe no se encontró");
                }

                informeVM.IdInforme = informe.IdInforme;
                informeVM.IdTipoUsuario = informe.IdTipoUsuario;
                informeVM.IdDistrito = informe.IdDistrito;
                informeVM.IdSector = informe.IdSector;
                informeVM.LugarReunion = informe.LugarReunion;
                informeVM.FechaReunion = informe.FechaReunion;
                informeVM.Status = informe.Status;
                informeVM.Usu_Id_Usuario = informe.Usu_Id_Usuario;
                informeVM.FechaRegistro = informe.FechaRegistro;
                informeVM.Mes = informe.Mes;
                informeVM.NombreMes = meses[informe.Mes];
                informeVM.Anio = informe.Anio;

                VisitasPastor visitasPastor = _context.VisitasPastor
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if(visitasPastor != null)
                {
                    informeVM.VisitasPastor = visitasPastor;
                }

                CultosSector cultosSector = _context.CultosSector
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (cultosSector != null)
                {
                    informeVM.CultosSector = cultosSector;
                }

                EstudiosSector estudiosSector = _context.EstudiosSector
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoEstudio == 1)
                    .FirstOrDefault();

                if (estudiosSector != null)
                {
                    informeVM.EstudiosSector = estudiosSector;
                }

                EstudiosSector conferenciasSector = _context.EstudiosSector
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoEstudio == 2)
                    .FirstOrDefault();

                if (conferenciasSector != null)
                {
                    informeVM.ConferenciasSector = conferenciasSector;
                }


                TrabajoEvangelismo trabajoEvangelismo = _context.TrabajoEvangelismo
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (trabajoEvangelismo != null)
                {
                    informeVM.TrabajoEvangelismo = trabajoEvangelismo;
                }

                List<CultosMisionSector> cultosMisionSector = _context.CultosMisionSector
                    .Where(w => w.IdInforme == id)
                    .ToList();

                if (cultosMisionSector != null)
                {
                    informeVM.CultosMisionSector = cultosMisionSector;
                }

                Organizaciones organizaciones = _context.Organizaciones
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (organizaciones != null)
                {
                    informeVM.Organizaciones = organizaciones;
                }

                AdquisicionesSector adquisicionesSector = _context.AdquisicionesSector
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (adquisicionesSector != null)
                {
                    informeVM.AdquisicionesSector = adquisicionesSector;
                }

                SesionesReunionesSector reunionesSector = _context.SesionesReunionesSector
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoSesionReunion == 1)
                    .FirstOrDefault();

                if (reunionesSector != null)
                {
                    informeVM.Reuniones = reunionesSector;
                }

                SesionesReunionesSector sesionesSector = _context.SesionesReunionesSector
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoSesionReunion == 2)
                    .FirstOrDefault();

                if (sesionesSector != null)
                {
                    informeVM.Sesiones = sesionesSector;
                }

                Construcciones construccionesSectorInicio = _context.Construcciones
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoFaseConstruccion == 1)
                    .FirstOrDefault();

                if (construccionesSectorInicio != null)
                {
                    informeVM.ConstruccionesInicio = construccionesSectorInicio;
                }

                Construcciones construccionesSectorConclusion = _context.Construcciones
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoFaseConstruccion == 2)
                    .FirstOrDefault();

                if (construccionesSectorConclusion != null)
                {
                    informeVM.ConstruccionesConclusion = construccionesSectorConclusion;
                }

                Ordenaciones ordenaciones = _context.Ordenaciones
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (ordenaciones != null)
                {
                    informeVM.Ordenaciones = ordenaciones;
                }

                Dedicaciones dedicaciones = _context.Dedicaciones
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (dedicaciones != null)
                {
                    informeVM.Dedicaciones = dedicaciones;
                }

                LlamamientoDePersonal llamamientoDePersonal = _context.LlamamientoDePersonal
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (llamamientoDePersonal != null)
                {
                    informeVM.LlamamientoDePersonal = llamamientoDePersonal;
                }

                RegularizacionPrediosTemplos regularizacionPatNac = _context.RegularizacionPrediosTemplos
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoPatrimonio == 1)
                    .FirstOrDefault();

                if (regularizacionPatNac != null)
                {
                    informeVM.RegularizacionPatNac = regularizacionPatNac;
                }

                RegularizacionPrediosTemplos regularizacionPatIg = _context.RegularizacionPrediosTemplos
                    .Where(w => w.IdInforme == id)
                    .Where(w => w.IdTipoPatrimonio == 2)
                    .FirstOrDefault();

                if (regularizacionPatIg != null)
                {
                    informeVM.RegularizacionPatIg = regularizacionPatIg;
                }

                MovimientoEconomico movimientoEconomico = _context.MovimientoEconomico
                    .Where(w => w.IdInforme == id)
                    .FirstOrDefault();

                if (movimientoEconomico != null)
                {
                    informeVM.MovimientoEconomico = movimientoEconomico;
                }

                List<OtrasActividades> otrasActividades = _context.OtrasActividades
                    .Where(w => w.IdInforme == id)
                    .ToList();

                if (otrasActividades != null)
                {
                    informeVM.OtrasActividades = otrasActividades;
                }

                return Ok(informeVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<InformeAnualPastorController>
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Informe data)
        {
            try
            {

                List<Informe> informes = _context.Informe
                    .Where(s => s.IdTipoUsuario == data.IdTipoUsuario)
                    .Where(s => s.Anio == data.Anio)
                    .ToList();

                bool informeExiste = informes.Any(a => a.Mes == data.Mes);
                if (informeExiste)
                {

                    return Ok(new
                    {
                        status = "error",
                        message = $"Ya existe un informe para el mes {data.Mes}"
                    });

                }

                Informe informe = new Informe
                {
                    IdTipoUsuario = data.IdTipoUsuario,
                    Mes = data.Mes,
                    Anio = data.Anio,
                    IdDistrito = data.IdDistrito,
                    IdSector = data.IdSector,
                    LugarReunion = data.LugarReunion,
                    FechaReunion = data.FechaReunion,
                    Status = data.Status,
                    Usu_Id_Usuario = data.Usu_Id_Usuario,
                    FechaRegistro = DateTime.Now
                };
                _context.Informe.Add(informe);
                _context.SaveChanges();
                return Ok(informe);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        // PUT api/<InformeAnualPastorController>/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Put([FromBody] InformePastorViewModel data)
        {
            try
            {
                Informe informe = _context.Informe.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (informe != null)
                {
                    informe.LugarReunion = data.LugarReunion;
                    informe.FechaReunion = data.FechaReunion;
                    informe.IdTipoUsuario = data.IdTipoUsuario;
                    informe.IdSector = data.IdSector;
                    informe.Status = data.Status;
                    _context.Informe.Update(informe);
                    _context.SaveChanges();
                }

                VisitasPastor visitasPastor = _context.VisitasPastor.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (visitasPastor == null)
                {
                    var addVisitasPastor = new VisitasPastor
                    {
                        IdInforme = data.IdInforme,
                        PorPastor = data.VisitasPastor.PorPastor,
                        PorAncianosAux = data.VisitasPastor.PorAncianosAux,
                        PorDiaconos = data.VisitasPastor.PorDiaconos,
                        PorAuxiliares = data.VisitasPastor.PorAuxiliares,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now,
                    };
                    _context.VisitasPastor.Add(addVisitasPastor);
                    _context.SaveChanges();
                }
                else
                {
                    visitasPastor = data.VisitasPastor;
                    _context.VisitasPastor.Update(visitasPastor);
                    _context.SaveChanges();
                }

                CultosSector cultosSector = _context.CultosSector.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();

                if (cultosSector == null)
                {
                    var addCultosSector = new CultosSector
                    {
                        IdInforme = data.IdInforme,
                        Ordinarios = data.CultosSector.Ordinarios,
                        Especiales = data.CultosSector.Especiales,
                        DeAvivamiento = data.CultosSector.DeAvivamiento,
                        DeAniversario = data.CultosSector.DeAniversario,
                        PorElDistrito = data.CultosSector.PorElDistrito,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.CultosSector.Add(addCultosSector);
                    _context.SaveChanges();
                }
                else
                {
                    cultosSector = data.CultosSector;
                    _context.CultosSector.Update(cultosSector);
                    _context.SaveChanges();
                }

                EstudiosSector estudiosSector = _context.EstudiosSector
                    .Where(w => w.IdInforme == data.IdInforme)
                    .Where(w => w.IdTipoEstudio == 1)
                    .AsNoTracking()
                    .FirstOrDefault();
                if (estudiosSector == null)
                {
                    var addEstudiosSector = new EstudiosSector
                    {
                        IdInforme = data.IdInforme,
                        IdTipoEstudio = 1,
                        EscuelaDominical = data.EstudiosSector.EscuelaDominical,
                        Varonil = data.EstudiosSector.Varonil,
                        Femenil = data.EstudiosSector.Femenil,
                        Juvenil = data.EstudiosSector.Juvenil,
                        Infantil = data.EstudiosSector.Infantil,
                        Iglesia = data.EstudiosSector.Iglesia,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.EstudiosSector.Add(addEstudiosSector);
                    _context.SaveChanges();
                }
                else
                {
                    estudiosSector = data.EstudiosSector;
                    _context.EstudiosSector.Update(estudiosSector);
                    _context.SaveChanges();
                }

                EstudiosSector conferenciasSector = _context.EstudiosSector
                    .Where(w => w.IdInforme == data.IdInforme)
                    .Where(w => w.IdTipoEstudio == 2)
                    .AsNoTracking()
                    .FirstOrDefault();
                if (conferenciasSector == null)
                {
                    var addConferenciasSector = new EstudiosSector
                    {
                        IdInforme = data.IdInforme,
                        IdTipoEstudio = 2,
                        EscuelaDominical = data.ConferenciasSector.EscuelaDominical,
                        Varonil = data.ConferenciasSector.Varonil,
                        Femenil = data.ConferenciasSector.Femenil,
                        Juvenil = data.ConferenciasSector.Juvenil,
                        Infantil = data.ConferenciasSector.Infantil,
                        Iglesia = data.ConferenciasSector.Iglesia,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.EstudiosSector.Add(addConferenciasSector);
                    _context.SaveChanges();
                }
                else
                {
                    conferenciasSector = data.ConferenciasSector;
                    _context.EstudiosSector.Update(conferenciasSector);
                    _context.SaveChanges();
                }


                TrabajoEvangelismo trabajoEvangelismo = _context.TrabajoEvangelismo.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (trabajoEvangelismo == null)
                {
                    var addTrabajoEvangelismo = new TrabajoEvangelismo
                    {
                        IdInforme = data.IdInforme,
                        HogaresVisitados = data.TrabajoEvangelismo.HogaresVisitados,
                        HogaresConquistados = data.TrabajoEvangelismo.HogaresConquistados,
                        CultosPorLaLocalidad = data.TrabajoEvangelismo.CultosPorLaLocalidad,
                        CultosDeHogar = data.TrabajoEvangelismo.CultosDeHogar,
                        Campanias = data.TrabajoEvangelismo.Campanias,
                        AperturaDeMisiones = data.TrabajoEvangelismo.AperturaDeMisiones,
                        VisitantesPermanentes = data.TrabajoEvangelismo.VisitantesPermanentes,
                        Bautismos = data.TrabajoEvangelismo.Bautismos,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.TrabajoEvangelismo.Add(addTrabajoEvangelismo);
                    _context.SaveChanges();
                }
                else
                {
                    trabajoEvangelismo = data.TrabajoEvangelismo;
                    _context.TrabajoEvangelismo.Update(trabajoEvangelismo);
                    _context.SaveChanges();
                }

                foreach (var item in data.CultosMisionSector)
                {
                    if (item.Cultos > 0)
                    {
                        CultosMisionSector cultoMisionSector = _context.CultosMisionSector
                            .Where(w => w.IdInforme == data.IdInforme)
                            .Where(w => w.Ms_Id_MisionSector == item.Ms_Id_MisionSector)
                            .AsNoTracking()
                            .FirstOrDefault();
                        if (cultoMisionSector == null)
                        {
                            var addCultosMisionSector = new CultosMisionSector
                            {
                                IdInforme = data.IdInforme,
                                Ms_Id_MisionSector = item.Ms_Id_MisionSector,
                                Cultos = item.Cultos,
                                Usu_Id_Usuario = data.Usu_Id_Usuario,
                                FechaRegistro = DateTime.Now
                            };
                            _context.CultosMisionSector.Add(addCultosMisionSector);
                            _context.SaveChanges();
                        }
                        else
                        {
                            cultoMisionSector.Cultos = item.Cultos;
                            _context.CultosMisionSector.Update(cultoMisionSector);
                            _context.SaveChanges();
                        }
                    }
                }


                Organizaciones organizaciones = _context.Organizaciones.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (organizaciones == null)
                {
                    var addOrganizaciones = new Organizaciones
                    {
                        IdInforme = data.IdInforme,
                        SociedadFemenil = data.Organizaciones.SociedadFemenil,
                        SociedadJuvenil = data.Organizaciones.SociedadJuvenil,
                        DepartamentoFemenil = data.Organizaciones.DepartamentoFemenil,
                        DepartamentoJuvenil = data.Organizaciones.DepartamentoJuvenil,
                        DepartamentoInfantil = data.Organizaciones.DepartamentoInfantil,
                        Coros = data.Organizaciones.Coros,
                        GruposDeCanto = data.Organizaciones.GruposDeCanto,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.Organizaciones.Add(addOrganizaciones);
                    _context.SaveChanges();
                }
                else
                {
                    organizaciones = data.Organizaciones;
                    _context.Organizaciones.Update(organizaciones);
                    _context.SaveChanges();
                }

                AdquisicionesSector adquisicionesSector = _context.AdquisicionesSector.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (adquisicionesSector == null)
                {
                    var addAdquisicionesSector = new AdquisicionesSector
                    {
                        IdInforme = data.IdInforme,
                        Predios = data.AdquisicionesSector.Predios,
                        Casas = data.AdquisicionesSector.Casas,
                        Edificios = data.AdquisicionesSector.Edificios,
                        Templos = data.AdquisicionesSector.Templos,
                        Vehiculos = data.AdquisicionesSector.Vehiculos,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.AdquisicionesSector.Add(addAdquisicionesSector);
                    _context.SaveChanges();
                }
                else
                {
                    adquisicionesSector = data.AdquisicionesSector;
                    _context.AdquisicionesSector.Update(adquisicionesSector);
                    _context.SaveChanges();
                }

                SesionesReunionesSector reunionesSector = _context.SesionesReunionesSector.Where(w => w.IdInforme == data.IdInforme).Where(w => w.IdTipoSesionReunion == 1).AsNoTracking().FirstOrDefault();
                if (reunionesSector == null)
                {
                    var addReunionesSector = new SesionesReunionesSector
                    {
                        IdInforme = data.IdInforme,
                        IdTipoSesionReunion = 1,
                        EnElDistrito = data.Reuniones.EnElDistrito,
                        ConElPersonalDocente = data.Reuniones.ConElPersonalDocente,
                        ConSociedadesFemeniles = data.Reuniones.ConSociedadesFemeniles,
                        ConSociedadesJuveniles = data.Reuniones.ConSociedadesJuveniles,
                        ConDepartamentosInfantiles = data.Reuniones.ConDepartamentosInfantiles,
                        ConCorosYGruposDeCanto = data.Reuniones.ConCorosYGruposDeCanto,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.SesionesReunionesSector.Add(addReunionesSector);
                    _context.SaveChanges();
                }
                else
                {
                    reunionesSector = data.Reuniones;
                    _context.SesionesReunionesSector.Update(reunionesSector);
                    _context.SaveChanges();
                }

                SesionesReunionesSector sesionesSector = _context.SesionesReunionesSector.Where(w => w.IdInforme == data.IdInforme).Where(w => w.IdTipoSesionReunion == 2).AsNoTracking().FirstOrDefault();
                if (sesionesSector == null)
                {
                    var addSesionesSector = new SesionesReunionesSector
                    {
                        IdInforme = data.IdInforme,
                        IdTipoSesionReunion = 2,
                        EnElDistrito = data.Reuniones.EnElDistrito,
                        ConElPersonalDocente = data.Reuniones.ConElPersonalDocente,
                        ConSociedadesFemeniles = data.Reuniones.ConSociedadesFemeniles,
                        ConSociedadesJuveniles = data.Reuniones.ConSociedadesJuveniles,
                        ConDepartamentosInfantiles = data.Reuniones.ConDepartamentosInfantiles,
                        ConCorosYGruposDeCanto = data.Reuniones.ConCorosYGruposDeCanto,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.SesionesReunionesSector.Add(addSesionesSector);
                    _context.SaveChanges();
                }
                else
                {
                    sesionesSector = data.Sesiones;
                    _context.SesionesReunionesSector.Update(sesionesSector);
                    _context.SaveChanges();
                }

                Construcciones contruccionesSectorInicio = _context.Construcciones.Where(w => w.IdInforme == data.IdInforme).Where(w => w.IdTipoFaseConstruccion == 1).AsNoTracking().FirstOrDefault();
                if (contruccionesSectorInicio == null)
                {
                    var addContruccionesSectorInicio = new Construcciones
                    {
                        IdInforme = data.IdInforme,
                        IdTipoFaseConstruccion = 1,
                        ColocacionPrimeraPiedra = data.ConstruccionesInicio.ColocacionPrimeraPiedra,
                        Templo = data.ConstruccionesInicio.Templo,
                        CasaDeOracion = data.ConstruccionesInicio.CasaDeOracion,
                        CasaPastoral = data.ConstruccionesInicio.CasaPastoral,
                        Anexos = data.ConstruccionesInicio.Anexos,
                        Remodelacion = data.ConstruccionesInicio.Remodelacion,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.Construcciones.Add(addContruccionesSectorInicio);
                    _context.SaveChanges();
                }
                else
                {
                    contruccionesSectorInicio = data.ConstruccionesInicio;
                    _context.Construcciones.Update(contruccionesSectorInicio);
                    _context.SaveChanges();
                }

                Construcciones contruccionesSectorConclusion = _context.Construcciones.Where(w => w.IdInforme == data.IdInforme).Where(w => w.IdTipoFaseConstruccion == 2).AsNoTracking().FirstOrDefault();
                if (contruccionesSectorConclusion == null)
                {
                    var addContruccionesSectorConclusion = new Construcciones
                    {
                        IdInforme = data.IdInforme,
                        IdTipoFaseConstruccion = 2,
                        ColocacionPrimeraPiedra = data.ConstruccionesConclusion.ColocacionPrimeraPiedra,
                        Templo = data.ConstruccionesConclusion.Templo,
                        CasaDeOracion = data.ConstruccionesConclusion.CasaDeOracion,
                        CasaPastoral = data.ConstruccionesConclusion.CasaPastoral,
                        Anexos = data.ConstruccionesConclusion.Anexos,
                        Remodelacion = data.ConstruccionesConclusion.Remodelacion,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.Construcciones.Add(addContruccionesSectorConclusion);
                    _context.SaveChanges();
                }
                else
                {
                    contruccionesSectorConclusion = data.ConstruccionesConclusion;
                    _context.Construcciones.Update(contruccionesSectorConclusion);
                    _context.SaveChanges();
                }

                Ordenaciones ordenaciones = _context.Ordenaciones.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (ordenaciones == null)
                {
                    var addOrdenaciones = new Ordenaciones
                    {
                        IdInforme = data.IdInforme,
                        Ancianos = data.Ordenaciones.Ancianos,
                        Diaconos = data.Ordenaciones.Diaconos,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.Ordenaciones.Add(addOrdenaciones);
                    _context.SaveChanges();
                }
                else
                {
                    ordenaciones = data.Ordenaciones;
                    _context.Ordenaciones.Update(ordenaciones);
                    _context.SaveChanges();
                }

                Dedicaciones dedicaciones = _context.Dedicaciones.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (dedicaciones == null)
                {
                    var addDedicaciones = new Dedicaciones
                    {
                        IdInforme = data.IdInforme,
                        Templos = data.Dedicaciones.Templos,
                        CasasDeOracion = data.Dedicaciones.CasasDeOracion,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.Dedicaciones.Add(addDedicaciones);
                    _context.SaveChanges();
                }
                else
                {
                    dedicaciones = data.Dedicaciones;
                    _context.Dedicaciones.Update(dedicaciones);
                    _context.SaveChanges();
                }

                LlamamientoDePersonal llamamientoDePersonal = _context.LlamamientoDePersonal.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (llamamientoDePersonal == null)
                {
                    var addLlamamientoDePersonal = new LlamamientoDePersonal
                    {
                        IdInforme = data.IdInforme,
                        DiaconosAprueba = data.LlamamientoDePersonal.DiaconosAprueba,
                        Auxiliares = data.LlamamientoDePersonal.Auxiliares,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.LlamamientoDePersonal.Add(addLlamamientoDePersonal);
                    _context.SaveChanges();
                }
                else
                {
                    llamamientoDePersonal = data.LlamamientoDePersonal;
                    _context.LlamamientoDePersonal.Update(llamamientoDePersonal);
                    _context.SaveChanges();
                }


                RegularizacionPrediosTemplos regularizacionPatNac = _context.RegularizacionPrediosTemplos.Where(w => w.IdInforme == data.IdInforme).Where(w => w.IdTipoPatrimonio == 1).AsNoTracking().FirstOrDefault();
                if (regularizacionPatNac == null)
                {
                    var addRegularizacionPatNac = new RegularizacionPrediosTemplos
                    {
                        IdInforme = data.IdInforme,
                        IdTipoPatrimonio = 1,
                        Templos = data.RegularizacionPatNac.Templos,
                        CasasPastorales = data.RegularizacionPatNac.CasasPastorales,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.RegularizacionPrediosTemplos.Add(addRegularizacionPatNac);
                    _context.SaveChanges();
                }
                else
                {
                    regularizacionPatNac = data.RegularizacionPatNac;
                    _context.RegularizacionPrediosTemplos.Update(regularizacionPatNac);
                    _context.SaveChanges();
                }

                RegularizacionPrediosTemplos regularizacionPatIg = _context.RegularizacionPrediosTemplos.Where(w => w.IdInforme == data.IdInforme).Where(w => w.IdTipoPatrimonio == 2).AsNoTracking().FirstOrDefault();
                if (regularizacionPatIg == null)
                {
                    var addRegularizacionPatIg = new RegularizacionPrediosTemplos
                    {
                        IdInforme = data.IdInforme,
                        IdTipoPatrimonio = 2,
                        Templos = data.RegularizacionPatIg.Templos,
                        CasasPastorales = data.RegularizacionPatIg.CasasPastorales,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.RegularizacionPrediosTemplos.Add(addRegularizacionPatIg);
                    _context.SaveChanges();
                }
                else
                {
                    regularizacionPatIg = data.RegularizacionPatIg;
                    _context.RegularizacionPrediosTemplos.Update(regularizacionPatIg);
                    _context.SaveChanges();
                }

                MovimientoEconomico movimientoEconomico = _context.MovimientoEconomico.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
                if (movimientoEconomico == null)
                {
                    var addMovimientoEconomico = new MovimientoEconomico
                    {
                        IdInforme = data.IdInforme,
                        ExistenciaAnterior = data.MovimientoEconomico.ExistenciaAnterior,
                        EntradaMes = data.MovimientoEconomico.EntradaMes,
                        SumaTotal = data.MovimientoEconomico.SumaTotal,
                        GastosAdmon = data.MovimientoEconomico.GastosAdmon,
                        TransferenciasAentidadSuperior = data.MovimientoEconomico.TransferenciasAentidadSuperior,
                        ExistenciaEnCaja = data.MovimientoEconomico.ExistenciaEnCaja,
                        Usu_Id_Usuario = data.Usu_Id_Usuario,
                        FechaRegistro = DateTime.Now
                    };
                    _context.MovimientoEconomico.Add(addMovimientoEconomico);
                    _context.SaveChanges();
                }
                else
                {
                    movimientoEconomico = data.MovimientoEconomico;
                    _context.MovimientoEconomico.Update(movimientoEconomico);
                    _context.SaveChanges();
                }

                foreach (var actividad in data.OtrasActividades)
                {
                    OtrasActividades otraActividad = _context.OtrasActividades.Where(w => w.IdOtraActividad == actividad.IdOtraActividad).AsNoTracking().FirstOrDefault();
                    if (otraActividad == null)
                    {
                        var addOtrasActividades = new OtrasActividades
                        {
                            IdInforme = data.IdInforme,
                            Descripcion = actividad.Descripcion,
                            NumDeOrden = actividad.NumDeOrden,
                            Usu_Id_Usuario = data.Usu_Id_Usuario,
                            FechaRegistro = DateTime.Now
                        };
                        _context.OtrasActividades.Add(addOtrasActividades);
                        _context.SaveChanges();
                    }
                    else
                    {
                        otraActividad.Descripcion = actividad.Descripcion;
                        otraActividad.NumDeOrden = actividad.NumDeOrden;
                        _context.OtrasActividades.Update(actividad);
                        _context.SaveChanges();
                    }
                }



                foreach (var actividad in data.ActividadesEliminadas)
                {
                    if(actividad.IdOtraActividad != 0)
                    {
                        _context.OtrasActividades.Remove(actividad);
                        _context.SaveChanges();
                    }
                }

                return Ok();

            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        // DELETE api/<InformeAnualPastorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
