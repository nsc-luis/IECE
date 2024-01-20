using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IECE_WebApi.Models
{
    [Table("Mision_Sector",Schema = "dbo")]
    public class MisionSector
    {
        [Key]
        public int Ms_Id { get; set; }
        public int Ms_Numero { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string Ms_Alias { get; set; }
        public bool Ms_Activo { get; set; }
        public int Sec_Id_Sector { get; set; }
        public DateTime Fecha_Captura { get; set; }
        public int Usu_Usuario { get; set; }
    }
}
