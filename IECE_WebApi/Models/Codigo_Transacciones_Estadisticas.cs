using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Codigo_Transacciones_Estadisticas
    {
        [Key]
        public int ct_Id_Codigo { get; set; }
        public string ct_Categoria { get; set; }
        public string ct_Grupo { get; set; }
        public string ct_Tipo { get; set; }
        public string ct_Subtipo { get; set; }
        // LOS VALORES NUMERICOS SI SE ESPECIFICAN EN LA BASE DE DATOS
        // COMO NULOS SE DEBE ABREGAR Nullable<tipo_de_variable>
        // ESTO SE DEBE HACER CON LOS VALORES NULOS DE NUMERO, FECHA Y BOOLEANOS
        public Nullable<int> ct_Codigo { get; set; }
    }
}
