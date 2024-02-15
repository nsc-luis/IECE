using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class Ordenaciones
    {
        [Key]
        public int IdOrdenacion { get; set; }
        public int IdInforme { get; set; }
        public int? Ancianos { get; set; }
        public int? Diaconos { get; set; }
        public int UsuIdUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
