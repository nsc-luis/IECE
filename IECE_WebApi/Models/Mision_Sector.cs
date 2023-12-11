using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Mision_Sector
    {
        [Key]
        public int ms_Id { get; set; }
        public int ms_Numero { get; set; }
        public string ms_Alias { get; set; }
        public bool ms_Activo { get; set; }
        public int sec_Id_Sector { get; set; }
        public DateTime ms_Fecha_Captura { get; set; }
        public int Usu_Usuario { get; set; }
    }
}
