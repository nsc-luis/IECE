using DocumentFormat.OpenXml.Math;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Registro_TransaccionesController : ControllerBase
    {
        private readonly AppDbContext context;
        public Registro_TransaccionesController(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;

        public class FiltroHistTransEstDelMes
        {
            public int sec_Id_Sector { get; set; }
            public int year { get; set; }
            public int mes { get; set; }
        }

        public class BautizadosByMesSector
        {
            public int adulto_hombre { get; set; }
            public int adulto_mujer { get; set; }
            public int joven_hombre { get; set; }
            public int joven_mujer { get; set; }
        }

        public class NoBautizadosByMesSector
        {
            public int joven_hombre { get; set; }
            public int joven_mujer { get; set; }
            public int nino { get; set; }
            public int nina { get; set; }
        }

        public class HistTransEstBySectorMes
        {
            public class altas
            {
                public class bautizados
                {
                    public int BAUTISMO { get; set; }
                    public int RESTITUCIÓN { get; set; }
                    public int CAMBIODEDOMINTERNO { get; set; }
                    public int CAMBIODEDOMEXTERNO { get; set; }
                    public int ALTAHOGAR { get; set; }
                }
                public class noBautizados
                {
                    public int NUEVOINGRESO { get; set; }
                    public int CAMBIODEDOMINTERNO { get; set; }
                    public int CAMBIODEDOMEXTERNO { get; set; }
                    public int REACTIVACION { get; set; }
                }
            }
            public class bajas
            {
                public class bautizados
                {
                    public int DEFUNCION { get; set; }
                    public int EXCOMUNIONTEMPORAL { get; set; }
                    public int EXCOMUNION { get; set; }
                    public int CAMBIODEDOMINTERNO { get; set; }
                    public int CAMBIODEDOMEXTERNO { get; set; }
                    public int BAJAHOGAR { get; set; }
                }
                public class noBautizados
                {
                    public int DEFUNCION { get; set; }
                    public int ALEJAMIENTO { get; set; }
                    public int CAMBIODEDOMICILIOINTERNO { get; set; }
                    public int CAMBIODEDOMICILIOEXTERNO { get; set; }
                    public int PASAAPERSONALBAUTIZADO { get; set; }
                    public int PORBAJADEPADRES { get; set; }
                }
            }
        }


        // METODO PARA ALTA DE REGISTRO HISTORICO DE TRANSACCIONES MINISTERIALES
        [HttpPost]
        [Route("[action]/{pem_Id_Ministro}/{sec_Id_Sector}/{mov_Tipo_Mov}/{mov_Detalle}/{mov_Comentario}/{hte_Fecha_Transaccion=}/{mov_Ejecutor_Id}/{Usu_Usuario_Id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult RegistroHistorico(
            int pem_Id_Ministro,
            int sec_Id_Sector,
            string mov_Tipo_Mov,
            string mov_Detalle,
            string mov_Comentario,
            DateTime? hte_Fecha_Transaccion,
            int mov_Ejecutor_Id,
            int Usu_Usuario_Id
        )
        {
            try
            {
                var query = (from s in context.Sector
                             join d in context.Distrito
                             on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where s.sec_Id_Sector == sec_Id_Sector
                             select new
                             {
                                 s.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Id_Sector,
                                 s.sec_Alias
                             }).ToList();
                Registro_Transacciones nvoRegistro = new Registro_Transacciones();

                nvoRegistro.mov_Tipo_Mov = mov_Tipo_Mov;
                nvoRegistro.mov_Detalle = mov_Detalle;
                nvoRegistro.mov_Comentario = mov_Comentario;
                nvoRegistro.mov_Fecha_Mov = hte_Fecha_Transaccion;
                nvoRegistro.mov_Id_Ministro = pem_Id_Ministro;
                nvoRegistro.mov_Id_Distrito = query[0].dis_Id_Distrito;
                nvoRegistro.mov_Distrito = query[0].dis_Alias;
                nvoRegistro.mov_Id_Sector = query[0].sec_Id_Sector;
                nvoRegistro.mov_Sector = query[0].sec_Alias;
                nvoRegistro.mov_Id_Ejecutor_Mov = mov_Ejecutor_Id;
                nvoRegistro.mov_Usuario = Usu_Usuario_Id;
                nvoRegistro.mov_Fecha_Captura = fechayhora;


                // ALTA DE REGISTRO PARA HISTORICO
                context.Registro_Transacciones.Add(nvoRegistro);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    registro = nvoRegistro
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }

        // METODO PARA REPORTE DE MOVIMIENTOS ESTADISTICOS POR MES POR SECTOR
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult movimientosEstadisticosReporteBySector([FromBody] FiltroHistTransEstDelMes fhte)
        {
            try
            {
                SubConsultas subConsultas = new SubConsultas(context);
                SubConsultas.movimientosEstadisticosReporteBySector consulta = new SubConsultas.movimientosEstadisticosReporteBySector();
                consulta = subConsultas.SubMovimientosEstadisticosReporteBySector(fhte);

                DateTime fechaInicial = new DateTime(fhte.year, fhte.mes, 1);
                DateTime fechaFinal = fechaInicial.AddMonths(1);
                fechaFinal = fechaFinal.AddDays(-1);
                Historial_Transacciones_EstadisticasController.FechasSectorDistrito fsd = new Historial_Transacciones_EstadisticasController.FechasSectorDistrito
                {
                    idSectorDistrito = fhte.sec_Id_Sector,
                    fechaInicial = fechaInicial,
                    fechaFinal = fechaFinal
                };

                var detalle = subConsultas.SubHistorialPorFechaSector(fsd);

                return Ok(new
                {
                    status = "success",
                    consulta.personasBautizadas,
                    consulta.personasNoBautizadas,
                    consulta.personasBautizadasAlFinalDelMes,
                    consulta.personasNoBautizadasAlFinalDelMes,
                    consulta.hogares,
                    consulta.hogaresAlFinalDelMes,
                    consulta.matrimonios,
                    consulta.legalizaciones,
                    consulta.presentaciones,
                    consulta.altasBautizados,
                    consulta.altasNoBautizados,
                    consulta.bajasBautizados,
                    consulta.bajasNoBautizados,
                    consulta.hombresBautizados,
                    consulta.mujeresBautizadas,
                    consulta.jovenesHombresBautizados,
                    consulta.jovenesMujeresBautizadas,
                    consulta.jovenesHombresNoBautizados,
                    consulta.jovenesMujeresNoBautizadas,
                    consulta.ninos,
                    consulta.ninas,
                    consulta.bautizadosByMesSector,
                    consulta.noBautizadosByMesSector,
                    detalle
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }
    }
}
