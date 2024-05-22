using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class ConstruccionesDistrito
    {
        [Key]
        public int idConstruccionDistrito { get; set; }
        public int idInforme { get; set; }
        public int idTipoFaseConstruccion { get; set; }
        public int? colocacionPrimeraPiedra { get; set; }
        public int? templo { get; set; }
        public int? casaDeOracion { get; set; }
        public int? anexos { get; set; }
        public int? remodelacion { get; set; }
        public int usu_Id_Usuario { get; set; }
        public  DateTime fechaRegistro { get; set; }
    }
}
