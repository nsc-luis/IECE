using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Agrupaciones_Musicales
    {
        [Key]
        public int am_Id_Agrupacion { get; set; }
        public string am_Nombre { get; set; }
        public int am_Id_Persona_Director { get; set; }
        public DateTime am_Fecha_Creacion { get; set; }
        public int sec_Id_Sector { get; set; }
        public int dis_Id_Distrito { get; set; }
        public int pem_Id_Ministro { get; set; }
    }
}
