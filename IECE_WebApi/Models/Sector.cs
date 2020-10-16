using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Sector
    {
        [Key]
        public int sec_Id_Sector { get; set; }
        public string sec_Tipo { get; set; }
        public int sec_Numero { get; set; }
        public string sec_Localidad { get; set; }
        public int dis_Id_Distrito { get; set; }
        public int pem_Id_Miembro_Pastor { get; set; }
        public int pem_Id_Miembro_Secretario { get; set; }
        public int pem_Id_Miembro_Tesorero { get; set; }
        public string sec_Soc_Femenil { get; set; }
        public DateTime sec_Fecha_Org_Soc_Femenil { get; set; }
        public string sec_Depto_Femenil { get; set; }
        public DateTime sec_Fecha_Org_Depto_Femenil { get; set; }
        public string sec_Soc_Juvenil { get; set; }
        public DateTime sec_Fecha_Org_Soc_Juvenil { get; set; }
        public string sec_Depto_Juvenil { get; set; }
        public DateTime sec_Fecha_Org_Dpto_Juvenil { get; set; }
        public string sec_Depto_Infantil { get; set; }
        public DateTime sec_Fecha_Org_Depto_Infantil { get; set; }
        public string sec_Coro_Oficial { get; set; }
        public bool sw_Registro { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public int usu_Id_Usuario { get; set; }
    }
}
