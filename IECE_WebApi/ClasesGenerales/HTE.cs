using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.ClasesGenerales
{
    public class HTE
    {
        private readonly AppDbContext context;

        public HTE(AppDbContext context)
        {
            this.context = context;
        }
        private DateTime fechayhora = DateTime.UtcNow;

        public static string ActulizaciónGeneralPersona(int idPersona) {
            try {
                return "success";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}
