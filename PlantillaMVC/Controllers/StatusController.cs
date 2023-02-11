using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Modelo;
using PlantillaMVC.Filters;
using Newtonsoft.Json;
using System.Web.Services;

namespace PlantillaMVC.Controllers
{
    public class StatusController : Controller
    {
        // GET: Status
        public PartialViewResult Index()
        {
            List<EStatus> Lista = Utilidades.negocio.GetStatus();
            return PartialView(Lista);
        }

        [Autentificado]
        public PartialViewResult Create()
        {
            //if (!Utilidades.ValidaSesion(Session, HttpContext))
            //    return View("Login");

            return PartialView();
        }

        [HttpPost]
        [Autentificado]
        [ValidateAntiForgeryToken]
        public PartialViewResult Create(EStatus modelo)
        {
            //modelo.Visible = true;
            Utilidades.negocio.SetStatus(modelo);

            List<EStatus> Lista = Utilidades.negocio.GetStatus();
            return PartialView("Index", Lista);
        }

        [Autentificado]
        public PartialViewResult Edit(int Id)
        {
            //if (!Utilidades.ValidaSesion(Session, HttpContext))
            //    return View("Login");
            EStatus binn = new EStatus();
            binn = Utilidades.negocio.GetStatusById(Id);

            return PartialView(binn);
        }

        [HttpPost]
        [Autentificado]
        [ValidateAntiForgeryToken]
        public PartialViewResult Edit(EStatus modelo)
        {
            //if (!Utilidades.ValidaSesion(Session, HttpContext))
            //    return View("Login");
            bool saved = Utilidades.negocio.SetStatus(modelo);

            List<EStatus> Lista = Utilidades.negocio.GetStatus();

            return PartialView("Index", Lista);
        }

        [Autentificado]
        public PartialViewResult Detail(int Id)
        {
            //Modelo.Clases.CEvent cmodelo = Utilidades.negocio.GetEventById(Id);
            EStatus country = new EStatus();
            country = Utilidades.negocio.GetStatusById(Id);
            return PartialView(country);
        }

        public ActionResult AjaxStatus()
        {
            try
            {
                var listaStatus = Utilidades.negocio.GetStatus();

                object json = new { data = listaStatus.ToArray() };
                //object data = new { data = JsonConvert.SerializeObject(listaStatus) };

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "OK" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}