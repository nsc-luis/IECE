﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Templates
{
    public class TiposDeDatosParaPDF
    {
        // Objeto para plantilla ReporteMovimientosEstadisticos
        public class oReporteMovimientosEstadisticos
        {
            public DateTime FechaInicial { get; set; }
            public DateTime FechaFinal { get; set; }
            public int AdultosBautizados { get; set; }
            public int AltasBautizadosBautismo { get; set; }
            public int AltasBautizadosCambioDomicilio { get; set; }
            public int AltasBautizadosRestitucion { get; set; }
            public int AltasNoBautizadosCambioDomicilio { get; set; }
            public int AltasNoBautizadosNuevoIngreso { get; set; }
            public int AltasNoBautizadosReactivacion { get; set; }
            public int BajasBautizadosCambioDomicilio { get; set; }
            public int BajasBautizadosDefuncion { get; set; }
            public int BajasBautizadosExcomunion { get; set; }
            public int BajasNoBautizadosAlejamiento { get; set; }
            public int BajasNoBautizadosCambioDomicilio { get; set; }
            public int BajasNoBautizadosDefuncion { get; set; }
            public int BautizadosAdultoHombre { get; set; }
            public int BautizadosAdultoMujer { get; set; }
            public int BautizadosJovenHombre { get; set; }
            public int BautizadosJovenMujer { get; set; }
            public int JovenesBautizados { get; set; }
            public int JovenesNoBautizados { get; set; }
            public int Legalizaciones { get; set; }
            public int Matrimonios { get; set; }
            public int Ninas { get; set; }
            public int Ninos { get; set; }
            public int NoBautizadosJovenHombre { get; set; }
            public int NoBautizadosJovenMujer { get; set; }
            public int Presentaciones { get; set; }
            public int Total { get; set; }
            public int TotalAltasBautizados { get; set; }
            public int TotalAltasNoBautizados { get; set; }
            public int TotalBajasBautizados { get; set; }
            public int TotalBajasNoBautizados { get; set; }
            public string Secretario { get; set; }
            public string Ministro { get; set; }
            public string Transacciones { get; set; }
        }
    }
}
