using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;

namespace IECE_WebApi.Controllers
{

    //Modelo
    public class Integrantes_Comisiones_Distrital
    {
        public string Comision { get; set; }
        public int Comision_Id { get; set; }
        public List<Integrantes> Integrantes { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class Integrante_Comision_DistritalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Integrante_Comision_DistritalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Integrante_Comision_Distrital
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Integrante_Comision_Distrital>>> GetIntegrante_Comision_Distrital()
        {
            return await _context.Integrante_Comision_Distrital.ToListAsync();
        }

        // GET: api/Integrante_Comision_Distrital/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Integrante_Comision_Distrital>> GetIntegrante_Comision_Distrital(int id)
        {
            var integrante_Comision_Distrital = await _context.Integrante_Comision_Distrital.FindAsync(id);

            if (integrante_Comision_Distrital == null)
            {
                return NotFound();
            }

            return integrante_Comision_Distrital;
        }

        // PUT: api/Integrante_Comision_Distrital/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIntegrante_Comision_Distrital(int id, Integrante_Comision_Distrital integrante_Comision_Distrital)
        {
            if (id != integrante_Comision_Distrital.Id_Integrante_Comision)
            {
                return BadRequest();
            }

            _context.Entry(integrante_Comision_Distrital).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Integrante_Comision_DistritalExists(id))
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

        // POST: api/Integrante_Comision_Distrital
        
        [Route("[action]")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult PostIntegrante_Comision_Distrital([FromBody]Integrante_Comision_Distrital integrante_Comision_Distrital)
        {
            Integrante_Comision_Distrital NuevoIntegrante = integrante_Comision_Distrital;

            //Verifica si debe mover las Jerarquías
            var Integrantes = (from icl in _context.Integrante_Comision_Distrital
                               where icl.Distrito_Id == integrante_Comision_Distrital.Distrito_Id && icl.Comision_Id == integrante_Comision_Distrital.Comision_Id && icl.Activo == true
                               orderby icl.Jerarquia_Integrante
                               select icl).ToList();

            if (Integrantes.Count > 0)
            {
                foreach (var integrante in Integrantes)
                {
                    if (integrante_Comision_Distrital.Jerarquia_Integrante <= integrante.Jerarquia_Integrante)
                    {

                        integrante.Jerarquia_Integrante = integrante.Jerarquia_Integrante + 1;
                        _context.SaveChanges();

                    }
                }
            }

            try
            {
                _context.Integrante_Comision_Distrital.Add(NuevoIntegrante);
                 _context.SaveChanges();
                //return CreatedAtAction("GetIntegrante_Comision_Distrital", new { id = integrante_Comision_Distrital.Id_Integrante_Comision }, integrante_Comision_Distrital);
                             
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

        // DELETE: api/Integrante_Comision_Distrital/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Integrante_Comision_Distrital>> DeleteIntegrante_Comision_Distrital(int id)
        {
            var integrante_Comision_Distrital = await _context.Integrante_Comision_Distrital.FindAsync(id);
            if (integrante_Comision_Distrital == null)
            {
                return NotFound();
            }

            _context.Integrante_Comision_Distrital.Remove(integrante_Comision_Distrital);
            await _context.SaveChangesAsync();

            return integrante_Comision_Distrital;
        }

        private bool Integrante_Comision_DistritalExists(int id)
        {
            return _context.Integrante_Comision_Distrital.Any(e => e.Id_Integrante_Comision == id);
        }

        // GET: api/Integrante_Comision_Distrital/GetComisionesByDistrito/{Distrito_Id]
        // METODO PARA OBTENER Integrantes de Comisiones By Distrito
        [Route("[action]/{Distrito_Id}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetComisionesByDistrito(int Distrito_Id)
        {
            List<Integrantes_Comisiones_Distrital> integrantes = new List<Integrantes_Comisiones_Distrital> { };
            try
            {
                var comisionesDistritales = (from cl in _context.Comision_Distrital
                                        select new
                                        {
                                            ComisionId = cl.ComisionDistrital_Id,
                                            ComisionNombre = cl.Nombre
                                        }).ToList();

                foreach (var cl in comisionesDistritales)
                {
                    var integrantes_comisiones = new Integrantes_Comisiones_Distrital
                    {
                        Comision = cl.ComisionNombre,
                        Comision_Id = cl.ComisionId,
                        Integrantes = (from icl in _context.Integrante_Comision_Distrital
                                       join d in _context.Distrito on icl.Distrito_Id equals d.dis_Id_Distrito
                                       join c in _context.Comision_Distrital on icl.Comision_Id equals c.ComisionDistrital_Id
                                       join p in _context.Persona on icl.Integrante_Id equals p.per_Id_Persona
                                       where d.dis_Id_Distrito == Distrito_Id && c.ComisionDistrital_Id == cl.ComisionId && icl.Activo == true
                                       orderby icl.Jerarquia_Integrante
                                       select new Integrantes
                                       {
                                           Integrante_Comision_Id = icl.Id_Integrante_Comision,
                                           Comision = c.Nombre,
                                           Integrante = p.per_Nombre + ' ' + p.per_Apellido_Paterno + ' ' + (p.per_Apellido_Materno != null? p.per_Apellido_Materno: ""),
                                           Jerarquia = icl.Jerarquia_Integrante
                                       }).ToList()
                    };
                integrantes.Add(integrantes_comisiones);
            }

               
                return Ok(
            new
            {
                status = true,
                comisiones = integrantes,
            }
          );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new object[]
                    {
                        new
                        {
                            status= false,
                            mensaje = ex.Message
                        }
                    }
                );
            }
        }

        // GET: api/Integrante_Comision_Distrital/BajaDeIntegranteComision/{Intgrante_Comision_id]
        // METODO PARA DAR DE BAJA Integrantes de Comisiones en el Distrito
        [Route("[action]/{Intgrante_Comision_id}")]
        [HttpPut]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaDeIntegranteComision(int Intgrante_Comision_id)
        {
            try
            {
                var integrante_comision = _context.Integrante_Comision_Distrital.FirstOrDefault(icl => icl.Id_Integrante_Comision == Intgrante_Comision_id);

                if (integrante_comision == null)
                {
                    return NotFound();
                }
                else
                {
                    //Verifica si debe mover las Jerarquías
                    var Integrantes = (from icl in _context.Integrante_Comision_Distrital
                                       where icl.Distrito_Id == integrante_comision.Distrito_Id 
                                       && icl.Comision_Id == integrante_comision.Comision_Id 
                                       && icl.Activo == true 
                                       && icl.Id_Integrante_Comision != integrante_comision.Id_Integrante_Comision
                                       orderby icl.Jerarquia_Integrante
                                       select icl).ToList();

                    if (Integrantes.Count > 0)
                    {
                        foreach (var integrante in Integrantes)
                        {
                            if (integrante_comision.Jerarquia_Integrante < integrante.Jerarquia_Integrante)
                            {
                                integrante.Jerarquia_Integrante = integrante.Jerarquia_Integrante - 1;
                                _context.SaveChanges();
                            }
                        }
                    }

                    //Inactiva al Integrante
                    integrante_comision.Activo = false;
                    _context.SaveChanges();

                    return Ok(new
                    {
                        status = "success"
                    });
                }
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
