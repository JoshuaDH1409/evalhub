using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using General;
using CRUD.Transaction;
using CapaLogica.Funciones;
using PlantillaMVC.Filters;
using PlantillaMVC.Models;
using Modelo;
using CRUD;



namespace PlantillaMVC.Controllers
{
    public class HistorialUsrsController : Controller
    {
        // GET: HistorialUsrs
        public ActionResult Index()
        {
            return View();
        }

        [Autentificado]
        public ActionResult HistorialUsrs()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            PlantillaMVC.Models.EPeriodosDash item = new EPeriodosDash();

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a Historial Usuarios", "HistorialUsrs/HistorialUsrs ");
            
            List<SelectListItem> ListAreas = DropPais(sesion.Login);
            ViewData["ListaPais"] = ListAreas;

            return PartialView(item);  //ver carga de datos  
        }

        [HttpGet]
        public PartialViewResult ValidaPeriodo(int id)
        {
            PlantillaMVC.Models.EPeriodosDash item = new EPeriodosDash();
            List<SelectListItem> ListaPeriodos = DropPeriodos(id);
            ViewData["ListaPeriodos"] = ListaPeriodos;
            return PartialView(item);
        }

        public PartialViewResult ConsultarUsuarios(EPeriodosDash item)
        {
            List<Modelo.Clases.CSessionEval> Temp = Utilidades.negocio.RecuperaEvaluacionesPeriodoUsuarios(item.Periodo);
            return PartialView(Temp);
        }

        
        #region Drops
        
        public List<SelectListItem> DropPais(ELogin Usr)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPais> ListaRoles = Utilidades.negocio.RecuperaPaises().Where(t=>t.Activo).ToList();

            foreach (EPais item in ListaRoles)
            {
                //if(item.Activo==1)
                if (Usr.perfil == 1)
                    Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
                else if(Usr.perfil == 2 && Usr.paisesRegion.Split(',').ToList().Contains(item.id.ToString()))
                    Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
                else
                {
                    if(item.id == Usr.Pais)
                        Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
                }
            }
            return Respuesta;
        }

        public List<SelectListItem> DropPeriodos(int pais)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPeriodos> ListaPeriodos = Utilidades.negocio.RecuperaPeriodos();

            ListaPeriodos = (from lista in ListaPeriodos
                             where lista.Country == pais
                             select lista).ToList();

            foreach (EPeriodos item in ListaPeriodos)
            {
                Respuesta.Add(new SelectListItem { Text = item.Llave, Value = item.id.ToString() });
            }
            return Respuesta;
        }

        #endregion



    }
}