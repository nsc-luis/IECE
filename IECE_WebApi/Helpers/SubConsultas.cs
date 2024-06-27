using DocumentFormat.OpenXml.Drawing;
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
            public Informe informe { get; set; }
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
            public SesionesReunionesDistrito Sesiones { get; set; }
            public SesionesReunionesDistrito Reuniones { get; set; }
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
            var mesSiguienteDelReporte = new DateTime(fhte.year, fhte.mes, 1).AddMonths(1);
            DateTime mesActualUltimoDia = mesSiguienteDelReporte.AddDays(-1);

         //------------------------SECCIÓN PARA CALCULAR MEMBRESÍA BAUTIZADA Y NO BAUTIZADA AL INICIO DEL MES EN CONSULTA --------------------

            // Transacciones de Todas las Altas de Bautizados que ha habido en el Sector
            var altasBaseBautizados = (from hte in context.Historial_Transacciones_Estadisticas
                      join p in context.Persona on hte.per_Persona_Id equals p.per_Id_Persona
                      where hte.sec_Sector_Id == fhte.sec_Id_Sector
                      && new[] { 11001, 11002, 11003, 11004 }.Contains(hte.ct_Codigo_Transaccion)
                      select new
                      {
                          hte.hte_Id_Transaccion,
                          hte.ct_Codigo_Transaccion,
                          hte.hte_Fecha_Transaccion,
                          p.per_Categoria,
                          p.per_Id_Persona
                      }).ToList();

            // Transacciones de Todas las Altas de Bautizados que ha habido en el Sector a partir del primero del Mes en Consulta
            var altas = altasBaseBautizados.Where(hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)).ToList();

            // Filtro para saber sólo las Altas Del Mes en consulta
            var altasDelMes = altasBaseBautizados.Where(
                hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1) 
                && hte.hte_Fecha_Transaccion <= mesActualUltimoDia).ToList();

            // Transacciones de Todas las Bajas de Bautizados del Sector que ha habido en el tiempo
            var bajasBaseBautizados = (from hte in context.Historial_Transacciones_Estadisticas
                      join p in context.Persona on hte.per_Persona_Id equals p.per_Id_Persona
                      where hte.sec_Sector_Id == fhte.sec_Id_Sector
                      && new[] { 11101, 11102, 11103, 11104, 11105}.Contains(hte.ct_Codigo_Transaccion)
                      select new
                      {
                          hte.hte_Id_Transaccion,
                          hte.ct_Codigo_Transaccion,
                          hte.hte_Fecha_Transaccion,
                          p.per_Categoria,
                          p.per_Id_Persona
                      }).ToList();
            // Todas las Bajas de Bautizados que ha habido en el Sector a partir del primero del Mes en Consulta
            var bajas = bajasBaseBautizados.Where(hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)).ToList();
            // Filtro para saber sólo las Bajas Del Mes en consulta
            var bajasDelMes = bajasBaseBautizados.Where(
                hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                && hte.hte_Fecha_Transaccion <= mesActualUltimoDia).ToList();

            // Todas las personas del sector vivas y activas al día de hoy
            var personas = (from p in context.Persona
                            where p.sec_Id_Sector == fhte.sec_Id_Sector
                            && p.per_Vivo == true
                            && p.per_Activo == true
                            select p).ToList();

            // FILTRA A SÓLO PERSONAS BAUTIZADAS Y EN COMUNION AL DÍA DE HOY
            var pb = personas.Where(
                p => p.per_Bautizado == true
                && p.per_En_Comunion == true).ToList();
            // && p.per_Fecha_Bautismo < mesActualUltimoDia).ToList();

            // Personas bautizadas al principio del mes
            int personasBautizadasInicioDelMes = pb.Count() - altas.Count() + bajas.Count();
            // Personas bautizadas al final del mes
            int personasBautizadasFinDelMes = personasBautizadasInicioDelMes + altasDelMes.Count() - bajasDelMes.Count();

            //PERSONAL NO BAUTIZADOS
            // Transacciones de Todas las Altas de Bautizados que ha habido en el Sector
            var altasBaseNoBautizados = (from hte in context.Historial_Transacciones_Estadisticas
                                       join p in context.Persona on hte.per_Persona_Id equals p.per_Id_Persona
                                       where hte.sec_Sector_Id == fhte.sec_Id_Sector
                                       && new[] { 12001, 12002, 12003, 12004 }.Contains(hte.ct_Codigo_Transaccion)
                                       select new
                                       {
                                           hte.hte_Id_Transaccion,
                                           hte.ct_Codigo_Transaccion,
                                           hte.hte_Fecha_Transaccion,
                                           p.per_Categoria,
                                           p.per_Id_Persona
                                       }).ToList();

            // Transacciones de Todas las Altas de Bautizados que ha habido en el Sector a partir del primero del Mes en Consulta
            var altasNB = altasBaseNoBautizados.Where(hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)).ToList();

            // Filtro para saber sólo las Altas Del Mes en consulta
            var altasNBDelMes = altasBaseNoBautizados.Where(
                hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                && hte.hte_Fecha_Transaccion <= mesActualUltimoDia).ToList();

            // Transacciones de Todas las Bajas de Bautizados del Sector que ha habido en el tiempo
            var bajasBaseNoBautizados = (from hte in context.Historial_Transacciones_Estadisticas
                                       join p in context.Persona on hte.per_Persona_Id equals p.per_Id_Persona
                                       where hte.sec_Sector_Id == fhte.sec_Id_Sector
                                       && new[] { 12101, 12102, 12103, 12104, 12105,12106 }.Contains(hte.ct_Codigo_Transaccion)
                                       select new
                                       {
                                           hte.hte_Id_Transaccion,
                                           hte.ct_Codigo_Transaccion,
                                           hte.hte_Fecha_Transaccion,
                                           p.per_Categoria,
                                           p.per_Id_Persona
                                       }).ToList();
            // Todas las Bajas de Bautizados que ha habido en el Sector a partir del primero del Mes en Consulta
            var bajasNB = bajasBaseNoBautizados.Where(hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)).ToList();
            // Filtro para saber sólo las Bajas Del Mes en consulta
            var bajasNBDelMes = bajasBaseNoBautizados.Where(
                hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                && hte.hte_Fecha_Transaccion <= mesActualUltimoDia).ToList();

            // FILTRA PERSONAS NO BAUTIZAS AL FIN DEL MES DE CONSULTA
            var pnb = personas.Where(
                p => p.per_Bautizado == false
                && p.per_En_Comunion == false).ToList();

            // Personas bautizadas al principio del mes
            int personasNoBautizadasInicioDelMes = pnb.Count() - altasNB.Count() + bajasNB.Count();
            // Personas bautizadas al final del mes
            int personasNoBautizadasFinDelMes = personasNoBautizadasInicioDelMes + altasNBDelMes.Count() - bajasNBDelMes.Count();




            //------------------------SECCIÓN PARA CALCULAR DESGLOSE DE CATEGORIAS DE MEMBRESIA AL INICIO DEL MES EN CONSULTA --------------------
            var altasBase = (from hte in context.Historial_Transacciones_Estadisticas
                             join p in context.Persona on hte.per_Persona_Id equals p.per_Id_Persona
                             where hte.sec_Sector_Id == fhte.sec_Id_Sector
                             && new[] { 11001, 11002, 11003, 11004,12001,12002,12003,12004 }.Contains(hte.ct_Codigo_Transaccion)
                             select new
                             {
                                 hte.hte_Id_Transaccion,
                                 hte.ct_Codigo_Transaccion,
                                 hte.hte_Fecha_Transaccion,
                                 p.per_Categoria,
                                 p.per_Id_Persona
                             }).ToList();

            // Transacciones de Todas las Bajas de Bautizados del Sector que ha habido en el tiempo
           var  bajasBase = (from hte in context.Historial_Transacciones_Estadisticas
                             join p in context.Persona on hte.per_Persona_Id equals p.per_Id_Persona
                             where hte.sec_Sector_Id == fhte.sec_Id_Sector
                             && new[] { 11101, 11102, 11103, 11104, 11105,12101,12102,12103,12104,12105,12106 }.Contains(hte.ct_Codigo_Transaccion)
                             select new
                             {
                                 hte.hte_Id_Transaccion,
                                 hte.ct_Codigo_Transaccion,
                                 hte.hte_Fecha_Transaccion,
                                 p.per_Categoria,
                                 p.per_Id_Persona
                             }).ToList();

            // altasPosteriores Al Mes de Consulta
            var x9 = altasBase.Where(hte => hte.hte_Fecha_Transaccion >= mesSiguienteDelReporte).ToList();
            // bajasPosteriores Al Mes de Consulta
            var x10 = bajasBase.Where(hte => hte.hte_Fecha_Transaccion >= mesSiguienteDelReporte).ToList();

            //Variables para contabilizar Personas NB que pasaron a Bautizados y que en la consulta están ahora como Bautizados.
            int nbjhQuePasaronABautizados = 0;
            int nbjmQuePasaronABautizados = 0;

            // De la Membresía Actual se Sumará a las personas B y NB que causaron Baja despues del mes de consulta
            foreach (var x in x10)
            {
                var personasParaReactivar = (from p in context.Persona
                                             where p.per_Id_Persona == x.per_Id_Persona
                                             select p).FirstOrDefault();

                personas.Insert(0, personasParaReactivar);

                if (x.ct_Codigo_Transaccion == 12105)
                {
                    if (personasParaReactivar.per_Categoria == "JOVEN_HOMBRE")
                    {
                        nbjhQuePasaronABautizados++;
                    }
                    if (personasParaReactivar.per_Categoria == "JOVEN_MUJER")
                    {
                        nbjmQuePasaronABautizados++;
                    }
                }
            }


            // De la Membresía Actual se Eliminará a las personas  B y NB que causaron Alta despues del mes de consulta
            for (int i = personas.Count - 1; i >= 0; i--)
            {
                foreach (var x in x9)
                {
                    if (personas[i].per_Id_Persona == x.per_Id_Persona)
                    {
                        personas.RemoveAt(i);
                        break; // Salir del foreach una vez que se encuentra y elimina una coincidencia
                    }
                }
            }

            // FILTRA PERSONAS BAUTIZADAS Y EN COMUNION AL FINAL DEL MES EN CONSULTA
            var newpb = personas.Where(
                p => p.per_Bautizado == true).ToList();
            // && p.per_Fecha_Bautismo < mesActualUltimoDia).ToList();

            int personasBautizadas = newpb.Count;

            // Desglose: hombres bautizados hasta el mes de consulta
            var hb = newpb.Where(p => p.per_Categoria == "ADULTO_HOMBRE").ToList();

            // Desglose: mujeres bautizadas hasta el mes de consulta
            var mb = newpb.Where(p => p.per_Categoria == "ADULTO_MUJER").ToList();

            // Desglose: jovenes hombres bautizados hasta el mes de consulta
            var jhb = newpb.Where(p => p.per_Categoria == "JOVEN_HOMBRE").ToList();

            // Desglose: jovenes mujeres bautizadas hasta el mes de consulta
            var jmb = newpb.Where(p => p.per_Categoria == "JOVEN_MUJER").ToList();

            BautizadosByMesSector bautizadosByMesSector = new BautizadosByMesSector
            {
                adulto_hombre = hb.Count(),
                adulto_mujer = mb.Count(),
                joven_hombre = jhb.Count(),
                joven_mujer = jmb.Count()
            };

            // FILTRA PERSONAS NO BAUTIZAS AL FIN DEL MES DE CONSULTA
            var newpnb = personas.Where(
                p => p.per_Bautizado == false
                && p.per_En_Comunion == false
                && p.per_Fecha_Nacimiento <= mesActualUltimoDia).ToList();

            int personasNoBautizadas = newpnb.Count;

            // Desglose: jovenes hombres no bautizados hasta el mes de consulta
            var jhnb = newpnb.Where(p => p.per_Categoria == "JOVEN_HOMBRE").ToList();

            // Desglose: jovenes mujeres no bautizadas hasta el mes de consulta
            var jmnb = newpnb.Where(p => p.per_Categoria == "JOVEN_MUJER").ToList();

            // Desglose: niños
            var ninos = newpnb.Where(p => p.per_Categoria == "NIÑO").ToList();

            // Desglose: niñas
            var ninas = newpnb.Where(p => p.per_Categoria == "NIÑA").ToList();

            NoBautizadosByMesSector noBautizadosByMesSector = new NoBautizadosByMesSector
            {
                joven_hombre = jhnb.Count()+ nbjhQuePasaronABautizados,
                joven_mujer = jmnb.Count()+ nbjmQuePasaronABautizados,
                nino = ninos.Count(),
                nina = ninas.Count()
            };


         //------------------------SECCIÓN PARA CALCULAR MOVIMIENTO ESTADÍSTICO EN EL MES EN CONSULTA --------------------
            // historial transacciones estadisticas del sector en el mes de consulta
            var hteDelMesConsultado = (from hte in context.Historial_Transacciones_Estadisticas
                                       where (hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                                       && hte.hte_Fecha_Transaccion <= mesActualUltimoDia)
                                       && hte.sec_Sector_Id == fhte.sec_Id_Sector
                                       select hte).ToList();

            // posible duplisidad para futuras altas            

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



            // Total de altas bautizados del mes
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
            // Total de bajas bautizados del mes
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

            // Total de altas no bautizados del mes
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
            // Total de bajas no bautizados del mes
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

         //------------------------SECCIÓN PARA CALCULAR MOVIMIENTO DE HOGARES EN EL MES EN CONSULTA --------------------
            // hogares Actuales al día de hoy en el Sector
            var hogares = (from p in context.Persona
                           join hp in context.Hogar_Persona on p.per_Id_Persona equals hp.per_Id_Persona
                           where hp.hp_Jerarquia == 1 && p.sec_Id_Sector == fhte.sec_Id_Sector 
                           && p.per_Vivo == true
                           && p.per_Activo == true
                           select hp).ToList().Count();

            var hteTodas = (from hte in context.Historial_Transacciones_Estadisticas
                                       where hte.sec_Sector_Id == fhte.sec_Id_Sector
                                       select hte).ToList();

            // Transacciones de Todas las Altas de Bautizados que ha habido en el Sector a partir del primero del Mes en Consulta
            var altasHogares = hteTodas.Where(hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1) && hte.ct_Codigo_Transaccion == 31001).ToList();

            // Transacciones de Todas las Altas de Bautizados que ha habido en el Sector a partir del primero del Mes en Consulta
            var bajasHogares = hteTodas.Where(hte => hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1) && hte.ct_Codigo_Transaccion == 31102).ToList();

            // alta hogares
            var ah = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 31001).ToList().Count();
            // baja hogares
            var bh = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 31102).ToList().Count();


            var hogaresInicioDeMes = hogares - altasHogares.Count() + bajasHogares.Count();

            var hogaresFinalDelMes = hogaresInicioDeMes + ah - bh;


         //------------------------SECCIÓN PARA CALCULAR SUCESOS ESTADÍSTICOS EN EL MES EN CONSULTA --------------------
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

            //resultado.personasBautizadas = personasBautizadas;
            //resultado.personasNoBautizadas = personasNoBautizadas;
            //resultado.personasBautizadasAlFinalDelMes = personasBautizadas + movAltaBautizado - movBajaBautizado;
            //resultado.personasNoBautizadasAlFinalDelMes = personasNoBautizadas + movAltaNB - movBajaNB;
            //resultado.personasBautizadas = personasBautizadas - movAltaBautizado;
            //resultado.personasNoBautizadas = personasNoBautizadas - movAltaNB;
            //resultado.personasBautizadasAlFinalDelMes = personasBautizadas - movBajaBautizado;
            //resultado.personasNoBautizadasAlFinalDelMes = personasNoBautizadas - movBajaNB;
            resultado.personasBautizadas = personasBautizadasInicioDelMes;
            //resultado.personasNoBautizadas = personasNoBautizadas - movAltaNB;
            resultado.personasNoBautizadas = personasNoBautizadas;
            resultado.personasBautizadasAlFinalDelMes = personasBautizadasFinDelMes;
            //resultado.personasNoBautizadasAlFinalDelMes = personasNoBautizadas - movBajaNB;
            resultado.personasNoBautizadasAlFinalDelMes = personasNoBautizadasFinDelMes;
            resultado.hogares = hogares;
            resultado.hogaresAlFinalDelMes = hogaresFinalDelMes;
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
            resultado.jovenesHombresNoBautizados = jhnb.Count() + nbjhQuePasaronABautizados;
            resultado.jovenesMujeresNoBautizadas = jmnb.Count() + nbjmQuePasaronABautizados;
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
            // SE CONSULTA LISTADO DE INFORMES DE LOS SECTORES DEL DISTRITO
            List<InformePastorViewModel> informesSectores = new List<InformePastorViewModel>();

            var informes = (from i in context.Informe
                            where i.Anio == year && i.Mes == mes
                            && i.IdDistrito == idDistrito && i.IdTipoUsuario == 1
                            select new
                            {
                                i.IdInforme,
                                i.IdSector
                            }).ToList();

            // CONSULTA DE INFORME DEL OBISPO POR idInforme Y idTipoUsuario = OBISPO
            int idInformeObispo = context.Informe.FirstOrDefault(i=>i.Anio == year && i.Mes == mes && i.IdDistrito == idDistrito && i.IdTipoUsuario == 2).IdInforme;

            // CONSULTA SECTORES Y MISIONES DE DISTRITO ACTIVOS
            var sectores = context.Sector.Where(s => s.dis_Id_Distrito == idDistrito && s.sec_Activo == true).ToList();

            // PREPARA OBJETOS DE LA ACTIVDAD DEL OBISPO EN EL DISTRITO
            List<actividadEnSectorPorObispo> actividadEnSectorPorObispo = new List<actividadEnSectorPorObispo>();
            List<actividadEnMisionPorObispo> actividadEnMisionPorObispo = new List<actividadEnMisionPorObispo>();

            // INICIA EL POBLADO DEL OBJETO DE LA ACTIVADAD DEL OBISPO
            foreach (var s in sectores)
            {
                if (s.sec_Tipo_Sector == "SECTOR")
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
                else
                {
                    VisitasObispo visitasObispo = context.VisitasObispo.FirstOrDefault(vo => vo.IdInforme == idInformeObispo);
                    CultosDistrito cultosDistrito = context.CultosDistrito.FirstOrDefault(cd => cd.IdInforme == idInformeObispo);
                    ConcentracionesDistrito concentracionesDistrito = context.ConcentracionesDistrito.FirstOrDefault(c => c.idInforme == idInformeObispo);
                    ConferenciasDistrito conferenciasDistrito = context.ConferenciasDistrito.FirstOrDefault(cfd => cfd.idInforme == idInformeObispo);

                    actividadEnMisionPorObispo.Add(new actividadEnMisionPorObispo
                    {
                        mision = s,
                        VisitasObispo = visitasObispo,
                        CultosDistrito = cultosDistrito,
                        ConcentracionesDistrito = concentracionesDistrito,
                        ConferenciasDistrito = conferenciasDistrito
                    });
                }
            }

            objActividadDelObispo actividadObispo = new objActividadDelObispo
            {
                sectores = actividadEnSectorPorObispo,
                misiones = actividadEnMisionPorObispo,
                AdquisicionesDistrito = context.AdquisicionesDistrito.FirstOrDefault(ad => ad.IdInforme == idInformeObispo),
                ConstruccionesDistritoInicio = context.ConstruccionesDistrito.FirstOrDefault(cdi => cdi.idInforme == idInformeObispo && cdi.idTipoFaseConstruccion == 1),
                ConstruccionesDistritoFinal = context.ConstruccionesDistrito.FirstOrDefault(cdf => cdf.idInforme == idInformeObispo && cdf.idTipoFaseConstruccion == 2)
                //Agregar Sesiones de Distrito, Reuniones de Distrito, Dedicaciones Distrito y Regularizaciones Distrito.
            };

            // CONSULTA ACTIVIDAD DEL OBISPO DE LA SECCION MovimientoAdministrativoMaterial
            var sesiones = context.SesionesReunionesDistrito.FirstOrDefault(srd2 => srd2.IdInforme == idInformeObispo && srd2.IdTipoSesionReunion == 1);
            var reuniones = context.SesionesReunionesDistrito.FirstOrDefault(srd2 => srd2.IdInforme == idInformeObispo && srd2.IdTipoSesionReunion == 2);
            var adquisicionesDistrito = context.AdquisicionesDistrito.FirstOrDefault(ad => ad.IdInforme == idInformeObispo);
            var construccionesDistritoInicio = context.ConstruccionesDistrito.FirstOrDefault(c => c.idInforme == idInformeObispo && c.idTipoFaseConstruccion == 1);
            var construccionesDistritoFin = context.ConstruccionesDistrito.FirstOrDefault(c => c.idInforme == idInformeObispo && c.idTipoFaseConstruccion == 2);

            // INSTANCIA DE OBJETOS DE movimientosEstadisticos y movimientosAdministrativoMaterial
            // PARA SUMARIZAR TODOS LOS SECTORES
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
                Sesiones = new SesionesReunionesDistrito
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
                    usu_Id_Usuario = 0,
                    FechaRegistro = DateTime.Now
                },
                Reuniones = new SesionesReunionesDistrito
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
                    usu_Id_Usuario = 0,
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

            // CONTINUA POBLADO DE ACTIVIDADES DEL OBISPO EN LA SECCION DE MOVIMIENTO ESTADISTIVO
            // Y MATERIAL EN TABLAS CORRESPONDIENTES SEGUN EL idInforme
            smae.Sesiones = new SesionesReunionesDistrito
            {
                EnElDistrito = sesiones?.EnElDistrito,
                ConElPersonalDocente = sesiones?.ConElPersonalDocente,
                ConSociedadesFemeniles = sesiones?.ConSociedadesFemeniles,
                ConSociedadesJuveniles = sesiones?.ConSociedadesJuveniles,
                ConDepartamentosInfantiles = sesiones?.ConDepartamentosInfantiles,
                ConCorosYGruposDeCanto = sesiones?.ConCorosYGruposDeCanto
            };
            smae.Reuniones = new SesionesReunionesDistrito
            {
                EnElDistrito = reuniones?.EnElDistrito,
                ConElPersonalDocente = reuniones?.ConElPersonalDocente,
                ConSociedadesFemeniles = reuniones?.ConSociedadesFemeniles,
                ConSociedadesJuveniles = reuniones?.ConSociedadesJuveniles,
                ConDepartamentosInfantiles = reuniones?.ConDepartamentosInfantiles,
                ConCorosYGruposDeCanto = reuniones?.ConCorosYGruposDeCanto
            };
            smae.AdquisicionesSector = new AdquisicionesSector
            {
                Predios = smae.AdquisicionesSector?.Predios + adquisicionesDistrito?.Predios,
                Casas = smae.AdquisicionesSector?.Casas + adquisicionesDistrito?.Casas,
                Edificios = smae.AdquisicionesSector?.Edificios + adquisicionesDistrito?.Edificios,
                Templos = smae.AdquisicionesSector?.Templos + adquisicionesDistrito?.Templos,
                Vehiculos = smae.AdquisicionesSector?.Vehiculos + adquisicionesDistrito?.Vehiculos
            };
            smae.ConstruccionesInicio = new Construcciones
            {
                Templo = smae.ConstruccionesInicio?.Templo + construccionesDistritoInicio?.templo,
                CasaDeOracion = smae.ConstruccionesInicio?.CasaDeOracion + construccionesDistritoInicio?.casaDeOracion,
                CasaPastoral = smae.ConstruccionesInicio?.CasaPastoral + construccionesDistritoInicio?.casaPastoral,
                Anexos = smae.ConstruccionesInicio?.Anexos + construccionesDistritoInicio?.anexos,
                Remodelacion = smae.ConstruccionesInicio?.Remodelacion + construccionesDistritoInicio?.remodelacion
            };
            smae.ConstruccionesConclusion = new Construcciones
            {
                Templo = smae.ConstruccionesConclusion?.Templo + construccionesDistritoFin?.templo,
                CasaDeOracion = smae.ConstruccionesConclusion?.CasaDeOracion + construccionesDistritoFin?.casaDeOracion,
                CasaPastoral = smae.ConstruccionesConclusion?.CasaPastoral + construccionesDistritoFin?.casaPastoral,
                Anexos = smae.ConstruccionesConclusion?.Anexos + construccionesDistritoFin?.anexos,
                Remodelacion = smae.ConstruccionesConclusion?.Remodelacion + construccionesDistritoFin?.remodelacion
            };

            // INICIA CICLO PARA SUMARIZAR LA ACTIVADA DE CADA SECTOR
            foreach (var i in informes)
            {
                InformePastorViewModel tempInforme = SubInformePastoral(i.IdInforme);
                informesSectores.Add(SubInformePastoral(i.IdInforme));

                // SUMARIZA SECCION DE MovimientoAdministrativoMaterial DE CADA SECTOR
                smae.Organizaciones = new Organizaciones
                {
                    SociedadFemenil = smae.Organizaciones?.SociedadFemenil + tempInforme.Organizaciones?.SociedadFemenil,
                    SociedadJuvenil = smae.Organizaciones?.SociedadJuvenil + tempInforme.Organizaciones?.SociedadJuvenil,
                    DepartamentoFemenil = smae.Organizaciones?.DepartamentoFemenil + tempInforme.Organizaciones?.DepartamentoFemenil,
                    DepartamentoJuvenil = smae.Organizaciones?.DepartamentoJuvenil + tempInforme.Organizaciones?.DepartamentoJuvenil,
                    DepartamentoInfantil = smae.Organizaciones?.DepartamentoInfantil + tempInforme.Organizaciones?.DepartamentoInfantil,
                    Coros = smae.Organizaciones?.Coros + tempInforme.Organizaciones?.Coros,
                    GruposDeCanto = smae.Organizaciones?.GruposDeCanto + tempInforme.Organizaciones?.GruposDeCanto
                };
                smae.AdquisicionesSector = new AdquisicionesSector
                {
                    Predios = smae.AdquisicionesSector?.Predios + tempInforme.AdquisicionesSector?.Predios,
                    Casas = smae.AdquisicionesSector?.Casas + tempInforme.AdquisicionesSector?.Casas,
                    Edificios = smae.AdquisicionesSector?.Edificios + tempInforme.AdquisicionesSector?.Edificios,
                    Templos = smae.AdquisicionesSector?.Templos + tempInforme.AdquisicionesSector?.Templos,
                    Vehiculos = smae.AdquisicionesSector?.Vehiculos + tempInforme.AdquisicionesSector?.Vehiculos
                };
                smae.ConstruccionesInicio = new Construcciones
                {
                    Templo = smae.ConstruccionesInicio?.Templo + tempInforme.ConstruccionesInicio?.Templo,
                    CasaDeOracion = smae.ConstruccionesInicio?.CasaDeOracion + tempInforme.ConstruccionesInicio?.CasaDeOracion,
                    CasaPastoral = smae.ConstruccionesInicio?.CasaPastoral + tempInforme.ConstruccionesInicio?.CasaPastoral,
                    Anexos = smae.ConstruccionesInicio?.Anexos + tempInforme.ConstruccionesInicio?.Anexos,
                    Remodelacion = smae.ConstruccionesInicio?.Remodelacion + tempInforme.ConstruccionesInicio?.Remodelacion
                };
                smae.ConstruccionesConclusion = new Construcciones
                {
                    Templo = smae.ConstruccionesConclusion?.Templo + tempInforme.ConstruccionesConclusion?.Templo,
                    CasaDeOracion = smae.ConstruccionesConclusion?.CasaDeOracion + tempInforme.ConstruccionesConclusion?.CasaDeOracion,
                    CasaPastoral = smae.ConstruccionesConclusion?.CasaPastoral + tempInforme.ConstruccionesConclusion?.CasaPastoral,
                    Anexos = smae.ConstruccionesConclusion?.Anexos + tempInforme.ConstruccionesConclusion?.Anexos,
                    Remodelacion = smae.ConstruccionesConclusion?.Remodelacion + tempInforme.ConstruccionesConclusion?.Remodelacion
                };
                smae.Ordenaciones = new Ordenaciones
                {
                    Ancianos = smae.Ordenaciones?.Ancianos + tempInforme.Ordenaciones?.Ancianos,
                    Diaconos = smae.Ordenaciones?.Diaconos + tempInforme.Ordenaciones?.Diaconos
                };
                smae.Dedicaciones = new Dedicaciones
                {
                    Templos = smae.Dedicaciones?.Templos + tempInforme.Dedicaciones?.Templos,
                    CasasDeOracion = smae.Dedicaciones?.CasasDeOracion + tempInforme.Dedicaciones?.CasasDeOracion
                };
                smae.LlamamientoDePersonal = new LlamamientoDePersonal
                {
                    DiaconosAprueba = smae.LlamamientoDePersonal?.DiaconosAprueba + tempInforme.LlamamientoDePersonal?.DiaconosAprueba,
                    Auxiliares = smae.LlamamientoDePersonal?.Auxiliares + tempInforme.LlamamientoDePersonal?.Auxiliares
                };
                smae.RegularizacionPatNac = new RegularizacionPrediosTemplos
                {
                    Templos = smae.RegularizacionPatNac?.Templos + tempInforme.RegularizacionPatNac?.Templos,
                    CasasPastorales = smae.RegularizacionPatNac?.CasasPastorales | +tempInforme.RegularizacionPatNac?.CasasPastorales
                };
                smae.RegularizacionPatIg = new RegularizacionPrediosTemplos
                {
                    Templos = smae.RegularizacionPatIg?.Templos + tempInforme.RegularizacionPatIg?.Templos,
                    CasasPastorales = smae.RegularizacionPatIg?.CasasPastorales | +tempInforme.RegularizacionPatIg?.CasasPastorales
                };

                FiltroHistTransEstDelMes filtroHistTransEstDelMes = new FiltroHistTransEstDelMes
                {
                    sec_Id_Sector = i.IdSector.Value,
                    year = year,
                    mes = mes
                };

                // SUMARIZA SECCION DE MovimientoEstadistico DE CADA SECTOR
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

            // POBLANDO SECCION DE MOVIMIENTO ECONOMICO DEL DISTRITO
            var movEconomicoDistrito = context.MovimientoEconomico.FirstOrDefault(me => me.IdInforme == idInformeObispo);
            smae.MovimientoEconomico = new MovimientoEconomico
            {
                ExistenciaAnterior = smae.MovimientoEconomico?.ExistenciaAnterior + movEconomicoDistrito?.ExistenciaAnterior,
                EntradaMes = smae.MovimientoEconomico?.EntradaMes + movEconomicoDistrito?.EntradaMes,
                SumaTotal = smae.MovimientoEconomico?.SumaTotal + movEconomicoDistrito?.SumaTotal,
                GastosAdmon = smae.MovimientoEconomico?.GastosAdmon + movEconomicoDistrito?.GastosAdmon,
                TransferenciasAentidadSuperior = smae.MovimientoEconomico?.TransferenciasAentidadSuperior + movEconomicoDistrito?.TransferenciasAentidadSuperior,
                ExistenciaEnCaja = smae.MovimientoEconomico?.ExistenciaEnCaja + movEconomicoDistrito?.ExistenciaEnCaja
            };

            // POBLANDO OBJETO FINAL PARA DESPLIEGE DE INFORMACION
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