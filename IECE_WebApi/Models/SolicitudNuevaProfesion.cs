using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class SolicitudNuevaProfesion
    {
        [Key]
        public int idNvaProfesion { get; set; }
        public string descNvaProfesion { get; set; }
        public bool solicitudAtendida { get; set; }
        public int usu_Id_Usuario { get; set; }
        public DateTime fechaSolicitud { get; set; }
    }
}
