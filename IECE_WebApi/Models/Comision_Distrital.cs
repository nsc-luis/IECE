using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Comision_Distrital
    {
        [Key]
        public int ComisionDistrital_Id { get; set; }
        public bool? Activa { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public DateTime? Fecha_Registro { get; set; }
    }
}
