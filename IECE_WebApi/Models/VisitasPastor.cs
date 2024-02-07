using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class VisitasPastor
    {
        [Key]
        public int IdVisitasPastor {  get; set; }
        public int IdInforme { get; set; }
        public int PorPastor { get; set; }
        public int PorAncianosAux { get; set; }
        public int PorDiaconos {  get; set; }
        public int PorAuxiliares { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
