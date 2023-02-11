using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Modelo;
using CRUD;
using System.Reflection;
using General;
using CRUD.Transaction;
using CapaLogica.Funciones;
using PlantillaMVC.Filters;
using System.Web.Security;


namespace PlantillaMVC.Controllers
{
    public class CorreosController : Controller
    {
        // GET: Correos
        public ActionResult Index()
        {
            return View();
        }

        [Autentificado]
        public ActionResult Correo()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a correo", "Correo/Correo ");

            List<ECorreos> LiCorreos = Utilidades.negocio.RecuperaLiCorreos();
            return PartialView(LiCorreos);
        }

        [Autentificado]
        public ActionResult EdicionCorreo(int Id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a correo", "Correo/EdicionCorreo ");

            ECorreos Modelo = new ECorreos();
            if (Id > 0)
            {
                Modelo = Utilidades.negocio.RecuperaUnCorreo(Id);

                if (Id == 18)
                {
                    ViewBag.Nodos = "<li id=\"node1\">Nombre</li>  <li id = \"node2\">Puesto</li>  <li id = \"node3\">Area</li>  <li id = \"node4\">Jefe</li>   <li id = \"node5\">Liga</li> <li id = \"node6\">Motivo</li> <li id = \"node7\">Calif</li> ";
                }
                else if (Id == 3)
                {
                    ViewBag.Nodos = "<li id=\"node1\">Nombre</li> <li id = \"node2\">Puesto</li> <li id = \"node3\">Area</li>  <li id = \"node4\">Jefe</li>   <li id = \"node5\">Liga</li> <li id = \"node6\">Motivo</li> ";
                }
                else
                {
                    ViewBag.Nodos = "<li id=\"node1\">Nombre</li> <li id = \"node2\">Puesto</li> <li id = \"node3\">Area</li>  <li id = \"node4\">Jefe</li>   <li id = \"node5\">Liga</li> ";
                } 
            }
            else
            {
                return PartialView("Respuesta", "ocurrio un error");
            }
            return PartialView(Modelo);
        }


        [HttpPost]
        public ActionResult SaveEmail(Modelo.ECorreos aux)
        {
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
       
            if (Utilidades.negocio.GuardaCorreo(aux))
            {

                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto  + " Guardo cambios en el correo " + aux.id.ToString() , "Correo/EdicionCorreo ");
                return PartialView("Respuesta", "Se guardó correctamente ");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ocurrió un error al intentar guardar " + aux.id.ToString(), "Correo/EdicionCorreo ");
                return PartialView("Respuesta", "ocurrio un error");
            }
            //return PartialView("Respuesta", "ocurrio un error");
        }




    }
}