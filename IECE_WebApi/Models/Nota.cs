using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Nota
    {
        [Key]
        public int N_Id { get; set; }
        public int Vp_Id_Visitante { get; set; }
        public string N_Nota { get; set; }
        public DateTime N_Fecha_Nota { get; set; }
        public DateTime? Fecha_Registro { get; set; }
        public int Usu_Id_Usuario { get; set; }
    }
}
