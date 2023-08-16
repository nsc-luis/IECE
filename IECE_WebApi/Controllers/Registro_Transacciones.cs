using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IECE_WebApi.Controllers
{
    public class Registro_TransaccionesController : ControllerBase
    {
        private readonly AppDbContext context;

        public Registro_TransaccionesController(AppDbContext context)
        {
            this.context = context;
        }


        private DateTime fechayhora = DateTime.UtcNow;


        // METODO PARA ALTA DE REGISTRO HISTORICO DE TRANSACCIONES MINISTERIALES
        [HttpPost]
        [Route("[action]/{pem_Id_Ministro}/{sec_Id_Sector}/{mov_Tipo_Mov}/{mov_Detalle}/{mov_Comentario}/{hte_Fecha_Transaccion=}/{mov_Ejecutor_Id}/{Usu_Usuario_Id}")]
        [EnableCors("AllowOrigin")]
        public IActionResult RegistroHistorico(
            int pem_Id_Ministro,
            int sec_Id_Sector,
            string mov_Tipo_Mov,
            string mov_Detalle,
            string mov_Comentario,
            DateTime? hte_Fecha_Transaccion,
            int mov_Ejecutor_Id,
            int Usu_Usuario_Id
        )
        {
            try
            {
                var query = (from s in context.Sector
                             join d in context.Distrito
                             on s.dis_Id_Distrito equals d.dis_Id_Distrito
                             where s.sec_Id_Sector == sec_Id_Sector
                             select new
                             {
                                 s.dis_Id_Distrito,
                                 d.dis_Alias,
                                 s.sec_Id_Sector,
                                 s.sec_Alias
                             }).ToList();
                Registro_Transacciones nvoRegistro = new Registro_Transacciones();

                nvoRegistro.mov_Tipo_Mov = mov_Tipo_Mov;
                nvoRegistro.mov_Detalle = mov_Detalle;
                nvoRegistro.mov_Comentario = mov_Comentario;
                nvoRegistro.mov_Fecha_Mov = hte_Fecha_Transaccion;
                nvoRegistro.mov_Id_Ministro = pem_Id_Ministro;
                nvoRegistro.mov_Id_Distrito = query[0].dis_Id_Distrito;
                nvoRegistro.mov_Distrito = query[0].dis_Alias;
                nvoRegistro.mov_Id_Sector = query[0].sec_Id_Sector;
                nvoRegistro.mov_Sector = query[0].sec_Alias;
                nvoRegistro.mov_Id_Ejecutor_Mov = mov_Ejecutor_Id;
                nvoRegistro.mov_Usuario = Usu_Usuario_Id;
                nvoRegistro.mov_Fecha_Captura = fechayhora;


                // ALTA DE REGISTRO PARA HISTORICO
                context.Registro_Transacciones.Add(nvoRegistro);
                context.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    registro = nvoRegistro
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
