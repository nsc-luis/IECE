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
        public DbSet<Domicilio> Domicilio { get; set; }
        public DbSet<Hogar> Hogar { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Estado_Civil> Estado_Civil { get; set; }
        public DbSet<Profesion_Oficio> Profesion_Oficio { get; set; }
        public DbSet<Distrito> Distrito { get; set; }
        public DbSet<Sector> Sector { get; set; }
        public DbSet<Bautismo> Bautismo { get; set; }
    }
}
