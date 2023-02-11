using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantillaMVC.Filters;
using Modelo;


namespace PlantillaMVC.Controllers
{
    public class EvaluacionController : Controller
    {
        // GET: Evaluacion
        public ActionResult Index()
        {
            return View();
        }

        [Autentificado]
        public ActionResult Principal()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a Principal", "Evaluacion/Principal ");

            ViewBag.Paginas = "paginas";

            ViewBag.Rol = sesion.Login.perfil;//aqui enviamos el rol del usuario  ojo
            EPeriodos periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);
            if (periodo != null)
            {
                if (periodo.Etapa == 0)
                {
                    ViewBag.Titulo = "Carga de objetivos";
                }
                else if (periodo.Etapa == 1)
                {
                    ViewBag.Titulo = "Evaluación de medio año";
                }
                else if (periodo.Etapa == 2)
                {
                    ViewBag.Titulo = "Calibración primer semestre";
                }
                else if (periodo.Etapa == 3)
                {
                    ViewBag.Titulo = "Evaluación fin de año";
                }
                else if (periodo.Etapa == 4)
                {
                    ViewBag.Titulo = "Calibración fin de año";
                }
            }
            else {
                ViewBag.Titulo = "--";
            }
            ViewBag.NomUsr = "Bienvenido: " + sesion.Login.NombreCompleto;
            ViewBag.UserBono = sesion.Login.bonoAnual;
            return View();
        }
    }
}