using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IECE_WebApi.Migrations
{
    public partial class UsuarioMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Distrito",
            //    columns: table => new
            //    {
            //        dis_Id_Distrito = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        dis_Activo = table.Column<bool>(nullable: false),
            //        dis_Alias = table.Column<string>(nullable: true),
            //        dis_Tipo_Distrito = table.Column<string>(nullable: true),
            //        dis_Numero = table.Column<int>(nullable: false),
            //        sec_Id_Sector_Base = table.Column<int>(nullable: false),
            //        dis_Area = table.Column<string>(nullable: true),
            //        pem_Id_Obispo = table.Column<int>(nullable: false),
            //        pem_Id_Obispo_Suplente = table.Column<int>(nullable: false),
            //        pem_Id_Secretario = table.Column<int>(nullable: false),
            //        pem_Id_Sub_Secretario = table.Column<int>(nullable: false),
            //        pem_Id_Tesorero = table.Column<int>(nullable: false),
            //        pem_Id_Sub_Tesorero = table.Column<int>(nullable: false),
            //        dis_Fecha_Organizacion = table.Column<DateTime>(nullable: false),
            //        dis_Fecha_Cambio_Obispo = table.Column<DateTime>(nullable: false),
            //        dis_Fecha_Ultimo_Cambio_Admon = table.Column<DateTime>(nullable: false),
            //        dis_Domicilio_Especial_Correspondencia = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Distrito", x => x.dis_Id_Distrito);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Estado",
            //    columns: table => new
            //    {
            //        est_Id_Estado = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        est_Nombre_Corto = table.Column<string>(nullable: true),
            //        est_Pais = table.Column<string>(nullable: true),
            //        est_Nombre = table.Column<string>(nullable: true),
            //        pais_Id_Pais = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Estado", x => x.est_Id_Estado);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Hogar_Persona",
            //    columns: table => new
            //    {
            //        hp_Id_Hogar_Persona = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        hd_Id_Hogar = table.Column<int>(nullable: false),
            //        per_Id_Persona = table.Column<int>(nullable: false),
            //        hp_Jerarquia = table.Column<int>(nullable: false),
            //        usu_Id_Usuario = table.Column<int>(nullable: false),
            //        Fecha_Registro = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Hogar_Persona", x => x.hp_Id_Hogar_Persona);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "HogarDomicilio",
            //    columns: table => new
            //    {
            //        hd_Id_Hogar = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        hd_Calle = table.Column<string>(nullable: true),
            //        hd_Numero_Exterior = table.Column<string>(nullable: true),
            //        hd_Numero_Interior = table.Column<string>(nullable: true),
            //        hd_Tipo_Subdivision = table.Column<string>(nullable: true),
            //        hd_Subdivision = table.Column<string>(nullable: true),
            //        hd_Localidad = table.Column<string>(nullable: true),
            //        hd_Municipio_Ciudad = table.Column<string>(nullable: true),
            //        pais_Id_Pais = table.Column<int>(nullable: false),
            //        est_Id_Estado = table.Column<int>(nullable: false),
            //        hd_Telefono = table.Column<string>(nullable: true),
            //        usu_Id_Usuario = table.Column<int>(nullable: false),
            //        Fecha_Registro = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HogarDomicilio", x => x.hd_Id_Hogar);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Pais",
            //    columns: table => new
            //    {
            //        pais_Id_Pais = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        pais_Nombre_Corto = table.Column<string>(nullable: true),
            //        pais_Nombre = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Pais", x => x.pais_Id_Pais);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Persona",
            //    columns: table => new
            //    {
            //        per_Id_Persona = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        per_Activo = table.Column<bool>(nullable: false),
            //        per_En_Comunion = table.Column<bool>(nullable: false),
            //        per_Vivo = table.Column<bool>(nullable: false),
            //        per_Visibilidad_Abierta = table.Column<bool>(nullable: false),
            //        sec_Id_Sector = table.Column<int>(nullable: false),
            //        per_Categoria = table.Column<string>(nullable: false),
            //        per_Nombre = table.Column<string>(nullable: false),
            //        per_Apellido_Paterno = table.Column<string>(nullable: false),
            //        per_Apellido_Materno = table.Column<string>(nullable: false),
            //        per_Fecha_Nacimiento = table.Column<DateTime>(nullable: false),
            //        per_Lugar_De_Nacimiento = table.Column<string>(nullable: true),
            //        per_Nacionalidad = table.Column<string>(nullable: true),
            //        per_RFC_Sin_Homo = table.Column<string>(nullable: false),
            //        per_Nombre_Padre = table.Column<string>(nullable: true),
            //        per_Nombre_Madre = table.Column<string>(nullable: true),
            //        per_Nombre_Abuelo_Paterno = table.Column<string>(nullable: true),
            //        per_Nombre_Abuela_Paterna = table.Column<string>(nullable: true),
            //        per_Nombre_Abuelo_Materno = table.Column<string>(nullable: true),
            //        per_Nombre_Abuela_Materna = table.Column<string>(nullable: true),
            //        pro_Id_Profesion_Oficio1 = table.Column<int>(nullable: false),
            //        pro_Id_Profesion_Oficio2 = table.Column<int>(nullable: false),
            //        per_Telefono_Movil = table.Column<string>(nullable: true),
            //        per_Email_Personal = table.Column<string>(nullable: true),
            //        per_foto = table.Column<string>(nullable: true),
            //        per_Bautizado = table.Column<bool>(nullable: false),
            //        per_Lugar_Bautismo = table.Column<string>(nullable: true),
            //        per_Fecha_Bautismo = table.Column<DateTime>(nullable: false),
            //        per_Ministro_Que_Bautizo = table.Column<string>(nullable: true),
            //        per_Fecha_Recibio_Espiritu_Santo = table.Column<DateTime>(nullable: false),
            //        per_Bajo_Imposicion_De_Manos = table.Column<string>(nullable: true),
            //        per_Cargos_Desempenados = table.Column<string>(nullable: true),
            //        per_Cambios_De_Domicilio = table.Column<string>(nullable: true),
            //        per_Estado_Civil = table.Column<string>(nullable: true),
            //        per_Nombre_Conyuge = table.Column<string>(nullable: true),
            //        per_Fecha_Boda_Civil = table.Column<DateTime>(nullable: false),
            //        per_Num_Acta_Boda_Civil = table.Column<string>(nullable: true),
            //        per_Libro_Acta_Boda_Civil = table.Column<string>(nullable: true),
            //        per_Oficialia_Boda_Civil = table.Column<string>(nullable: true),
            //        per_Registro_Civil = table.Column<string>(nullable: true),
            //        per_Fecha_Boda_Eclesiastica = table.Column<DateTime>(nullable: false),
            //        per_Lugar_Boda_Eclesiastica = table.Column<string>(nullable: true),
            //        per_Cantidad_Hijos = table.Column<int>(nullable: false),
            //        per_Nombre_Hijos = table.Column<string>(nullable: true),
            //        usu_Id_Usuario = table.Column<int>(nullable: false),
            //        Fecha_Registro = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Persona", x => x.per_Id_Persona);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Profesion_Oficio",
            //    columns: table => new
            //    {
            //        pro_Id_Profesion_Oficio = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        pro_Sub_Categoria = table.Column<string>(nullable: false),
            //        pro_Categoria = table.Column<string>(nullable: false),
            //        usu_Id_Usuario = table.Column<int>(nullable: false),
            //        Fecha_Registro = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Profesion_Oficio", x => x.pro_Id_Profesion_Oficio);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Sector",
            //    columns: table => new
            //    {
            //        sec_Id_Sector = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        sec_Activo = table.Column<bool>(nullable: false),
            //        sec_Tipo_Sector = table.Column<string>(nullable: true),
            //        sec_Numero = table.Column<int>(nullable: false),
            //        sec_Alias = table.Column<string>(nullable: true),
            //        dis_Id_Distrito = table.Column<int>(nullable: false),
            //        pem_Id_Pastor = table.Column<int>(nullable: false),
            //        sec_Fecha_Inicio_Pastorado_Actual = table.Column<DateTime>(nullable: false),
            //        pem_Id_Secretario = table.Column<int>(nullable: false),
            //        pem_Id_Tesorero = table.Column<int>(nullable: false),
            //        sec_Fecha_Organizacion = table.Column<DateTime>(nullable: false),
            //        sec_Membresia = table.Column<int>(nullable: false),
            //        Fecha_Registro = table.Column<DateTime>(nullable: false),
            //        usu_Id_Usuario = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Sector", x => x.sec_Id_Sector);
            //    });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "PersonaDomicilio",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        PersonaEntityper_Id_Persona = table.Column<int>(nullable: true),
            //        HogarDomicilioEntityhd_Id_Hogar = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PersonaDomicilio", x => x.id);
            //        table.ForeignKey(
            //            name: "FK_PersonaDomicilio_HogarDomicilio_HogarDomicilioEntityhd_Id_Hogar",
            //            column: x => x.HogarDomicilioEntityhd_Id_Hogar,
            //            principalTable: "HogarDomicilio",
            //            principalColumn: "hd_Id_Hogar",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_PersonaDomicilio_Persona_PersonaEntityper_Id_Persona",
            //            column: x => x.PersonaEntityper_Id_Persona,
            //            principalTable: "Persona",
            //            principalColumn: "per_Id_Persona",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_PersonaDomicilio_HogarDomicilioEntityhd_Id_Hogar",
            //    table: "PersonaDomicilio",
            //    column: "HogarDomicilioEntityhd_Id_Hogar");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PersonaDomicilio_PersonaEntityper_Id_Persona",
            //    table: "PersonaDomicilio",
            //    column: "PersonaEntityper_Id_Persona");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Distrito");

            //migrationBuilder.DropTable(
            //    name: "Estado");

            //migrationBuilder.DropTable(
            //    name: "Hogar_Persona");

            //migrationBuilder.DropTable(
            //    name: "Pais");

            //migrationBuilder.DropTable(
            //    name: "PersonaDomicilio");

            //migrationBuilder.DropTable(
            //    name: "Profesion_Oficio");

            //migrationBuilder.DropTable(
            //    name: "Sector");

            migrationBuilder.DropTable(
                name: "Usuario");

            //migrationBuilder.DropTable(
            //    name: "HogarDomicilio");

            //migrationBuilder.DropTable(
            //    name: "Persona");
        }
    }
}
