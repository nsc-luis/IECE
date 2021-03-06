﻿using IECE_WebApi.Models;
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
        //  public DbSet<Foto> Foto { get; set; }
        public DbSet<Personal_Ministerial> Personal_Ministerial { get; set; }
        public DbSet<Ministro_Usuario> Ministro_Usuario { get; set; }
    }
}
