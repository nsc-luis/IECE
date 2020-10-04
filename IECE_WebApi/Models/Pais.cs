using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Pais
    {
        [Key]
        public int pais_Id_Pais { get; set; }
        [Display(Name = "Nombre corto")]
        public string pais_Nombre_Corto { get; set; }
        [Display(Name = "Nombre")]
        public string pais_Nombre { get; set; }
    }
}
