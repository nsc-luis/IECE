using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class CultosMisionSector
    {
        [Key]
        public int IdCultosMisionSector {  get; set; }
        public int IdInforme { get; set; }
        public int Ms_Id_MisionSector { get; set; }
        public int Cultos {  get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
