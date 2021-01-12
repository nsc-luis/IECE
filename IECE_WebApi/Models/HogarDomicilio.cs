using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class HogarDomicilio
    {
        [Key]
        [Required]
        public int hd_Id_Hogar { get; set; }
        [Display(Name = "Calle")]
        public string hd_Calle { get; set; }
        [Display(Name = "Numero exterior")]
        public string hd_Numero_Exterior { get; set; }
        [Display(Name = "Numero interior")]
        public string hd_Numero_Interior { get; set; }
        [Display(Name = "Tipo de subdivision")]
        public string hd_Tipo_Subdivision { get; set; }
        [Display(Name = "Subdivision")]
        public string hd_Subdivision { get; set; }
        [Display(Name = "Localidad")]
        public string hd_Localidad { get; set; }
        [Display(Name = "Municipio/Cuidad")]
        public string hd_Municipio_Ciudad { get; set; }
        public int pais_Id_Pais { get; set; }
        public int est_Id_Estado { get; set; }
        [Display(Name = "Telefono")]
        public string hd_Telefono { get; set; }
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
