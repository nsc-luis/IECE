using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Nota
    {
        [Key]
        public int n_Id { get; set; }
        public int vp_Id_Visitante { get; set; }
        public string n_Nota { get; set; }
        public DateTime n_Fecha_Nota { get; set; }
        public DateTime? Fecha_Registro { get; set; }
        public int usu_Id_Usuario { get; set; }
    }
}
