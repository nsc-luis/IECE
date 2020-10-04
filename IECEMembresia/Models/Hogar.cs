using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IECEMembresia.Models
{
    [Table("Hogar", Schema = "dbo")]
    public class Hogar
    {
        [Key]
        public int hog_Id_Hogar { get; set; }
        public int dom_Id_Domicilio { get; set; }
        public int per_Id_Persona { get; set; }
        [Required]
        [Display(Name = "Jerarquia")]
        public int jerarquia { get; set; }
    }
}
