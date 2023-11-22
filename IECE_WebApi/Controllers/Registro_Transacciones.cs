using DocumentFormat.OpenXml.Math;
using IECE_WebApi.Contexts;
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
                // preparacion del mes del cual se solicita el reporte
                var mesSiguienteDelResporte = new DateTime(fhte.year, fhte.mes + 1, 1);
                DateTime mesActualDelReporte = mesSiguienteDelResporte.AddDays(-1);

                // personas del sector activas y vivas hasta el mes de consulta
                var personas = (from p in context.Persona
                                where p.sec_Id_Sector == fhte.sec_Id_Sector
                                && p.per_Vivo == true
                                && p.per_Activo == true
                                select p).ToList();

                // PERSONAS BAUTIZAS Y EN COMUNION HASTA EL MES DE CONSULTA
                var pb = personas.Where(
                    p => p.per_Bautizado == true
                    && p.per_En_Comunion == true
                    && p.per_Fecha_Bautismo <= mesActualDelReporte).ToList();

                int personasBautizadas = pb.Count;

                // hombres bautizados hasta el mes de consulta
                var hb = pb.Where(p => p.per_Categoria == "ADULTO_HOMBRE").ToList();

                // mujeres bautizadas hasta el mes de consulta
                var mb = pb.Where(p => p.per_Categoria == "ADULTO_MUJER").ToList();

                // jovenes hombres bautizados hasta el mes de consulta
                var jhb = pb.Where(p => p.per_Categoria == "JOVEN_HOMBRE").ToList();

                // jovenes mujeres bautizadas hasta el mes de consulta
                var jmb = pb.Where(p => p.per_Categoria == "JOVEN_MUJER").ToList();

                // PERSONAS NO BAUTIZAS HASTA EL MES DE CONSULTA
                var pnb = personas.Where(
                    p => p.per_Bautizado == false
                    && p.per_En_Comunion == false
                    && p.per_Fecha_Nacimiento <= mesActualDelReporte).ToList();

                int personasNoBautizadas = pnb.Count;

                // jovenes hombres no bautizados hasta el mes de consulta
                var jhnb = pnb.Where(p => p.per_Categoria == "JOVEN_HOMBRE").ToList();

                // jovenes mujeres no bautizadas hasta el mes de consulta
                var jmnb = pnb.Where(p => p.per_Categoria == "JOVEN_MUJER").ToList();

                // niños
                var ninos = pnb.Where(p => p.per_Categoria == "NIÑO").ToList();

                // niñas
                var ninas = pnb.Where(p => p.per_Categoria == "NIÑA").ToList();

                // HISTORIAL DE TRANSACCIONES ESTADISTICAS
                // historial transacciones estadisticas del sector y mes de consulta
                var hteDelMesConsultado = (from hte in context.Historial_Transacciones_Estadisticas
                                           where (hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                                           && hte.hte_Fecha_Transaccion <= mesActualDelReporte)
                                           && hte.sec_Sector_Id == fhte.sec_Id_Sector
                                           select hte).ToList();

                // altas bautizados del mes
                int[] codAlta = { 11001, 11002, 11003, 11004, 11005, 12001, 12002, 12003, 12004 };
                int movAltaBautizado = 0;
                foreach (var ca in codAlta)
                {
                    var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                    if (a.Count() > 0)
                    {
                        movAltaBautizado++;
                    }
                }
                // bajas bautizados del mes
                int[] codBaja = { 11101, 11102, 11103, 11004, 11005, 12101, 12102, 12103, 12104, 12105, 12106 };
                int movBajaBautizado = 0;
                foreach (var ca in codBaja)
                {
                    var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                    if (a.Count() > 0)
                    {
                        movBajaBautizado++;
                    }
                }

                // altas no bautizados del mes
                int[] codAltaNB = { 12001, 12002, 12003, 12004 };
                int movAltaNB = 0;
                foreach (var ca in codAltaNB)
                {
                    var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                    if (a.Count() > 0)
                    {
                        movAltaNB++;
                    }
                }
                // bajas no bautizados del mes
                int[] codBajaNB = { 12101, 12102, 12103, 12104, 12105, 12106 };
                int movBajaNB = 0;
                foreach (var ca in codBajaNB)
                {
                    var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                    if (a.Count() > 0)
                    {
                        movBajaNB++;
                    }
                }

                // ALTAS BAUTIZADOS
                // alta por bautismo
                int ab = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11001).ToList().Count();

                // alta por restitucion
                var ar = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11002).ToList().Count();

                // alta bautizado por cambio de domicilio interno
                var abcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11003).ToList().Count();

                // alta bautizado por cambio de domicilio externo
                var abcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11004).ToList().Count();

                // BAJAS BAUTIZADOS
                // defuncion
                var bd = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11101).ToList().Count();

                // baja excomunion temporal
                var bet = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11102).ToList().Count();

                // baja excomunion
                var be = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11103).ToList().Count();

                // baja cambio de domicilio interno
                var bcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11004).ToList().Count();

                // baja cambio de domicilio externo
                var bcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11005).ToList().Count();

                // ALTAS NO BAUTIZADOS
                // nuevo ingreso
                var anbni = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12001).ToList().Count();

                // reactivacion
                var anbr = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12004).ToList().Count();

                // alta no bautizado por cambio de domicilio interno
                var anbcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12002).ToList().Count();

                // alta bautizado por cambio de domicilio externo
                var anbce = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12003).ToList().Count();

                // BAJAS NO BAUTIZADOS
                // defuncion
                var bnbd = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12101).ToList().Count();

                // alejamiento
                var bnba = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12102).ToList().Count();

                // baja cambio de domicilio interno
                var bnbcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12103).ToList().Count();

                // baja cambio de domicilio externo
                var bnbcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12104).ToList().Count();

                // baja porque pasa a bautizado
                var bnbpb = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12105).ToList().Count();

                // baja por baja de padres
                var bnbbp = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12106).ToList().Count();

                // HOGARES
                // hogares por sector
                var hogares = (from p in personas
                               join hp in context.Hogar_Persona on p.per_Id_Persona equals hp.per_Id_Persona
                               where hp.hp_Jerarquia == 1 && p.sec_Id_Sector == fhte.sec_Id_Sector
                               select hp).ToList().Count();
                // alta hogares
                var ah = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 31001).ToList().Count();
                // baja hogares
                var bh = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 31102).ToList().Count();

                // construye resultado de la consulta
                HistTransEstBySectorMes.altas.bautizados altasBautizados = new HistTransEstBySectorMes.altas.bautizados
                {
                    BAUTISMO = ab,
                    RESTITUCIÓN = ar,
                    CAMBIODEDOMINTERNO = abcdi,
                    CAMBIODEDOMEXTERNO = abcde,
                    ALTAHOGAR = ah
                };

                HistTransEstBySectorMes.altas.noBautizados altasNoBautizados = new HistTransEstBySectorMes.altas.noBautizados
                {
                    NUEVOINGRESO = anbni,
                    REACTIVACION = anbr,
                    CAMBIODEDOMINTERNO = anbcdi,
                    CAMBIODEDOMEXTERNO = anbce
                };

                HistTransEstBySectorMes.bajas.bautizados bajasBautizados = new HistTransEstBySectorMes.bajas.bautizados
                {
                    DEFUNCION = bd,
                    EXCOMUNIONTEMPORAL = bet,
                    EXCOMUNION = be,
                    CAMBIODEDOMINTERNO = bcdi,
                    CAMBIODEDOMEXTERNO = bcde,
                    BAJAHOGAR = bh
                };

                HistTransEstBySectorMes.bajas.noBautizados bajasNoBautizados = new HistTransEstBySectorMes.bajas.noBautizados
                {
                    DEFUNCION = bnbd,
                    ALEJAMIENTO = bnba,
                    CAMBIODEDOMICILIOINTERNO = bnbcdi,
                    CAMBIODEDOMICILIOEXTERNO = bnbcde,
                    PASAAPERSONALBAUTIZADO = bnbpb,
                    PORBAJADEPADRES = bnbbp
                };

                return Ok(new
                {
                    status = "success",
                    personasBautizadas,
                    personasNoBautizadas,
                    personasBautizadasAlFinalDelMes = personasBautizadas + movAltaBautizado - movBajaBautizado,
                    personasNoBautizadasAlFinalDelMes = personasNoBautizadas + movAltaNB - movBajaNB,
                    hogares,
                    hogaresAlFinalDelMes = hogares + ah - bh,
                    altasBautizados,
                    altasNoBautizados,
                    bajasBautizados,
                    bajasNoBautizados
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
