using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Personal_Ministerial
    {
        [Key]
        public int pem_Id_Ministro { get; set; }
        public bool pem_Activo { get; set; }
        public Nullable<int> per_Id_Miembro { get; set; }
        public string pem_Nombre { get; set; }
        public Nullable<DateTime> pem_Fecha_Nacimiento { get; set; }
        public Nullable<int> sec_Id_Congregacion { get; set; }
        public string pem_Grado_Ministerial { get; set; }
        public Nullable<DateTime> pem_Fecha_Prueba_Diaconado { get; set; }
        public Nullable<DateTime> pem_Fecha_Ordenacion_Diacono { get; set; }
        public Nullable<DateTime> pem_Fecha_Ordenacion_Anciano { get; set; }
        public bool pem_Dedicado { get; set; }
        public bool pem_En_Permiso { get; set; }
        public string pem_Tipo_Jubilacion { get; set; }
        public bool pem_Registrado_Gobernacion { get; set; }
        public bool pem_Necesita_Credencial { get; set; }
        public string pem_Documento_Personal { get; set; }
        public string pem_Foto_Ministro { get; set; }
        public string pem_Cel1 { get; set; }
        public string pem_Cel2 { get; set; }
        public string pem_emailIECE { get; set; }
        public string pem_email_Personal { get; set; }
        public Nullable<bool> sw_Registro { get; set; }
        public Nullable<DateTime> Fecha_Registro { get; set; }
        public Nullable<int> usu_Id_Usuario { get; set; }
    }
}
