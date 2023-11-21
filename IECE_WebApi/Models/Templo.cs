using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Templo
    {
        [Key]
        public int tem_Id_Templo { get; set; }
        public bool tem_Activo { get; set; }

        public Nullable<DateTime> tem_Fecha_Primera_Piedra { get; set; }

        public string tem_Tipo_Templo { get; set; }
        public string tem_Propiedad_De { get; set; }
        public Nullable<DateTime> tem_Fecha_Dedicacion { get; set; }
        public Nullable<int> tem_Aforo { get; set; }
        public string tem_comentario { get; set; }
        public string tem_Foto_Frente { get; set; }
        public string tem_Telefono { get; set; }
        public int dom_Id_Domicilio { get; set; }

        public int sec_Id_Sector { get; set; }
        public Nullable<DateTime> tem_Fecha_Captura { get; set; }
        public Nullable<int> usu_Usuario { get; set; }
    }
}
