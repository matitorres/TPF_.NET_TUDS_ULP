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
    public class PagoController : Controller
    {

        private readonly IRepositorioContrato contratos;
        private readonly IRepositorioPago pagos;
        private readonly IConfiguration config;

        public PagoController(IRepositorioContrato contratos, IRepositorioPago pagos, IConfiguration config)
        {
            this.contratos = contratos;
            this.pagos = pagos;
            this.config = config;
        }

        // GET: Pago
        [Authorize]
        public ActionResult Index()
        {
            if (!TempData.ContainsKey("IdContrato")) return RedirectToAction("Index", "Contrato");

            Contrato contrato = new Contrato
            {
                Id = Convert.ToInt32(TempData["IdContrato"]),
                Precio = Convert.ToDecimal(TempData["PrecioContrato"])
            };

            var listaPagos = pagos.BuscarPorContrato(contrato);

            if (listaPagos.Count == 0)
            {
                listaPagos.Add(new Pago
                {
                    Id = 0,
                    Contrato = contrato
                });
            }

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(listaPagos);
        }

        // GET: Pago/Create
        [Authorize]
        public ActionResult Create(int id, int idcontrato)
        {
            Contrato contrato = contratos.ObtenerPorId(idcontrato);
            Pago pago = ObtenerPago(id, contrato);

            try
            {
                pago.NumeroPago = (Convert.ToInt32(pago.NumeroPago) + 1).ToString();
                pagos.Alta(pago);

                TempData["IdContrato"] = pago.Contrato.Id;
                TempData["PrecioContrato"] = pago.Contrato.Precio;
                TempData["Id"] = pago.Id;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;

                TempData["IdContrato"] = pago.Contrato.Id;
                TempData["PrecioContrato"] = pago.Contrato.Precio;

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Pago/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Pago pago = pagos.ObtenerPorId(id);

            return View(pago);
        }

        // POST: Pago/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Pago pago = pagos.ObtenerPorId(id);

                TempData["IdContrato"] = pago.Contrato.Id;
                TempData["PrecioContrato"] = pago.Contrato.Precio;

                pagos.Baja(id);

                TempData["Mensaje"] = "Se ha eliminado el pago con ID: " + pago.Id;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;

                return View();
            }
        }

        public Pago ObtenerPago(int id, Contrato contrato)
        {
            Pago pago;

            if (id != 0)
            {
                pago = pagos.ObtenerPorId(id);
            }
            else
            {
                pago = new Pago
                {
                    NumeroPago = "0",
                    Contrato = contrato
                };
            }

            return pago;
        }
    }
}