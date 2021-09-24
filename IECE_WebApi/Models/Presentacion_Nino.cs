using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Presentacion_Nino
    {
        [Key]
        public int pdn_Id_Presentacion { get; set; }
        [Required]
        public int per_Id_Persona { get; set; }
        [Required]
        public string pdn_Ministro_Oficiante { get; set; }
        [Required]
        public Nullable<DateTime> pdn_Fecha_Presentacion { get; set; }
        [Required]
        public Nullable<int> sec_Id_Sector { get; set; }
        [Required]
        public Nullable<int> usu_Id_Usuario { get; set; }
        [Required]
        public Nullable<DateTime> Fecha_Registro { get; set; }
    }
}
