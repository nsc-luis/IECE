﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Domicilio
    {
        [Key]
        [Required]
        public int dom_Id_Domicilio { get; set; }

        public string dom_Calle { get; set; }

        public string dom_Numero_Exterior { get; set; }

        public string dom_Numero_Interior { get; set; }

        public string dom_Tipo_Subdivision { get; set; }

        public string dom_Subdivision { get; set; }

        public string dom_Localidad { get; set; }

        public string dom_Municipio_Ciudad { get; set; }

        public int pais_Id_Pais { get; set; }

        public int est_Id_Estado { get; set; }

        public string dom_Codigo_Postal { get; set; }

        public DateTime dom_Fecha_Captura { get; set; }

        public int dom_Usuario { get; set; }

    }
}
