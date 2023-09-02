using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class PersonaHogarExistente
    {
        [Key]
        public int id { get; set; }
        public virtual Persona PersonaEntity { get; set; }
        public int jerarquia { get; set; }
        public int hdId { get; set; }
        public string nvaProfesionOficio1 { get; set; }
        public string nvaProfesionOficio2 { get; set; }
        public DateTime? FechaTransaccionHistorica { get; set; }
        public int idSectorBautismo { get; set; }
        public int idOficio1 { get; set; }
        public int idOficio2 { get; set; }
    }
}
