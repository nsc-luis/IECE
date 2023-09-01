using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Sociedades
    {
        [Key]
        public int soc_Id_Sociedad { get; set; }
        public string soc_Nombre { get; set; }
        public string soc_Tipo_Sociedad { get; set; }
        public int soc_Id_Persona_Presidente { get; set; }
        public int soc_Id_Persona_Secretario { get; set; }
        public int soc_Id_Persona_Tesorero { get; set; }
        public DateTime soc_Fecha_Creacion { get; set; }
        public int sec_Id_Sector { get; set; }
        public int dis_Id_Distrito { get; set; }
        public int pem_Id_Ministro { get; set; }
    }
}
