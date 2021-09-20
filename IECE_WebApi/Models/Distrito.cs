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
        public bool dis_Activo { get; set; }
        public string dis_Alias { get; set; }
        public string dis_Tipo_Distrito { get; set; }
        public int dis_Numero { get; set; }
        public Nullable<int> sec_Id_Sector_Base { get; set; }
        public string dis_Area { get; set; }
        public Nullable<int> pem_Id_Obispo { get; set; }
        public Nullable<int> pem_Id_Obispo_Suplente { get; set; }
        public Nullable<int> pem_Id_Secretario { get; set; }
        public Nullable<int> pem_Id_Sub_Secretario { get; set; }
        public Nullable<int> pem_Id_Tesorero { get; set; }
        public Nullable<int> pem_Id_Sub_Tesorero { get; set; }
        public Nullable<DateTime> dis_Fecha_Organizacion { get; set; }
        public Nullable<DateTime> dis_Fecha_Cambio_Obispo { get; set; }
        public Nullable<DateTime> dis_Fecha_Ultimo_Cambio_Admon { get; set; }
        public string dis_Domicilio_Especial_Correspondencia { get; set; }
        //public bool sw_Registro { get; set; }
    }
}
