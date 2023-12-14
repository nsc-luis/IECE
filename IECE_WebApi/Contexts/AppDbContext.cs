using IECE_WebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Contexts
{
    public class AppDbContext : IdentityDbContext<Usuario>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Estado> Estado { get; set; }
        public DbSet<Pais> Pais { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Profesion_Oficio> Profesion_Oficio { get; set; }
        public DbSet<Distrito> Distrito { get; set; }
        public DbSet<Sector> Sector { get; set; }
        public DbSet<HogarDomicilio> HogarDomicilio { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Hogar_Persona> Hogar_Persona { get; set; }
        public DbSet<PersonaDomicilio> PersonaDomicilio { get; set; }
        public DbSet<Foto> Foto { get; set; }
        public DbSet<Personal_Ministerial> Personal_Ministerial { get; set; }
        public DbSet<Ministro_Usuario> Ministro_Usuario { get; set; }
        public DbSet<Presentacion_Nino> Presentacion_Nino { get; set; }
        public DbSet<Matrimonio_Legalizacion> Matrimonio_Legalizacion { get; set; }
        public DbSet<Codigo_Transacciones_Estadisticas> Codigo_Transacciones_Estadisticas { get; set; }
        public DbSet<Historial_Transacciones_Estadisticas> Historial_Transacciones_Estadisticas { get; set; }
        public DbSet<PersonaComentarioHTE> PersonaComentarioHTE { get; set; }
        public DbSet<AltaCambioDomicilioRestitucionReactivacion_NuevoDomicilio> AltaCambioDomicilioRestitucionReactivacion_NuevoDomicilio { get; set; }
        public DbSet<AltaCambioDomicilioRestitucionReactivacion_HogarExistente> AltaCambioDomicilioRestitucionReactivacion_HogarExistente { get; set; }
        public DbSet<PersonaParaCambioDomicilioReactivacionRestitucion> PersonaParaCambioDomicilioReactivacionRestitucion { get; set; }
        public DbSet<Valida_Cambio_Contrasena> Valida_Cambio_Contrasena { get; set; }
        public DbSet<SolicitudNuevaProfesion> SolicitudNuevaProfesion { get; set; }
        public DbSet<SolicitudNvoEstado> SolicitudNvoEstado { get; set; }
        public DbSet<PersonaHogarExistente> PersonaHogarExistente { get; set; }
        public DbSet<Registro_Transacciones> Registro_Transacciones { get; set; }
        public DbSet<Comision_Local> Comision_Local { get; set; }
        public DbSet<Integrante_Comision_Local> Integrante_Comision_Local { get; set; }

        public DbSet<Comision_Distrital> Comision_Distrital { get; set; }
        public DbSet<Integrante_Comision_Distrital> Integrante_Comision_Distrital { get; set; }
        public DbSet<Organismo_Interno> Organismo_Interno { get; set; }
        public DbSet<Templo> Templo { get; set; }
        public DbSet<Domicilio> Domicilio { get; set; }
        public DbSet<CasaPastoral> CasaPastoral { get; set; }
        public DbSet<Mision_Sector> Mision_Sector { get; set; }
    }
}
