using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace IECE_WebApi.Models
{
    public class Visitante
    {
        [Key]
        public int vp_Id_Visitante { get; set; }
        public bool? vp_Activo { get; set; }
        public int vp_Numero_Lista { get; set; }
        public string vp_Nombre { get; set; }
        public string vp_Telefono_Contacto { get; set; }
        public string vp_Direccion { get; set; }
        public string vp_Tipo_Visitante { get; set; }
        public int sec_Id_Sector { get; set; }
        public DateTime? Fecha_Registro { get; set; }
        public int usu_Id_Usuario { get; set; }
    }
}
