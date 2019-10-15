using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositorio<Inquilino> inquilinos;
        private readonly IConfiguration config;

        public InquilinoController(IRepositorio<Inquilino> inquilinos, IConfiguration config)
        {
            this.inquilinos= inquilinos;
            this.config = config;
        }

        // GET: Inquilino
        [Authorize]
        public ActionResult Index()
        {
            var listaInquilinos = inquilinos.ObtenerTodos();

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(listaInquilinos);
        }

        // GET: Inquilino/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inquilino/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                inquilinos.Alta(inquilino);

                TempData["Id"] = inquilino.Id;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                return View();
            }
        }

        // GET: Inquilino/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Inquilino inquilino = inquilinos.ObtenerPorId(id);
            return View(inquilino);
        }

        // POST: Inquilino/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                inquilino.Id = id;
                inquilinos.Modificacion(inquilino);

                TempData["Mensaje"] = "Se han actualizado los datos del inquilino";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                return View();
            }
        }

        [Authorize]
        public ActionResult MostrarContratos(int id)
        {
            TempData["IdInquilino"] = id;

            return RedirectToAction("Index", "Contrato");
        }
    }
}