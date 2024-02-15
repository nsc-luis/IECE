using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class Organizaciones
    {
        [Key]
        public int IdOrganizacion { get; set; }
        public int IdInforme { get; set; }
        public int? SociedadFemenil { get; set; }
        public int? SociedadJuvenil { get; set; }
        public int? DepartamentoFemenil { get; set; }
        public int? DepartamentoJuvenil { get; set; }
        public int? DepartamentoInfantil { get; set; }
        public int? Coros { get; set; }
        public int? GruposDeCanto { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
