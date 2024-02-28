using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class RegularizacionPrediosTemplos
    {
        [Key]
        public int IdRegularizacionPrediosTemplos { get; set; }
        public int IdInforme { get; set; }
        public int IdTipoPatrimonio { get; set; }
        public int? Templos { get; set; }
        public int? CasasPastorales { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
