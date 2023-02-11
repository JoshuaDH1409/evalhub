using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using General;
using System.IO;
using System.Web;
using PlantillaMVC.Filters;

namespace PlantillaMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Subir(HttpPostedFileBase file)
        {
            if (file == null)
                return PartialView("Respuesta", "Error de carga!");

            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                file.SaveAs(path);
                return PartialView("Respuesta", "Se Guardó Correctamente");
            }
            catch (Exception ex) {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        

        [HttpPost]
        [Autentificado]
        public ActionResult Regresar()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            return RedirectToAction("Principal", "Devoluciones");
        }


    }
}