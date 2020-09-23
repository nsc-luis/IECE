using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECEMembresia.Models
{
    public class Personal
    {
        [Key]
        public int per_Id_Miembro { get; set; }
        [Required]
        public bool per_Activo { get; set; }
        public bool per_En_Comunion { get; set; }
        [Required]
        public bool per_Vivo { get; set; }
        [Required]
        public int sec_Id_Sector { get; set; }
        [Required]
        public string per_Categoria { get; set; }
        [Required]
        public string per_Nombre { get; set; }
        [Required]
        public string per_Apellido_Paterno { get; set; }
        [Required]
        public string per_Apellido_Materno { get; set; }
        public DateTime per_Fecha_Nacimiento { get; set; }
        [Required]
        public string per_Nombre_Padre { get; set; }
        public string per_Nombre_Abuelo_Paterno { get; set; }
        public string per_Nombre_Abuela_Paterna { get; set; }
        [Required]
        public string per_Estado_Civil { get; set; }
        public DateTime per_Fecha_Recibio_Espiritu_Santo { get; set; }
        public string per_Cambios_De_Domicilio { get; set; }
        public int pro_Id_Profesion_Oficio1 { get; set; }
        public int pro_Id_Profesion_Oficio2 { get; set; }
        public string per_Telefono_Fijo { get; set; }
        public string per_Telefono_Movil { get; set; }
        public string per_Email_Personal { get; set; }
        public string per_foto { get; set; }
        public bool sw_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        [Required]
        public int usu_Id_Usuario { get; set; }
        [Required]
        public bool per_Bautizado { get; set; }
        [Required]
        public bool per_Visibilidad_Abierta { get; set; }
        public string per_Nombre_Abuelo_Materno { get; set; }
        public string per_Nombre_Abuela_Materna { get; set; }
        public string per_Bajo_Imposicion_De_Manos { get; set; }
        [Required]
        public bool per_Solicitud_De_Traslado { get; set; }
    }
}
