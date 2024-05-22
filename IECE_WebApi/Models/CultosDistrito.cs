using System.ComponentModel.DataAnnotations;
using System;

namespace IECE_WebApi.Models
{
    public class CultosDistrito
    {
        [Key]
        public int idCultoDistrito { get; set; }
        public int IdInforme { get; set; }
        public int idSector { get; set; }
        public int Ordinarios { get; set; }
        public int Especiales { get; set; }
        public int DeAvivamiento { get; set; }
        public int Evangelismo { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
