using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IECE_WebApi.Models
{
    public class TipoEstudio
    {
        [Key]
        public int IdTipoEstudio { get; set; }
        [Column(("TipoEstudio"),TypeName = "varchar(20)")]
        public string TipoEstudioNombre {  get; set; }
    }
}
