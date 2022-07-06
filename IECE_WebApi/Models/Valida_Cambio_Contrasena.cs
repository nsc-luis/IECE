using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Valida_Cambio_Contrasena
    {
        [Key]
        public int vcc_Id_Cambio_Contrasena { get; set; }
        public string vcc_Cadena { get; set; }
        public string vcc_Correo { get; set; }
        public DateTime vcc_Caducidad { get; set; }
    }
}
