using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Estado_Civil
    {
        [Key]
        [Required]
        public int eci_Id_Relacion_Civil { get; set; }
        [Required]
        public int per_Id_Miembro { get; set; }
        [Required]
        [Display(Name = "Fecha de boda civil")]
        public DateTime eci_Fecha_Boda_Civil { get; set; }
        [Display(Name = "Acta de boda civil")]
        public string eci_Num_Acta_Boda_Civil { get; set; }
        [Display(Name = "Libro de acta de boda civil")]
        public string eci_Libro_Acta_Boda_Civil { get; set; }
        [Display(Name = "Oficialia de boda civil")]
        public string eci_Oficialia_Boda_Civil { get; set; }
        [Display(Name = "Registro de boda civil")]
        public string eci_Registro_Civil { get; set; }
        [Display(Name = "Fecha de boda eclesiastica")]
        public DateTime eci_Fecha_Boda_Eclesiastica { get; set; }
        [Display(Name = "Lugar de boda eclesiastica")]
        public string eci_Lugar_Boda_Eclesiastica { get; set; }
        [Display(Name = "Nombre de conyuge")]
        public string eci_Nombre_Conyuge { get; set; }
        [Display(Name = "Cantidad de hijos")]
        public int eci_Cantidad_Hijos { get; set; }
        [Display(Name = "Nombre de hijos")]
        public string eci_Nombre_Hijos { get; set; }
        public Boolean sw_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public int usu_Id_Usuario { get; set; }
    }
}
