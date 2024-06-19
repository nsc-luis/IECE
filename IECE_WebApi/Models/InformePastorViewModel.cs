using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

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

        public VisitasPastor VisitasPastor { get; set; } = new VisitasPastor();
        public VisitasObispo VisitasObispo { get; set; } = new VisitasObispo();
        public CultosSector CultosSector { get; set; } = new CultosSector();
        public EstudiosSector EstudiosSector { get; set; } = new EstudiosSector();
        public EstudiosSector ConferenciasSector { get; set; } = new EstudiosSector();
        public List<CultosMisionSector> CultosMisionSector { get; set; } = new List<CultosMisionSector>();
        public TrabajoEvangelismo TrabajoEvangelismo { get; set; } = new TrabajoEvangelismo();
        public Organizaciones Organizaciones { get; set; } = new Organizaciones();
        public AdquisicionesSector AdquisicionesSector { get; set; } = new AdquisicionesSector();
        public SesionesReunionesSector Sesiones { get; set; } = new SesionesReunionesSector();
        public SesionesReunionesSector Reuniones { get; set; } = new SesionesReunionesSector();
        public Construcciones ConstruccionesInicio { get; set; } = new Construcciones();
        public Construcciones ConstruccionesConclusion { get; set; } = new Construcciones();
        public Ordenaciones Ordenaciones { get; set; } = new Ordenaciones();
        public Dedicaciones Dedicaciones { get; set; } = new Dedicaciones();
        public LlamamientoDePersonal LlamamientoDePersonal { get; set; } = new LlamamientoDePersonal();
        public RegularizacionPrediosTemplos RegularizacionPatNac { get; set; } = new RegularizacionPrediosTemplos();
        public RegularizacionPrediosTemplos RegularizacionPatIg { get; set; } = new RegularizacionPrediosTemplos();
        public MovimientoEconomico MovimientoEconomico { get; set; } = new MovimientoEconomico();
        public List<OtrasActividades> OtrasActividades { get; set; } = new List<OtrasActividades>();
        public List<OtrasActividades> ActividadesEliminadas { get; set; } = new List<OtrasActividades>();
        public List<ActividadesObispo> ActividadesObispo { get; set; } = new List<ActividadesObispo>();
    }

    public class ActividadesObispo
    {
        public Sector Sector { get; set; }
        public VisitasObispo VisitasObispo { get; set; }
        public CultosDistrito CultosDistrito { get; set; }
        public ConferenciasDistrito ConferenciasDistrito { get; set; }
        public ConcentracionesDistrito ConcentracionesDistrito { get; set; }
    }
}
