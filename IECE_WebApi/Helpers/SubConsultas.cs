using IECE_WebApi.Contexts;
using IECE_WebApi.Controllers;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using static IECE_WebApi.Controllers.Registro_TransaccionesController;

namespace IECE_WebApi.Helpers
{
    public class SubConsultas
    {
        private readonly AppDbContext context;
        public SubConsultas(AppDbContext context)
        {
            this.context = context;
        }
        public class movimientosEstadisticosReporteBySector
        {
            public int personasBautizadas { get; set; }
            public int personasNoBautizadas { get; set; }
            public int personasBautizadasAlFinalDelMes { get; set; }
            public int personasNoBautizadasAlFinalDelMes { get; set; }
            public int hogares { get; set; }
            public int hogaresAlFinalDelMes { get; set; }
            public int matrimonios { get; set; }
            public int legalizaciones { get; set; }
            public int presentaciones { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.altas.bautizados altasBautizados { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.altas.noBautizados altasNoBautizados { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.bajas.bautizados bajasBautizados { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.bajas.noBautizados bajasNoBautizados { get; set; }
            public int hombresBautizados { get; set; }
            public int mujeresBautizadas { get; set; }
            public int jovenesHombresBautizados { get; set; }
            public int jovenesMujeresBautizadas { get; set; }
            public int jovenesHombresNoBautizados { get; set; }
            public int jovenesMujeresNoBautizadas { get; set; }
            public int ninos { get; set; }
            public int ninas { get; set; }
            public virtual BautizadosByMesSector bautizadosByMesSector { get; set; }
            public virtual NoBautizadosByMesSector noBautizadosByMesSector { get; set; }
        }
        public class HistorialPorFechaSector
        {
            public int hte_Id_Transaccion { get; set; }
            public int ct_Codigo_Transaccion { get; set; }
            public string ct_Grupo { get; set; }
            public string ct_Tipo { get; set; }
            public string ct_Subtipo { get; set; }
            public string per_Nombre { get; set; }
            public string per_Apellido_Paterno { get; set; }
            public string per_Apellido_Materno { get; set; }
            public string per_Apellido_Casada { get; set; }
            public string apellidoPrincipal { get; set; }
            public bool per_Bautizado { get; set; }
            public string per_Categoria { get; set; }
            public string hte_Comentario { get; set; }
            public DateTime? hte_Fecha_Transaccion { get; set; }
            public string dis_Distrito_Alias { get; set; }
            public string sec_Sector_Alia { get; set; }
        }
        public class MonthsOfYear
        {
            public static Dictionary<int, string> months = new Dictionary<int, string> {
                    {1, "enero"},
                    {2, "febrero"},
                    {3, "marzo"},
                    {4, "abril"},
                    {5, "mayo"},
                    {6, "junio"},
                    {7, "julio"},
                    {8, "agosto"},
                    {9, "septiembre"},
                    {10, "octubre"},
                    {11, "noviembre"},
                    {12, "diciembre"}
                };
        }

        public class objInformeObispo
        {
            public objActividadDelObispo actividadObispo { get; set; }
            public List<InformePastorViewModel> InformesSectores { get; set; }
            public SumaMovtosAdministrativoEconomico MovtosAdministrativoEconomico { get; set; }
            public movimientosEstadisticosReporteBySector MovtosEstadisticos { get; set; }
        }

        public class SumaMovtosAdministrativoEconomico
        {
            public Organizaciones Organizaciones { get; set; }
            public AdquisicionesSector AdquisicionesSector { get; set; }
            //public SesionesReunionesSector Sesiones { get; set; }
            //public SesionesReunionesSector Reuniones { get; set; }
            public SesionesReunionesDistrito2 Sesiones { get; set; }
            public SesionesReunionesDistrito2 Reuniones { get; set; }
            public Construcciones ConstruccionesInicio { get; set; }
            public Construcciones ConstruccionesConclusion { get; set; }
            public Ordenaciones Ordenaciones { get; set; }
            public Dedicaciones Dedicaciones { get; set; }
            public LlamamientoDePersonal LlamamientoDePersonal { get; set; }
            public RegularizacionPrediosTemplos RegularizacionPatNac { get; set; }
            public RegularizacionPrediosTemplos RegularizacionPatIg { get; set; }
            public MovimientoEconomico MovimientoEconomico { get; set; }
        }

        public class actividadEnSectorPorObispo
        {
            public Sector sector { get; set; }
            public VisitasObispo VisitasObispo { get; set; }
            public CultosDistrito CultosDistrito { get; set; }
            public ConcentracionesDistrito ConcentracionesDistrito { get; set; }
            public ConferenciasDistrito ConferenciasDistrito { get; set; }
        }
        public class actividadEnMisionPorObispo
        {
            public Sector mision { get; set; }
            public VisitasObispo VisitasObispo { get; set; }
            public CultosDistrito CultosDistrito { get; set; }
            public ConcentracionesDistrito ConcentracionesDistrito { get; set; }
            public ConferenciasDistrito ConferenciasDistrito { get; set; }
        }

        public class objActividadDelObispo
        {
            public List<actividadEnSectorPorObispo> sectores { get; set; }
            public List<actividadEnMisionPorObispo> misiones { get; set; }
            //public SesionesReunionesDistrito SesionesDistrito { get; set; }
            //public SesionesReunionesDistrito ReunionesDistrito { get; set; }
            public AdquisicionesDistrito AdquisicionesDistrito { get; set; }
            public ConstruccionesDistrito ConstruccionesDistritoInicio { get; set; }
            public ConstruccionesDistrito ConstruccionesDistritoFinal { get; set; }
        }

        public movimientosEstadisticosReporteBySector SubMovimientosEstadisticosReporteBySector(FiltroHistTransEstDelMes fhte)
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

            BautizadosByMesSector bautizadosByMesSector = new BautizadosByMesSector
            {
                adulto_hombre = hb.Count(),
                adulto_mujer = mb.Count(),
                joven_hombre = jhb.Count(),
                joven_mujer = jmb.Count()
            };

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

            NoBautizadosByMesSector noBautizadosByMesSector = new NoBautizadosByMesSector
            {
                joven_hombre = jhnb.Count(),
                joven_mujer = jmnb.Count(),
                nino = ninos.Count(),
                nina = ninas.Count()
            };

            // HISTORIAL DE TRANSACCIONES ESTADISTICAS
            // historial transacciones estadisticas del sector y mes de consulta
            var hteDelMesConsultado = (from hte in context.Historial_Transacciones_Estadisticas
                                       where (hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                                       && hte.hte_Fecha_Transaccion <= mesActualDelReporte)
                                       && hte.sec_Sector_Id == fhte.sec_Id_Sector
                                       select hte).ToList();

            // posible duplisidad para futuras altas

            // altas bautizados del mes
            int[] codAlta = { 11001, 11002, 11003, 11004 };
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
            int[] codBaja = { 11101, 11102, 11103, 11104, 11105 };
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
            var bcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11104).ToList().Count();

            // baja cambio de domicilio externo
            var bcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11105).ToList().Count();

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

            var matrimonios = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 21001).ToList().Count();
            var legalizacion = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 21102).ToList().Count();
            var presentaciones = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 23203).ToList().Count();

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

            movimientosEstadisticosReporteBySector resultado = new movimientosEstadisticosReporteBySector();

            resultado.personasBautizadas = personasBautizadas;
            resultado.personasNoBautizadas = personasNoBautizadas;
            resultado.personasBautizadasAlFinalDelMes = personasBautizadas + movAltaBautizado - movBajaBautizado;
            resultado.personasNoBautizadasAlFinalDelMes = personasNoBautizadas + movAltaNB - movBajaNB;
            resultado.hogares = hogares;
            resultado.hogaresAlFinalDelMes = hogares + ah - bh;
            resultado.matrimonios = matrimonios;
            resultado.legalizaciones = legalizacion;
            resultado.presentaciones = presentaciones;
            resultado.altasBautizados = altasBautizados;
            resultado.altasNoBautizados = altasNoBautizados;
            resultado.bajasBautizados = bajasBautizados;
            resultado.bajasNoBautizados = bajasNoBautizados;
            resultado.hombresBautizados = hb.Count();
            resultado.mujeresBautizadas = mb.Count();
            resultado.jovenesHombresBautizados = jhb.Count();
            resultado.jovenesMujeresBautizadas = jmb.Count();
            resultado.jovenesHombresNoBautizados = jhnb.Count();
            resultado.jovenesMujeresNoBautizadas = jmnb.Count();
            resultado.ninos = ninos.Count();
            resultado.ninas = ninas.Count();
            resultado.noBautizadosByMesSector = noBautizadosByMesSector;
            resultado.bautizadosByMesSector = bautizadosByMesSector;
            // agregar 
            // sucesos estadisticos y
            // desglose de movimientos estadisticos: Historial_Transacciones_EstadisticasController.HistorialPorFechaSector

            return resultado;
        }

        public List<HistorialPorFechaSector> SubHistorialPorFechaSector(Historial_Transacciones_EstadisticasController.FechasSectorDistrito fsd)
        {
            List<HistorialPorFechaSector> resultado = new List<HistorialPorFechaSector>();
            var query = (from hte in context.Historial_Transacciones_Estadisticas
                         join cte in context.Codigo_Transacciones_Estadisticas
                         on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                         join per in context.Persona on hte.per_Persona_Id equals per.per_Id_Persona
                         where hte.sec_Sector_Id == fsd.idSectorDistrito
                         && (hte.hte_Fecha_Transaccion >= fsd.fechaInicial && hte.hte_Fecha_Transaccion <= fsd.fechaFinal)
                         orderby cte.ct_Tipo ascending
                         select new
                         {
                             hte.hte_Id_Transaccion,
                             hte.ct_Codigo_Transaccion,
                             cte.ct_Grupo,
                             cte.ct_Tipo,
                             cte.ct_Subtipo,
                             per.per_Nombre,
                             per.per_Apellido_Paterno,
                             per.per_Apellido_Materno,
                             per.per_Apellido_Casada,
                             apellidoPrincipal = (per.per_Apellido_Casada == "" || per.per_Apellido_Casada == null) ? per.per_Apellido_Paterno : (per.per_Apellido_Casada + "* " + per.per_Apellido_Paterno),
                             per.per_Bautizado,
                             per.per_Categoria,
                             hte.hte_Comentario,
                             hte.hte_Fecha_Transaccion,
                             hte.dis_Distrito_Alias,
                             hte.sec_Sector_Alias
                         }).ToList();
            foreach (var q in query)
            {
                resultado.Add(new HistorialPorFechaSector
                {
                    hte_Id_Transaccion = q.hte_Id_Transaccion,
                    ct_Codigo_Transaccion = q.ct_Codigo_Transaccion,
                    ct_Grupo = q.ct_Grupo,
                    ct_Tipo = q.ct_Tipo,
                    ct_Subtipo = q.ct_Subtipo,
                    per_Nombre = q.per_Nombre,
                    per_Apellido_Paterno = q.per_Apellido_Paterno,
                    per_Apellido_Materno = q.per_Apellido_Materno,
                    per_Apellido_Casada = q.per_Apellido_Casada,
                    apellidoPrincipal = (q.per_Apellido_Casada == "" || q.per_Apellido_Casada == null) ? q.per_Apellido_Paterno : (q.per_Apellido_Casada + "* " + q.per_Apellido_Paterno),
                    per_Bautizado = q.per_Bautizado,
                    per_Categoria = q.per_Categoria,
                    hte_Comentario = q.hte_Comentario,
                    hte_Fecha_Transaccion = q.hte_Fecha_Transaccion,
                    dis_Distrito_Alias = q.dis_Distrito_Alias,
                    sec_Sector_Alia = q.sec_Sector_Alias
                });
            }
            return resultado;
        }

        public InformePastorViewModel SubInformePastoral(int id)
        {
            //Obtiene el informe correspondiente al Id especificado
            InformePastorViewModel informeVM = new InformePastorViewModel();
            Informe informe = context.Informe
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (informe == null)
            {
                return informeVM = null;
            }

            informeVM.IdInforme = informe.IdInforme;
            informeVM.IdTipoUsuario = informe.IdTipoUsuario;
            informeVM.IdDistrito = informe.IdDistrito;
            informeVM.IdSector = informe.IdSector;
            informeVM.LugarReunion = informe.LugarReunion;
            informeVM.FechaReunion = informe.FechaReunion;
            informeVM.Status = informe.Status;
            informeVM.Usu_Id_Usuario = informe.Usu_Id_Usuario;
            informeVM.FechaRegistro = informe.FechaRegistro;
            informeVM.Mes = informe.Mes;
            informeVM.NombreMes = MonthsOfYear.months[informe.Mes];
            informeVM.Anio = informe.Anio;

            VisitasPastor visitasPastor = context.VisitasPastor
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (visitasPastor != null)
            {
                informeVM.VisitasPastor = visitasPastor;
            }

            CultosSector cultosSector = context.CultosSector
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (cultosSector != null)
            {
                informeVM.CultosSector = cultosSector;
            }

            EstudiosSector estudiosSector = context.EstudiosSector
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoEstudio == 1)
                .FirstOrDefault();

            if (estudiosSector != null)
            {
                informeVM.EstudiosSector = estudiosSector;
            }

            EstudiosSector conferenciasSector = context.EstudiosSector
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoEstudio == 2)
                .FirstOrDefault();

            if (conferenciasSector != null)
            {
                informeVM.ConferenciasSector = conferenciasSector;
            }


            TrabajoEvangelismo trabajoEvangelismo = context.TrabajoEvangelismo
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (trabajoEvangelismo != null)
            {
                informeVM.TrabajoEvangelismo = trabajoEvangelismo;
            }

            List<CultosMisionSector> cultosMisionSector = context.CultosMisionSector
                .Where(w => w.IdInforme == id)
                .ToList();

            if (cultosMisionSector != null)
            {
                informeVM.CultosMisionSector = cultosMisionSector;
            }

            Organizaciones organizaciones = context.Organizaciones
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (organizaciones != null)
            {
                informeVM.Organizaciones = organizaciones;
            }

            AdquisicionesSector adquisicionesSector = context.AdquisicionesSector
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (adquisicionesSector != null)
            {
                informeVM.AdquisicionesSector = adquisicionesSector;
            }

            SesionesReunionesSector reunionesSector = context.SesionesReunionesSector
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoSesionReunion == 1)
                .FirstOrDefault();

            if (reunionesSector != null)
            {
                informeVM.Reuniones = reunionesSector;
            }

            SesionesReunionesSector sesionesSector = context.SesionesReunionesSector
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoSesionReunion == 2)
                .FirstOrDefault();

            if (sesionesSector != null)
            {
                informeVM.Sesiones = sesionesSector;
            }

            Construcciones construccionesSectorInicio = context.Construcciones
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoFaseConstruccion == 1)
                .FirstOrDefault();

            if (construccionesSectorInicio != null)
            {
                informeVM.ConstruccionesInicio = construccionesSectorInicio;
            }

            Construcciones construccionesSectorConclusion = context.Construcciones
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoFaseConstruccion == 2)
                .FirstOrDefault();

            if (construccionesSectorConclusion != null)
            {
                informeVM.ConstruccionesConclusion = construccionesSectorConclusion;
            }

            Ordenaciones ordenaciones = context.Ordenaciones
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (ordenaciones != null)
            {
                informeVM.Ordenaciones = ordenaciones;
            }

            Dedicaciones dedicaciones = context.Dedicaciones
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (dedicaciones != null)
            {
                informeVM.Dedicaciones = dedicaciones;
            }

            LlamamientoDePersonal llamamientoDePersonal = context.LlamamientoDePersonal
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (llamamientoDePersonal != null)
            {
                informeVM.LlamamientoDePersonal = llamamientoDePersonal;
            }

            RegularizacionPrediosTemplos regularizacionPatNac = context.RegularizacionPrediosTemplos
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoPatrimonio == 1)
                .FirstOrDefault();

            if (regularizacionPatNac != null)
            {
                informeVM.RegularizacionPatNac = regularizacionPatNac;
            }

            RegularizacionPrediosTemplos regularizacionPatIg = context.RegularizacionPrediosTemplos
                .Where(w => w.IdInforme == id)
                .Where(w => w.IdTipoPatrimonio == 2)
                .FirstOrDefault();

            if (regularizacionPatIg != null)
            {
                informeVM.RegularizacionPatIg = regularizacionPatIg;
            }

            MovimientoEconomico movimientoEconomico = context.MovimientoEconomico
                .Where(w => w.IdInforme == id)
                .FirstOrDefault();

            if (movimientoEconomico != null)
            {
                informeVM.MovimientoEconomico = movimientoEconomico;
            }

            List<OtrasActividades> otrasActividades = context.OtrasActividades
                .Where(w => w.IdInforme == id)
                .ToList();

            if (otrasActividades != null)
            {
                informeVM.OtrasActividades = otrasActividades;
            }

            return informeVM;
        }

        public objInformeObispo SubInformeObispo(int idDistrito, int year, int mes)
        {
            List<InformePastorViewModel> informesSectores = new List<InformePastorViewModel>();

            var informes = (from i in context.Informe
                            where i.Anio == year && i.Mes == mes
                            && i.IdDistrito == idDistrito && i.IdTipoUsuario == 1
                            select new
                            {
                                i.IdInforme,
                                i.IdSector
                            }).ToList();

            int idInformeObispo = context.Informe.FirstOrDefault(i=>i.Anio == year && i.Mes == mes && i.IdDistrito == idDistrito && i.IdTipoUsuario == 2).IdInforme;

            var sectores = context.Sector.Where(s => s.dis_Id_Distrito == idDistrito).ToList();
            List<Mision_Sector> misiones = new List<Mision_Sector>();
            List<actividadEnSectorPorObispo> actividadEnSectorPorObispo = new List<actividadEnSectorPorObispo>();
            List<actividadEnMisionPorObispo> actividadEnMisionPorObispo = new List<actividadEnMisionPorObispo>();

            foreach (var s in sectores.Where(sec => sec.sec_Tipo_Sector == "SECTOR"))
            {
                VisitasObispo visitasObispo = context.VisitasObispo.FirstOrDefault(vo => vo.IdInforme == idInformeObispo);
                CultosDistrito cultosDistrito = context.CultosDistrito.FirstOrDefault(cd => cd.IdInforme == idInformeObispo);
                ConcentracionesDistrito concentracionesDistrito = context.ConcentracionesDistrito.FirstOrDefault(c => c.idInforme == idInformeObispo);
                ConferenciasDistrito conferenciasDistrito = context.ConferenciasDistrito.FirstOrDefault(cfd => cfd.idInforme == idInformeObispo);

                actividadEnSectorPorObispo.Add(new actividadEnSectorPorObispo
                {
                    sector = s,
                    VisitasObispo = visitasObispo,
                    CultosDistrito = cultosDistrito,
                    ConcentracionesDistrito = concentracionesDistrito,
                    ConferenciasDistrito = conferenciasDistrito
                });
            }

            foreach (var m in sectores.Where(sec => sec.sec_Tipo_Sector == "MISIÓN"))
            {
                VisitasObispo visitasObispo = context.VisitasObispo.FirstOrDefault(vo => vo.IdInforme == idInformeObispo);
                CultosDistrito cultosDistrito = context.CultosDistrito.FirstOrDefault(cd => cd.IdInforme == idInformeObispo);
                ConcentracionesDistrito concentracionesDistrito = context.ConcentracionesDistrito.FirstOrDefault(c => c.idInforme == idInformeObispo);
                ConferenciasDistrito conferenciasDistrito = context.ConferenciasDistrito.FirstOrDefault(cfd => cfd.idInforme == idInformeObispo);

                actividadEnMisionPorObispo.Add(new actividadEnMisionPorObispo
                {
                    mision = m,
                    VisitasObispo = visitasObispo,
                    CultosDistrito = cultosDistrito,
                    ConcentracionesDistrito = concentracionesDistrito,
                    ConferenciasDistrito = conferenciasDistrito
                });
            }

            objActividadDelObispo actividadObispo = new objActividadDelObispo
            {
                sectores = actividadEnSectorPorObispo,
                misiones = actividadEnMisionPorObispo,
                AdquisicionesDistrito = context.AdquisicionesDistrito.FirstOrDefault(ad => ad.IdInforme == idInformeObispo),
                ConstruccionesDistritoInicio = context.ConstruccionesDistrito.FirstOrDefault(cdi => cdi.idInforme == idInformeObispo && cdi.idTipoFaseConstruccion == 1),
                ConstruccionesDistritoFinal = context.ConstruccionesDistrito.FirstOrDefault(cdf => cdf.idInforme == idInformeObispo && cdf.idTipoFaseConstruccion == 2)
            };

            var sesiones = context.SesionesReunionesDistrito2.FirstOrDefault(srd2 => srd2.IdInforme == idInformeObispo && srd2.IdTipoSesionReunion == 1);
            var reuniones = context.SesionesReunionesDistrito2.FirstOrDefault(srd2 => srd2.IdInforme == idInformeObispo && srd2.IdTipoSesionReunion == 2);

            movimientosEstadisticosReporteBySector resultadoMovtos = new movimientosEstadisticosReporteBySector
            {
                personasBautizadas = 0,
                personasNoBautizadas = 0,
                personasBautizadasAlFinalDelMes = 0,
                personasNoBautizadasAlFinalDelMes = 0,
                hogares = 0,
                hogaresAlFinalDelMes = 0,
                matrimonios = 0,
                legalizaciones = 0,
                presentaciones = 0,
                altasBautizados = new HistTransEstBySectorMes.altas.bautizados
                {
                    BAUTISMO = 0,
                    RESTITUCIÓN = 0,
                    CAMBIODEDOMEXTERNO = 0,
                    CAMBIODEDOMINTERNO = 0,
                },
                altasNoBautizados = new HistTransEstBySectorMes.altas.noBautizados
                {
                    REACTIVACION = 0,
                    NUEVOINGRESO = 0,
                    CAMBIODEDOMINTERNO = 0,
                    CAMBIODEDOMEXTERNO = 0
                },
                bajasBautizados = new HistTransEstBySectorMes.bajas.bautizados
                {
                    DEFUNCION = 0,
                    CAMBIODEDOMEXTERNO = 0,
                    CAMBIODEDOMINTERNO = 0,
                    EXCOMUNION = 0,
                    EXCOMUNIONTEMPORAL = 0
                },
                bajasNoBautizados = new HistTransEstBySectorMes.bajas.noBautizados
                {
                    DEFUNCION = 0,
                    ALEJAMIENTO = 0,
                    CAMBIODEDOMICILIOEXTERNO = 0,
                    CAMBIODEDOMICILIOINTERNO = 0,
                    PASAAPERSONALBAUTIZADO = 0,
                    PORBAJADEPADRES = 0
                },
                hombresBautizados = 0,
                mujeresBautizadas = 0,
                jovenesHombresBautizados = 0,
                jovenesMujeresBautizadas = 0,
                jovenesHombresNoBautizados = 0,
                jovenesMujeresNoBautizadas = 0,
                ninos = 0,
                ninas = 0,
                bautizadosByMesSector = new BautizadosByMesSector
                {
                    adulto_hombre = 0,
                    adulto_mujer = 0,
                    joven_hombre = 0,
                    joven_mujer = 0
                },
                noBautizadosByMesSector = new NoBautizadosByMesSector
                {
                    nina = 0,
                    nino = 0,
                    joven_mujer = 0,
                    joven_hombre = 0
                }
            };
            SumaMovtosAdministrativoEconomico smae = new SumaMovtosAdministrativoEconomico
            {
                Organizaciones = new Organizaciones
                {
                    IdOrganizacion = 0,
                    IdInforme = 0,
                    SociedadFemenil = 0,
                    SociedadJuvenil = 0,
                    DepartamentoFemenil = 0,
                    DepartamentoJuvenil = 0,
                    DepartamentoInfantil = 0,
                    Coros = 0,
                    GruposDeCanto = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                AdquisicionesSector = new AdquisicionesSector
                {
                    IdAdquisicionSector = 0,
                    IdInforme = 0,
                    Predios = 0,
                    Casas = 0,
                    Edificios = 0,
                    Templos = 0,
                    Vehiculos = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                Sesiones = new SesionesReunionesDistrito2
                {
                    IdSesionReunionDistrito = 0,
                    IdInforme = 0,
                    IdTipoSesionReunion = 0,
                    EnElDistrito = 0,
                    ConElPersonalDocente = 0,
                    ConSociedadesFemeniles = 0,
                    ConSociedadesJuveniles = 0,
                    ConDepartamentosInfantiles = 0,
                    ConCorosYGruposDeCanto = 0,
                    UsuIdUsuario = 0,
                    FechaRegistro = DateTime.Now
                },
                Reuniones = new SesionesReunionesDistrito2
                {
                    IdSesionReunionDistrito = 0,
                    IdInforme = 0,
                    IdTipoSesionReunion = 0,
                    EnElDistrito = 0,
                    ConElPersonalDocente = 0,
                    ConSociedadesFemeniles = 0,
                    ConSociedadesJuveniles = 0,
                    ConDepartamentosInfantiles = 0,
                    ConCorosYGruposDeCanto = 0,
                    UsuIdUsuario = 0,
                    FechaRegistro = DateTime.Now
                },
                ConstruccionesInicio = new Construcciones
                {
                    IdConstruccion = 0,
                    IdInforme = 0,
                    IdTipoFaseConstruccion = 0,
                    ColocacionPrimeraPiedra = 0,
                    Templo = 0,
                    CasaDeOracion = 0,
                    CasaPastoral = 0,
                    Anexos = 0,
                    Remodelacion = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                ConstruccionesConclusion = new Construcciones
                {
                    IdConstruccion = 0,
                    IdInforme = 0,
                    IdTipoFaseConstruccion = 0,
                    ColocacionPrimeraPiedra = 0,
                    Templo = 0,
                    CasaDeOracion = 0,
                    CasaPastoral = 0,
                    Anexos = 0,
                    Remodelacion = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                Ordenaciones = new Ordenaciones
                {
                    IdOrdenacion = 0,
                    IdInforme = 0,
                    Ancianos = 0,
                    Diaconos = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                Dedicaciones = new Dedicaciones
                {
                    IdDedicacion = 0,
                    IdInforme = 0,
                    Templos = 0,
                    CasasDeOracion = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                LlamamientoDePersonal = new LlamamientoDePersonal
                {
                    IdLlamamientoDePersonal = 0,
                    IdInforme = 0,
                    DiaconosAprueba = 0,
                    Auxiliares = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                RegularizacionPatNac = new RegularizacionPrediosTemplos
                {
                    IdRegularizacionPrediosTemplos = 0,
                    IdInforme = 0,
                    IdTipoPatrimonio = 0,
                    Templos = 0,
                    CasasPastorales = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                RegularizacionPatIg = new RegularizacionPrediosTemplos
                {
                    IdRegularizacionPrediosTemplos = 0,
                    IdInforme = 0,
                    IdTipoPatrimonio = 0,
                    Templos = 0,
                    CasasPastorales = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                MovimientoEconomico = new MovimientoEconomico
                {
                    IdMovimientoEconomico = 0,
                    IdInforme = 0,
                    ExistenciaAnterior = 0,
                    EntradaMes = 0,
                    SumaTotal = 0,
                    GastosAdmon = 0,
                    TransferenciasAentidadSuperior = 0,
                    ExistenciaEnCaja = 0,
                    Usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                }
            };

            smae.Sesiones = new SesionesReunionesDistrito2
            {
                EnElDistrito = sesiones.EnElDistrito,
                ConElPersonalDocente = sesiones.ConElPersonalDocente,
                ConSociedadesFemeniles = sesiones.ConSociedadesFemeniles,
                ConSociedadesJuveniles = sesiones.ConSociedadesJuveniles,
                ConDepartamentosInfantiles = sesiones.ConDepartamentosInfantiles,
                ConCorosYGruposDeCanto = sesiones.ConCorosYGruposDeCanto
            };
            smae.Reuniones = new SesionesReunionesDistrito2
            {
                EnElDistrito = reuniones.EnElDistrito,
                ConElPersonalDocente = reuniones.ConElPersonalDocente,
                ConSociedadesFemeniles = reuniones.ConSociedadesFemeniles,
                ConSociedadesJuveniles = reuniones.ConSociedadesJuveniles,
                ConDepartamentosInfantiles = reuniones.ConDepartamentosInfantiles,
                ConCorosYGruposDeCanto = reuniones.ConCorosYGruposDeCanto
            };

            foreach (var i in informes)
            {
                InformePastorViewModel tempInforme = SubInformePastoral(i.IdInforme);
                informesSectores.Add(SubInformePastoral(i.IdInforme));

                smae.Organizaciones = new Organizaciones
                {
                    SociedadFemenil = smae.Organizaciones.SociedadFemenil + tempInforme.Organizaciones.SociedadFemenil,
                    SociedadJuvenil = smae.Organizaciones.SociedadJuvenil + tempInforme.Organizaciones.SociedadJuvenil,
                    DepartamentoFemenil = smae.Organizaciones.DepartamentoFemenil + tempInforme.Organizaciones.DepartamentoFemenil,
                    DepartamentoJuvenil = smae.Organizaciones.DepartamentoJuvenil + tempInforme.Organizaciones.DepartamentoJuvenil,
                    DepartamentoInfantil = smae.Organizaciones.DepartamentoInfantil + tempInforme.Organizaciones.DepartamentoInfantil,
                    Coros = smae.Organizaciones.Coros + tempInforme.Organizaciones.Coros,
                    GruposDeCanto = smae.Organizaciones.GruposDeCanto + tempInforme.Organizaciones.GruposDeCanto
                };
                smae.AdquisicionesSector = new AdquisicionesSector
                {
                    Predios = smae.AdquisicionesSector.Predios + tempInforme.AdquisicionesSector.Predios,
                    Casas = smae.AdquisicionesSector.Casas + tempInforme.AdquisicionesSector.Casas,
                    Edificios = smae.AdquisicionesSector.Edificios + tempInforme.AdquisicionesSector.Edificios,
                    Templos = smae.AdquisicionesSector.Templos + tempInforme.AdquisicionesSector.Templos,
                    Vehiculos = smae.AdquisicionesSector.Vehiculos + tempInforme.AdquisicionesSector.Vehiculos
                };
                //smae.Sesiones = new SesionesReunionesSector
                //{
                //    EnElDistrito = smae.Sesiones.EnElDistrito + tempInforme.Sesiones.EnElDistrito,
                //    ConElPersonalDocente = smae.Sesiones.ConElPersonalDocente + tempInforme.Sesiones.ConElPersonalDocente,
                //    ConSociedadesFemeniles = smae.Sesiones.ConSociedadesFemeniles + tempInforme.Sesiones.ConSociedadesFemeniles,
                //    ConSociedadesJuveniles = smae.Sesiones.ConSociedadesJuveniles + tempInforme.Sesiones.ConSociedadesJuveniles,
                //    ConDepartamentosInfantiles = smae.Sesiones.ConDepartamentosInfantiles + tempInforme.Sesiones.ConDepartamentosInfantiles,
                //    ConCorosYGruposDeCanto = smae.Sesiones.ConCorosYGruposDeCanto + tempInforme.Sesiones.ConCorosYGruposDeCanto
                //};
                //smae.Reuniones = new SesionesReunionesSector
                //{
                //    EnElDistrito = smae.Sesiones.EnElDistrito + tempInforme.Sesiones.EnElDistrito,
                //    ConElPersonalDocente = smae.Sesiones.ConElPersonalDocente + tempInforme.Sesiones.ConElPersonalDocente,
                //    ConSociedadesFemeniles = smae.Sesiones.ConSociedadesFemeniles + tempInforme.Sesiones.ConSociedadesFemeniles,
                //    ConSociedadesJuveniles = smae.Sesiones.ConSociedadesJuveniles + tempInforme.Sesiones.ConSociedadesJuveniles,
                //    ConDepartamentosInfantiles = smae.Sesiones.ConDepartamentosInfantiles + tempInforme.Sesiones.ConDepartamentosInfantiles,
                //    ConCorosYGruposDeCanto = smae.Sesiones.ConCorosYGruposDeCanto + tempInforme.Sesiones.ConCorosYGruposDeCanto
                //};
                smae.ConstruccionesInicio = new Construcciones
                {
                    Templo = smae.ConstruccionesInicio.Templo + tempInforme.ConstruccionesInicio.Templo,
                    CasaDeOracion = smae.ConstruccionesInicio.CasaDeOracion + tempInforme.ConstruccionesInicio.CasaDeOracion,
                    CasaPastoral = smae.ConstruccionesInicio.CasaPastoral = tempInforme.ConstruccionesInicio.CasaPastoral,
                    Anexos = smae.ConstruccionesInicio.Anexos + tempInforme.ConstruccionesInicio.Anexos,
                    Remodelacion = smae.ConstruccionesInicio.Remodelacion + tempInforme.ConstruccionesInicio.Remodelacion
                };
                smae.ConstruccionesConclusion = new Construcciones
                {
                    Templo = smae.ConstruccionesConclusion.Templo + tempInforme.ConstruccionesConclusion.Templo,
                    CasaDeOracion = smae.ConstruccionesConclusion.CasaDeOracion + tempInforme.ConstruccionesConclusion.CasaDeOracion,
                    CasaPastoral = smae.ConstruccionesConclusion.CasaPastoral = tempInforme.ConstruccionesConclusion.CasaPastoral,
                    Anexos = smae.ConstruccionesConclusion.Anexos + tempInforme.ConstruccionesConclusion.Anexos,
                    Remodelacion = smae.ConstruccionesConclusion.Remodelacion + tempInforme.ConstruccionesConclusion.Remodelacion
                };
                smae.Ordenaciones = new Ordenaciones
                {
                    Ancianos = smae.Ordenaciones.Ancianos + tempInforme.Ordenaciones.Ancianos,
                    Diaconos = smae.Ordenaciones.Diaconos + tempInforme.Ordenaciones.Diaconos
                };
                smae.Dedicaciones = new Dedicaciones
                {
                    Templos = smae.Dedicaciones.Templos + tempInforme.Dedicaciones.Templos,
                    CasasDeOracion = smae.Dedicaciones.CasasDeOracion + tempInforme.Dedicaciones.CasasDeOracion
                };
                smae.LlamamientoDePersonal = new LlamamientoDePersonal
                {
                    DiaconosAprueba = smae.LlamamientoDePersonal.DiaconosAprueba + tempInforme.LlamamientoDePersonal.DiaconosAprueba,
                    Auxiliares = smae.LlamamientoDePersonal.Auxiliares + tempInforme.LlamamientoDePersonal.Auxiliares
                };
                smae.RegularizacionPatNac = new RegularizacionPrediosTemplos
                {
                    Templos = smae.RegularizacionPatNac.Templos + tempInforme.RegularizacionPatNac.Templos,
                    CasasPastorales = smae.RegularizacionPatNac.CasasPastorales | +tempInforme.RegularizacionPatNac.CasasPastorales
                };
                smae.RegularizacionPatIg = new RegularizacionPrediosTemplos
                {
                    Templos = smae.RegularizacionPatIg.Templos + tempInforme.RegularizacionPatIg.Templos,
                    CasasPastorales = smae.RegularizacionPatIg.CasasPastorales | +tempInforme.RegularizacionPatIg.CasasPastorales
                };
                smae.MovimientoEconomico = new MovimientoEconomico
                {
                    ExistenciaAnterior = smae.MovimientoEconomico.ExistenciaAnterior + tempInforme.MovimientoEconomico.ExistenciaAnterior,
                    EntradaMes = smae.MovimientoEconomico.EntradaMes + tempInforme.MovimientoEconomico.EntradaMes,
                    SumaTotal = smae.MovimientoEconomico.SumaTotal + tempInforme.MovimientoEconomico.SumaTotal,
                    GastosAdmon = smae.MovimientoEconomico.GastosAdmon + tempInforme.MovimientoEconomico.GastosAdmon,
                    TransferenciasAentidadSuperior = smae.MovimientoEconomico.TransferenciasAentidadSuperior + tempInforme.MovimientoEconomico.TransferenciasAentidadSuperior,
                    ExistenciaEnCaja = smae.MovimientoEconomico.ExistenciaEnCaja + tempInforme.MovimientoEconomico.ExistenciaEnCaja
                };

                FiltroHistTransEstDelMes filtroHistTransEstDelMes = new FiltroHistTransEstDelMes
                {
                    sec_Id_Sector = i.IdSector,
                    year = year,
                    mes = mes
                };

                movimientosEstadisticosReporteBySector tempMovtos = SubMovimientosEstadisticosReporteBySector(filtroHistTransEstDelMes);
                resultadoMovtos.personasBautizadas = resultadoMovtos.personasBautizadas + tempMovtos.personasBautizadas;
                resultadoMovtos.personasNoBautizadas = resultadoMovtos.personasNoBautizadas + tempMovtos.personasNoBautizadas;
                resultadoMovtos.personasBautizadasAlFinalDelMes = resultadoMovtos.personasBautizadasAlFinalDelMes + tempMovtos.personasBautizadasAlFinalDelMes;
                resultadoMovtos.personasNoBautizadasAlFinalDelMes = resultadoMovtos.personasNoBautizadasAlFinalDelMes + tempMovtos.personasNoBautizadasAlFinalDelMes;
                resultadoMovtos.hogares = resultadoMovtos.hogares + tempMovtos.hogares;
                resultadoMovtos.hogaresAlFinalDelMes = resultadoMovtos.hogaresAlFinalDelMes + tempMovtos.hogaresAlFinalDelMes;
                resultadoMovtos.matrimonios = resultadoMovtos.matrimonios + tempMovtos.matrimonios;
                resultadoMovtos.legalizaciones = resultadoMovtos.legalizaciones + tempMovtos.legalizaciones;
                resultadoMovtos.presentaciones = resultadoMovtos.presentaciones + tempMovtos.presentaciones;
                resultadoMovtos.altasBautizados = new HistTransEstBySectorMes.altas.bautizados
                {
                    BAUTISMO = resultadoMovtos.altasBautizados.BAUTISMO + tempMovtos.altasBautizados.BAUTISMO,
                    RESTITUCIÓN = resultadoMovtos.altasBautizados.RESTITUCIÓN + tempMovtos.altasBautizados.RESTITUCIÓN,
                    CAMBIODEDOMINTERNO = resultadoMovtos.altasBautizados.CAMBIODEDOMINTERNO + tempMovtos.altasBautizados.CAMBIODEDOMINTERNO,
                    CAMBIODEDOMEXTERNO = resultadoMovtos.altasBautizados.CAMBIODEDOMEXTERNO + tempMovtos.altasBautizados.CAMBIODEDOMEXTERNO,
                };
                resultadoMovtos.altasNoBautizados = new HistTransEstBySectorMes.altas.noBautizados
                {
                    REACTIVACION = resultadoMovtos.altasNoBautizados.REACTIVACION + tempMovtos.altasNoBautizados.REACTIVACION,
                    NUEVOINGRESO = resultadoMovtos.altasNoBautizados.NUEVOINGRESO + tempMovtos.altasNoBautizados.NUEVOINGRESO,
                    CAMBIODEDOMINTERNO = resultadoMovtos.altasNoBautizados.CAMBIODEDOMINTERNO + tempMovtos.altasNoBautizados.CAMBIODEDOMINTERNO,
                    CAMBIODEDOMEXTERNO = resultadoMovtos.altasNoBautizados.CAMBIODEDOMEXTERNO + tempMovtos.altasNoBautizados.CAMBIODEDOMEXTERNO
                };
                resultadoMovtos.bajasBautizados = new HistTransEstBySectorMes.bajas.bautizados
                {
                    DEFUNCION = resultadoMovtos.bajasBautizados.DEFUNCION + tempMovtos.bajasBautizados.DEFUNCION,
                    CAMBIODEDOMEXTERNO = resultadoMovtos.bajasBautizados.CAMBIODEDOMEXTERNO + tempMovtos.bajasBautizados.CAMBIODEDOMEXTERNO,
                    CAMBIODEDOMINTERNO = resultadoMovtos.bajasBautizados.CAMBIODEDOMINTERNO + tempMovtos.bajasBautizados.CAMBIODEDOMINTERNO,
                    EXCOMUNION = resultadoMovtos.bajasBautizados.EXCOMUNION + tempMovtos.bajasBautizados.EXCOMUNION,
                    EXCOMUNIONTEMPORAL = resultadoMovtos.bajasBautizados.EXCOMUNIONTEMPORAL + tempMovtos.bajasBautizados.EXCOMUNIONTEMPORAL
                };
                resultadoMovtos.bajasNoBautizados = new HistTransEstBySectorMes.bajas.noBautizados
                {
                    DEFUNCION = resultadoMovtos.bajasNoBautizados.DEFUNCION + tempMovtos.bajasNoBautizados.DEFUNCION,
                    ALEJAMIENTO = resultadoMovtos.bajasNoBautizados.ALEJAMIENTO + tempMovtos.bajasNoBautizados.ALEJAMIENTO,
                    CAMBIODEDOMICILIOEXTERNO = resultadoMovtos.bajasNoBautizados.CAMBIODEDOMICILIOEXTERNO + tempMovtos.bajasNoBautizados.CAMBIODEDOMICILIOEXTERNO,
                    CAMBIODEDOMICILIOINTERNO = resultadoMovtos.bajasNoBautizados.CAMBIODEDOMICILIOINTERNO + tempMovtos.bajasNoBautizados.CAMBIODEDOMICILIOINTERNO,
                    PASAAPERSONALBAUTIZADO = resultadoMovtos.bajasNoBautizados.PASAAPERSONALBAUTIZADO + tempMovtos.bajasNoBautizados.PASAAPERSONALBAUTIZADO,
                    PORBAJADEPADRES = resultadoMovtos.bajasNoBautizados.PORBAJADEPADRES + tempMovtos.bajasNoBautizados.PORBAJADEPADRES
                };
                resultadoMovtos.hombresBautizados = resultadoMovtos.hombresBautizados + tempMovtos.hombresBautizados;
                resultadoMovtos.mujeresBautizadas = resultadoMovtos.mujeresBautizadas + tempMovtos.mujeresBautizadas;
                resultadoMovtos.jovenesHombresBautizados = resultadoMovtos.jovenesHombresBautizados + tempMovtos.jovenesHombresBautizados;
                resultadoMovtos.jovenesMujeresBautizadas = resultadoMovtos.jovenesMujeresBautizadas + tempMovtos.jovenesMujeresBautizadas;
                resultadoMovtos.jovenesHombresNoBautizados = resultadoMovtos.jovenesHombresNoBautizados + tempMovtos.jovenesHombresNoBautizados;
                resultadoMovtos.jovenesMujeresNoBautizadas = resultadoMovtos.jovenesMujeresNoBautizadas + tempMovtos.jovenesMujeresNoBautizadas;
                resultadoMovtos.ninos = resultadoMovtos.ninos + tempMovtos.ninos;
                resultadoMovtos.ninas = resultadoMovtos.ninas + tempMovtos.ninas;
                resultadoMovtos.bautizadosByMesSector = new BautizadosByMesSector
                {
                    adulto_hombre = resultadoMovtos.bautizadosByMesSector.adulto_hombre + tempMovtos.bautizadosByMesSector.adulto_hombre,
                    adulto_mujer = resultadoMovtos.bautizadosByMesSector.adulto_mujer + tempMovtos.bautizadosByMesSector.adulto_mujer,
                    joven_mujer = resultadoMovtos.bautizadosByMesSector.joven_mujer + tempMovtos.bautizadosByMesSector.joven_mujer,
                    joven_hombre = resultadoMovtos.bautizadosByMesSector.joven_hombre + tempMovtos.bautizadosByMesSector.joven_hombre
                };
                resultadoMovtos.noBautizadosByMesSector = new NoBautizadosByMesSector
                {
                    nina = resultadoMovtos.noBautizadosByMesSector.nina + tempMovtos.noBautizadosByMesSector.nina,
                    nino = resultadoMovtos.noBautizadosByMesSector.nino + tempMovtos.noBautizadosByMesSector.nino,
                    joven_mujer = resultadoMovtos.noBautizadosByMesSector.joven_mujer + tempMovtos.noBautizadosByMesSector.joven_mujer,
                    joven_hombre = resultadoMovtos.noBautizadosByMesSector.joven_hombre + tempMovtos.noBautizadosByMesSector.joven_hombre
                };
            }

            objInformeObispo objInformeObispo = new objInformeObispo
            {
                actividadObispo = actividadObispo,
                InformesSectores = informesSectores,
                MovtosAdministrativoEconomico = smae,
                MovtosEstadisticos = resultadoMovtos
            };

            return objInformeObispo;
        }
    }
}