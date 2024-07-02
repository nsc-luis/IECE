using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Directivo_General
    {
        [Key]
        public int dg_Id { get; set; }
        public string dg_Periodo { get; set; }
        public string dg_Cargo { get; set; }
        public int pem_Id_Integrante { get; set; }
    }
}
