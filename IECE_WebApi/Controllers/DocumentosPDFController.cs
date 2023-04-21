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
            bool underline = false)
        {
            var bm = bookmarks.FirstOrDefault(bms => bms.Name == NombreMarcador);
            Run r = bm.Parent.InsertAfter(new Run(), bm);
            RunProperties rp = new RunProperties();
            if (bold)
            {
                rp.AppendChild<Bold>(new Bold());
            }
            if (underline)
            {
                rp.AppendChild<Underline>(new Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single });
            }
            r.AppendChild(rp);
            r.AppendChild(new Text(valor));
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
                    AgregarTextoAlMarcador(bookmarksHeader, "FechaInicial", orme.FechaInicial.ToString("yyyy-MM-dd"), true, true);
                    AgregarTextoAlMarcador(bookmarksHeader, "FechaFinal", orme.FechaFinal.ToString("yyyy-MM-dd"), true, true);
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
                    //var bookmarksHeader = wordDoc.MainDocumentPart.HeaderParts.FirstOrDefault().RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    var bookmarks = main.Descendants<DocumentFormat.OpenXml.Wordprocessing.BookmarkStart>().ToList();
                    AgregarTextoAlMarcador(bookmarks, "NombreCompleto", ohde.NombreCompleto, true, true);
                    AgregarTextoAlMarcador(bookmarks, "edad", ohde.edad.ToString(), true, true);
                    AgregarTextoAlMarcador(bookmarks, "Nacionalidad", ohde.Nacionalidad, true, true);
                    AgregarTextoAlMarcador(bookmarks, "LugarNacimiento", ohde.LugarNacimiento, true, true);
                    AgregarTextoAlMarcador(bookmarks, "FechaNacimiento", ohde.FechaNacimiento.ToString("yyyy-MM-dd"), true, true);
                    AgregarTextoAlMarcador(bookmarks, "NombreDePadres", ohde.NombreDePadres, true, true);
                    AgregarTextoAlMarcador(bookmarks, "PadresPaternos", ohde.PadresPaternos, true, true);
                    AgregarTextoAlMarcador(bookmarks, "PadresMaternos", ohde.PadresMaternos, true, true);
                    AgregarTextoAlMarcador(bookmarks, "EstadoCivil", ohde.EstadoCivil, true, true);
                    AgregarTextoAlMarcador(bookmarks, "FechaBodaCivil", ohde.FechaBodaCivil?.ToString("yyyy-MM-dd"), true, true);
                    AgregarTextoAlMarcador(bookmarks, "Acta", ohde.Acta, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Libro", ohde.Libro, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Oficialia", ohde.Oficialia, true, true);
                    AgregarTextoAlMarcador(bookmarks, "RegistroCivil", ohde.RegistroCivil, true, true);
                    AgregarTextoAlMarcador(bookmarks, "FechaBodaEclesiastica", ohde.FechaBodaEclesiastica?.ToString("yyyy-MM-dd"), true, true);
                    AgregarTextoAlMarcador(bookmarks, "LugarBodaEclesiastica", ohde.LugarBodaEclesiastica, true, true);
                    AgregarTextoAlMarcador(bookmarks, "NombreConyugue", ohde.NombreConyugue, true, true);
                    AgregarTextoAlMarcador(bookmarks, "CantidadHijos", ohde.CantidadHijos.ToString(), true, true);
                    AgregarTextoAlMarcador(bookmarks, "NombreHijos", ohde.NombreHijos, true, true);
                    AgregarTextoAlMarcador(bookmarks, "LugarBautismo", ohde.LugarBautismo, true, true);
                    AgregarTextoAlMarcador(bookmarks, "FechaBautismo", ohde.FechaBautismo.ToString("yyyy-MM-dd"), true, true);
                    AgregarTextoAlMarcador(bookmarks, "QuienBautizo", ohde.QuienBautizo, true, true);
                    AgregarTextoAlMarcador(bookmarks, "FechaPromesaEspiritu", ohde.FechaPromesaEspiritu?.ToString("yyyy-MM-dd"), true, true);
                    AgregarTextoAlMarcador(bookmarks, "BajoImposicionDeManos", ohde.BajoImposicionDeManos, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Puestos", ohde.Puestos, true, true);
                    AgregarTextoAlMarcador(bookmarks, "CambiosDomicilio", ohde.CambiosDomicilio, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Domicilio", ohde.Domicilio, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Telefonos", ohde.Telefonos, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Oficio1", ohde.Oficio1, true, true);
                    AgregarTextoAlMarcador(bookmarks, "Oficio2", ohde.Oficio2, true, true);
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
    }
}