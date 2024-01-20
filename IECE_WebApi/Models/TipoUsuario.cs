using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IECE_WebApi.Models
{
    public class TipoUsuario
    {
        [Key]
        public int IdTipoUsuario { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Tipo {  get; set; }
    }
}
