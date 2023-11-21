using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Helpers;
using IECE_WebApi.Models;
using IECE_WebApi.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CasaPastoralController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CasaPastoralController(AppDbContext context)
        {
            _context = context;
        }


        public class casaPastoralConDomicilio
        {

            public string direccion { get; set; }
            public string telFijo { get; set; }
            public byte[] foto { get; set; }
            public string MIMEType { get; set; }
            public CasaPastoral casaPastoral { get; set; }
            public Domicilio domicilio { get; set; }

            public Sector sector { get; set; }

        }

        public class cpcd
        {
            public virtual CasaPastoral casaPastoral { get; set; }
            public virtual Domicilio domicilio { get; set; }
        }

        // GET: api/CasaPastorals
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetCasaPastoral()
        {
            var cp = _context.CasaPastoral.ToList();

            return Ok(new
            {
                status = "success",
                casasPastorales = cp
            });
        }

        // GET: api/CasaPastorals/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult GetCasaPastoral(int id)
        {
            var casaPastoral = _context.CasaPastoral.Find(id);

            if (casaPastoral == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                status = "success",
                casaPastorale = casaPastoral
            });
        }

        // PUT: api/CasaPastoral/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCasaPastoral(int id, [FromBody] cpcd cpcd)
        {
            if (id != cpcd.casaPastoral.cp_Id_CasaPastoral)
            {
                return BadRequest();
            }

            _context.Entry(cpcd.casaPastoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CasaPastoralExists(id))
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

        // POST: api/CasaPastorals

        [HttpPost]        
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult<CasaPastoral>> PostCasaPastoral(CasaPastoral casaPastoral)
        {
            _context.CasaPastoral.Add(casaPastoral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCasaPastoral", new { id = casaPastoral.cp_Id_CasaPastoral }, casaPastoral);
        }

        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public  IActionResult PostCasaPastoralConDomicilio([FromBody] cpcd cpcd)
        {
            var dom_Id = 0;
            try
            {
                //Se crea Nuevo Registro o se actualiza el Domicilio sólo si la casaPastoral está en diferente ubicación a Santuario
                if (cpcd.casaPastoral.cp_Mismo_Predio_Templo != true) {

                    if (cpcd.domicilio.dom_Id_Domicilio != 0) //Si ya existía el Domicilio lo Editará
                    {
                        _context.Entry(cpcd.domicilio).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else //Si No existía el Domicilio lo Registrará como Nuevo Registro
                    {
                        _context.Domicilio.Add(cpcd.domicilio);
                        _context.SaveChanges();

                        //Si agrega el Id del Domicilio recien creado al Registro de Casa Pastoral
                        dom_Id = cpcd.domicilio.dom_Id_Domicilio;
                        cpcd.casaPastoral.cp_Id_Domicilio = dom_Id;
                    }

                    
                }


                if (cpcd.casaPastoral.cp_Id_CasaPastoral !=0 ) // Si ya existía la Casa Pastoral, sólo lo editará
                {                    
                    //Si la edición consiste en que la casa Pastoral está en el mismo predió, borra el Domicilio_Id, si acaso tenía alguno.
                    if (cpcd.casaPastoral.cp_Mismo_Predio_Templo == true)
                    {
                        cpcd.casaPastoral.cp_Id_Domicilio = null;
                    }
                     
                    //Actualiza el Registro de casa Pastoral
                    _context.Entry(cpcd.casaPastoral).State = EntityState.Modified;
                    _context.SaveChanges();

                } else { //Si no existía la Casa Pastoral, lo registrará como Nuevo Registro
                    _context.CasaPastoral.Add(cpcd.casaPastoral);
                    _context.SaveChanges();
                }

                return Ok(new
                {
                    status = "success"
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

        // DELETE: api/CasaPastorals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CasaPastoral>> DeleteCasaPastoral(int id)
        {
            var casaPastoral = await _context.CasaPastoral.FindAsync(id);
            if (casaPastoral == null)
            {
                return NotFound();
            }

            _context.CasaPastoral.Remove(casaPastoral);
            await _context.SaveChangesAsync();

            return casaPastoral;
        }

        private bool CasaPastoralExists(int id)
        {
            return _context.CasaPastoral.Any(e => e.cp_Id_CasaPastoral == id);
        }

        // GET: api/CasaPastoral/GetCasaPastoralyDomicilioBySector/5
        [Route("[action]/{id}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]

        public IActionResult GetCasaPastoralyDomicilioBySector(int id)
        {
            try
            {
                var hogares = new Hogares(_context);
                var casaPastoralConDomicilio = new casaPastoralConDomicilio();

                var casaPastoral = (from sec in _context.Sector
                                    join cp in _context.CasaPastoral on sec.sec_Id_Sector equals cp.cp_Id_Sector
                                    join dom in _context.Domicilio on cp.cp_Id_Domicilio equals dom.dom_Id_Domicilio into domicilioGroup
                                    from dom in domicilioGroup.DefaultIfEmpty()
                                    join e in _context.Estado on dom != null ? dom.est_Id_Estado : 0 equals e.est_Id_Estado into estadoGroup
                                    from e in estadoGroup.DefaultIfEmpty()
                                    join pais in _context.Pais on (dom != null ? dom.pais_Id_Pais : 0) equals pais.pais_Id_Pais into paisGroup
                                    from pais in paisGroup.DefaultIfEmpty()
                                    where cp.cp_Id_Sector == id
                                    select new
                                    {
                                        sec,
                                        propiedadDe = cp.cp_Propiedad_De,
                                        mismoPredioSantuario = cp.cp_Mismo_Predio_Templo,
                                        activo = cp.cp_Activo,
                                        tel = cp.cp_Tel_Fijo,
                                        Calle = (dom != null ? dom.dom_Calle : null),
                                        Numero_Exterior = (dom != null ? dom.dom_Numero_Exterior : null),
                                        Numero_Interior = (dom != null ? dom.dom_Numero_Interior : null),
                                        Tipo_Subdivision = (dom != null ? dom.dom_Tipo_Subdivision : null),
                                        Subdivision = (dom != null ? dom.dom_Subdivision : null),
                                        Localidad = (dom != null ? dom.dom_Localidad : null),
                                        Municipio_Ciudad = (dom != null ? dom.dom_Municipio_Ciudad : null),
                                        Id_Pais = (dom != null ? dom.pais_Id_Pais : 0),
                                        pais = (pais != null ? pais.pais_Nombre_Corto : null),
                                        Id_Estado = (dom != null ? dom.est_Id_Estado : 0),
                                        estado = (e != null ? e.est_Nombre : null),
                                        Codigo_Postal = (dom != null ? dom.dom_Codigo_Postal : null),
                                        domicilio = (dom != null ? dom : null),
                                        casaPastoral = (cp != null ? cp : null),
                                    }).FirstOrDefault();


                if (casaPastoral != null)
                {

                    casaPastoralConDomicilio = new casaPastoralConDomicilio
                    {
                        casaPastoral = casaPastoral.casaPastoral,
                        domicilio = (Domicilio)casaPastoral.domicilio,
                        direccion = hogares.getDireccion(casaPastoral.Calle, casaPastoral.Numero_Exterior, 
                                    casaPastoral.Numero_Interior, 
                                    casaPastoral.Tipo_Subdivision, 
                                    casaPastoral.Subdivision, 
                                    casaPastoral.Localidad, 
                                    casaPastoral.Municipio_Ciudad, 
                                    casaPastoral.estado, 
                                    casaPastoral.pais, 
                                    casaPastoral.Codigo_Postal),
                        sector = casaPastoral.sec
                    };
                }

                List<casaPastoralConDomicilio> casasPastoralesConDomicilio = new List<casaPastoralConDomicilio> { casaPastoralConDomicilio };


                if (casaPastoral == null)
                {
                    return Ok(new
                    {
                        status = "notFound",
                        casaPastoralConDomicilio = new casaPastoralConDomicilio
                        {
                            casaPastoral = null,
                            domicilio=null,
                            direccion= null
                        }
                    });
                }

                return Ok(new
                {
                    status = "success",
                    casaPastoralConDomicilio = casasPastoralesConDomicilio
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


        // GET: api/CasaPastoral/GetCasaPastoralyDomicilioBySector/5
        [Route("[action]/{id}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]

        public IActionResult GetCasaPastoralyDomicilioByDistrito(int id)
        {
            try
            {
                var hogares = new Hogares(_context);
                var casaPastoralConDomicilio = new casaPastoralConDomicilio();
                List<casaPastoralConDomicilio> casasPastoralesConDomicilio = new List<casaPastoralConDomicilio> {};

                var sectores = (from dis in _context.Distrito
                                 join sec in _context.Sector on dis.dis_Id_Distrito equals sec.dis_Id_Distrito
                                 where dis.dis_Id_Distrito == id && sec.sec_Tipo_Sector == "Sector"
                                 orderby sec.sec_Numero
                                 select new {sec, sec.sec_Id_Sector}).ToList();


                foreach (var sector in sectores)
                {
                    var casaPastoral = (from sec in _context.Sector
                                        join cp in _context.CasaPastoral on sec.sec_Id_Sector equals cp.cp_Id_Sector
                                        join dom in _context.Domicilio on cp.cp_Id_Domicilio equals dom.dom_Id_Domicilio into domicilioGroup
                                        from dom in domicilioGroup.DefaultIfEmpty()
                                        join e in _context.Estado on dom != null ? dom.est_Id_Estado : 0 equals e.est_Id_Estado into estadoGroup
                                        from e in estadoGroup.DefaultIfEmpty()
                                        join pais in _context.Pais on (dom != null ? dom.pais_Id_Pais : 0) equals pais.pais_Id_Pais into paisGroup
                                        from pais in paisGroup.DefaultIfEmpty()
                                        where cp.cp_Id_Sector == sector.sec_Id_Sector
                                        select new
                                        {
                                            sec,
                                            propiedadDe = cp.cp_Propiedad_De,
                                            mismoPredioSantuario = cp.cp_Mismo_Predio_Templo,
                                            activo = cp.cp_Activo,
                                            tel = cp.cp_Tel_Fijo,
                                            Calle = (dom != null ? dom.dom_Calle : null),
                                            Numero_Exterior = (dom != null ? dom.dom_Numero_Exterior : null),
                                            Numero_Interior = (dom != null ? dom.dom_Numero_Interior : null),
                                            Tipo_Subdivision = (dom != null ? dom.dom_Tipo_Subdivision : null),
                                            Subdivision = (dom != null ? dom.dom_Subdivision : null),
                                            Localidad = (dom != null ? dom.dom_Localidad : null),
                                            Municipio_Ciudad = (dom != null ? dom.dom_Municipio_Ciudad : null),
                                            Id_Pais = (dom != null ? dom.pais_Id_Pais : 0),
                                            pais = (pais != null ? pais.pais_Nombre_Corto : null),
                                            Id_Estado = (dom != null ? dom.est_Id_Estado : 0),
                                            estado = (e != null ? e.est_Nombre : null),
                                            Codigo_Postal = (dom != null ? dom.dom_Codigo_Postal : null),
                                            domicilio = (dom != null ? dom : null),
                                            casaPastoral = (cp != null ? cp : null),
                                        }).FirstOrDefault();


                    if (casaPastoral != null)
                    {

                        casaPastoralConDomicilio = new casaPastoralConDomicilio
                        {
                            casaPastoral = casaPastoral.casaPastoral,
                            domicilio = (Domicilio)casaPastoral.domicilio,
                            direccion = hogares.getDireccion(casaPastoral.Calle, casaPastoral.Numero_Exterior,
                                        casaPastoral.Numero_Interior,
                                        casaPastoral.Tipo_Subdivision,
                                        casaPastoral.Subdivision,
                                        casaPastoral.Localidad,
                                        casaPastoral.Municipio_Ciudad,
                                        casaPastoral.estado,
                                        casaPastoral.pais,
                                        casaPastoral.Codigo_Postal),
                            sector = casaPastoral.sec
                        };

                        //casasPastoralesConDomicilio.Add(casaPastoralConDomicilio);
                    }
                    else
                    {
                        casaPastoralConDomicilio = new casaPastoralConDomicilio
                        {
                            casaPastoral = null,
                            domicilio = null,
                            direccion = null,
                            sector = sector.sec
                        };
                    }

                    casasPastoralesConDomicilio.Add(casaPastoralConDomicilio);


                }


                if (casasPastoralesConDomicilio == null)
                {
                    return Ok(new
                    {
                        status = "notFound",
                        casaPastoralConDomicilio = new casaPastoralConDomicilio
                        {
                            casaPastoral = null,
                            domicilio = null,
                            direccion = null
                        }
                    });
                }

                return Ok(new
                {
                    status = "success",
                    casaPastoralConDomicilio = casasPastoralesConDomicilio
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
