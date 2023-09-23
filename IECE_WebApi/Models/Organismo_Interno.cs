﻿using System;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class Organismo_Interno
    {
        [Key]
        public int org_Id { get; set; }
        public bool org_Activo { get; set; }
        public int org_Id_Sector { get; set; }
        public string org_Tipo_Organismo { get; set; }
        public string org_Categoria { get; set; }
        public string org_Nombre { get; set; }
        public Nullable<DateTime> org_Fecha_Organizacion { get; set; }
        public Nullable<DateTime> org_Fecha_Captura { get; set; }
        public Nullable<int> org_Usuario { get; set; }
    }
}
