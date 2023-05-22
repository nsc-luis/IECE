using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;


namespace IECE_WebApi.Models
{
    public class Registro_Transacciones
    {
        [Key]
        public int mov_ID { get; set; }
        public string mov_Tipo_Mov { get; set; }
        public string mov_Detalle { get; set; }
        public string mov_Comentario { get; set; }
        public DateTime? mov_Fecha_Mov { get; set; }
        public int mov_Id_Ministro { get; set; }
        public int mov_Id_Sector { get; set; }
        public string mov_Sector { get; set; }
        public int mov_Id_Distrito { get; set; }
        public string mov_Distrito { get; set; }
        public int mov_Id_Ejecutor_Mov { get; set; }
        public DateTime? mov_Fecha_Captura { get; set; }
        public int mov_Usuario { get; set; }

    }
}
