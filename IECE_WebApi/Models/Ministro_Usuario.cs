using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Ministro_Usuario
    {
        [Key]
        public int mu_Id { get; set; }
        public string mu_aspNetUsers_Id { get; set; }
        public int mu_pem_Id_Pastor { get; set; }
        public string mu_permiso { get; set; }
    }
}
