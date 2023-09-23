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
    public class Integrantes_Comisiones_Local
    {
        public string Comision { get; set; }
        public int Comision_Id { get; set; }
        public List<Integrantes> Integrantes { get; set; }
    }

    public class Integrantes
    {
        public int Integrante_Comision_Id { get; set; }
        public string Comision { get; set; }
        public string Integrante { get; set; }
        public int Jerarquia { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class Integrante_Comision_LocalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Integrante_Comision_LocalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Integrante_Comision_Local
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Integrante_Comision_Local>>> GetIntegrante_Comision_Local()
        {
            return await _context.Integrante_Comision_Local.ToListAsync();
        }

        // GET: api/Integrante_Comision_Local/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Integrante_Comision_Local>> GetIntegrante_Comision_Local(int id)
        {
            var integrante_Comision_Local = await _context.Integrante_Comision_Local.FindAsync(id);

            if (integrante_Comision_Local == null)
            {
                return NotFound();
            }

            return integrante_Comision_Local;
        }

        // PUT: api/Integrante_Comision_Local/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIntegrante_Comision_Local(int id, Integrante_Comision_Local integrante_Comision_Local)
        {
            if (id != integrante_Comision_Local.Id_Integrante_Comision)
            {
                return BadRequest();
            }

            _context.Entry(integrante_Comision_Local).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Integrante_Comision_LocalExists(id))
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

        // POST: api/Integrante_Comision_Local
        [HttpPost]
        [Route("[action]")]
        [EnableCors("AllowOrigin")]
        public IActionResult PostIntegrante_Comision_Local(Integrante_Comision_Local integrante_Comision_Local)
        {
            Integrante_Comision_Local NuevoIntegrante = integrante_Comision_Local;

            //Verifica si debe mover las Jerarquías
            var Integrantes = (from icl in _context.Integrante_Comision_Local
                               where icl.Sector_Id == integrante_Comision_Local.Sector_Id && icl.Comision_Id == integrante_Comision_Local.Comision_Id && icl.Activo == true
                               orderby icl.Jerarquia_Integrante
                               select icl).ToList();

            if (Integrantes.Count > 0)
            {
                foreach (var integrante in Integrantes)
                {
                    if (integrante_Comision_Local.Jerarquia_Integrante <= integrante.Jerarquia_Integrante)
                    {

                        integrante.Jerarquia_Integrante = integrante.Jerarquia_Integrante + 1;
                        _context.SaveChanges();

                    }
                }
            }

            try
            {
                _context.Integrante_Comision_Local.Add(NuevoIntegrante);
                 _context.SaveChanges();
                //return CreatedAtAction("GetIntegrante_Comision_Local", new { id = integrante_Comision_Local.Id_Integrante_Comision }, integrante_Comision_Local);
                             
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

        // DELETE: api/Integrante_Comision_Local/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Integrante_Comision_Local>> DeleteIntegrante_Comision_Local(int id)
        {
            var integrante_Comision_Local = await _context.Integrante_Comision_Local.FindAsync(id);
            if (integrante_Comision_Local == null)
            {
                return NotFound();
            }

            _context.Integrante_Comision_Local.Remove(integrante_Comision_Local);
            await _context.SaveChangesAsync();

            return integrante_Comision_Local;
        }

        private bool Integrante_Comision_LocalExists(int id)
        {
            return _context.Integrante_Comision_Local.Any(e => e.Id_Integrante_Comision == id);
        }

        // GET: api/Integrante_Comision_Local/GetComisionesBySector/{sector_Id]
        // METODO PARA OBTENER Integrantes de Comisiones By Sector
        [Route("[action]/{sector_Id}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public IActionResult GetComisionesBySector(int sector_Id)
        {
            List<Integrantes_Comisiones_Local> integrantes = new List<Integrantes_Comisiones_Local> { };
            try
            {
                var comisionesLocales = (from cl in _context.Comision_Local
                                        select new
                                        {
                                            ComisionId = cl.ComisionLocal_Id,
                                            ComisionNombre = cl.Nombre
                                        }).ToList();

                foreach (var cl in comisionesLocales)
                {
                    var integrantes_comisiones = new Integrantes_Comisiones_Local
                    {
                        Comision = cl.ComisionNombre,
                        Comision_Id = cl.ComisionId,
                        Integrantes = (from icl in _context.Integrante_Comision_Local
                                       join s in _context.Sector on icl.Sector_Id equals s.sec_Id_Sector
                                       join c in _context.Comision_Local on icl.Comision_Id equals c.ComisionLocal_Id
                                       join p in _context.Persona on icl.Integrante_Id equals p.per_Id_Persona
                                       where s.sec_Id_Sector == sector_Id && c.ComisionLocal_Id == cl.ComisionId && icl.Activo == true
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

        // GET: api/Integrante_Comision_Local/BajaDeIntegranteComision/{Intgrante_Comision_id]
        // METODO PARA DAR DE BAJA Integrantes de Comisiones en el Sector
        [Route("[action]/{Intgrante_Comision_id}")]
        [HttpPut]
        [EnableCors("AllowOrigin")]
        public IActionResult BajaDeIntegranteComision(int Intgrante_Comision_id)
        {
            try
            {
                var integrante_comision = _context.Integrante_Comision_Local.FirstOrDefault(icl => icl.Id_Integrante_Comision == Intgrante_Comision_id);

                if (integrante_comision == null)
                {
                    return NotFound();
                }
                else
                {
                    //Verifica si debe mover las Jerarquías
                    var Integrantes = (from icl in _context.Integrante_Comision_Local
                                       where icl.Sector_Id == integrante_comision.Sector_Id 
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
