using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Persona
    {
        [Key]
        [Required]
        public int per_Id_Persona { get; set; }
        [Required]
        [Display(Name = "Activo")]
        [DefaultValue("true")]
        public bool per_Activo { get; set; }
        [Required]
        [Display(Name = "Comunion")]
        [DefaultValue("true")]
        public bool per_En_Comunion { get; set; }
        [Required]
        [Display(Name = "Vivo")]
        [DefaultValue("true")]
        public bool per_Vivo { get; set; }
        [Required]
        [Display(Name = "Visibilidad")]
        [DefaultValue("true")]
        public bool per_Visibilidad_Abierta { get; set; }
        public int sec_Id_Sector { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public string per_Categoria { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string per_Nombre { get; set; }
        [Required]
        [Display(Name = "Apellido paterno")]
        public string per_Apellido_Paterno { get; set; }
        [Required]
        [Display(Name = "Apellido materno")]
        public string per_Apellido_Materno { get; set; }
        [Required]
        [Display(Name = "Fecha nacimiento")]
        public DateTime per_Fecha_Nacimiento { get; set; }
        [Required]
        [Display(Name = "RFC")]
        public string per_RFC_Sin_Homo { get; set; }
        public string per_Nombre_Padre { get; set; }
        public string per_Nombre_Madre { get; set; }
        public string per_Nombre_Abuelo_Paterno { get; set; }
        public string per_Nombre_Abuela_Paterna { get; set; }
        public string per_Nombre_Abuelo_Materno { get; set; }
        public string per_Nombre_Abuela_Materna { get; set; }
        public int pro_Id_Profesion_Oficio1 { get; set; }
        public int pro_Id_Profesion_Oficio2 { get; set; }
        public string per_Telefono_Fijo { get; set; }
        public string per_Telefono_Movil { get; set; }
        public string per_Email_Personal { get; set; }
        [Required]
        [Display(Name = "Foto")]
        public string per_foto { get; set; }
        public string per_Observaciones { get; set; }
        [Required]
        [Display(Name = "Bautizado")]
        public bool per_Bautizado { get; set; }
        [Display(Name = "Lugar de bautismo")]
        public string per_Lugar_Bautismo { get; set; }
        public DateTime per_Fecha_Bautismo { get; set; }
        public string per_Ministro_Que_Bautizo { get; set; }
        public DateTime per_Fecha_Recibio_Espiritu_Santo { get; set; }
        public string per_Bajo_Imposicion_De_Manos { get; set; }
        public string per_Cargos_Desempenados { get; set; }
        public string per_Cambios_De_Domicilio { get; set; }
        [Required]
        [Display(Name = "Estado civil")]
        public string per_Estado_Civil { get; set; }
        public string per_Nombre_Conyuge { get; set; }
        public DateTime per_Fecha_Boda_Civil { get; set; }
        public string per_Num_Acta_Boda_Civil { get; set; }
        public string per_Libro_Acta_Boda_Civil { get; set; }
        public string per_Oficialia_Boda_Civil { get; set; }
        public string per_Registro_Civil { get; set; }
        public DateTime per_Fecha_Boda_Eclesiastica { get; set; }
        public string per_Lugar_Boda_Eclesiastica { get; set; }
        public int per_Cantidad_Hijos { get; set; }
        public string per_Nombre_Hijos { get; set; }
    }
}
