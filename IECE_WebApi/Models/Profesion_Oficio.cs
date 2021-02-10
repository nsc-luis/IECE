using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Profesion_Oficio
    {
        [Key]
        public int pro_Id_Profesion_Oficio { get; set; }
        [Required]
        [Display(Name = "Descripcion del oficio")]
        public string pro_Sub_Categoria { get; set; }
        [Required]
        [Display(Name = "Categoria del oficio")]
        public string pro_Categoria { get; set; }
        [Required]
        [DefaultValue(0)]
        public int usu_Id_Usuario { get; set; }
        [Required]
        [DefaultValue("1900-01-01")]
        public DateTime Fecha_Registro { get; set; }
    }
}
