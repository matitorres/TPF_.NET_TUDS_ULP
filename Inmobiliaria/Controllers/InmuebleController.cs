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
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble inmuebles;
        private readonly IRepositorioPropietario propietarios;
        private readonly IRepositorioContrato contratos;
        private readonly IConfiguration config;

        public InmuebleController(IRepositorioInmueble inmuebles, IRepositorioPropietario propietarios, IRepositorioContrato contratos, IConfiguration config)
        {
            this.inmuebles = inmuebles;
            this.propietarios = propietarios;
            this.contratos = contratos;
            this.config = config;
        }

        // GET: Inmueble
        [Authorize]
        public ActionResult Index()
        {
            IList<Inmueble> listaInmuebles;

            if (TempData["IdPropietario"]!=null)
            {
                int id = int.Parse(TempData["IdPropietario"].ToString());
                listaInmuebles = inmuebles.BuscarPorPropietario(id);
                if (listaInmuebles.Count == 0)
                {
                    TempData["Mensaje"] = "Nada para mostrar";
                    return RedirectToAction("Index", "Propietario");
                }

            } else if (TempData["Disponibles"] != null)
            {
                listaInmuebles = inmuebles.ObtenerDisponibles();
                if (listaInmuebles.Count == 0)
                {
                    ViewBag.Mensaje = "Nada para mostrar";
                }

            } else if (TempData["Tipo"] != null)
            {
                Inmueble inmueble = new Inmueble
                {
                    Tipo = TempData["Tipo"].ToString(),
                    Uso = TempData["Uso"].ToString(),
                    Ambientes = Convert.ToInt32(TempData["Ambientes"]),
                    Precio = Convert.ToDecimal(TempData["Precio"]),
                };
                listaInmuebles = inmuebles.Buscar(inmueble);
                if (listaInmuebles.Count == 0)
                {
                    ViewBag.Mensaje = "Nada para mostrar";
                }

            } else
            {
                listaInmuebles = inmuebles.ObtenerTodos();
            }

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(listaInmuebles);
        }

        // GET: Inmueble/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Propietarios = propietarios.ObtenerTodos();
            return View();
        }

        // POST: Inmueble/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                entidad.Propietario = propietarios.ObtenerPorId(Convert.ToInt32(Request.Form["Propietario"]));

                if (ModelState.IsValid)
                {
                    inmuebles.Alta(entidad);

                    TempData["Id"] = entidad.Id;

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = propietarios.ObtenerTodos();
                    return View(entidad);
                }
            }
            catch (Exception e)
            {
                ViewBag.Propietarios = propietarios.ObtenerTodos();
                ViewBag.Error = e.Message;

                return View(entidad);
            }
        }

        // GET: Inmueble/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var inmueble = inmuebles.ObtenerPorId(id);

            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                inmueble.Id = id;
                inmuebles.Modificacion(inmueble);

                TempData["Mensaje"] = "Se han actualizado los datos del inmueble";

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
            TempData["IdInmueble"] = id;

            return RedirectToAction("Index", "Contrato");

        }

        [Authorize]
        public ActionResult MostrarDisponibles()
        {
            TempData["Disponibles"] = 1;

            return RedirectToAction("Index");

        }

        // GET
        [Authorize]
        public ActionResult Buscar()
        {
            return View();

        }

        // POST: Inmueble/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buscar(int id, Inmueble inmueble)
        {
            try
            {
                String tipo = "", uso = "";

                if (inmueble.Tipo != null)
                {
                    tipo = inmueble.Tipo;
                }

                if (inmueble.Uso != null)
                {
                    uso = inmueble.Uso;
                }

                TempData["Tipo"] = tipo;
                TempData["Uso"] = uso;
                TempData["Ambientes"] = inmueble.Ambientes;
                TempData["Precio"] = inmueble.Precio;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                return View();
            }
        }

        // GET
        [Authorize]
        public ActionResult CrearContrato(int id)
        {
            TempData["IdInmueble"] = id;

            return RedirectToAction("Create", "Contrato");
        }
    }
}