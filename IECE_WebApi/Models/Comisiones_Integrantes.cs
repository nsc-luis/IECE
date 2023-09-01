using System;

namespace IECE_WebApi.Models
{
    public class Comisiones_Integrantes
    {
        public int ci_Id_ComisionIntegrante { get; set; }
        public int com_Id_Comision { get; set; }
        public int per_Id_Persona { get; set; }
        public DateTime ci_Fecha_Ingreso { get; set; }
        public int pem_Id_Ministro { get; set; }
    }
}
