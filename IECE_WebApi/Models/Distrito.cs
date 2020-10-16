using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Distrito
    {
        [Key]
        public int dis_Id_Distrito { get; set; }
        public string dis_Localidad { get; set; }
        public string dis_Tipo_Localidad { get; set; }
        public int dis_Numero { get; set; }
        public int sec_Id_Sector { get; set; }
        public int pem_Id_Miembro { get; set; }
        public int pem_Id_Miembro_Obispo_Suplente { get; set; }
        public int pem_Id_Miembro_Secretario { get; set; }
        public int pem_Id_Miembro_SubSecretario { get; set; }
        public int pem_Id_Miembro_Tesorero { get; set; }
        public int pem_Id_Miembro_SubTesorero { get; set; }
        public DateTime dis_Fecha_Organizacion { get; set; }
        public string dis_Domicilio_Especial_Correspondencia { get; set; }
        public bool sw_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public int usu_Id_Usuario { get; set; }
    }
}
