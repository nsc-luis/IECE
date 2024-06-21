using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class SesionesReunionesDistrito2
    {
        [Key]
        public int IdSesionReunionDistrito { get; set; }
        public int IdInforme { get; set; }
        public int IdTipoSesionReunion { get; set; }
        public int? EnElDistrito { get; set; } = 0;
        public int? ConElPersonalDocente { get; set; } = 0;
        public int? ConSociedadesFemeniles { get; set; } = 0;
        public int? ConSociedadesJuveniles { get; set; } = 0;
        public int? ConDepartamentosInfantiles { get; set; } = 0;
        public int? ConCorosYGruposDeCanto { get; set; } = 0;
        public int usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
