﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BautismoController : ControllerBase
    {
        private readonly AppDbContext context;

        public BautismoController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/Bautismo
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Bautismo>> Get()
        {
            try
            {
                return Ok(context.Bautismo.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Bautismo/5
        [HttpGet("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Get(int id)
        {
            Bautismo bautismo = new Bautismo();
            try
            {
                bautismo = context.Bautismo.FirstOrDefault(bau => bau.bau_Id_Bautismo == id);
                return Ok(bautismo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Bautismo/GetByIdPersona/5
        [Route("[action]/{idPersona}")]
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult GetByIdPersona(int idPersona)
        {
            Bautismo bautismo = new Bautismo();
            try
            {
                bautismo = context.Bautismo.FirstOrDefault(bau => bau.per_Id_Persona == idPersona);
                return Ok(bautismo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Bautismo
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public ActionResult Post([FromBody] Bautismo bautismo)
        {
            try
            {
                context.Bautismo.Add(bautismo);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Bautismo/5
        [HttpPut("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Put(int id, [FromBody] Bautismo bautismo)
        {
            if(bautismo.bau_Id_Bautismo == id)
            {
                context.Entry(bautismo).State = EntityState.Modified;
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [EnableCors("AllowOrigin")]
        public ActionResult Delete(int id)
        {
            Bautismo bautismo = new Bautismo();
            bautismo = context.Bautismo.FirstOrDefault(bau => bau.bau_Id_Bautismo == id);
            if (bautismo != null)
            {
                context.Bautismo.Remove(bautismo);
                context.SaveChanges();
                return Ok();
            } else
            {
                return BadRequest();
            }
        }
    }
}