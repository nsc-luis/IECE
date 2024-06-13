using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IECE_WebApi.Models
{
    public class Informe
    {
        [Key]
        public int IdInforme { get; set; }
        public int IdTipoUsuario { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int IdDistrito { get; set; }
        public int? IdSector { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string LugarReunion {  get; set; }
        public DateTime FechaReunion { get; set; }
        public int Status { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
