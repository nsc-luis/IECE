using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Persona
    {
        // DATOS GENERALES //
        [Key]
        public int per_Id_Persona { get; set; }
        [Required]
        [Display(Name = "Activo")]
        public bool per_Activo { get; set; }
        [Display(Name = "En comunion")]
        public bool per_En_Comunion { get; set; }
        [Required]
        [Display(Name = "Vivo")]
        public bool per_Vivo { get; set; }
        [Required]
        [Display(Name = "Sector")]
        public int sec_Id_Sector { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public string per_Categoria { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string per_Nombre { get; set; }
        [Required]
        [Display(Name = "Apellido Paterno")]
        public string per_Apellido_Paterno { get; set; }
        [Required]
        [Display(Name = "Apellido Materno")]
        public string per_Apellido_Materno { get; set; }
        [Required]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime per_Fecha_Nacimiento { get; set; }
        [Display(Name = "Padre")]
        public string per_Nombre_Padre { get; set; }
        [Display(Name = "Madre")]
        public string per_Nombre_Madre { get; set; }
        [Display(Name = "Abuelo paterno")]
        public string per_Nombre_Abuelo_Paterno { get; set; }
        [Display(Name = "Abuela paterna")]
        public string per_Nombre_Abuela_Paterna { get; set; }
        [Display(Name = "Abuelo materno")]
        public string per_Nombre_Abuelo_Materno { get; set; }
        [Display(Name = "Abuela materna")]
        public string per_Nombre_Abuela_Materna { get; set; }
        [Display(Name = "Oficio/profesion 1")]
        public int pro_Id_Profesion_Oficio1 { get; set; }
        [Display(Name = "Oficio/profesion 2")]
        public int pro_Id_Profesion_Oficio2 { get; set; }
        [Display(Name = "Telefono fijo")]
        public string per_Telefono_Fijo { get; set; }
        [Display(Name = "Telefono movil")]
        public string per_Telefono_Movil { get; set; }
        [Display(Name = "Email")]
        public string per_Email_Personal { get; set; }
        [Display(Name = "Foto")]
        public string per_foto { get; set; }
        [Required]
        [Display(Name = "Visibilidad abierta")]
        public bool per_Visibilidad_Abierta { get; set; }
        [Display(Name = "Observaciones")]
        public string per_Observaciones { get; set; }
        [Display(Name = "Cargos desempeñados")]
        public string per_Cargos_Desempenados { get; set; }

        // DATOS DE ESTADO CIVIL //
        [Required]
        [Display(Name = "Estado civil")]
        public string per_Estado_Civil { get; set; }

        // DATOS ECLESIASTICOS //
        [Required]
        [Display(Name = "Bautizado")]
        public bool per_Bautizado { get; set; }

        [Display(Name = "Fecha recibio el Espiritu Santo")]
        public DateTime per_Fecha_Recibio_Espiritu_Santo { get; set; }
        [Display(Name = "Bajo imposicion de manos")]
        public string per_Bajo_Imposicion_De_Manos { get; set; }
        [Display(Name = "Cambios de domicilio")]
        public string per_Cambios_De_Domicilio { get; set; }
        
        // CONTROL DE DATOS //
        public bool sw_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        [Required]
        public int usu_Id_Usuario { get; set; }
        
    }
}
