using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class OtrasActividades
    {
        [Key]
        public int IdOtraActividad { get; set; }
        public int IdInforme { get; set; }
        public string Descripcion { get; set; }
        public int? NumDeOrden { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
