﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly DateTime fechayhora = DateTime.UtcNow;
        private readonly IConfiguration _configuration;
        private readonly string superSecreto = global.LlaveAltaUsuario;
        // CLAVE PARA FASE 1 DE ALTA DE USUARIO, METODO DE VERIFICACION DE CORREO
        //private readonly string cf1 = "1dc5cfa8-b770-43f4-ad11-51126df7f8ad";
        // CLAVE PARA FASE 2 DE ALTA DE USUARIO, METODO DE ALTA DE USUARIO
        //private readonly string cf2 = "d41a3311-1098-4639-9c55-e2dd88351fa9";

        public object JwTRegistredClaimName { get; private set; }

        public class objInfoSesion
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public int mu_pem_Id_Pastor { get; set; }
            public string mu_permiso { get; set; }
            public int pem_Id_Ministro { get; set; }
            public string pem_Nombre { get; set; }
            public string pem_Grado_Ministerial { get; set; }
            public string pem_Foto_Ministro { get; set; }
            public string pem_email_Personal { get; set; }
            public string pem_Cel1 { get; set; }
            public string pem_Cel2 { get; set; }
            public bool dg { get; set; } // Boleano que define si es miembro del directivo
        }

        public UsuarioController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._configuration = configuration;
            this.context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public ActionResult<IEnumerable<Usuario>> Get()
        {
            try
            {
                return Ok(context.Usuario.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // METODO QUE CONSTRUYE EL TOKEN
        private IActionResult BuildToken(UserInfo usuario, bool dg) // dg = Boleano que define si es miembro del directivo
        {
            List<objInfoSesion> info = new List<objInfoSesion>();
            if (dg)
            {
                var infoSesion = (from u in context.Usuario
                                  join mu in context.Ministro_Usuario on u.Id equals mu.mu_aspNetUsers_Id
                                  join pem in context.Personal_Ministerial on mu.mu_pem_Id_Pastor equals pem.pem_Id_Ministro
                                  where u.Email == usuario.Email
                                  select new
                                  {
                                      Id = u.Id,
                                      Email = u.Email,
                                      mu_pem_Id_Pastor = mu.mu_pem_Id_Pastor,
                                      mu_permiso = mu.mu_permiso,
                                      pem.pem_Id_Ministro,
                                      pem_Nombre = pem.pem_Nombre,
                                      pem_Grado_Ministerial = pem.pem_Grado_Ministerial,
                                      pem_Foto_Ministro = pem.pem_Foto_Ministro,
                                      pem_email_Personal = pem.pem_email_Personal,
                                      pem_Cel1 = pem.pem_Cel1,
                                      pem_Cel2 = pem.pem_Cel2
                                  }).ToList();

                info.Add(new objInfoSesion
                {
                    Id = infoSesion[0].Id,
                    Email = infoSesion[0].Email,
                    mu_pem_Id_Pastor = infoSesion[0].mu_pem_Id_Pastor,
                    mu_permiso = infoSesion[0].mu_permiso,
                    pem_Id_Ministro = infoSesion[0].pem_Id_Ministro,
                    pem_Nombre = infoSesion[0].pem_Nombre,
                    pem_Grado_Ministerial = infoSesion[0].pem_Grado_Ministerial,
                    pem_Foto_Ministro = infoSesion[0].pem_Foto_Ministro,
                    pem_email_Personal = infoSesion[0].pem_email_Personal,
                    pem_Cel1 = infoSesion[0].pem_Cel1,
                    pem_Cel2 = infoSesion[0].pem_Cel2,
                    dg = dg // Boleano que define si es miembro del directivo
                });
            }
            else
            {
                var infoSesion = (from u in context.Usuario
                                  join mu in context.Ministro_Usuario on u.Id equals mu.mu_aspNetUsers_Id
                                  join pem in context.Personal_Ministerial on mu.mu_pem_Id_Pastor equals pem.pem_Id_Ministro
                                  join s in context.Sector on pem.pem_Id_Ministro equals s.pem_Id_Pastor
                                  join d in context.Distrito on s.dis_Id_Distrito equals d.dis_Id_Distrito
                                  where u.Email == usuario.Email
                                  select new
                                  {
                                      Id = u.Id,
                                      Email = u.Email,
                                      mu_pem_Id_Pastor = mu.mu_pem_Id_Pastor,
                                      mu_permiso = mu.mu_permiso,
                                      pem.pem_Id_Ministro,
                                      pem_Nombre = pem.pem_Nombre,
                                      pem_Grado_Ministerial = pem.pem_Grado_Ministerial,
                                      pem_Foto_Ministro = pem.pem_Foto_Ministro,
                                      pem_email_Personal = pem.pem_email_Personal,
                                      pem_Cel1 = pem.pem_Cel1,
                                      pem_Cel2 = pem.pem_Cel2
                                  }).ToList();

                info.Add(new objInfoSesion
                {
                    Id = infoSesion[0].Id,
                    Email = infoSesion[0].Email,
                    mu_pem_Id_Pastor = infoSesion[0].mu_pem_Id_Pastor,
                    mu_permiso = infoSesion[0].mu_permiso,
                    pem_Id_Ministro = infoSesion[0].pem_Id_Ministro,
                    pem_Nombre = infoSesion[0].pem_Nombre,
                    pem_Grado_Ministerial = infoSesion[0].pem_Grado_Ministerial,
                    pem_Foto_Ministro = infoSesion[0].pem_Foto_Ministro,
                    pem_email_Personal = infoSesion[0].pem_email_Personal,
                    pem_Cel1 = infoSesion[0].pem_Cel1,
                    pem_Cel2 = infoSesion[0].pem_Cel2,
                    dg = dg // Boleano que define si es miembro del directivo
                });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Llave_super_secreta"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("E38shaGPv6xDgUHN4BffduCQx5fXSMRhyEY2r5tD"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(5);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "iece-tpr.ddns.net",
                audience: "iece",
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return Ok(new
            {
                status = "success",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiration,
                message = "Credenciales correctas! Cargando aplicacion...",
                infoSesion = info
            });
        }

        // POST: api/CrearUsuario
        // METODO PARA AGREGAR CREDENCIALES DE USUARIO PARA INICIO DE SESION
        [HttpPost]
        [Route("Create")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> CrearUsuario([FromBody] UserInfo usuario)
        {
            if (usuario.superSecreto != superSecreto)
            // if (usuario.superSecreto != cf2)
            {
                return Ok(
                    new
                    {
                        status = "error",
                        mensaje = "Error!. La clave 'super secreto' es incorrecta, el proceso se ha cancelado."
                    });
            }
            if (ModelState.IsValid)
            {
                var user = new Usuario { UserName = usuario.Email, Email = usuario.Email };
                var result = await _userManager.CreateAsync(user, usuario.Password);

                if (result.Succeeded)
                {
                    // BuildToken(usuario);
                    var query = (from pem in context.Personal_Ministerial
                                 where (pem.pem_emailIECE == user.Email || pem.pem_email_Personal == user.Email)
                                 && pem.pem_Activo == true
                                 select new
                                 {
                                     pem.pem_Id_Ministro,
                                     pem.pem_Nombre
                                 }).ToList();

                    Ministro_Usuario mu = new Ministro_Usuario();
                    mu.mu_aspNetUsers_Id = user.Id;
                    mu.mu_pem_Id_Pastor = query[0].pem_Id_Ministro;
                    mu.mu_permiso = "CRUD";

                    context.Ministro_Usuario.Add(mu);
                    context.SaveChanges();
                    return Ok(
                        new
                        {
                            status = "success",
                            mensaje = "Usuario creado satisfactoriamente.",
                            nvoUsuario = user
                        });
                }
                else
                {
                    return Ok(
                        new
                        {
                            status = "error",
                            mensaje = result.Errors
                        });
                }
            }
            else
            {
                return Ok(ModelState);
            }
        }

        // POST: api/Login
        // METODO PARA VALIDAR CREDENCIALES DE INICIO DE SESION
        [HttpPost]
        [Route("Login")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Login([FromBody] UserInfo usuario)
        {
            if (ModelState.IsValid)
            {
                bool esDelDirectivo = false; // Boleano que define si es miembro del directivo
                var result = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)//Si Credenciales están correctas, procede a verificar que esté Activo, Que sea Pastor de Sector u Obispo.
                {
                    //VERIFICA SI ES MIEMBRO DEL DIRECTIVO
                    var miembroDirectivo = (from u in context.Usuario
                                            join mu in context.Ministro_Usuario on u.Id equals mu.mu_aspNetUsers_Id
                                            join dg in context.Directivo_General on mu.mu_pem_Id_Pastor equals dg.pem_Id_Integrante
                                            where u.Email == usuario.Email
                                            select dg).ToList();

                    //Verifica si coincide el email, se está Activo y si es Pastor o Encargado de un Sector
                    var ministroActivo = (from u in context.Usuario
                                          join mu in context.Ministro_Usuario on u.Id equals mu.mu_aspNetUsers_Id
                                          join pm in context.Personal_Ministerial on mu.mu_pem_Id_Pastor equals pm.pem_Id_Ministro
                                          join S in context.Sector on pm.pem_Id_Ministro equals S.pem_Id_Pastor
                                          where u.Email == usuario.Email && pm.pem_Activo == true && S.sec_Tipo_Sector == "SECTOR"
                                          select pm.pem_Activo).ToList();

                    //Verifica si coincide el email, se está Activo y si es Obispo
                    var ministroActivo2 = (from u in context.Usuario
                                           join mu in context.Ministro_Usuario on u.Id equals mu.mu_aspNetUsers_Id
                                           join pm in context.Personal_Ministerial on mu.mu_pem_Id_Pastor equals pm.pem_Id_Ministro
                                           join D in context.Distrito on pm.pem_Id_Ministro equals D.pem_Id_Obispo
                                           where u.Email == usuario.Email && pm.pem_Activo == true
                                           select pm.pem_Activo).ToList();

                    if (miembroDirectivo.Count > 0)
                    {
                        esDelDirectivo = true;
                        return BuildToken(usuario, esDelDirectivo);
                    }
                    else if (ministroActivo.Count > 0 || ministroActivo2.Count > 0)
                    {
                        esDelDirectivo = false;
                        return BuildToken(usuario, esDelDirectivo);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error: El Usuario no tiene permisos de acceso.");
                        return Ok(new
                        {
                            status = "error",
                            message = "Error: El Usuario no tiene permisos de acceso."
                        });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: Credenciales incorrectas! Vuelva a intentar.");
                    return Ok(new
                    {
                        status = "error",
                        message = "Error: Credenciales incorrectas! Vuelva a intentar."
                    });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ingrese los campos requeridos.");
                return Ok(new
                {
                    status = "error",
                    message = "Ingrese los campos requeridos."
                });
            }
        }

        // GET: api/VerificaEmail
        // METODO PARA LA FASE1 DE ALTA DE USUARIOS
        [HttpGet]
        // [Route("[action]/{email}/{claveFase1}")]
        [Route("[action]/{email}")]
        [EnableCors("AllowOrigin")]
        // public IActionResult VerificaEmail(string email, string claveFase1)
        public IActionResult VerificaEmail(string email)
        {
            try
            {
                //Verifica si el email ya se había registrado como Usuario
                var query = (from u in context.Usuario
                             where u.Email == email
                             select new
                             {
                                 u.Id,
                                 u.Email
                             }).ToList();
                if (query.Count > 0)
                {
                    return Ok(new
                    {
                        status = "error",
                        mensaje = "El email ya está registado como Usuario en la Base de Datos."
                    });
                }
                else // Si no se encuentra aún registrado en la Tabla Usuario.
                {
                    //Debe coincidir el email ya sea con el email IECE.mx o uno Personal
                    //El Elemento del Personal Ministerial debe estar Activo
                    //Debe Estar como Encargado o Pastor de un Sector
                    var query1 = (from pem in context.Personal_Ministerial
                                  join S in context.Sector on pem.pem_Id_Ministro equals S.pem_Id_Pastor
                                  where (pem.pem_emailIECE == email || pem.pem_email_Personal == email)
                                  && pem.pem_Activo == true
                                  && S.sec_Tipo_Sector == "SECTOR"
                                  select new
                                  {
                                      pem.pem_Id_Ministro,
                                      pem.pem_Nombre
                                  }).ToList();

                    //Si no está como Pastor o Encargado de un Sector, Puede Estar Sólo como Obispo
                    var query2 = (from pem in context.Personal_Ministerial
                                  join D in context.Distrito on pem.pem_Id_Ministro equals D.pem_Id_Obispo
                                  where (pem.pem_emailIECE == email || pem.pem_email_Personal == email)
                                  && pem.pem_Activo == true
                                  select new
                                  {
                                      pem.pem_Id_Ministro,
                                      pem.pem_Nombre
                                  }).ToList();

                    if (query1.Count > 0 || query2.Count > 0)
                    {
                        return Ok(new
                        {
                            status = "success",
                            datos = query1.Count > 0 ? query1 : query2
                        }); ;
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = "error",
                            mensaje = "El email no se encontró registrado" +
                            "o no corresponde al de un Pastor o Encargado de un Sector ni al de un Obispo"
                        });
                    }
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
            //if (cf1 == claveFase1)
            //{
            //    try
            //    {
            //        var query = (from u in context.Usuario
            //                     where u.Email == email
            //                     select new
            //                     {
            //                         u.Id,
            //                         u.Email
            //                     }).ToList();
            //        if (query.Count > 0)
            //        {
            //            return Ok(new
            //            {
            //                status = "error",
            //                mensaje = "El email ya esta registado como usuario en la base de datos."
            //            });
            //        }
            //        else
            //        {
            //            var query1 = (from pem in context.Personal_Ministerial
            //                          where (pem.pem_emailIECE == email || pem.pem_email_Personal == email)
            //                          && pem.pem_Activo == true
            //                          select new
            //                          {
            //                              pem.pem_Id_Ministro,
            //                              pem.pem_Nombre
            //                          }).ToList();
            //            if (query1.Count > 0)
            //            {
            //                return Ok(new
            //                {
            //                    status = "success",
            //                    datos = query1
            //                });
            //            }
            //            else
            //            {
            //                return Ok(new
            //                {
            //                    status = "error",
            //                    mensaje = "No existe el correo registrado para ningun ministro."
            //                });
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        return Ok(new
            //        {
            //            status = "error",
            //            mensaje = ex.Message
            //        });
            //    }
            //}
            //else
            //{
            //    return Ok(new
            //    {
            //        status = "error",
            //        mensaje = "La clave de la Fase 1 de validación es incorrecta."
            //    });
            //}
        }

        // POST: api/Usuario
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult Post([FromBody] Usuario usuario)
        {
            try
            {
                context.Usuario.Add(usuario);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
