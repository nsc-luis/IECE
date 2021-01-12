using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Hogar_Persona
    {
        [Key]
        [Required]
        public int hp_Id_Hogar_Persona { get; set; }
        [Required]
        public int hd_Id_Hogar { get; set; }
        [Required]
        public int per_Id_Persona { get; set; }
        [Required]
        [Display(Name = "Jerarquia")]
        public int hp_Jerarquia { get; set; }
        [Required]
        [DefaultValue(0)]
        public int usu_Id_Usuario { get; set; }
        [Required]
        [DefaultValue("1890-01-01")]
        public DateTime Fecha_Registro { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool sw_Registro { get; set; }
    }
}
