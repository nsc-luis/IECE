using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class AdquisicionesSector
    {
        [Key]
        public int IdAdquisicionSector { get; set; }
        public int IdInforme { get; set; }
        public int? Predios { get; set; }
        public int? Casas { get; set; }
        public int? Edificios { get; set; }
        public int? Templos { get; set; }
        public int? Vehiculos { get; set; }
        public int UsuIdUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
