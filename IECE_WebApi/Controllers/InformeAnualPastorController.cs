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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InformeAnualPastorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InformeAnualPastorController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Informe>> Get()
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
                return Ok(_context.Informe.Select(s => new
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
                    .Where(s => s.Anio == data.Anio)
                    .ToList();

                bool informeExiste = informes.Any(a => a.Mes == data.Mes);
                if (informeExiste)
                {
                    return BadRequest($"Ya existe un informe para el mes {data.Mes}");
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
                if(cultosSector == null)
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

                Organizaciones organizaciones  = _context.Organizaciones.Where(w => w.IdInforme == data.IdInforme).AsNoTracking().FirstOrDefault();
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
