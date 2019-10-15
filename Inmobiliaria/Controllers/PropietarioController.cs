using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario propietarios;
        private readonly IConfiguration config;

        public PropietarioController(IRepositorioPropietario propietarios, IConfiguration config)
        {
            this.propietarios = propietarios;
            this.config = config;
        }

        // GET: Propietario
        [Authorize]
        public ActionResult Index()
        {
            var listaPropietarios = propietarios.ObtenerTodos();

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(listaPropietarios);
        }

        // GET: Propietario/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    propietario.Salt = "123";
                    propietarios.Alta(propietario);

                    TempData["Id"] = propietario.Id;

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(propietario);
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                return View();
            }
        }

        // GET: Propietario/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var propietario = propietarios.ObtenerPorId(id);
            return View(propietario);
        }

        // POST: Propietario/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                propietario.Id = id;
                propietarios.Modificacion(propietario);

                TempData["Mensaje"] = "Se han actualizado los datos del propietario";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                return View();
            }
        }

        [Authorize]
        public ActionResult MostrarInmuebles(int id)
        {
            TempData["IdPropietario"] = id;

            return RedirectToAction("Index", "Inmueble");
        }
    }
}