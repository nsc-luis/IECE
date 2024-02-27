using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class Dedicaciones
    {
        [Key]
        public int IdDedicacion { get; set; }
        public int IdInforme { get; set; }
        public int? Templos { get; set; }
        public int? CasasDeOracion { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
