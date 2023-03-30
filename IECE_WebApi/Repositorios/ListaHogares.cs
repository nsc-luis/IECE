using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Repositorios
{
    //Modelo
    public class Hogar:HogarDomicilio
    {
        public string pais_Nombre { get; set; }
        public string pais_Nombre_Corto { get; set; }
        public string est_Nombre { get; set; }
        public string est_Nombre_Corto { get; set; }
    }

    public class Homes
    {
        public int indice { get; set; }
        public string tel { get; set; }
        public string direccion { get; set; }
        public List<IntegrantesHogar> integrantes { get; set; }
    }

    public class IntegrantesHogar
    {
        public string grupo { get; set; }
        public DateTime nacimiento { get; set; }
        public string nombre { get; set; }
        public int edad { get; set; }
        public string cel { get; set; }
    }

    
    public class PersonaDireccion
    {
        public string nombre { get; set; }
        public string direccion{ get; set; }
    }

    //Clase para generar Lista de Hogares
    public class Hogares
    {
        private AppDbContext context;

        public Hogares(AppDbContext context)
        {
            this.context = context;
        }

        private DateTime fechayhora = DateTime.UtcNow;
        public string getDireccion (string st, string n_ext, string n_int,string t_asent, string asent, string poblado, string municipio, string estado, string pais, string cp )
        {
            var noExterior = (n_ext == null || n_ext == "") ? "S/N" : n_ext;
            var noInterior = (n_int == null || n_int == "") ? "" : " Int. " + n_int;
            var numero = noInterior != "" ? (noExterior + "," + noInterior): (noExterior);
            var tipoAsentamiento = (t_asent == null || t_asent == "") ? "" : t_asent;
            var asentamiento = (asent == null || asent == "") ? "" : $"{tipoAsentamiento} {asent}";
            var localidad = (poblado == null || poblado == "") ? "" : poblado;
            var ciudad = (localidad == municipio || localidad == "") ? municipio : (localidad + ", " + municipio);
            var direccion = "";
            if (pais == "USA" || pais == "CAN")
            {
                var calle = st;
                direccion = $"{numero} {calle}, {asentamiento}, {ciudad}, {estado}, {pais}.";
            }
            else
            {
                var calle = st != null || st != "" ? st : "DOMICILIO CONOCIDO";
                direccion = $"{calle} {numero}, {asentamiento}, {ciudad}, {estado}, {pais}.";
            }

            return (direccion);
        }

        public List<Hogar> Address(int id)
        {
            var hogardomicilio = (from hd in context.HogarDomicilio
                                  join pais in context.Pais on hd.pais_Id_Pais equals pais.pais_Id_Pais
                                  join est in context.Estado on hd.est_Id_Estado equals est.est_Id_Estado
                                  where hd.hd_Id_Hogar == id
                                  select new Hogar
                                  {
                                      hd_Activo = hd.hd_Activo,
                                      hd_Calle = hd.hd_Calle,
                                      hd_Id_Hogar = hd.hd_Id_Hogar,
                                      hd_Localidad = hd.hd_Localidad,
                                      hd_Municipio_Ciudad = hd.hd_Municipio_Ciudad,
                                      hd_Numero_Exterior = hd.hd_Numero_Exterior,
                                      hd_Numero_Interior = hd.hd_Numero_Interior,
                                      hd_Subdivision = hd.hd_Subdivision,
                                      hd_Telefono = hd.hd_Telefono,
                                      hd_Tipo_Subdivision = hd.hd_Tipo_Subdivision,
                                      pais_Nombre = pais.pais_Nombre,
                                      pais_Nombre_Corto = pais.pais_Nombre_Corto,
                                      est_Nombre = est.est_Nombre,
                                      est_Nombre_Corto = est.est_Nombre_Corto
                                  }).ToList();

            return (hogardomicilio);

        }

        public string getDireccion(int id )
        {

            var hogardomicilio = Address(id);
            var hd = hogardomicilio[0];

            var direccion = getDireccion(hd.hd_Calle, hd.hd_Numero_Exterior, hd.hd_Numero_Interior, hd.hd_Tipo_Subdivision, hd.hd_Subdivision, hd.hd_Localidad, hd.hd_Municipio_Ciudad, hd.est_Nombre_Corto, hd.pais_Nombre_Corto, "");

            return (direccion);
        }

        public List<Homes> ListaHogaresBySector(int sec_Id_Sector)
        {
            List<Homes> hogares = new List<Homes>();

            //Query que muestra todos los hogares que pertenecen a las personas de un sector, sin repetidos.
            var query = (from p in context.Persona
                         join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                         where p.sec_Id_Sector == sec_Id_Sector && p.per_Activo == true
                         join hp in context.Hogar_Persona on p.per_Id_Persona equals hp.per_Id_Persona

                         select new 
                         {
                             hogarId = hp.hd_Id_Hogar
                         }).Distinct().ToList();

            //Query por cada Hogar encontrado en el query anterior para buscar sus datos y tambien sus Integrantes ordenados por herarquía
            int loop = 0;
            foreach (var hogar in query)
            {
                loop++;
                 var query2 = (from hp in context.Hogar_Persona
                              join hd in context.HogarDomicilio
                              on hp.hd_Id_Hogar equals hd.hd_Id_Hogar
                              join e in context.Estado on hd.est_Id_Estado equals e.est_Id_Estado into domicilioPais
                              from state in domicilioPais.DefaultIfEmpty()
                              join pais in context.Pais on hd.pais_Id_Pais equals pais.pais_Id_Pais into domicilioEstado
                              from country in domicilioEstado.DefaultIfEmpty()
                              join p in context.Persona
                              on hp.per_Id_Persona equals p.per_Id_Persona
                              where hp.hd_Id_Hogar == hogar.hogarId
                              && hp.hp_Jerarquia == 1
                              select new Homes
                              {
                                  indice= loop,
                                  direccion = getDireccion(hd.hd_Calle,hd.hd_Numero_Exterior, hd.hd_Numero_Interior,hd.hd_Tipo_Subdivision,hd.hd_Subdivision,hd.hd_Localidad, hd.hd_Municipio_Ciudad, state.est_Nombre_Corto, country.pais_Nombre_Corto,""),
                                  tel = hd.hd_Telefono,
                                  //Query que obiene los integrantes del Hogar en que está siendo analizado en el bucle foreach.
                                  integrantes = (from hp in context.Hogar_Persona
                                                  join p in context.Persona
                                                  on hp.per_Id_Persona equals p.per_Id_Persona
                                                  where hp.hd_Id_Hogar == hogar.hogarId && p.per_Activo == true
                                                  orderby (hp.hp_Jerarquia)
                                                  select new IntegrantesHogar
                                                  {
                                                    nombre = p.per_Nombre + " " + p.per_Apellido_Paterno + " " + (p.per_Apellido_Materno != null ? p.per_Apellido_Materno : ""),
                                                    cel=p.per_Telefono_Movil,
                                                    nacimiento=p.per_Fecha_Nacimiento,
                                                    edad= (fechayhora - p.per_Fecha_Nacimiento).Days / 365,
                                                    grupo=p.per_Bautizado==true?"B":"NB"
                                                  }).ToList()

                              }).ToList();

                hogares.Add (new Homes
                { 
                indice=query2[0].indice,
                direccion = query2[0].direccion,
                tel=query2[0].tel,
                integrantes=query2[0].integrantes
                });
            }

            
            return (hogares);
        }




        public List<Homes> ListaHogaresByDistrito(int dis_Id_Distrito)
        {
            List<Homes> hogares = new List<Homes>();

            //Query que muestra todos los hogares que pertenecen a las personas de un sector, sin repetidos.
            var query = (from p in context.Persona
                         join s in context.Sector on p.sec_Id_Sector equals s.sec_Id_Sector
                         join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                         where d.dis_Id_Distrito == dis_Id_Distrito && p.per_Activo == true
                         join hp in context.Hogar_Persona on p.per_Id_Persona equals hp.per_Id_Persona

                         select new
                         {
                             hogarId = hp.hd_Id_Hogar
                         }).Distinct().ToList();

            //Query por cada Hogar encontrado en el query anterior para buscar sus datos y tambien sus Integrantes ordenados por herarquía
            int loop = 0;
            foreach (var hogar in query)
            {
                loop++;
                var query2 = (from hp in context.Hogar_Persona
                              join hd in context.HogarDomicilio
                              on hp.hd_Id_Hogar equals hd.hd_Id_Hogar
                              join e in context.Estado on hd.est_Id_Estado equals e.est_Id_Estado into domicilioPais
                              from state in domicilioPais.DefaultIfEmpty()
                              join pais in context.Pais on hd.pais_Id_Pais equals pais.pais_Id_Pais into domicilioEstado
                              from country in domicilioEstado.DefaultIfEmpty()
                              join p in context.Persona
                              on hp.per_Id_Persona equals p.per_Id_Persona
                              where hp.hd_Id_Hogar == hogar.hogarId
                              && hp.hp_Jerarquia == 1
                              select new Homes
                              {
                                  indice = loop,
                                  direccion = getDireccion(hd.hd_Calle, hd.hd_Numero_Exterior, hd.hd_Numero_Interior, hd.hd_Tipo_Subdivision, hd.hd_Subdivision, hd.hd_Localidad, hd.hd_Municipio_Ciudad, state.est_Nombre_Corto, country.pais_Nombre_Corto, ""),
                                  tel = hd.hd_Telefono,
                                  //Query que obiene los integrantes del Hogar en que está siendo analizado en el bucle foreach.
                                  integrantes = (from hp in context.Hogar_Persona
                                                 join p in context.Persona
                                                 on hp.per_Id_Persona equals p.per_Id_Persona
                                                 where hp.hd_Id_Hogar == hogar.hogarId && p.per_Activo == true
                                                 orderby (hp.hp_Jerarquia)
                                                 select new IntegrantesHogar
                                                 {
                                                     nombre = p.per_Nombre + " " + p.per_Apellido_Paterno + " " + (p.per_Apellido_Materno != null ? p.per_Apellido_Materno : ""),
                                                     cel = p.per_Telefono_Movil,
                                                     nacimiento = p.per_Fecha_Nacimiento,
                                                     edad = (fechayhora - p.per_Fecha_Nacimiento).Days / 365,
                                                     grupo = p.per_Bautizado == true ? "B" : "NB"
                                                 }).ToList()

                              }).ToList();

                hogares.Add(new Homes
                {
                    indice = query2[0].indice,
                    direccion = query2[0].direccion,
                    tel = query2[0].tel,
                    integrantes = query2[0].integrantes
                });
            }


            return (hogares);
        }

    }
}
