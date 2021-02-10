using IECE_WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Contexts
{
    public class AppDbContext : DbContext
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
    }
}
