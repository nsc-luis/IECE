using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class CultosSector
    {
        [Key]
        public int IdCultoSector { get; set;}
        public int IdInforme { get; set;}
        public int? Ordinarios { get; set;}
        public int? Especiales { get; set;}
        public int? DeAvivamiento { get; set;}
        public int? DeAniversario { get; set;}
        public int? PorElDistrito { get; set;}
        public int Usu_Id_Usuario { get; set;}
        public DateTime FechaRegistro { get; set;}
    }
}
