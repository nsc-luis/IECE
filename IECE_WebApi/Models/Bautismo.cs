using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Bautismo
    {
        [Key]
        public int bau_Id_Bautismo { get; set; }
        [Required]
        public int per_Id_Persona { get; set; }
        [Display(Name = "Lugar de bautismo")]
        public string bau_Lugar_Bautismo { get; set; }
        [Display(Name = "Fecha de bautismo")]
        public DateTime bau_Fecha_Bautismo { get; set; }
        [Display(Name = "Ministro que bautisó")]
        public string bau_Ministro_Que_Bautizo { get; set; }
        public Boolean sw_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public int usu_Id_Usuario { get; set; }
    }
}
