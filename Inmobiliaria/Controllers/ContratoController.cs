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
    public class ContratoController : Controller
    {
        private readonly IRepositorioContrato contratos;
        private readonly IRepositorioInmueble inmuebles;
        private readonly IRepositorioPago pagos;
        private readonly IRepositorio<Inquilino> inquilinos;
        private readonly IConfiguration config;

        public ContratoController(IRepositorioContrato contratos, IRepositorioInmueble inmuebles, IRepositorioPago pagos, IRepositorio<Inquilino> inquilinos, IConfiguration config)
        {
            this.contratos = contratos;
            this.inmuebles = inmuebles;
            this.pagos = pagos;
            this.inquilinos= inquilinos;
            this.config = config;
        }

        // GET: Contrato
        [Authorize]
        public ActionResult Index()
        {
            IList<Contrato> listaContratos;

            if (TempData.ContainsKey("IdInquilino"))
            {
                int idInquilino = Convert.ToInt32(TempData["IdInquilino"]);
                listaContratos = contratos.BuscarPorInquilino(idInquilino);
                if (listaContratos.Count == 0)
                {
                    TempData["Mensaje"] = "Nada para mostrar";
                    return RedirectToAction("Index", "Inquilino");
                }
            }
            else if (TempData.ContainsKey("IdInmueble"))
            {
                int idInmueble = Convert.ToInt32(TempData["IdInmueble"]);
                listaContratos = contratos.BuscarPorInmueble(idInmueble);
                if (listaContratos.Count == 0)
                {
                    TempData["Mensaje"] = "Nada para mostrar";
                    return RedirectToAction("Index", "Inmueble");
                }
            }
            else if (TempData["Vigentes"] != null)
            {
                listaContratos = contratos.BuscarVigentes();
                if (listaContratos.Count == 0)
                {
                    ViewBag.Mensaje = "Nada para mostrar";
                }
            }
            else
            {
                listaContratos = contratos.ObtenerTodos();
            }

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(listaContratos);
        }

        // GET: Contrato/Create
        [Authorize]
        public ActionResult Create()
        {
            if (TempData["IdInmueble"] != null)
            {
                ViewBag.IdInmueble = TempData["IdInmueble"];
            } else
            {
                ViewBag.IdInmueble = 1;
            }

            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            ViewBag.Inmuebles = inmuebles.ObtenerTodos();
            ViewBag.Inquilinos = inquilinos.ObtenerTodos();

            return View();
        }

        // POST: Contrato/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                contrato.Inquilino = inquilinos.ObtenerPorId(int.Parse(Request.Form["Inquilino"]));
                contrato.Inmueble = inmuebles.ObtenerPorId(int.Parse(Request.Form["Inmueble"]));
                contrato.Agente = new Agente { Id = 10 };

                if (ValidarFechasContrato(contrato) && contrato.Inmueble.Estado)
                {
                    if (ModelState.IsValid)
                    {
                        contratos.Alta(contrato);

                        TempData["Id"] = contrato.Id;

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.Inmuebles = inmuebles.ObtenerTodos();
                        ViewBag.Inquilinos = inquilinos.ObtenerTodos();

                        return View(contrato);
                    }
                } else
                {
                    ViewBag.Error = "Conflicto con otros contratos o inmueble deshabilitado";

                    ViewBag.Inmuebles = inmuebles.ObtenerTodos();
                    ViewBag.Inquilinos = inquilinos.ObtenerTodos();

                    return View(contrato);
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                ViewBag.Inmuebles = inmuebles.ObtenerTodos();
                ViewBag.Inquilinos = inquilinos.ObtenerTodos();

                return View();
            }
        }

        // GET: Contrato/Create
        [Authorize]
        public ActionResult Renovar(int id)
        {
            Contrato contrato = contratos.ObtenerPorId(id);

            return View(contrato);
        }

        // POST: Contrato/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(Contrato contrato)
        {
            try
            {
                contrato.Inquilino = inquilinos.ObtenerPorId(int.Parse(Request.Form["Inquilino"]));
                contrato.Inmueble = inmuebles.ObtenerPorId(int.Parse(Request.Form["Inmueble"]));
                contrato.Agente = new Agente { Id = 10 };

                if (ValidarFechasContrato(contrato))
                {
                    if (ModelState.IsValid)
                    {
                        contratos.Alta(contrato);

                        TempData["Mensaje"] = "Se renovó el contrato con éxito";

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.Inmuebles = inmuebles.ObtenerTodos();
                        ViewBag.Inquilinos = inquilinos.ObtenerTodos();

                        return View(contrato);
                    }
                }
                else
                {
                    ViewBag.Error = "Conflicto con otros contratos";

                    ViewBag.Inmuebles = inmuebles.ObtenerTodos();
                    ViewBag.Inquilinos = inquilinos.ObtenerTodos();

                    return View(contrato);
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                ViewBag.Inmuebles = inmuebles.ObtenerTodos();
                ViewBag.Inquilinos = inquilinos.ObtenerTodos();

                return View();
            }
        }

        // GET: Inmueble/Delete/5
        [Authorize]
        public ActionResult Cancelar(int id)
        {
            var contrato = contratos.ObtenerPorId(id);
            TempData["Multa"] = CalcularMulta(contrato);

            if (!ValidarPagos(contrato))
            {
                TempData["Error"] = "Pagos pendientes";

                return RedirectToAction(nameof(Index));
            }

            return View(contrato);
        }

        // POST: Inmueble/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancelar(int id, IFormCollection collection)
        {
            try
            {
                Contrato contrato = contratos.ObtenerPorId(id);

                contrato.FechaFin = DateTime.Now.AddDays(-1);

                contratos.Modificacion(contrato);

                TempData["Mensaje"] = "Se canceló el contrato con éxito";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                var contrato = contratos.ObtenerPorId(id);

                return View(contrato);
            }
        }

        [Authorize]
        public ActionResult MostrarPagos(int id)
        {
            Contrato contrato = contratos.ObtenerPorId(id);

            TempData["IdContrato"] = id;
            TempData["PrecioContrato"] = contrato.Precio;

            return RedirectToAction("Index", "Pago");
        }

        [Authorize]
        public ActionResult MostrarVigentes()
        {
            TempData["Vigentes"] = 1;

            return RedirectToAction(nameof(Index));
        }

        public Boolean ValidarFechasContrato(Contrato contrato)
        {
            Boolean res;

            var contratosInmueble = contratos.BuscarPorInmueble(contrato.Inmueble.Id);

            if (contratosInmueble.Count > 0)
            {
                DateTime fechaFinUltimoContrato = contratosInmueble[0].FechaFin;
                DateTime fechaInicioNuevoContrato = contrato.FechaInicio;
                int result = DateTime.Compare(fechaFinUltimoContrato, fechaInicioNuevoContrato);

                if (result <= 0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            } else
            {
                res = true;
            }


            return res;
        }

        public Boolean ValidarPagos(Contrato contrato)
        {
            Boolean res = false;
            int cantidadMeses = ObtenerCantidadMeses(contrato);
            int cantidadPagos = ObtenerCantidadPagos(contrato);

            if (cantidadPagos >= cantidadMeses)
            {
                res = true;
            }

            return res;
        }

        public int ObtenerCantidadMeses(Contrato contrato)
        {
            int res = 0;

            DateTime fechaInicioContrato = contrato.FechaInicio;
            DateTime fechaActual = DateTime.Now;

            while (fechaInicioContrato.Date < fechaActual.Date)
            {
                res ++;
                fechaInicioContrato = fechaInicioContrato.AddMonths(1);
            }

            return res;
        }

        public int ObtenerCantidadPagos(Contrato contrato)
        {
            int res = 0;

            var listaPagos = pagos.BuscarPorContrato(contrato);
            res = listaPagos.Count;

            return res;
        }

        public Decimal CalcularMulta(Contrato contrato)
        {
            Decimal multa;

            var tiempoMedioContrato = contrato.FechaFin.Subtract(contrato.FechaInicio).Ticks / 2;
            var tiempoCumplido = DateTime.Now.Subtract(contrato.FechaInicio).Ticks;

            if (tiempoCumplido >= tiempoMedioContrato)
            {
                multa = contrato.Precio;
            }
            else
            {
                multa = contrato.Precio * 2;
            }

            return multa;
        }
    }
}