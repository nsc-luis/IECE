using System;

namespace IECE_WebApi.Models
{
    public class Comisiones
    {
        public int com_Id_Comision { get; set; }
        public string com_Nombre { get; set; }
        public int com_Id_Persona { get; set; }
        public DateTime com_Fecha_De_Inicio { get; set; }
        public int sec_Id_Sector { get; set; }
        public int dis_Id_Distrito { get; set; }
        public int pem_Id_Ministro { get; set; }
    }
}
