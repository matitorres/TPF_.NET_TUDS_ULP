using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class AgenteController : Controller
    {
        private readonly IRepositorioAgente agentes;
        private readonly IConfiguration config;

        public AgenteController(IRepositorioAgente agentes, IConfiguration config)
        {
            this.agentes= agentes;
            this.config = config;
        }

        // GET: Agente
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            var listaAgentes = agentes.ObtenerTodos();

            return View(listaAgentes);
        }

        // GET: Agente/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agente/Create
        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Agente agente)
        {
            try
            {
                Agente agenteExistente = agentes.GetPass(agente.Mail);

                if (agenteExistente != null)
                {
                    return View();
                }

                agente.Salt = GenerarSalt();
                agente.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: agente.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(agente.Salt),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                agentes.Alta(agente);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Agente/Edit/5
        [Authorize]
        public ActionResult EditClave(int id)
        {
            var agente = agentes.ObtenerPorId(id);

            return View(agente);
        }

        // POST: Agente/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClave(int id, Agente agenteClaveNueva)
        {
            try
            {
                string hashedClaveActual = agentes.GetPassPorId(id).Clave;
                string saltActual = agentes.GetPassPorId(id).Salt;
                string hashedClaveIngresada = HashClave(Request.Form["clave-actual"], saltActual);

                if (hashedClaveActual.Equals(hashedClaveIngresada))
                {
                    agenteClaveNueva.Salt = GenerarSalt();
                    agenteClaveNueva.Clave = HashClave(agenteClaveNueva.Clave, agenteClaveNueva.Salt);

                    agentes.ModificarClave(agenteClaveNueva);

                    if (User.IsInRole("Administrador"))
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    return RedirectToAction("Index", "Home");

                } else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        public string GenerarSalt()
        {
            Guid guid = Guid.NewGuid();
            string salt = Convert.ToBase64String(guid.ToByteArray());
            salt = salt.Replace("=", "").Replace("+", "").Replace("/", "");

            return salt;
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