using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using IECE_WebApi.Repositorios;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TemploController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TemploController(AppDbContext context)
        {
            _context = context;
        }


        public class templosConFotoDto : Templo
        {
            public string domicilio  { get; set; }
            public string telFijo { get; set; }
            public byte[] foto { get; set; }
            public string MIMEType { get; set; }

            public Sector sector { get; set; }
            public Templo templo { get; set; }
        }


            // GET: api/Temploes
            [HttpGet]
        public async Task<ActionResult<IEnumerable<Templo>>> GetTemplo()
        {
            return await _context.Templo.ToListAsync();
        }

        // GET: api/Temploes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Templo>> GetTemplo(int id)
        {
            var templo = await _context.Templo.FindAsync(id);

            if (templo == null)
            {
                return NotFound();
            }

            return templo;
        }

        // PUT: api/Temploes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemplo(int id, Templo templo)
        {
            if (id != templo.tem_Id_Templo)
            {
                return BadRequest();
            }

            _context.Entry(templo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemploExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Temploes
        [HttpPost]
        public async Task<ActionResult<Templo>> PostTemplo(Templo templo)
        {
            _context.Templo.Add(templo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTemplo", new { id = templo.tem_Id_Templo }, templo);
        }

        // DELETE: api/Temploes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Templo>> DeleteTemplo(int id)
        {
            var templo = await _context.Templo.FindAsync(id);
            if (templo == null)
            {
                return NotFound();
            }

            _context.Templo.Remove(templo);
            await _context.SaveChangesAsync();

            return templo;
        }

        private bool TemploExists(int id)
        {
            return _context.Templo.Any(e => e.tem_Id_Templo == id);
        }


        // GET: api/Templo/5
        [Route("[action]/{id}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]

        public IActionResult GetTemployDomicilioBySector(int id)
        {
            try
            {
                var hogares = new Hogares(_context);
                var santuarioConFoto = new templosConFotoDto();
                List<templosConFotoDto> santuarioconFotoArray = new List<templosConFotoDto>() { };



                var santuario = (from sec in _context.Sector
                                 join tem in _context.Templo on sec.sec_Id_Sector equals tem.sec_Id_Sector into templos
                                 from tem in templos.DefaultIfEmpty()
                                 join dom in _context.Domicilio on tem.dom_Id_Domicilio equals dom.dom_Id_Domicilio into domicilios
                                 from dom in domicilios.DefaultIfEmpty()
                                 join e in _context.Estado on dom.est_Id_Estado equals e.est_Id_Estado into estados
                                 from e in estados.DefaultIfEmpty()
                                 join pais in _context.Pais on dom.pais_Id_Pais equals pais.pais_Id_Pais into paises
                                 from pais in paises.DefaultIfEmpty()
                                 where sec.sec_Id_Sector == id
                                 select new
                                 {
                                     sec,
                                     tem,
                                     tipoSantuario = tem != null ? tem.tem_Tipo_Templo : null,
                                     propiedadDe = tem != null ? tem.tem_Propiedad_De : null,
                                     aforo = tem != null ? tem.tem_Aforo : 0,
                                     tel = tem != null ? tem.tem_Telefono : null,
                                     Calle = dom != null ? dom.dom_Calle : null,
                                     Numero_Exterior = dom != null ? dom.dom_Numero_Exterior : null,
                                     Numero_Interior = dom != null ? dom.dom_Numero_Interior : null,
                                     Tipo_Subdivision = dom != null ? dom.dom_Tipo_Subdivision : null,
                                     Subdivision = dom != null ? dom.dom_Subdivision : null,
                                     Localidad = dom != null ? dom.dom_Localidad : null,
                                     Municipio_Ciudad = dom != null ? dom.dom_Municipio_Ciudad : null,
                                     Id_Pais = dom != null ? dom.pais_Id_Pais : 0,
                                     pais = pais != null ? pais.pais_Nombre_Corto : null,
                                     Id_Estado = dom != null ? dom.est_Id_Estado : 0,
                                     estado = e != null ? e.est_Nombre : null,
                                     Codigo_Postal = dom != null ? dom.dom_Codigo_Postal : null,
                                     foto = (tem != null && tem.tem_Foto_Frente != null && tem.tem_Foto_Frente != "") ? tem.tem_Foto_Frente.Replace("\\\\192.168.0.11", "c:\\DoctosCompartidos") : "c:\\DoctosCompartidos\\FotosTemplos\\SinFoto.jpg"
                                 }).FirstOrDefault();

                var imagenPath = santuario.foto; // Ruta de la imagen en el servidor
                var mimeType = "image/jpg"; // Establece el tipo MIME adecuado

                if (System.IO.File.Exists(imagenPath) )
                {
                    // Lee los datos binarios de la imagen
                    var imagenBytes = System.IO.File.ReadAllBytes(imagenPath);

                    santuarioConFoto = new templosConFotoDto
                    {
                        tem_Tipo_Templo = santuario.tipoSantuario,
                        tem_Propiedad_De = santuario.propiedadDe,
                        tem_Aforo = santuario.aforo,
                        telFijo = santuario.tel,
                        foto = imagenBytes,
                        domicilio = hogares.getDireccion(santuario.Calle, santuario.Numero_Exterior, santuario.Numero_Interior, santuario.Tipo_Subdivision, santuario.Subdivision, santuario.Localidad, santuario.Municipio_Ciudad, santuario.estado, santuario.pais, santuario.Codigo_Postal),
                        MIMEType = mimeType,
                        sector = santuario.sec,
                        templo = santuario.tem
                    };

                    santuarioconFotoArray.Add(santuarioConFoto);
                } else
                {
                    santuarioConFoto = new templosConFotoDto
                    {
                        sector = santuario.sec,
                        
                        templo = santuario.tem
                    };
                    santuarioconFotoArray.Add(santuarioConFoto);
                }

                if (santuario == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    status = "success",
                    santuarioConFoto = santuarioconFotoArray,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }

        }


        // GET: api/Templo/5
        [Route("[action]/{id}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]

        public IActionResult GetTemployDomicilioByDistrito(int id)
        {
            try
            {
                var hogares = new Hogares(_context);
                var santuarioConFoto = new templosConFotoDto();
                List<templosConFotoDto> santuarioconFotoArray = new List<templosConFotoDto>() { };

                var sectores = (from dis in _context.Distrito
                                join sec in _context.Sector on dis.dis_Id_Distrito equals sec.dis_Id_Distrito
                                where dis.dis_Id_Distrito == id && sec.sec_Tipo_Sector == "Sector"
                                orderby sec.sec_Numero
                                select new { sec, sec.sec_Id_Sector }).ToList();


                foreach (var sector in sectores)
                {

                    var santuario = (from sec in _context.Sector
                                     join tem in _context.Templo on sec.sec_Id_Sector equals tem.sec_Id_Sector into templos
                                     from tem in templos.DefaultIfEmpty()
                                     join dom in _context.Domicilio on tem.dom_Id_Domicilio equals dom.dom_Id_Domicilio into domicilios
                                     from dom in domicilios.DefaultIfEmpty()
                                     join e in _context.Estado on dom.est_Id_Estado equals e.est_Id_Estado into estados
                                     from e in estados.DefaultIfEmpty()
                                     join pais in _context.Pais on dom.pais_Id_Pais equals pais.pais_Id_Pais into paises
                                     from pais in paises.DefaultIfEmpty()
                                     where sec.sec_Id_Sector == sector.sec_Id_Sector
                                     select new
                                     {
                                         sec,
                                         tem,
                                         tipoSantuario = tem != null ? tem.tem_Tipo_Templo : null,
                                         propiedadDe = tem != null ? tem.tem_Propiedad_De : null,
                                         aforo = tem != null ? tem.tem_Aforo : 0,
                                         tel = tem != null ? tem.tem_Telefono : null,
                                         Calle = dom != null ? dom.dom_Calle : null,
                                         Numero_Exterior = dom != null ? dom.dom_Numero_Exterior : null,
                                         Numero_Interior = dom != null ? dom.dom_Numero_Interior : null,
                                         Tipo_Subdivision = dom != null ? dom.dom_Tipo_Subdivision : null,
                                         Subdivision = dom != null ? dom.dom_Subdivision : null,
                                         Localidad = dom != null ? dom.dom_Localidad : null,
                                         Municipio_Ciudad = dom != null ? dom.dom_Municipio_Ciudad : null,
                                         Id_Pais = dom != null ? dom.pais_Id_Pais : 0,
                                         pais = pais != null ? pais.pais_Nombre_Corto : null,
                                         Id_Estado = dom != null ? dom.est_Id_Estado : 0,
                                         estado = e != null ? e.est_Nombre : null,
                                         Codigo_Postal = dom != null ? dom.dom_Codigo_Postal : null,
                                         foto = (tem != null && tem.tem_Foto_Frente != null && tem.tem_Foto_Frente != "") ? tem.tem_Foto_Frente.Replace("\\\\192.168.0.11", "c:\\DoctosCompartidos") : "c:\\DoctosCompartidos\\FotosTemplos\\SinFoto.jpg"
                                     }).FirstOrDefault();

                    var imagenPath = santuario.foto; // Ruta de la imagen en el servidor
                    var mimeType = "image/jpg"; // Establece el tipo MIME adecuado

                    if (System.IO.File.Exists(imagenPath))
                    {
                        // Lee los datos binarios de la imagen
                        var imagenBytes = System.IO.File.ReadAllBytes(imagenPath);

                        santuarioConFoto = new templosConFotoDto
                        {
                            tem_Tipo_Templo = santuario.tipoSantuario,
                            tem_Propiedad_De = santuario.propiedadDe,
                            tem_Aforo = santuario.aforo,
                            telFijo = santuario.tel,
                            foto = imagenBytes,
                            domicilio = hogares.getDireccion(santuario.Calle, santuario.Numero_Exterior, santuario.Numero_Interior, santuario.Tipo_Subdivision, santuario.Subdivision, santuario.Localidad, santuario.Municipio_Ciudad, santuario.estado, santuario.pais, santuario.Codigo_Postal),
                            MIMEType = mimeType,
                            sector = santuario.sec,
                            templo = santuario.tem
                        };

                        santuarioconFotoArray.Add(santuarioConFoto);

                    }
                    else
                    {
                        santuarioConFoto = new templosConFotoDto
                        {
                            sector = santuario.sec,
                            templo = santuario.tem
                        };
                        santuarioconFotoArray.Add(santuarioConFoto);
                    }
                }

                if (santuarioconFotoArray == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    status = "success",
                    santuarioConFoto = santuarioconFotoArray,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }

        }
    }
}
