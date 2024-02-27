using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public partial class MovimientoEconomico
    {
        [Key]
        public int IdMovimientoEconomico { get; set; }
        public int IdInforme { get; set; }
        public decimal? ExistenciaAnterior { get; set; }
        public decimal? EntradaMes { get; set; }
        public decimal? SumaTotal { get; set; }
        public decimal? GastosAdmon { get; set; }
        public decimal? TransferenciasAentidadSuperior { get; set; }
        public decimal? ExistenciaEnCaja { get; set; }
        public int Usu_Id_Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
