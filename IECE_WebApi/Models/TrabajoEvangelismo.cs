using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class TrabajoEvangelismo
    {
        [Key]
        public int IdTrabajoEvangelismo { get; set; }
        public int IdInforme { get; set; }
        public int HogaresVisitados { get; set; }
        public int HogaresConquistados { get; set; }
        public int CultosPorLaLocalidad { get; set; }
        public int CultosDeHogar {  get; set; }
        public int Campanias {  get; set; }
        public int AperturaDeMisiones { get; set; }
        public int VisitantesPermanentes { get; set; }
        public int Bautismos { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
