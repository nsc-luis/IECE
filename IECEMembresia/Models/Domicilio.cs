using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IECEMembresia.Models
{
    [Table("Domicilio", Schema = "dbo")]
    public class Domicilio
    {
        [Key]
        public int dom_Id_Domicilio { get; set; }
        public string dom_Calle { get; set; }
        public int dom_Numero_Exterior { get; set; }
        public string dom_Numero_Interior { get; set; }
        public string dom_Tipo_Subdivision { get; set; }
        public string dom_Subdivision_ { get; set; }
        public string dom_Localidad { get; set; }
        public string dom_Municipio_Cuidad { get; set; }
        public int pais_Id_Pais { get; set; }
        public int est_Id_Estado { get; set; }
        public string dom_Telefono { get; set; }

    }
}
