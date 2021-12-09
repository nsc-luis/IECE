using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Matrimonio_Legalizacion
    {
        [Key]
        public int mat_Id_MatrimonioLegalizacion { get; set; }
        public string mat_Tipo_Enlace { get; set; }
        public Nullable<int> per_Id_Persona_Hombre { get; set; }
        public Nullable<int> per_Id_Persona_Mujer { get; set; }
        public string mat_Nombre_Contrayente_Hombre_Foraneo { get; set; }
        public string mat_Nombre_Contrayente_Mujer_Foraneo { get; set; }
        public Nullable<DateTime> mat_Fecha_Boda_Civil { get; set; }
        public string mat_Numero_Acta { get; set; }
        public string mat_Libro_Acta { get; set; }
        public string mat_Oficialia { get; set; }
        public string mat_Registro_Civil { get; set; }
        public Nullable<DateTime> mat_Fecha_Boda_Eclesiastica { get; set; }
        public int mat_Cantidad_Hijos { get; set; }
        public string mat_Nombre_Hijos { get; set; }
        public int dis_Id_Distrito { get; set; }
        public int sec_Id_Sector { get; set; }
        public int usu_Id_Usuario { get; set; }
        public DateTime Fecha_Registro { get; set; }
    }
}
