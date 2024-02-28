using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class LlamamientoDePersonal
    {
        [Key]
        public int IdLlamamientoDePersonal { get; set; }
        public int IdInforme { get; set; }
        public int? DiaconosAprueba { get; set; }
        public int? Auxiliares { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
