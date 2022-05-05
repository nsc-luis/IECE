using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class AltaCambioDomicilioRestitucionReactivacion_HogarExistente
    {
        [Key]
        public int id { get; set; }
        public int per_Id_Persona { get; set; }
        public int sec_Id_Sector { get; set; }
        public int ct_Codigo_Transaccion { get; set; }
        public int Usu_Usuario_Id { get; set; }
        public DateTime hte_Fecha_Transaccion { get; set; }
        public string hte_Comentario { get; set; }
        public int jerarquia { get; set; }
        public int hp_Id_Hogar_Persona { get; set; }
    }
}
