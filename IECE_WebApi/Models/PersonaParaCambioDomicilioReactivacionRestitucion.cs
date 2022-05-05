using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class PersonaParaCambioDomicilioReactivacionRestitucion
    {
        [Key]
        public int id { get; set; }
        public bool bautizado { get; set; }
        public bool activo { get; set; }
        public bool visibiliadaAbierta { get; set; }
        public int sectorPersona { get; set; }
        public int sectorUsuario { get; set; }
    }
}
