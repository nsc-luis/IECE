using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using IECE_WebApi.Helpers;

namespace IECE_WebApi.Models
{
    public class InformePastorViewModel
    {
        public int IdInforme { get; set; }
        public int IdTipoUsuario { get; set; }
        public int IdDistrito { get; set; }
        public int? IdSector { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string LugarReunion { get; set; }
        public DateTime FechaReunion { get; set; }
        public int Status { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; }
        public int Anio { get; set; }
        public Sector Sector { get; set; }
        public VisitasPastor VisitasPastor { get; set; } = new VisitasPastor();
        public VisitasObispo VisitasObispo { get; set; } = new VisitasObispo();
        public CultosSector CultosSector { get; set; } = new CultosSector();
        public CultosDistrito CultosDistrito { get; set; } = new CultosDistrito();
        public EstudiosSector EstudiosSector { get; set; } = new EstudiosSector();
        public EstudiosSector ConferenciasSector { get; set; } = new EstudiosSector();
        public ConferenciasDistrito ConferenciasDistrito { get; set; } = new ConferenciasDistrito();
        public List<CultosMisionSector> CultosMisionSector { get; set; } = new List<CultosMisionSector>();
        public List<Mision_Sector> MisionesSector { get; set; } = new List<Mision_Sector>();
        public TrabajoEvangelismo TrabajoEvangelismo { get; set; } = new TrabajoEvangelismo();
        public Organizaciones Organizaciones { get; set; } = new Organizaciones();
        public AdquisicionesSector AdquisicionesSector { get; set; } = new AdquisicionesSector();
        public AdquisicionesDistrito AdquisicionesDistrito { get; set; } = new AdquisicionesDistrito();
        public SesionesReunionesSector Sesiones { get; set; } = new SesionesReunionesSector();
        public SesionesReunionesSector Reuniones { get; set; } = new SesionesReunionesSector();
        public SesionesReunionesDistrito SesionesDistrito { get; set; } = new SesionesReunionesDistrito();
        public SesionesReunionesDistrito ReunionesDistrito { get; set; } = new SesionesReunionesDistrito();
        public Construcciones ConstruccionesInicio { get; set; } = new Construcciones();
        public Construcciones ConstruccionesConclusion { get; set; } = new Construcciones();
        public ConstruccionesDistrito ConstruccionesDistritoInicio { get; set; } = new ConstruccionesDistrito();
        public ConstruccionesDistrito ConstruccionesDistritoConclusion { get; set; } = new ConstruccionesDistrito();
        public Ordenaciones Ordenaciones { get; set; } = new Ordenaciones();
        public Dedicaciones Dedicaciones { get; set; } = new Dedicaciones();
        public LlamamientoDePersonal LlamamientoDePersonal { get; set; } = new LlamamientoDePersonal();
        public RegularizacionPrediosTemplos RegularizacionPatNac { get; set; } = new RegularizacionPrediosTemplos();
        public RegularizacionPrediosTemplos RegularizacionPatIg { get; set; } = new RegularizacionPrediosTemplos();
        public MovimientoEconomico MovimientoEconomico { get; set; } = new MovimientoEconomico();
        public List<OtrasActividades> OtrasActividades { get; set; } = new List<OtrasActividades>();
        public List<OtrasActividades> ActividadesEliminadas { get; set; } = new List<OtrasActividades>();
        public List<ActividadesObispo> ActividadesObispo { get; set; } = new List<ActividadesObispo>();
        //public SubConsultas.objInformeObispo ActividadesObispoPutModel { get; set; }
        public ConcentracionesDistrito ConcentracionesDistrito { get; set; } = new ConcentracionesDistrito();

    }

    public class ActividadesObispo
    {
        public Sector Sector { get; set; } = new Sector();
        public VisitasObispo VisitasObispo { get; set; }
        public CultosDistrito CultosDistrito { get; set; }
        public ConferenciasDistrito ConferenciasDistrito { get; set; }
        public ConcentracionesDistrito ConcentracionesDistrito { get; set; }
    }
}
