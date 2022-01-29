using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Historial_Transacciones_Estadisticas
    {
        [Key]
        public int hte_Id_Transaccion { get; set; }
        public bool hte_Cancelado { get; set; }
        public int dis_Distrito_Id { get; set; }
        public string dis_Distrito_Alias { get; set; }
        public int sec_Sector_Id { get; set; }
        public string sec_Sector_Alias { get; set; }
        public int ct_Codigo_Transaccion { get; set; }
        public int per_Persona_Id { get; set; }
        public string hte_Comentario { get; set; }
        public DateTime hte_Fecha_Transaccion { get; set; }
        public int Usu_Usuario_Id { get; set; }
    }
}
