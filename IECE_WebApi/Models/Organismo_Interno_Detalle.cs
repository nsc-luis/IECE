using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace IECE_WebApi.Models
{
    public class Organismo_Interno_Detalle
    {
        [Key]
        public int oid_Id { get; set; }
        public int oid_Director { get; set; }
        public int org_Id { get; set; }
        public int oid_Presidente { get; set; }
        public int oid_Vice_Presidente { get; set; }
        public int oid_Secretario { get; set; }
        public int oid_Sub_Secretario { get; set; }
        public int oid_Tesorero { get; set; }
        public int oid_Sub_Tesorero { get; set; }
    }
}
