using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class SesionesReunionesSector
    {
        [Key]
        public int IdSesionReunionSector { get; set; }
        public int IdInforme { get; set; }
        public int IdTipoSesionReunion { get; set; }
        public int? EnElDistrito { get; set; }
        public int? ConElPersonalDocente { get; set; }
        public int? ConSociedadesFemeniles { get; set; }
        public int? ConSociedadesJuveniles { get; set; }
        public int? ConDepartamentosInfantiles { get; set; }
        public int? ConCorosYGruposDeCanto { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
