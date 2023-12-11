using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class CasaPastoral
    {
        [Key]
        public int cp_Id_CasaPastoral { get; set; }

        [Required]
        public int cp_Id_Sector { get; set; }
        [Required]
        public bool cp_Activo { get; set; }

        public bool cp_Mismo_Predio_Templo { get; set; }
        public string cp_Propiedad_De { get; set; }
        public int? cp_Id_Domicilio { get; set; }
        public string cp_Tel_Fijo { get; set; }

        public string cp_Foto { get; set; }
        public DateTime? cp_Fecha_Registro { get; set; }
        public int? cp_Usuario { get; set; }

    }
}
