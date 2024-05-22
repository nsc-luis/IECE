using System.ComponentModel.DataAnnotations;
using System;

namespace IECE_WebApi.Models
{
    public class VisitasObispo
    {
        [Key]
        public int idVisitasObispo { get; set; }
        public int IdInforme { get; set; }
        public int idSector { get; set; }
        public int? aSectores { get; set; }
        public int? aHogares { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
