using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class SolicitudNvoEstado
    {
        [Key]
        public int idNvaSolEstado { get; set; }
        public int pais_Id_Pais { get; set; }
        public string nombreNvoEstado { get; set; }
        public Boolean solicitudAtendida { get; set; }
        public int usu_Id_Usuario { get; set; }
        public Nullable<DateTime> fechaSolicitud { get; set; }
    }
}
