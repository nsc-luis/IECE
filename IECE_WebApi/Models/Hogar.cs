using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Hogar
    {
        [Key]
        public int hog_Id_Hogar { get; set; }
        public int per_Id_Persona { get; set; }
        [Required]
        [Display(Name = "Jerarquia")]
        public int jerarquia { get; set; }
    }
}
