using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Integrante_Comision_Local
    {
        [Key]
        public int Id_Integrante_Comision { get; set; }
        public bool Activo { get; set; }
        public int Sector_Id { get; set; }
        public int Comision_Id { get; set; }
        public int Integrante_Id { get; set; }
        public int Jerarquia_Integrante { get; set; }
        public string Descripcion_Adicional { get; set; }
        public DateTime? Fecha_Registro { get; set; }
    }
}
