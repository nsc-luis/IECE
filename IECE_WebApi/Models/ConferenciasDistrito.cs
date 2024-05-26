using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class ConferenciasDistrito
    {
        [Key]
        public int IdConferenciaDistrito { get; set; }
        public int idInforme { get; set; }
        public int idSector { get; set; }
        public int? iglesia { get; set; }
        public int? sectorVaronil { get; set; }
        public int? sociedadFemenil { get; set; }
        public int? sociedadJuvenil { get; set; }
        public int? sectorInfantil { get; set; }
        public int usu_Id_Usuario { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}
