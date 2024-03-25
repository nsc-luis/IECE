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
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult InformePastorPorSector([FromBody] FiltroHistTransEstDelMes ftem)
        {
            try
            {
                var sector = context.Sector.FirstOrDefault(s => s.sec_Id_Sector == ftem.sec_Id_Sector);
                var distrito = context.Distrito.FirstOrDefault(d => d.dis_Id_Distrito == sector.dis_Id_Distrito);
                var ministro = context.Personal_Ministerial.FirstOrDefault(pm => pm.pem_Id_Ministro == sector.pem_Id_Pastor);
                var fechayhora = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");
                //string pathPlantilla = $"{Environment.CurrentDirectory}\\Templates\\InformePastorPorSector_Plantilla.docx";
                string pathPlantilla = $"D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi\\Templates\\InformePastorPorSector_Plantilla.docx";

                // NOMBRE DEL PDF QUE SE CREARA
                //string archivoDeSalida = $"{Environment.CurrentDirectory}\\Temp\\InformePastorPorSector_{fechayhora}.pdf";
                string archivoDeSalida = $"D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi\\Temp\\InformePastorPorSector_{fechayhora}.pdf";

                // ARCHIVO TEMPORAL EN BASE A LA PLANTILLA
                //string archivoTemporal = $"{Environment.CurrentDirectory}\\Temp\\InformePastorPorSector_{fechayhora}.docx";
                string archivoTemporal = $"D:\\Users\\Lenovo\\Desktop\\IECEMembresia\\IECE_WebApi\\Temp\\InformePastorPorSector_{fechayhora}.docx";

                // Create shadow File
                System.IO.File.Copy(pathPlantilla, archivoTemporal, true);

                SubConsultas sub = new SubConsultas(context);
                var movtos = sub.SubMovimientosEstadisticosReporteBySector(ftem);
                MonthsOfYear monthsOfYear = new MonthsOfYear();

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
                string desglose = "";
                foreach (HistorialPorFechaSector d in detalle)
                {
                    desglose = desglose + $"{d.ct_Subtipo}: {d.per_Nombre} {d.per_Apellido_Paterno} {d.per_Apellido_Materno} ({d.hte_Fecha_Transaccion}), ";
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(archivoTemporal, true))
                {
                    var main = wordDoc.MainDocumentPart.Document;
                    //var bookmarksHeader = wordDoc.MainDocumentPart.HeaderParts.FirstOrDefault().RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    var bookmarks = main.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();

                    AgregarTextoAlMarcador(bookmarks, "noSector", (sector.sec_Numero).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "sectorAlias", (sector.sec_Alias).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "noDistrito", (distrito.dis_Numero).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "distritoAlias", (distrito.dis_Alias).ToString(), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "mesReporte", (MonthsOfYear.months[ftem.mes].ToString()), true, true, "Aptos", "18");
                    AgregarTextoAlMarcador(bookmarks, "añoReporte", (ftem.year).ToString(), true, true, "Aptos", "18");

                    AgregarTextoAlMarcador(bookmarks, "bautismo", (movtos.altasBautizados.BAUTISMO).ToString(),false,false,"Aptos","15");
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

                    AgregarTextoAlMarcador(bookmarks, "detalle", (desglose), false, true, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "pastorDeLaIglesia", (ministro.pem_Nombre), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "lugarDeReunion", (sector.sec_Alias), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "diaActual", (DateTime.Now.Day.ToString()), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "mesActual", (MonthsOfYear.months[DateTime.Now.Month].ToString()), false, false, "Aptos", "15");
                    AgregarTextoAlMarcador(bookmarks, "añoActual", (DateTime.Now.Year.ToString()), false, false, "Aptos", "15");
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