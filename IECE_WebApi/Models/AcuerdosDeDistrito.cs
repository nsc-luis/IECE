using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class AcuerdosDeDistrito
    {
        [Key]
        public int IdAcuerdoDeDistrito { get; set; }
        public int IdInforme { get; set; }
        public string Descripcion { get; set; }
        public int? NumDeOrdenDeAcuerdo { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
