using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Foto
    {
        [Key]
        public int idFoto { get; set; }
        public string guid { get; set; }
        public string extension { get; set; }
        public string path { get; set; }
        public string mimeType { get; set; }
        public int size { get; set; }
    }
}
