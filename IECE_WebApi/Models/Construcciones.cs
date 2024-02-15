using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class Construcciones
    {
        [Key]
        public int IdConstruccion { get; set; }
        public int IdInforme { get; set; }
        public int IdTipoFaseConstruccion { get; set; }
        public int? ColocacionPrimeraPiedra { get; set; }
        public int? Templo { get; set; }
        public int? CasaDeOracion { get; set; }
        public int? CasaPastoral { get; set; }
        public int? Anexos { get; set; }
        public int? Remodelacion { get; set; }
        public int UsuIdUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
