using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Inmobiliaria.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositorioAgente agentes;
        private readonly IConfiguration config;

        public HomeController(IRepositorioAgente agentes, IConfiguration config)
        {
            this.agentes = agentes;
            this.config = config;
        }

        [Authorize]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Inmueble");
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginView loginView)
        {
            try
            {
                Agente usuario = agentes.GetPass(loginView.Mail);

                if (usuario != null)
                {
                    usuario.Mail = loginView.Mail;

                    string clave= usuario.Clave;
                    string salt = usuario.Salt;
                    string hashedClaveIngresada = HashClave(loginView.Clave, salt);

                    if (clave != hashedClaveIngresada)
                    {
                        ViewBag.Mensaje = "Datos inválidos";
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Mail),
                        new Claim(ClaimTypes.Role, usuario.Rol),
                        new Claim("http://example/identity/claims/id", usuario.Id.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index");

                } else
                {
                    ViewBag.Mensaje = "Datos inválidos";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Home/Login
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        public string HashClave(string clave, string salt)
        {
            string hashedClave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(salt),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));

            return hashedClave;
        }

    }
}