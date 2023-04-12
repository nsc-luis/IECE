using System;
using IECE_WebApi.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static IECE_WebApi.Templates.TiposDeDatosParaPDF;
using Microsoft.AspNetCore.Hosting;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DocumentosPDFController : ControllerBase
    {
        private readonly AppDbContext context;
        public DocumentosPDFController(AppDbContext context)
        {
            this.context = context;
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

                // open word
                Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = app.Documents.Open(archivoTemporal);

                object oBookMark = "FechaInicial";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.FechaInicial.ToString("yyyy-MM-dd");

                oBookMark = "FechaFinal";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.FechaFinal.ToString("yyyy-MM-dd");

                oBookMark = "AdultosBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AdultosBautizados.ToString();

                oBookMark = "AltasBautizadosBautismo";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AltasBautizadosBautismo.ToString();

                oBookMark = "AltasBautizadosCambioDomicilio";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AltasBautizadosCambioDomicilio.ToString();

                oBookMark = "AltasBautizadosRestitucion";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AltasBautizadosRestitucion.ToString();

                oBookMark = "AltasNoBautizadosCambioDomicilio";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AltasNoBautizadosCambioDomicilio.ToString();

                oBookMark = "AltasNoBautizadosNuevoIngreso";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AltasNoBautizadosNuevoIngreso.ToString();

                oBookMark = "AltasNoBautizadosReactivacion";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.AltasNoBautizadosReactivacion.ToString();

                oBookMark = "BajasBautizadosCambioDomicilio";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BajasBautizadosCambioDomicilio.ToString();

                oBookMark = "BajasBautizadosDefuncion";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BajasBautizadosDefuncion.ToString();

                oBookMark = "BajasBautizadosExcomunion";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BajasBautizadosExcomunion.ToString();

                oBookMark = "BajasNoBautizadosAlejamiento";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BajasNoBautizadosAlejamiento.ToString();

                oBookMark = "BajasNoBautizadosCambioDomicilio";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BajasNoBautizadosCambioDomicilio.ToString();

                oBookMark = "BajasNoBautizadosDefuncion";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BajasNoBautizadosDefuncion.ToString();

                oBookMark = "BautizadosAdultoHombre";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BautizadosAdultoHombre.ToString();

                oBookMark = "BautizadosAdultoMujer";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BautizadosAdultoMujer.ToString();

                oBookMark = "BautizadosJovenHombre";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BautizadosJovenHombre.ToString();

                oBookMark = "BautizadosJovenMujer";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.BautizadosJovenMujer.ToString();

                oBookMark = "JovenesBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.JovenesBautizados.ToString();

                oBookMark = "JovenesNoBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.JovenesNoBautizados.ToString();

                oBookMark = "Legalizaciones";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Legalizaciones.ToString();

                oBookMark = "Matrimonios";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Matrimonios.ToString();

                oBookMark = "Ninas";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Ninas.ToString();

                oBookMark = "Ninos";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Ninos.ToString();

                oBookMark = "NoBautizadosJovenHombre";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.NoBautizadosJovenHombre.ToString();

                oBookMark = "NoBautizadosJovenMujer";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.NoBautizadosJovenMujer.ToString();

                oBookMark = "Presentaciones";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Presentaciones.ToString();

                oBookMark = "Total";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Total.ToString();

                oBookMark = "TotalAltasBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.TotalAltasBautizados.ToString();

                oBookMark = "TotalAltasNoBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.TotalAltasNoBautizados.ToString();

                oBookMark = "TotalBajasBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.TotalBajasBautizados.ToString();

                oBookMark = "TotalBajasNoBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.TotalBajasNoBautizados.ToString();

                oBookMark = "Ministro";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Ministro;

                oBookMark = "Secretario";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Secretario;

                oBookMark = "Transacciones";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = orme.Transacciones;

                oBookMark = "TotalNinos";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = (orme.Ninas + orme.Ninos).ToString();

                oBookMark = "TotalBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = (orme.AdultosBautizados + orme.JovenesBautizados).ToString();

                oBookMark = "TotalNoBautizados";
                doc.Bookmarks.get_Item(ref oBookMark).Range.Text = (orme.JovenesNoBautizados + orme.Ninas + orme.Ninos).ToString();

                doc.ExportAsFixedFormat(archivoDeSalida, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);

                doc.Close();
                app.Quit();
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