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
        public int IdSector { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string LugarReunion { get; set; }
        public DateTime FechaReunion { get; set; }
        public int Status { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; }
        public int Anio { get; set; }

        public VisitasPastor VisitasPastor { get; set; }
        public CultosSector CultosSector { get; set; }
        public EstudiosSector EstudiosSector { get; set; }
        public EstudiosSector ConferenciasSector { get; set; }
        public List<CultosMisionSector> CultosMisionSector { get; set; }
        public TrabajoEvangelismo TrabajoEvangelismo { get; set; }
        public Organizaciones Organizaciones { get; set; }
        public AdquisicionesSector AdquisicionesSector { get; set; }
        public SesionesReunionesSector Sesiones { get; set; }
        public SesionesReunionesSector Reuniones { get; set; }
        public Construcciones ConstruccionesInicio { get; set; }
        public Construcciones ConstruccionesConclusion { get; set; }
    }

}
