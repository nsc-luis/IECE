using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Estado
    {
        [Key]
        public int est_Id_Estado { get; set; }
        [Display(Name = "Nombre corto")]
        public string est_Nombre_Corto { get; set; }
        [Display(Name = "Pais")]
        public string est_Pais { get; set; }
        [Display(Name = "Nombre")]
        public string est_Nombre { get; set; }
        public int pais_Id_Pais { get; set; }
    }
}
