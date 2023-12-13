using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace IECE_WebApi.Models
{
    public class Visitante
    {
        [Key]
        public int Vp_Id_Visitante { get; set; }
        public bool? Vp_Activo { get; set; }
        public int Vp_Numero_Lista { get; set; }
        public string Vp_Nombre { get; set; }
        public string Vp_Telefono_Contacto { get; set; }
        public string Vp_Direccion { get; set; }
        public string Vp_Tipo_Visitante { get; set; }
        public int sec_Id_Sector { get; set; }
        public DateTime? Fecha_Registro { get; set; }
        public int Usu_Id_Usuario { get; set; }
    }
}
