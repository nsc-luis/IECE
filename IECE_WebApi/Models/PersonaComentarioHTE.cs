using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class PersonaComentarioHTE
    {
        [Key]
        public int id { get; set; }
        public virtual Persona PersonaEntity { get; set; }
        public string ComentarioHTE { get; set; }
        public string nvaProfesionOficio1 { get; set; }
        public string nvaProfesionOficio2 { get; set; }
    }
}
