using System;
using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static IECE_WebApi.Templates.TiposDeDatosParaPDF;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using System.Collections.Generic;
using Spire.Doc;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using static IECE_WebApi.Controllers.Registro_TransaccionesController;
using IECE_WebApi.Helpers;
using static IECE_WebApi.Helpers.SubConsultas;
using static IECE_WebApi.Controllers.Historial_Transacciones_EstadisticasController;
using DocumentFormat.OpenXml.Office2010.Excel;
using IECE_WebApi.Models;
using System.Collections.Immutable;
using System.Globalization;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DocumentosPDFController : ControllerBase
    {
        private readonly AppDbContext context;
        public DocumentosPDFController(AppDbContext context)
        {
            this.context = context;
        }

        private void AgregarTextoAlMarcador(
            List<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart> bookmarks,
            string NombreMarcador,
            string valor,
            bool bold = false,
            bool underline = false,
            string fontFamily = "",
            string fontSize = "")
        {
            var bm = bookmarks.FirstOrDefault(bms => bms.Name == NombreMarcador);
            Run r = bm.Parent.InsertAfter(new Run(), bm);
            DocumentFormat.OpenXml.Wordprocessing.RunProperties rp = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
            if (bold)
            {
                rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Bold());
            }
            if (underline)
            {
                rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single });
            }
            if (fontFamily != "")
            {
                rp.AppendChild(new RunFonts { Ascii = fontFamily });
            }
            if (fontSize != "")
            {
                rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = new StringValue(fontSize) });
            }
            if (underline)
            {
                rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single });
            }
            r.AppendChild(rp);
            r.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text(valor));
        }

        private void AgregarListaTextosAlMarcador(
            List<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart> bookmarks,
            string NombreMarcador,
            List<string> lista,
            bool bold = false,
            bool underline = false,
            string fontFamily = "",
            string fontSize = "")
        {
        var bm = bookmarks.FirstOrDefault(bms => bms.Name == NombreMarcador);
        Run r = bm.Parent.InsertAfter(new Run(), bm);
        DocumentFormat.OpenXml.Wordprocessing.RunProperties rp = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
        if (bold)
        {
            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Bold());
        }
        if (underline)
        {
            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single });
        }
        if (fontFamily != "")
        {
            rp.AppendChild(new RunFonts { Ascii = fontFamily });
        }
        if (fontSize != "")
        {
            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = new StringValue(fontSize) });
        }
        if (underline)
        {
            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single });
        }
        r.AppendChild(rp);
        for (int i = 0; i < lista.Count; i++)
        {
            r.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text(lista[i]));
            r.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Break());

        }
        }

        private void AgregarImagenAlMarcador(
            List<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart> bookmarks,
            string NombreMarcador,
            string relationshipId)
        {

            var bm = bookmarks.FirstOrDefault(bms => bms.Name == NombreMarcador);
            Run r = bm.Parent.InsertAfter(new Run(GetImageElement(relationshipId)), bm);
        }

        private static DocumentFormat.OpenXml.Wordprocessing.Drawing GetImageElement(string relationshipId)
        {
            var element = new DocumentFormat.OpenXml.Wordprocessing.Drawing(
                new DW.Inline(
                     new DW.Extent() { Cx = 990000L, Cy = 990000L },
                     new DW.EffectExtent()
                     {
                         LeftEdge = 0L,
                         TopEdge = 0L,
                         RightEdge = 0L,
                         BottomEdge = 0L
                     },
                     new DW.DocProperties()
                     {
                         Id = (UInt32Value)1U,
                         Name = "Picture 1"
                     },
                     new DW.NonVisualGraphicFrameDrawingProperties(
                         new A.GraphicFrameLocks() { NoChangeAspect = true }),
                     new A.Graphic(
                         new A.GraphicData(
                             new PIC.Picture(
                                 new PIC.NonVisualPictureProperties(
                                     new PIC.NonVisualDrawingProperties()
                                     {
                                         Id = (UInt32Value)0U,
                                         Name = "New Bitmap Image.jpg"
                                     },
                                     new PIC.NonVisualPictureDrawingProperties()),
                                 new PIC.BlipFill(
                                     new A.Blip(
                                         new A.BlipExtensionList(
                                             new A.BlipExtension()
                                             {
                                                 Uri =
                                                    "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                             })
                                     )
                                     {
                                         Embed = relationshipId,
                                         CompressionState =
                                         A.BlipCompressionValues.Print
                                     },
                                     new A.Stretch(
                                         new A.FillRectangle())),
                                 new PIC.ShapeProperties(
                                     new A.Transform2D(
                                         new A.Offset() { X = 0L, Y = 0L },
                                         new A.Extents() { Cx = 990000L, Cy = 990000L }),
                                     new A.PresetGeometry(
                                         new A.AdjustValueList()
                                     )
                                     { Preset = A.ShapeTypeValues.Rectangle }))
                         )
                         { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                 )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = "50D07946"
                });
            return element;
        }

        //public WorksheetPart SeleccionarHojaDeTrabajo(WorkbookPart wbp, string HojaDeTrabajo)
        //{
        //    string relId = wbp.Workbook.Descendants<Sheet>().First(s => HojaDeTrabajo.Equals(s.Name)).Id;
        //    return (WorksheetPart)wbp.GetPartById(relId);
        //}

        private void RellenaCelda(Row fila, string datos, string celdaReferencia)
        {
            DocumentFormat.OpenXml.Spreadsheet.Cell celda = fila.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>().FirstOrDefault(c => c.CellReference == celdaReferencia);
            if (celda == null)
            {
                celda = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                celda.CellReference = celdaReferencia;
            }
            celda.CellValue = new CellValue(datos);
            celda.DataType = CellValues.String;
            fila.Append(celda);
        }

        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult ReporteMovimientosEstadisticos([FromBody] oReporteMovimientosEstadisticos orme)
        {
            try
            {
                var fechayhora = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");
                string pathPlantilla = $"{Environment.CurrentDirectory}\\Templates\\ReporteMovimientosEstadisticos_Plantilla.docx";

                // NOMBRE DEL PDF QUE SE CREARA
                string archivoDeSalida = $"{Environment.CurrentDirectory}\\Temp\\ReporteMovimientosEstadisticos_{fechayhora}.pdf";

                // ARCHIVO TEMPORAL EN BASE A LA PLANTILLA
                string archivoTemporal = $"{Environment.CurrentDirectory}\\Temp\\ReporteMovimientosEstadisticos_{fechayhora}.docx";

                // Create shadow File
                System.IO.File.Copy(pathPlantilla, archivoTemporal, true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(archivoTemporal, true))
                {
                    var main = wordDoc.MainDocumentPart.Document;
                    var bookmarksHeader = wordDoc.MainDocumentPart.HeaderParts.FirstOrDefault().RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    var bookmarks = main.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    AgregarTextoAlMarcador(bookmarksHeader, "FechaInicial", orme.FechaInicial, true, true);
                    AgregarTextoAlMarcador(bookmarksHeader, "FechaFinal", orme.FechaFinal, true, true);
                    AgregarTextoAlMarcador(bookmarks, "AdultosBautizados", orme.AdultosBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "AltasBautizadosBautismo", orme.AltasBautizadosBautismo.ToString());
                    AgregarTextoAlMarcador(bookmarks, "AltasBautizadosCambioDomicilio", orme.AltasBautizadosCambioDomicilio.ToString());
                    AgregarTextoAlMarcador(bookmarks, "AltasBautizadosRestitucion", orme.AltasBautizadosRestitucion.ToString());
                    AgregarTextoAlMarcador(bookmarks, "AltasNoBautizadosCambioDomicilio", orme.AltasNoBautizadosCambioDomicilio.ToString());
                    AgregarTextoAlMarcador(bookmarks, "AltasNoBautizadosNuevoIngreso", orme.AltasNoBautizadosNuevoIngreso.ToString());
                    AgregarTextoAlMarcador(bookmarks, "AltasNoBautizadosReactivacion", orme.AltasNoBautizadosReactivacion.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BajasBautizadosCambioDomicilio", orme.BajasBautizadosCambioDomicilio.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BajasBautizadosDefuncion", orme.BajasBautizadosDefuncion.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BajasBautizadosExcomunion", orme.BajasBautizadosExcomunion.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BajasNoBautizadosAlejamiento", orme.BajasNoBautizadosAlejamiento.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BajasNoBautizadosCambioDomicilio", orme.BajasNoBautizadosCambioDomicilio.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BajasNoBautizadosDefuncion", orme.BajasNoBautizadosDefuncion.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BautizadosAdultoHombre", orme.BautizadosAdultoHombre.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BautizadosAdultoMujer", orme.BautizadosAdultoMujer.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BautizadosJovenHombre", orme.BautizadosJovenHombre.ToString());
                    AgregarTextoAlMarcador(bookmarks, "BautizadosJovenMujer", orme.BautizadosJovenMujer.ToString());
                    AgregarTextoAlMarcador(bookmarks, "JovenesBautizados", orme.JovenesBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "JovenesNoBautizados", orme.JovenesNoBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Legalizaciones", orme.Legalizaciones.ToString());
                    AgregarTextoAlMarcador(bookmarks, "NoDeHogares", orme.NoDeHogares.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Altas_Hogares", orme.Altas_Hogares.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Bajas_Hogares", orme.Bajas_Hogares.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Matrimonios", orme.Matrimonios.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Ninas", orme.Ninas.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Ninos", orme.Ninos.ToString());
                    AgregarTextoAlMarcador(bookmarks, "NoBautizadosJovenHombre", orme.NoBautizadosJovenHombre.ToString());
                    AgregarTextoAlMarcador(bookmarks, "NoBautizadosJovenMujer", orme.NoBautizadosJovenMujer.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Presentaciones", orme.Presentaciones.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Total", orme.Total.ToString());
                    AgregarTextoAlMarcador(bookmarks, "TotalAltasBautizados", orme.TotalAltasBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "TotalAltasNoBautizados", orme.TotalAltasNoBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "TotalBajasBautizados", orme.TotalBajasBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "TotalBajasNoBautizados", orme.TotalBajasNoBautizados.ToString());
                    AgregarTextoAlMarcador(bookmarks, "Ministro", orme.Ministro);
                    AgregarTextoAlMarcador(bookmarks, "Secretario", orme.Secretario);
                    AgregarTextoAlMarcador(bookmarks, "Transacciones", orme.Transacciones);
                    AgregarTextoAlMarcador(bookmarks, "TotalNinos", (orme.Ninas + orme.Ninos).ToString());
                    AgregarTextoAlMarcador(bookmarks, "TotalBautizados", (orme.AdultosBautizados + orme.JovenesBautizados).ToString());
                    AgregarTextoAlMarcador(bookmarks, "TotalNoBautizados", (orme.JovenesNoBautizados + orme.Ninas + orme.Ninos).ToString());
                    main.Save();
                }

                Spire.Doc.Document document = new Spire.Doc.Document();
                document.LoadFromFile(archivoTemporal);
                document.SaveToFile(archivoDeSalida, FileFormat.PDF);
                System.IO.File.Delete(archivoTemporal);
                byte[] FileByteData = System.IO.File.ReadAllBytes(archivoDeSalida);
                return File(FileByteData, "application/pdf");
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex
                });
            }
        }

        [HttpPost]
        [Route("[action]/{id}")]
        [EnableCors("AllowOrigin")]
        //public IActionResult InformePastorPorSector([FromBody] FiltroHistTransEstDelMes ftem)
        public IActionResult InformePastorPorSector(int id)
        {
            try
            {
                SubConsultas sub = new SubConsultas(context);
                var informeVM = sub.SubInformePastoral(id);
                var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == informeVM.IdSector);
                var distrito = context.Distrito.FirstOrDefault(d => d.dis_Id_Distrito == sector.dis_Id_Distrito);
                var ministro = context.Personal_Ministerial.FirstOrDefault(pm => pm.pem_Id_Ministro == sector.pem_Id_Pastor);
                var fechayhora = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");


                var TemplateTempPath = $"{Environment.CurrentDirectory}\\IECEMembresia\\IECE_WebApi";

                // {Environment.CurrentDirectory}
                // C:\\Users\\JMR-20\\Documents\\GitHub\\IECE\\IECE_WebApi
                // C:\\Users\\victo\\source\\repos\\IECE\\IECE_WebApi
                // D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi

                string pathPlantilla = $"{TemplateTempPath}\\Templates\\InformePastorPorSector_Plantilla.docx";

                // NOMBRE DEL PDF QUE SE CREARA
                string archivoDeSalida = $"{TemplateTempPath}\\Temp\\InformePastorPorSector_{fechayhora}.pdf";

                // ARCHIVO TEMPORAL EN BASE A LA PLANTILLA
                string archivoTemporal = $"{TemplateTempPath}\\Temp\\InformePastorPorSector_{fechayhora}.docx";

                // Create shadow File
                System.IO.File.Copy(pathPlantilla, archivoTemporal, true);

                FiltroHistTransEstDelMes ftem = new FiltroHistTransEstDelMes
                {
                    sec_Id_Sector = informeVM.IdSector.Value,
                    year = informeVM.Anio,
                    mes = informeVM.Mes
                };

                // Obtener el nombre del mes en mayúsculas
                string nombreMes = informeVM.FechaReunion.ToString("MMMM", new CultureInfo("es-ES")).ToUpper();

                var movtos = sub.SubMovimientosEstadisticosReporteBySector(ftem); 
                DateTime fechaInicial = new DateTime(ftem.year, ftem.mes, 1);
                DateTime fechaFinal = fechaInicial.AddMonths(1);
                fechaFinal = fechaFinal.AddDays(-1);
                Historial_Transacciones_EstadisticasController.FechasSectorDistrito fsd = new Historial_Transacciones_EstadisticasController.FechasSectorDistrito
                {
                    idSectorDistrito = ftem.sec_Id_Sector,
                    fechaInicial = fechaInicial,
                    fechaFinal = fechaFinal
                };

                var detalle = sub.SubHistorialPorFechaSector(fsd);
                // Filtrar la lista para excluir los objetos con el código 11201.
                var detalleFiltrado = detalle.Where(obj => obj.ct_Codigo_Transaccion != 11201 && obj.ct_Codigo_Transaccion != 12201 && obj.ct_Codigo_Transaccion != 31203).ToList();
                string desglose = "";
                foreach (HistorialPorFechaSector d in detalleFiltrado)
                {
                    desglose = desglose + $"{d.ct_Tipo}-{d.ct_Subtipo}: {d.per_Nombre} {d.per_Apellido_Paterno} {d.per_Apellido_Materno} | ";
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(archivoTemporal, true))
                {
                    var main = wordDoc.MainDocumentPart.Document;
                    //var bookmarksHeader = wordDoc.MainDocumentPart.HeaderParts.FirstOrDefault().RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    var bookmarks = main.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    //ENCABEZADO
                    AgregarTextoAlMarcador(bookmarks, "noSector", (sector.sec_Numero).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "sectorAlias", (sector.sec_Alias).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "noDistrito", (distrito.dis_Numero).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "distritoAlias", (distrito.dis_Alias).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "mesReporte", (MonthsOfYear.months[ftem.mes].ToString().ToUpper()), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "añoReporte", (ftem.year).ToString(), true, true, "Aptos", "18");
                    //ACTIVIDADES DEL PERSONAL DOCENTE
                    AgregarTextoAlMarcador(bookmarks, "PorPastor", (informeVM.VisitasPastor.PorPastor).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "PorAncianosAux", (informeVM.VisitasPastor.PorAncianosAux).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "PorDiaconos", (informeVM.VisitasPastor.PorDiaconos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "PorAuxiliares", (informeVM.VisitasPastor.PorAuxiliares).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Ordinarios", (informeVM.CultosSector.Ordinarios).ToString(), false, false, "Aptos", "15"); 
                    AgregarTextoAlMarcador(bookmarks, "Especiales", (informeVM.CultosSector.Especiales).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "DeAvivamiento", (informeVM.CultosSector.DeAvivamiento).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "DeAniversario", (informeVM.CultosSector.DeAniversario).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "PorElDistrito", (informeVM.CultosSector.PorElDistrito).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EstudioEscuelaDominical", (informeVM.EstudiosSector.EscuelaDominical).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EstudioVaronil", (informeVM.EstudiosSector.Varonil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EstudioFemenil", (informeVM.EstudiosSector.Femenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EstudioJuvenil", (informeVM.EstudiosSector.Juvenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EstudioInfantil", (informeVM.EstudiosSector.Infantil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EstudioIglesia", (informeVM.EstudiosSector.Iglesia).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ConferenciaEscuelaDominical", (informeVM.ConferenciasSector.EscuelaDominical).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ConferenciaVaronil", (informeVM.ConferenciasSector.Varonil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ConferenciaFemenil", (informeVM.ConferenciasSector.Femenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ConferenciaJuvenil", (informeVM.ConferenciasSector.Juvenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ConferenciaInfantil", (informeVM.ConferenciasSector.Infantil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ConferenciaIglesia", (informeVM.ConferenciasSector.Iglesia).ToString(), false, false, "Aptos", "15");
                    List<Mision_Sector> misiones = context.Mision_Sector.Where(w => w.sec_Id_Sector == informeVM.IdSector).ToList();
                    foreach (var mision in misiones)
                    {
                        var misionConCultos = informeVM.CultosMisionSector.Where(w => w.Ms_Id_MisionSector == mision.ms_Id && w.Cultos > 0).FirstOrDefault();
                        if(misionConCultos != null)
                        {

                            AgregarTextoAlMarcador(bookmarks, $"M{mision.ms_Numero}", (mision.ms_Alias).ToString(), false, false, "Aptos", "12");
                            AgregarTextoAlMarcador(bookmarks, $"C{mision.ms_Numero}", (misionConCultos.Cultos).ToString(), false, false, "Aptos", "15");
                        }
                        else
                        {
                            AgregarTextoAlMarcador(bookmarks, $"M{mision.ms_Numero}", (mision.ms_Alias).ToString(), false, false, "Aptos", "12");
                            AgregarTextoAlMarcador(bookmarks, $"C{mision.ms_Numero}", ("0"), false, false, "Aptos", "15");
                        }
                    }
                    AgregarTextoAlMarcador(bookmarks, "HogaresVisitados", (informeVM.TrabajoEvangelismo.HogaresVisitados).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "HogaresConquistados", (informeVM.TrabajoEvangelismo.HogaresConquistados).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "CultosPorLaLocalidad", (informeVM.TrabajoEvangelismo.CultosPorLaLocalidad).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "CultosDeHogar", (informeVM.TrabajoEvangelismo.CultosDeHogar).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Campanias", (informeVM.TrabajoEvangelismo.Campanias).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "AperturaDeMisiones", (informeVM.TrabajoEvangelismo.AperturaDeMisiones).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "VisitantesPermanentes", (informeVM.TrabajoEvangelismo.VisitantesPermanentes).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "BautismosTE", (informeVM.TrabajoEvangelismo.Bautismos).ToString(), false, false, "Aptos", "15");

                    //DATOS DEL ESTADO ACTUAL DE LA IGLESIA
                    AgregarTextoAlMarcador(bookmarks, "personasBautizadasInicio", (movtos.personasBautizadas).ToString(), true, true, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "bautismo", (movtos.altasBautizados.BAUTISMO).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "altaCambioDomicilio", (movtos.altasBautizados.CAMBIODEDOMINTERNO + movtos.altasBautizados.CAMBIODEDOMEXTERNO).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "totalAltas", (movtos.altasBautizados.RESTITUCIÓN + movtos.altasBautizados.BAUTISMO + movtos.altasBautizados.CAMBIODEDOMINTERNO + movtos.altasBautizados.CAMBIODEDOMEXTERNO).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "excomunion", (movtos.bajasBautizados.EXCOMUNIONTEMPORAL + movtos.bajasBautizados.EXCOMUNION).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "defuncion", (movtos.bajasBautizados.DEFUNCION).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "bajaCambioDomicilio", (movtos.bajasBautizados.CAMBIODEDOMINTERNO + movtos.bajasBautizados.CAMBIODEDOMEXTERNO).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "totalBajas", (movtos.bajasBautizados.EXCOMUNION + movtos.bajasBautizados.EXCOMUNIONTEMPORAL + movtos.bajasBautizados.CAMBIODEDOMINTERNO + movtos.bajasBautizados.CAMBIODEDOMEXTERNO + movtos.bajasBautizados.DEFUNCION).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "matrimonios", (movtos.matrimonios).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "legalizaciones", (movtos.legalizaciones).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "presentaciones", (movtos.presentaciones).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "restitución", (movtos.altasBautizados.RESTITUCIÓN).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "hogares", (movtos.hogaresAlFinalDelMes).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "hb", (movtos.bautizadosByMesSector.adulto_hombre).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "jhb", (movtos.bautizadosByMesSector.joven_hombre).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "jhnb", (movtos.noBautizadosByMesSector.joven_hombre).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "jmb", (movtos.bautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "jmnb", (movtos.noBautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "mb", (movtos.bautizadosByMesSector.adulto_mujer).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ninas", (movtos.noBautizadosByMesSector.nina).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ninos", (movtos.noBautizadosByMesSector.nino).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "totalNinos", (movtos.noBautizadosByMesSector.nino + movtos.noBautizadosByMesSector.nina).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "totalAdultosBautizados", (movtos.bautizadosByMesSector.adulto_hombre + movtos.bautizadosByMesSector.adulto_mujer).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "totalJovenesBautizados", (movtos.bautizadosByMesSector.joven_hombre + movtos.bautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "totalJovenesNoBautizados", (movtos.noBautizadosByMesSector.joven_hombre + movtos.noBautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "personalQueIntegraLaIglesia", (movtos.personasBautizadasAlFinalDelMes + movtos.personasNoBautizadasAlFinalDelMes).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "personasBautizadas", (movtos.personasBautizadasAlFinalDelMes).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "personasNoBautizadas", (movtos.personasNoBautizadasAlFinalDelMes).ToString(), false, false, "Aptos", "15");
                    //MOVIMIENTO ADMINISTRATIVO Y MATERIAL
                    AgregarTextoAlMarcador(bookmarks, "SociedadFemenil", (informeVM.Organizaciones.SociedadFemenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SociedadJuvenil", (informeVM.Organizaciones.SociedadJuvenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "DepartamentoFemenil", (informeVM.Organizaciones.DepartamentoFemenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "DepartamentoJuvenil", (informeVM.Organizaciones.DepartamentoJuvenil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "DepartamentoInfantil", (informeVM.Organizaciones.DepartamentoInfantil).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Coros", (informeVM.Organizaciones.Coros).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "GruposDeCanto", (informeVM.Organizaciones.GruposDeCanto).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "Predios", (informeVM.AdquisicionesSector.Predios).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Casas", (informeVM.AdquisicionesSector.Casas).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Edificios", (informeVM.AdquisicionesSector.Edificios).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Templos", (informeVM.AdquisicionesSector.Templos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "Vehiculos", (informeVM.AdquisicionesSector.Vehiculos).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "SesionEnElDistrito", (informeVM.Sesiones.EnElDistrito).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SesionConElPersonalDocente", (informeVM.Sesiones.ConElPersonalDocente).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SesionConSociedadesFemeniles", (informeVM.Sesiones.ConSociedadesFemeniles).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SesionConSociedadesJuveniles", (informeVM.Sesiones.ConSociedadesJuveniles).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SesionConDepartamentosInfantiles", (informeVM.Sesiones.ConDepartamentosInfantiles).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SesionConCorosYGruposDeCanto", (informeVM.Sesiones.ConCorosYGruposDeCanto).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ReunionEnElDistrito", (informeVM.Reuniones.EnElDistrito).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ReunionConElPersonalDocente", (informeVM.Reuniones.ConElPersonalDocente).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ReunionConSociedadesFemeniles", (informeVM.Reuniones.ConSociedadesFemeniles).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ReunionConSociedadesJuveniles", (informeVM.Reuniones.ConSociedadesJuveniles).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ReunionConDepartamentosInfantiles", (informeVM.Reuniones.ConDepartamentosInfantiles).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ReunionConCorosYGruposDeCanto", (informeVM.Reuniones.ConCorosYGruposDeCanto).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "InicioColocacionPrimeraPiedra", (informeVM.ConstruccionesInicio.ColocacionPrimeraPiedra).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "InicioTemplo", (informeVM.ConstruccionesInicio.Templo).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "InicioCasaDeOracion", (informeVM.ConstruccionesInicio.CasaDeOracion).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "InicioCasaPastoral", (informeVM.ConstruccionesInicio.CasaPastoral).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "InicioAnexos", (informeVM.ConstruccionesInicio.Anexos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "InicioRemodelacion", (informeVM.ConstruccionesInicio.Remodelacion).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "FinColocacionPrimeraPiedra", (informeVM.ConstruccionesConclusion.ColocacionPrimeraPiedra).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "FinTemplo", (informeVM.ConstruccionesConclusion.Templo).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "FinCasaDeOracion", (informeVM.ConstruccionesConclusion.CasaDeOracion).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "FinCasaPastoral", (informeVM.ConstruccionesConclusion.CasaPastoral).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "FinAnexos", (informeVM.ConstruccionesConclusion.Anexos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "FinRemodelacion", (informeVM.ConstruccionesConclusion.Remodelacion).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "OrdenacionAncianos", (informeVM.Ordenaciones.Ancianos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "OrdenacionDiaconos", (informeVM.Ordenaciones.Diaconos).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "DedicacionTemplos", (informeVM.Dedicaciones.Templos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "DedicacionCasasDeOracion", (informeVM.Dedicaciones.CasasDeOracion).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "DiaconosAprueba", (informeVM.LlamamientoDePersonal.DiaconosAprueba).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "LlamamientoAuxiliares", (informeVM.LlamamientoDePersonal.Auxiliares).ToString(), false, false, "Aptos", "15");

                    AgregarTextoAlMarcador(bookmarks, "RegPatNacTemplos", (informeVM.RegularizacionPatNac.Templos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "RegPatNacCasasPastorales", (informeVM.RegularizacionPatNac.CasasPastorales).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "RegPatIgTemplos", (informeVM.RegularizacionPatIg.Templos).ToString(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "RegPatIgCasasPastorales", (informeVM.RegularizacionPatIg.CasasPastorales).ToString(), false, false, "Aptos", "15");
                    //MOVIMIENTO ECONOMICO
                    var cultureInfo = new CultureInfo("es-MX");
                    string existenciaAnteriorFormatted = informeVM.MovimientoEconomico.ExistenciaAnterior?.ToString("N", cultureInfo);
                    string entradaMesFormatted = informeVM.MovimientoEconomico.EntradaMes?.ToString("N", cultureInfo);
                    string sumaTotalFormatted = informeVM.MovimientoEconomico.SumaTotal?.ToString("N", cultureInfo);
                    string gastosAdmonFormatted = informeVM.MovimientoEconomico.GastosAdmon?.ToString("N", cultureInfo);
                    string transferenciasFormatted = informeVM.MovimientoEconomico.TransferenciasAentidadSuperior?.ToString("N", cultureInfo);
                    string existenciaCajaFormatted = informeVM.MovimientoEconomico.ExistenciaEnCaja?.ToString("N", cultureInfo);
                    AgregarTextoAlMarcador(bookmarks, "ExistenciaAnterior", existenciaAnteriorFormatted, false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "EntradaMes", entradaMesFormatted, false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "SumaTotal", sumaTotalFormatted, false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "GastosAdmon", gastosAdmonFormatted, false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "TransferenciasAentidadSuperior", transferenciasFormatted, false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "ExistenciaEnCaja", existenciaCajaFormatted, false, false, "Aptos", "15");
                    //OTRAS ACTIVIDADES
                    var descripcionesNumeradas = informeVM.OtrasActividades
                                                 .Select((s, index) => $"{index + 1}.- {s.Descripcion}")
                                                 .ToList();
                    AgregarListaTextosAlMarcador(bookmarks, "OtrasActividades", descripcionesNumeradas, false, false, "Aptos", "15");
                    //FINAL
                    AgregarTextoAlMarcador(bookmarks, "detalle", (desglose), false, true, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "pastorDeLaIglesia", (ministro.pem_Nombre), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "lugarDeReunion", string.IsNullOrEmpty(informeVM.LugarReunion) ? "LUGAR DE REUNIÓN PENDIENTE" : informeVM.LugarReunion.ToUpper(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "diaActual", (informeVM.FechaReunion.Day.ToString()), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "mesActual", nombreMes.ToUpper(), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "añoActual", (informeVM.FechaReunion.Year.ToString()), false, false, "Aptos", "15");
                    //AgregarTextoAlMarcador(bookmarks, "diaActual", (DateTime.Now.Day.ToString()), false, false, "Aptos", "15");
                    //AgregarTextoAlMarcador(bookmarks, "mesActual", (MonthsOfYear.months[DateTime.Now.Month].ToString().ToUpper()), false, false, "Aptos", "15");
                    //AgregarTextoAlMarcador(bookmarks, "añoActual", (DateTime.Now.Year.ToString()), false, false, "Aptos", "15");
                    main.Save();
                }

                Spire.Doc.Document document = new Spire.Doc.Document();
                document.LoadFromFile(archivoTemporal);
                document.SaveToFile(archivoDeSalida, FileFormat.PDF);
                System.IO.File.Delete(archivoTemporal);
                byte[] FileByteData = System.IO.File.ReadAllBytes(archivoDeSalida);
                return File(FileByteData, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("[action]/{id}")]
        [EnableCors("AllowOrigin")]
        //public IActionResult InformePastorPorSector([FromBody] FiltroHistTransEstDelMes ftem)
        public IActionResult InformeObispoPorId(int id)
        {
            try
            {
                var informe = context.Informe.FirstOrDefault(i => i.IdInforme == id && i.Status == 1);
                if (informe.IdInforme == id)
                {
                    SubConsultas sub = new SubConsultas(context);
                    Historial_Transacciones_EstadisticasController hist = new Historial_Transacciones_EstadisticasController(context);
                    objInformeObispo informeObispo = sub.SubInformeObispo(informe.IdDistrito, informe.Anio, informe.Mes);
                    var actividadObispo = informeObispo.actividadObispo;
                    var informes = informeObispo.InformesSectores;
                    var movtos = informeObispo.MovtosEstadisticos;
                    var informeVM = informeObispo.MovtosAdministrativoEconomico;
                    var infoDistrito = (from d in context.Distrito
                                        join pem in context.Personal_Ministerial on d.pem_Id_Obispo equals pem.pem_Id_Ministro
                                        where d.dis_Id_Distrito == informe.IdDistrito
                                        select new
                                        {
                                            d.dis_Numero,
                                            d.dis_Alias,
                                            pem.pem_Nombre
                                        }).ToList();

                    var fechayhora = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");

                    var TemplateTempPath = $"{Environment.CurrentDirectory}\\IECEMembresia\\IECE_WebApi";

                    // {Environment.CurrentDirectory}
                    // D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi

                    string pathPlantilla = $"{TemplateTempPath}\\Templates\\InformeObispo_Plantilla.docx";

                    // NOMBRE DEL PDF QUE SE CREARA
                    string archivoDeSalida = $"{TemplateTempPath}\\Temp\\InformeObispo_{fechayhora}.pdf";

                    // ARCHIVO TEMPORAL EN BASE A LA PLANTILLA
                    string archivoTemporal = $"{TemplateTempPath}\\Temp\\InformeObispo_{fechayhora}.docx";

                    // Create shadow File
                    System.IO.File.Copy(pathPlantilla, archivoTemporal, true);

                    FiltroHistTransEstDelMes ftem = new FiltroHistTransEstDelMes
                    {
                        sec_Id_Sector = informe.IdDistrito,
                        year = informe.Anio,
                        mes = informe.Mes
                    };

                    // Obtener el nombre del mes en mayúsculas
                    string nombreMes = informe.FechaReunion.ToString("MMMM", new CultureInfo("es-ES")).ToUpper();

                    //var movtos = sub.SubMovimientosEstadisticosReporteBySector(ftem);
                    DateTime fechaInicial = new DateTime(ftem.year, ftem.mes, 1);
                    DateTime fechaFinal = fechaInicial.AddMonths(1);
                    fechaFinal = fechaFinal.AddDays(-1);
                    Historial_Transacciones_EstadisticasController.FechasSectorDistrito fsd = new Historial_Transacciones_EstadisticasController.FechasSectorDistrito
                    {
                        idSectorDistrito = ftem.sec_Id_Sector,
                        fechaInicial = fechaInicial,
                        fechaFinal = fechaFinal
                    };

                    var detalle = sub.SubHistorialPorFechaDistrito(fsd);
                    // Filtrar la lista para excluir los objetos con el código 11201.
                    var detalleFiltrado = detalle.Where(obj => obj.ct_Codigo_Transaccion != 11201 && obj.ct_Codigo_Transaccion != 12201 && obj.ct_Codigo_Transaccion != 31203).ToList();
                    string desglose = "";
                    foreach (HistorialPorFechaSector d in detalleFiltrado)
                    {
                        desglose = desglose + $"{d.ct_Tipo}-{d.ct_Subtipo}: {d.per_Nombre} {d.per_Apellido_Paterno} {d.per_Apellido_Materno} | ";
                    }


                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(archivoTemporal, true))
                    {
                        var main = wordDoc.MainDocumentPart.Document;
                        //var bookmarksHeader = wordDoc.MainDocumentPart.HeaderParts.FirstOrDefault().RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                        var bookmarks = main.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                        //ENCABEZADO
                        AgregarTextoAlMarcador(bookmarks, "NombreObispo", (infoDistrito[0].pem_Nombre).ToString(), true, true, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "NoDistrito", (infoDistrito[0].dis_Numero).ToString(), true, true, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "AliasDistrito", (infoDistrito[0].dis_Alias).ToString(), true, true, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "Mes", MonthsOfYear.months[informe.Mes].ToString().ToUpper(), true, true, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "Year", (informe.Anio).ToString(), true, true, "Aptos", "13");

                        //ACTIVIDADES DEL OBISPO
                        string[] L = {"A", "B", "C", "D", "E", "F", "G", "H", "I"};

                        var sectores = actividadObispo.sectores;

                        // Separar en dos listas basadas en la propiedad 'categoria'
                        var listaSectores = sectores.Where(s => s.sector.sec_Tipo_Sector == "SECTOR").ToList();
                        var listaMisiones = sectores.Where(s => s.sector.sec_Tipo_Sector == "MISIÓN").ToList();

                        for (int i = 0; i < listaSectores.Count; i++)
                        {
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}1", (listaSectores[i].sector.sec_Alias).ToString(), false, false, "Aptos", "10");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}2", (listaSectores[i].VisitasObispo.aSectores).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}3", (listaSectores[i].VisitasObispo.aHogares).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}4", (listaSectores[i].CultosDistrito.Ordinarios).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}5", (listaSectores[i].CultosDistrito.Especiales).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}6", (listaSectores[i].CultosDistrito.DeAvivamiento).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}7", (listaSectores[i].CultosDistrito.Evangelismo).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}8", (listaSectores[i].ConferenciasDistrito.iglesia).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}9", (listaSectores[i].ConferenciasDistrito.sectorVaronil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}10", (listaSectores[i].ConferenciasDistrito.sociedadFemenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}11", (listaSectores[i].ConferenciasDistrito.sociedadJuvenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}12", (listaSectores[i].ConferenciasDistrito.sectorInfantil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}13", (listaSectores[i].ConcentracionesDistrito.iglesia).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}14", (listaSectores[i].ConcentracionesDistrito.sectorVaronil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}15", (listaSectores[i].ConcentracionesDistrito.sociedadFemenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}16", (listaSectores[i].ConcentracionesDistrito.sociedadJuvenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{L[i]}17", (listaSectores[i].ConcentracionesDistrito.sectorInfantil).ToString(), false, false, "Aptos", "13");
                        };

                        for (int i = 0; i < listaMisiones.Count; i++)
                        {
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}1", (listaMisiones[i].sector.sec_Alias).ToString(), false, false, "Aptos", "10");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}2", (listaMisiones[i].VisitasObispo.aSectores).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}3", (listaMisiones[i].VisitasObispo.aHogares).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}4", (listaMisiones[i].CultosDistrito.Ordinarios).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}5", (listaMisiones[i].CultosDistrito.Especiales).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}6", (listaMisiones[i].CultosDistrito.DeAvivamiento).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}7", (listaMisiones[i].CultosDistrito.Evangelismo).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}8", (listaMisiones[i].ConferenciasDistrito.iglesia).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}9", (listaMisiones[i].ConferenciasDistrito.sectorVaronil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}10", (listaMisiones[i].ConferenciasDistrito.sociedadFemenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}11", (listaMisiones[i].ConferenciasDistrito.sociedadJuvenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}12", (listaMisiones[i].ConferenciasDistrito.sectorInfantil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}13", (listaMisiones[i].ConcentracionesDistrito.iglesia).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}14", (listaMisiones[i].ConcentracionesDistrito.sectorVaronil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}15", (listaMisiones[i].ConcentracionesDistrito.sociedadFemenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}16", (listaMisiones[i].ConcentracionesDistrito.sociedadJuvenil).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{L[i]}17", (listaMisiones[i].ConcentracionesDistrito.sectorInfantil).ToString(), false, false, "Aptos", "13");
                        };

                        //ACTIVIDADES DEL PERSONAL DOCENTE
                        for (int i = 0; i < informes.Count; i++)
                        {
                            int visPersAux = (informes[i].VisitasPastor.PorAncianosAux??0 + informes[i].VisitasPastor.PorDiaconos??0 + informes[i].VisitasPastor.PorAuxiliares??0);
                            string sumaVisPersAux = visPersAux == 0 ? "" : visPersAux.ToString();

                            int escuelasDom = (informes[i].EstudiosSector.EscuelaDominical ?? 0) + (informes[i].ConferenciasSector.EscuelaDominical ?? 0);
                            string sumaEscDom = escuelasDom == 0 ? "" : escuelasDom.ToString();

                            int estVar = (informes[i].EstudiosSector.Varonil ?? 0) + (informes[i].ConferenciasSector.Varonil ?? 0);
                            string sumaEstVaronil = estVar == 0 ? "" : estVar.ToString();

                            int estFem = (informes[i].EstudiosSector.Femenil ?? 0) + (informes[i].ConferenciasSector.Femenil ?? 0);
                            string sumaEstFemenil = estFem == 0 ? "" : estFem.ToString();

                            int estJuv = (informes[i].EstudiosSector.Juvenil?? 0) + (informes[i].ConferenciasSector.Juvenil ?? 0);
                            string sumaEstJuvenil = estJuv == 0 ? "" : estJuv.ToString();

                            int estsInf = (informes[i].EstudiosSector.Infantil ?? 0) + (informes[i].ConferenciasSector.Infantil ?? 0);
                            string sumaEstInfantil = estsInf == 0 ? "" : estsInf.ToString();

                            int confIglesia = (informes[i].EstudiosSector.Iglesia ?? 0) + (informes[i].ConferenciasSector.Iglesia ?? 0);
                            string sumaConfIglesia = confIglesia == 0 ? "" : confIglesia.ToString();

                            int misiones = informes[i].CultosMisionSector.Count;
                            string sumaMisiones = misiones == 0 ? "" : misiones.ToString();

                            int vistPerm = (informes[i].TrabajoEvangelismo.VisitantesPermanentes ?? 0);
                            string sumaVisitPerm = vistPerm == 0 ? "" : vistPerm.ToString();

                            int baut = (informes[i].TrabajoEvangelismo.Bautismos ?? 0);
                            string sumaBautismos = baut == 0 ? "" : baut.ToString();


                            int j = i + 1;
                            var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == informes[i].IdSector);
                            AgregarTextoAlMarcador(bookmarks, $"S{j}", (sector.sec_Alias).ToString(), false, false, "Aptos", "10");
                            AgregarTextoAlMarcador(bookmarks, $"PP{j}", (informes[i].VisitasPastor.PorPastor).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"PA{j}", sumaVisPersAux, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"O{j}", (informes[i].CultosSector.Ordinarios).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"E{j}", (informes[i].CultosSector.Especiales).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"A{j}", (informes[i].CultosSector.DeAvivamiento).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"ANIV{j}", (informes[i].CultosSector.DeAniversario).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"D{j}", (informes[i].CultosSector.PorElDistrito).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"ED{j}", sumaEscDom, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"V{j}", sumaEstVaronil, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"F{j}", sumaEstFemenil, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"J{j}", sumaEstJuvenil, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"IN{j}", sumaEstInfantil, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"I{j}", sumaConfIglesia, false, false, "Aptos", "13");

                            AgregarTextoAlMarcador(bookmarks, $"NM{j}", sumaMisiones, false, false, "Aptos", "13");
                            int cm = 0;
                            string cultMisiones = "";
                            foreach (var m in informes[i].CultosMisionSector)
                            {
                                cm = cm + (m.Cultos ?? 0);
                                cultMisiones = cm == 0 ? "" : cm.ToString();
                            }
                            AgregarTextoAlMarcador(bookmarks, $"CM{j}", cultMisiones, false, false, "Aptos", "13");

                            AgregarTextoAlMarcador(bookmarks, $"HV{j}", (informes[i].TrabajoEvangelismo.HogaresVisitados).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"HC{j}", (informes[i].TrabajoEvangelismo.HogaresConquistados).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"CL{j}", (informes[i].TrabajoEvangelismo.CultosPorLaLocalidad).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"CH{j}", (informes[i].TrabajoEvangelismo.CultosDeHogar).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"C{j}", (informes[i].TrabajoEvangelismo.Campanias).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"AM{j}", (informes[i].TrabajoEvangelismo.AperturaDeMisiones).ToString(), false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"VP{j}", sumaVisitPerm, false, false, "Aptos", "13");
                            AgregarTextoAlMarcador(bookmarks, $"B{j}", sumaBautismos, false, false, "Aptos", "13");
                        }

                        //DATOS DEL ESTADO ACTUAL DE LA IGLESIA
                        AgregarTextoAlMarcador(bookmarks, "personasBautizadasInicio", (movtos.personasBautizadas).ToString(), true, true, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "bautismo", (movtos.altasBautizados.BAUTISMO).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "restitución", (movtos.altasBautizados.RESTITUCIÓN).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "altaCambioDomicilio", (movtos.altasBautizados.CAMBIODEDOMINTERNO + movtos.altasBautizados.CAMBIODEDOMEXTERNO).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "totalAltas", (movtos.altasBautizados.RESTITUCIÓN + movtos.altasBautizados.BAUTISMO + movtos.altasBautizados.CAMBIODEDOMINTERNO + movtos.altasBautizados.CAMBIODEDOMEXTERNO).ToString(), false, false, "Aptos", "15");

                        AgregarTextoAlMarcador(bookmarks, "excomunion", (movtos.bajasBautizados.EXCOMUNIONTEMPORAL + movtos.bajasBautizados.EXCOMUNION).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "defuncion", (movtos.bajasBautizados.DEFUNCION).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "bajaCambioDomicilio", (movtos.bajasBautizados.CAMBIODEDOMINTERNO + movtos.bajasBautizados.CAMBIODEDOMEXTERNO).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "totalBajas", (movtos.bajasBautizados.EXCOMUNION + movtos.bajasBautizados.EXCOMUNIONTEMPORAL + movtos.bajasBautizados.CAMBIODEDOMINTERNO + movtos.bajasBautizados.CAMBIODEDOMEXTERNO + movtos.bajasBautizados.DEFUNCION).ToString(), false, false, "Aptos", "15");

                        AgregarTextoAlMarcador(bookmarks, "matrimonios", (movtos.matrimonios).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "legalizaciones", (movtos.legalizaciones).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "presentaciones", (movtos.presentaciones).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "hogares", (movtos.hogares).ToString(), false, false, "Aptos", "15");

                        AgregarTextoAlMarcador(bookmarks, "hb", (movtos.bautizadosByMesSector.adulto_hombre).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "jhb", (movtos.bautizadosByMesSector.joven_hombre).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "jhnb", (movtos.noBautizadosByMesSector.joven_hombre).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "jmb", (movtos.bautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "jmnb", (movtos.noBautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "mb", (movtos.bautizadosByMesSector.adulto_mujer).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "ninas", (movtos.noBautizadosByMesSector.nina).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "ninos", (movtos.noBautizadosByMesSector.nino).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "totalNinos", (movtos.noBautizadosByMesSector.nino + movtos.noBautizadosByMesSector.nina).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "totalAdultosBautizados", (movtos.bautizadosByMesSector.adulto_hombre + movtos.bautizadosByMesSector.adulto_mujer).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "totalJovenesBautizados", (movtos.bautizadosByMesSector.joven_hombre + movtos.bautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "totalJovenesNoBautizados", (movtos.noBautizadosByMesSector.joven_hombre + movtos.noBautizadosByMesSector.joven_mujer).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "personalQueIntegraLaIglesia", (movtos.personasBautizadas + movtos.personasNoBautizadas).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "personasBautizadas", (movtos.personasBautizadas).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "personasNoBautizadas", (movtos.personasNoBautizadas).ToString(), false, false, "Aptos", "15");
                        
                        
                        
                        
                        //MOVIMIENTO ADMINISTRATIVO Y MATERIAL
                        AgregarTextoAlMarcador(bookmarks, "SociedadFemenil", (informeVM.Organizaciones.SociedadFemenil).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "SociedadJuvenil", (informeVM.Organizaciones.SociedadJuvenil).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "DepartamentoFemenil", (informeVM.Organizaciones.DepartamentoFemenil).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "DepartamentoJuvenil", (informeVM.Organizaciones.DepartamentoJuvenil).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "DepartamentoInfantil", (informeVM.Organizaciones.DepartamentoInfantil).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "Coros", (informeVM.Organizaciones.Coros).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "GruposDeCanto", (informeVM.Organizaciones.GruposDeCanto).ToString(), false, false, "Aptos", "15");

                        //ADQUISICIONES
                        int adqPredios = (informeVM.AdquisicionesSector.Predios ?? 0) + (actividadObispo.adquisicionesDistrito.Predios?? 0);
                        string sumaAdqPredios = adqPredios == 0 ? "" : adqPredios.ToString();
                        AgregarTextoAlMarcador(bookmarks, "Predios", sumaAdqPredios, false, false, "Aptos", "15");

                        int adqCasas = (informeVM.AdquisicionesSector.Casas ?? 0) + (actividadObispo.adquisicionesDistrito.Casas ?? 0);
                        string sumaAdqCasas = adqCasas == 0 ? "" : adqCasas.ToString();
                        AgregarTextoAlMarcador(bookmarks, "Casas", sumaAdqCasas, false, false, "Aptos", "15");

                        int adqEdificios = (informeVM.AdquisicionesSector.Edificios ?? 0) + (actividadObispo.adquisicionesDistrito.Edificios ?? 0);
                        string sumaAdqEdificios = adqEdificios == 0 ? "" : adqEdificios.ToString();
                        AgregarTextoAlMarcador(bookmarks, "Edificios", sumaAdqEdificios, false, false, "Aptos", "15");

                        int adqTemplos = (informeVM.AdquisicionesSector.Templos ?? 0) + (actividadObispo.adquisicionesDistrito.Templos ?? 0);
                        string sumaAdqTemplos = adqTemplos == 0 ? "" : adqTemplos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "Templos", sumaAdqTemplos, false, false, "Aptos", "15");

                        int adqVehiculos = (informeVM.AdquisicionesSector.Vehiculos ?? 0) + (actividadObispo.adquisicionesDistrito.Vehiculos ?? 0);
                        string sumaAdqVehiculos = adqVehiculos == 0 ? "" : adqVehiculos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "Vehiculos", sumaAdqVehiculos, false, false, "Aptos", "15");

                        //SESIONES O REUNIONES
                        int sesDistrito = actividadObispo.sesionesObispo.EnElDistrito ?? 0;
                        string sumaSesDistrito = sesDistrito == 0 ? "" : sesDistrito.ToString();
                        AgregarTextoAlMarcador(bookmarks, "SesionEnElDistrito", sumaSesDistrito, false, false, "Aptos", "15");

                        int sesConElPersonalDocente = actividadObispo.sesionesObispo.ConElPersonalDocente ?? 0;
                        string sumaSesConElPersonalDocente = sesConElPersonalDocente == 0 ? "" : sesConElPersonalDocente.ToString();
                        AgregarTextoAlMarcador(bookmarks, "SesionConElPersonalDocente", sumaSesConElPersonalDocente, false, false, "Aptos", "15");

                        int sesConSociedadesFemeniles = actividadObispo.sesionesObispo.ConSociedadesFemeniles ?? 0;
                        string sumaSesConSociedadesFemeniles = sesConSociedadesFemeniles == 0 ? "" : sesConSociedadesFemeniles.ToString();
                        AgregarTextoAlMarcador(bookmarks, "SesionConSociedadesFemeniles", sumaSesConSociedadesFemeniles, false, false, "Aptos", "15");

                        int sesConSociedadesJuveniles = actividadObispo.sesionesObispo.ConSociedadesJuveniles ?? 0;
                        string sumaSesConSociedadesJuveniles = sesConSociedadesJuveniles == 0 ? "" : sesConSociedadesJuveniles.ToString();
                        AgregarTextoAlMarcador(bookmarks, "SesionConSociedadesJuveniles", sumaSesConSociedadesJuveniles, false, false, "Aptos", "15");

                        int sesConDepartamentosInfantiles = actividadObispo.sesionesObispo.ConDepartamentosInfantiles ?? 0;
                        string sumaSesConDepartamentosInfantiles = sesConDepartamentosInfantiles == 0 ? "" : sesConDepartamentosInfantiles.ToString();
                        AgregarTextoAlMarcador(bookmarks, "SesionConDepartamentosInfantiles", sumaSesConDepartamentosInfantiles, false, false, "Aptos", "15");

                        int sesConCorosYGruposDeCanto = actividadObispo.sesionesObispo.ConCorosYGruposDeCanto ?? 0;
                        string sumaSesConCorosYGruposDeCanto = sesConCorosYGruposDeCanto == 0 ? "" : sesConCorosYGruposDeCanto.ToString();
                        AgregarTextoAlMarcador(bookmarks, "SesionConCorosYGruposDeCanto", sumaSesConCorosYGruposDeCanto, false, false, "Aptos", "15");

                        int reuDistrito = actividadObispo.reunionesObispo.EnElDistrito ?? 0;
                        string sumaReuDistrito = reuDistrito == 0 ? "" : reuDistrito.ToString();
                        AgregarTextoAlMarcador(bookmarks, "ReunionEnElDistrito", sumaReuDistrito, false, false, "Aptos", "15");

                        int reuConElPersonalDocente = actividadObispo.reunionesObispo.ConElPersonalDocente ?? 0;
                        string sumaReuConElPersonalDocente = reuConElPersonalDocente == 0 ? "" : reuConElPersonalDocente.ToString();
                        AgregarTextoAlMarcador(bookmarks, "ReunionConElPersonalDocente", sumaReuConElPersonalDocente, false, false, "Aptos", "15");

                        int reuConSociedadesFemeniles = actividadObispo.reunionesObispo.ConSociedadesFemeniles ?? 0;
                        string sumaReuConSociedadesFemeniles = reuConSociedadesFemeniles == 0 ? "" : reuConSociedadesFemeniles.ToString();
                        AgregarTextoAlMarcador(bookmarks, "ReunionConSociedadesFemeniles", sumaReuConSociedadesFemeniles, false, false, "Aptos", "15");

                        int reuConSociedadesJuveniles = actividadObispo.reunionesObispo.ConSociedadesJuveniles ?? 0;
                        string sumaReuConSociedadesJuveniles = reuConSociedadesJuveniles == 0 ? "" : reuConSociedadesJuveniles.ToString();
                        AgregarTextoAlMarcador(bookmarks, "ReunionConSociedadesJuveniles", sumaReuConSociedadesJuveniles, false, false, "Aptos", "15");

                        int reuConDepartamentosInfantiles = actividadObispo.reunionesObispo.ConDepartamentosInfantiles ?? 0;
                        string sumaReuConDepartamentosInfantiles = reuConDepartamentosInfantiles == 0 ? "" : reuConDepartamentosInfantiles.ToString();
                        AgregarTextoAlMarcador(bookmarks, "ReunionConDepartamentosInfantiles", sumaReuConDepartamentosInfantiles, false, false, "Aptos", "15");


                        int reuConCorosYGruposDeCanto = actividadObispo.reunionesObispo.ConCorosYGruposDeCanto ?? 0;
                        string sumaReuConCorosYGruposDeCanto = reuConCorosYGruposDeCanto == 0 ? "" : reuConCorosYGruposDeCanto.ToString();
                        AgregarTextoAlMarcador(bookmarks, "ReunionConCorosYGruposDeCanto", sumaReuConCorosYGruposDeCanto, false, false, "Aptos", "15");

                        //CONSTRUCCIONES
                        int consIniColocacionPrimeraPiedra = (informeVM.ConstruccionesInicio.ColocacionPrimeraPiedra ?? 0) + (actividadObispo.construccionesDistritoInicio.colocacionPrimeraPiedra ?? 0);
                        string sumaConIniColocacionPrimeraPiedra = consIniColocacionPrimeraPiedra == 0 ? "" : consIniColocacionPrimeraPiedra.ToString();
                        AgregarTextoAlMarcador(bookmarks, "InicioColocacionPrimeraPiedra", sumaConIniColocacionPrimeraPiedra, false, false, "Aptos", "15");

                        int consIniTemplo = (informeVM.ConstruccionesInicio.Templo ?? 0) + (actividadObispo.construccionesDistritoInicio.templo?? 0);
                        string sumaConIniTemplo = consIniTemplo == 0 ? "" : consIniTemplo.ToString();
                        AgregarTextoAlMarcador(bookmarks, "InicioTemplo", sumaConIniTemplo, false, false, "Aptos", "15");

                        int consIniCasaDeOracion = (informeVM.ConstruccionesInicio.CasaDeOracion ?? 0) + (actividadObispo.construccionesDistritoInicio.casaDeOracion ?? 0);
                        string sumaConIniCasaDeOracion = consIniCasaDeOracion == 0 ? "" : consIniCasaDeOracion.ToString();
                        AgregarTextoAlMarcador(bookmarks, "InicioCasaDeOracion", sumaConIniCasaDeOracion, false, false, "Aptos", "15");

                        int consIniCasaPastoral = (informeVM.ConstruccionesInicio.CasaPastoral ?? 0) + (actividadObispo.construccionesDistritoInicio.casaPastoral ?? 0);
                        string sumaConIniCasaPastoral = consIniCasaPastoral == 0 ? "" : consIniCasaPastoral.ToString();
                        AgregarTextoAlMarcador(bookmarks, "InicioCasaPastoral", sumaConIniCasaPastoral, false, false, "Aptos", "15");

                        int consIniAnexos = (informeVM.ConstruccionesInicio.Anexos ?? 0) + (actividadObispo.construccionesDistritoInicio.anexos ?? 0);
                        string sumaConIniAnexos = consIniAnexos == 0 ? "" : consIniAnexos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "InicioAnexos", sumaConIniAnexos, false, false, "Aptos", "15");

                        int consIniRemodelacion = (informeVM.ConstruccionesInicio.Remodelacion ?? 0) + (actividadObispo.construccionesDistritoInicio.remodelacion ?? 0);
                        string sumaConIniRemodelacion = consIniRemodelacion == 0 ? "" : consIniRemodelacion.ToString();
                        AgregarTextoAlMarcador(bookmarks, "InicioRemodelacion", sumaConIniRemodelacion, false, false, "Aptos", "15");

                        int consFinColocacionPrimeraPiedra = (informeVM.ConstruccionesConclusion.ColocacionPrimeraPiedra ?? 0) + (actividadObispo.construccionesDistritoFinal.colocacionPrimeraPiedra ?? 0);
                        string sumaFinConcColocacionPrimeraPiedra = consFinColocacionPrimeraPiedra == 0 ? "" : consFinColocacionPrimeraPiedra.ToString();
                        AgregarTextoAlMarcador(bookmarks, "FinColocacionPrimeraPiedra", sumaFinConcColocacionPrimeraPiedra, false, false, "Aptos", "15");

                        int consFinTemplo = (informeVM.ConstruccionesConclusion.Templo ?? 0) + (actividadObispo.construccionesDistritoFinal.templo ?? 0);
                        string sumaConsFinTemplo = consFinTemplo == 0 ? "" : consFinTemplo.ToString();
                        AgregarTextoAlMarcador(bookmarks, "FinTemplo", sumaConsFinTemplo, false, false, "Aptos", "15");

                        int consFinCasaDeOracion = (informeVM.ConstruccionesConclusion.CasaDeOracion ?? 0) + (actividadObispo.construccionesDistritoFinal.casaDeOracion ?? 0);
                        string sumaConsFinCasaDeOracion = consFinCasaDeOracion == 0 ? "" : consFinCasaDeOracion.ToString();
                        AgregarTextoAlMarcador(bookmarks, "FinCasaDeOracion", sumaConsFinCasaDeOracion, false, false, "Aptos", "15");

                        int consFinCasaPastoral = (informeVM.ConstruccionesConclusion.CasaPastoral ?? 0) + (actividadObispo.construccionesDistritoFinal.casaPastoral ?? 0);
                        string sumaConsFinCasaPastoral = consFinCasaPastoral == 0 ? "" : consFinCasaPastoral.ToString();
                        AgregarTextoAlMarcador(bookmarks, "FinCasaPastoral", sumaConsFinCasaPastoral, false, false, "Aptos", "15");

                        int consFinAnexos = (informeVM.ConstruccionesConclusion.Anexos ?? 0) + (actividadObispo.construccionesDistritoFinal.anexos ?? 0);
                        string sumaConsFinAnexos = consFinAnexos == 0 ? "" : consFinAnexos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "FinAnexos", sumaConsFinAnexos, false, false, "Aptos", "15");

                        int consFinRemodelacion = (informeVM.ConstruccionesConclusion.Remodelacion ?? 0) + (actividadObispo.construccionesDistritoFinal.remodelacion ?? 0);
                        string sumaConsFinRemodelacion = consFinRemodelacion == 0 ? "" : consFinRemodelacion.ToString();
                        AgregarTextoAlMarcador(bookmarks, "FinRemodelacion", sumaConsFinRemodelacion, false, false, "Aptos", "15");

                        //ORDENACIONES
                        AgregarTextoAlMarcador(bookmarks, "OrdenacionAncianos", (informeVM.Ordenaciones.Ancianos).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "OrdenacionDiaconos", (informeVM.Ordenaciones.Diaconos).ToString(), false, false, "Aptos", "15");

                        //LLAMAMIENTOS
                        AgregarTextoAlMarcador(bookmarks, "DiaconosAprueba", (informeVM.LlamamientoDePersonal.DiaconosAprueba).ToString(), false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "LlamamientoAuxiliares", (informeVM.LlamamientoDePersonal.Auxiliares).ToString(), false, false, "Aptos", "15");

                        //DEDICACIONES
                        int dedTemplos = (informeVM.Dedicaciones.Templos ?? 0) + (actividadObispo.dedicacionesDistrito.Templos ?? 0);
                        string sumaDedTemplos = dedTemplos == 0 ? "" : dedTemplos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "DedicacionTemplos", sumaDedTemplos, false, false, "Aptos", "15");

                        int dedCasasDeOracion = (informeVM.Dedicaciones.CasasDeOracion ?? 0) + (actividadObispo.dedicacionesDistrito.CasasDeOracion ?? 0);
                        string sumaDedCasasDeOracion = dedCasasDeOracion == 0 ? "" : dedCasasDeOracion.ToString();
                        AgregarTextoAlMarcador(bookmarks, "DedicacionCasasDeOracion", sumaDedCasasDeOracion, false, false, "Aptos", "15");

                        //REGULARIZACIONES
                        int regNacTemplos = (informeVM.RegularizacionPatNac.Templos ?? 0) + (actividadObispo.regularizacionesPrediosTemplosNacionDistrito.Templos ?? 0);
                        string sumaRegNacTemplos = regNacTemplos == 0 ? "" : regNacTemplos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "RegPatNacTemplos", sumaRegNacTemplos, false, false, "Aptos", "15");

                        int regNacCasasPastorales = (informeVM.RegularizacionPatNac.CasasPastorales ?? 0) + (actividadObispo.regularizacionesPrediosTemplosNacionDistrito.CasasPastorales ?? 0);
                        string sumaRegNacCasasPastorales = regNacCasasPastorales == 0 ? "" : regNacCasasPastorales.ToString();
                        AgregarTextoAlMarcador(bookmarks, "RegPatNacCasasPastorales", sumaRegNacCasasPastorales, false, false, "Aptos", "15");

                        int regIglTemplos = (informeVM.RegularizacionPatIg.Templos ?? 0) + (actividadObispo.regularizacionesPrediosTemplosIglesiaDistrito.Templos ?? 0);
                        string sumaRegIglTemplos = regIglTemplos == 0 ? "" : regIglTemplos.ToString();
                        AgregarTextoAlMarcador(bookmarks, "RegPatIgTemplos", sumaRegIglTemplos, false, false, "Aptos", "15");

                        int regIglCasasPastorales = (informeVM.RegularizacionPatIg.CasasPastorales ?? 0) + (actividadObispo.regularizacionesPrediosTemplosIglesiaDistrito.CasasPastorales ?? 0);
                        string sumaRegIglCasasPastorales = regIglCasasPastorales == 0 ? "" : regIglCasasPastorales.ToString();
                        AgregarTextoAlMarcador(bookmarks, "RegPatIgCasasPastorales", sumaRegIglCasasPastorales, false, false, "Aptos", "15");


                        //MOVIMIENTO ECONOMICO
                        var cultureInfo = new CultureInfo("es-MX");
                        string existenciaAnteriorFormatted = informeVM.MovimientoEconomico.ExistenciaAnterior?.ToString("N", cultureInfo);
                        string entradaMesFormatted = informeVM.MovimientoEconomico.EntradaMes?.ToString("N", cultureInfo);
                        string sumaTotalFormatted = informeVM.MovimientoEconomico.SumaTotal?.ToString("N", cultureInfo);
                        string gastosAdmonFormatted = informeVM.MovimientoEconomico.GastosAdmon?.ToString("N", cultureInfo);
                        string transferenciasFormatted = informeVM.MovimientoEconomico.TransferenciasAentidadSuperior?.ToString("N", cultureInfo);
                        string existenciaCajaFormatted = informeVM.MovimientoEconomico.ExistenciaEnCaja?.ToString("N", cultureInfo);
                        AgregarTextoAlMarcador(bookmarks, "ExistenciaAnterior", existenciaAnteriorFormatted, false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "EntradaMes", entradaMesFormatted, false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "SumaTotal", sumaTotalFormatted, false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "GastosAdmon", gastosAdmonFormatted, false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "TransferenciasAentidadSuperior", transferenciasFormatted, false, false, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "ExistenciaEnCaja", existenciaCajaFormatted, false, false, "Aptos", "15");

                        //ACUERDOS DISTRITO
                        var acuerdosDistrito = actividadObispo.acuerdosDeDistrito
                                                     .Select((s, index) => $"{index + 1}.- {s.Descripcion}")
                                                     .ToList();
                        AgregarListaTextosAlMarcador(bookmarks, "AcuerdosDistrito", acuerdosDistrito, false, false, "Aptos", "15");

                        //OTRAS ACTIVIDADES
                        var descripcionesNumeradas = actividadObispo.otrasActividadesObispo
                                                     .Select((s, index) => $"{index + 1}.- {s.Descripcion}")
                                                     .ToList();
                        AgregarListaTextosAlMarcador(bookmarks, "OtrasActividades", descripcionesNumeradas, false, false, "Aptos", "15");


                        //FINAL
                        AgregarTextoAlMarcador(bookmarks, "detalle", (desglose), false, true, "Aptos", "15");
                        AgregarTextoAlMarcador(bookmarks, "NombreObispoFinal", (infoDistrito[0].pem_Nombre).ToString(), true, true, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "DiaFinal", DateTime.DaysInMonth(informe.Anio, informe.Mes).ToString(), false, false, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "MesFinal", (MonthsOfYear.months[informe.Mes].ToString().ToUpper()), false, false, "Aptos", "13");
                        AgregarTextoAlMarcador(bookmarks, "YearFinal", (informe.Anio.ToString()), false, false, "Aptos", "13");
                        main.Save();
                    }

                    Spire.Doc.Document document = new Spire.Doc.Document();
                    document.LoadFromFile(archivoTemporal);
                    document.SaveToFile(archivoDeSalida, FileFormat.PDF);
                    System.IO.File.Delete(archivoTemporal);
                    byte[] FileByteData = System.IO.File.ReadAllBytes(archivoDeSalida);
                    return File(FileByteData, "application/pdf");
                    //return Ok(informeObispo);
                }
                else
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "NO SE ENCONTRO EL ID DEL INFORME SOLICITADO O ESTA CANCELADO"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex
                });
            }
        }

        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult HojaDatosEstadisticos([FromBody] oHojaDatosEstadisticos ohde)
        {
            try
            {
                var fechayhora = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");
                string pathPlantilla = $"{Environment.CurrentDirectory}\\Templates\\HojaDatosEstadisticos_Plantilla.docx";

                // NOMBRE DEL PDF QUE SE CREARA
                string archivoDeSalida = $"{Environment.CurrentDirectory}\\Temp\\HojaDatosEstadisticos_{fechayhora}.pdf";

                // ARCHIVO TEMPORAL EN BASE A LA PLANTILLA
                string archivoTemporal = $"{Environment.CurrentDirectory}\\Temp\\HojaDatosEstadisticos_{fechayhora}.docx";

                // Create shadow File
                System.IO.File.Copy(pathPlantilla, archivoTemporal, true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(archivoTemporal, true))
                {
                    var main = wordDoc.MainDocumentPart.Document;
                    var mainPart = wordDoc.MainDocumentPart;

                    // CONSULTA IMAGEN DE LA FOTO
                    var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == ohde.idPersona);
                    var FotoInfo = context.Foto.FirstOrDefault(f => f.idFoto == p.idFoto);
                    string FotoPath = Path.Combine($"{FotoInfo.path}{FotoInfo.guid}{FotoInfo.extension}");

                    ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                    using (FileStream stream = new FileStream(FotoPath, FileMode.Open))
                    {
                        imagePart.FeedData(stream);
                    }

                    //var bookmarksHeader = wordDoc.MainDocumentPart.HeaderParts.FirstOrDefault().RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    var bookmarks = main.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    AgregarTextoAlMarcador(bookmarks, "NombreCompleto", ohde.NombreCompleto, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "edad", ohde.edad.ToString(), false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Nacionalidad", ohde.Nacionalidad, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "LugarNacimiento", ohde.LugarNacimiento, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "FechaNacimiento", ohde.FechaNacimiento, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "NombreDePadres", ohde.NombreDePadres, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "PadresPaternos", ohde.PadresPaternos, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "PadresMaternos", ohde.PadresMaternos, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "EstadoCivil", ohde.EstadoCivil, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "FechaBodaCivil", ohde.FechaBodaCivil, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Acta", ohde.Acta, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Libro", ohde.Libro, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Oficialia", ohde.Oficialia, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "RegistroCivil", ohde.RegistroCivil, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "FechaBodaEclesiastica", ohde.FechaBodaEclesiastica, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "LugarBodaEclesiastica", ohde.LugarBodaEclesiastica, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "NombreConyugue", ohde.NombreConyugue, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "CantidadHijos", ohde.CantidadHijos.ToString(), false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "NombreHijos", ohde.NombreHijos, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "LugarBautismo", ohde.LugarBautismo, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "FechaBautismo", ohde.FechaBautismo, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "QuienBautizo", ohde.QuienBautizo, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "FechaPromesaEspiritu", ohde.FechaPromesaEspiritu, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "BajoImposicionDeManos", ohde.BajoImposicionDeManos, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Puestos", ohde.Puestos, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "CambiosDomicilio", ohde.CambiosDomicilio, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Domicilio", ohde.Domicilio, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Telefonos", ohde.Telefonos, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Email", ohde.Email, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Oficio1", ohde.Oficio1, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Oficio2", ohde.Oficio2, false, true, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Fecha", ohde.Fecha, true, false, "Arial", "16");
                    AgregarTextoAlMarcador(bookmarks, "Secretario", ohde.Secretario, true, false, "Arial", "16");
                    AgregarImagenAlMarcador(bookmarks, "Foto", mainPart.GetIdOfPart(imagePart));
                    main.Save();
                }

                Spire.Doc.Document document = new Spire.Doc.Document();
                document.LoadFromFile(archivoTemporal);
                document.SaveToFile(archivoDeSalida, FileFormat.PDF);
                System.IO.File.Delete(archivoTemporal);
                byte[] FileByteData = System.IO.File.ReadAllBytes(archivoDeSalida);
                return File(FileByteData, "application/pdf");
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex
                });
            }
        }

        [HttpPost]
        [Route("[action]/{idDistrito}/{year}/{mes}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Prueba(int idDistrito, int year, int mes)
        {
            try
            {
                SubConsultas sub = new SubConsultas(context);
                var resultado = sub.SubInformeObispo(idDistrito, year, mes);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult PruebaExcel()
        {
            try
            {
                var fechayhora = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");
                //string pathPlantilla = $"{Environment.CurrentDirectory}\\Templates\\PruebaExcel_Plantilla.xlsx";
                string pathPlantilla = $"D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi\\Templates\\PruebaExcel_Plantilla.xlsx";

                // NOMBRE DEL EXCEL QUE SE CREARA
                //string archivoDeSalida = $"{Environment.CurrentDirectory}\\Temp\\PruebaExcel_{fechayhora}.xlsx";
                string archivoDeSalida = $"D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi\\Temp\\PruebaExcel_{fechayhora}.xlsx";

                // ARCHIVO TEMPORAL EN BASE A LA PLANTILLA
                //string archivoTemporal = $"{Environment.CurrentDirectory}\\Temp\\PruebaExcel_{fechayhora}.xlsx";
                string archivoTemporal = $"D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi\\Temp\\PruebaExcel_{fechayhora}.xlsx";

                // Create shadow File
                System.IO.File.Copy(pathPlantilla, archivoTemporal, true);

                using (SpreadsheetDocument docExcel = SpreadsheetDocument.Open(archivoTemporal, true))
                {
                    IEnumerable<Sheet> sheets =
                    docExcel.WorkbookPart.Workbook.GetFirstChild<Sheets>().
                    Elements<Sheet>().Where(s => s.Name == "Datos");

                    if (sheets?.Count() == 0)
                    {
                        // The specified worksheet does not exist.

                        return null;
                    }

                    string relationshipId = sheets?.First().Id;
                    WorksheetPart documento = (WorksheetPart)docExcel.WorkbookPart.GetPartById(relationshipId);

                    //WorksheetPart documento = SeleccionarHojaDeTrabajo(docExcel.WorkbookPart, "Datos");
                    Worksheet hojaDeTrabajo = documento.Worksheet;
                    SheetData sheetData = documento.Worksheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> filas = sheetData.Descendants<Row>();

                    Row fila = new Row();
                    fila.RowIndex = 10;

                    RellenaCelda(fila, "100", "B10");
                    sheetData.Append(fila);
                    docExcel.Save();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "success",
                    mensaje = ex
                });
            }
        }
    }
}