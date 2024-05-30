using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class TipoDistrito
    {
        [Key]
        public int Id_Tipo_Distrito { get; set; }
        public string td_Tipo_Distrito { get; set; }
        public int td_Codigo_Tipo_Distrito { get; set; }
    }
}
