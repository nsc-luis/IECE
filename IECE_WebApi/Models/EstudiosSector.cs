using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class EstudiosSector
    {
        [Key]
        public int IdEstudioSector {  get; set; }
        public int IdInfomre { get; set; }
        public int IdTipoEstudio {  get; set; }
        public int EscuelaDominial { get; set; }
        public int Varonil { get; set; }
        public int Femenil { get; set; }
        public int Juvenil { get; set; }
        public int Infantil { get; set; }
        public int Iglesia { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
