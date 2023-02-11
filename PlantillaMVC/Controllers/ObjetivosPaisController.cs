using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Modelo;
using Modelo.Clases;
using PlantillaMVC.Filters;
using PlantillaMVC.Models;

namespace PlantillaMVC.Controllers
{
    public class ObjetivosPaisController : Controller
    {
        CSession sesion = new CSession();
        // GET: ObjetivosPais
        public ActionResult Index()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            sesion = (CSession)Session[Utilidades.session];         
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " ingreso a ObjetivosPais", "Usuario/Usuarios ");
            List<EObjetivoPais> objetivos = new List<EObjetivoPais>();
            EPeriodos periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);
            //if(sesion.Login.PerfilId==(int)EnumPerfil.Sistemas)//Acceso a toda la información, los objetivos de todos los paises
            //{
            //    objetivos = entities.TBL_Objetivo_Pais.Where(t=>t.TBL_Periodo.Activo==true).OrderBy(t=>t.Pais_Id).ToList();
            //}
            //else if(sesion.Login.PerfilId == (int)EnumPerfil.Regional)
            //{
            //    if(!String.IsNullOrEmpty(sesion.Login.paisesRegion))
            //    {
            //        foreach(var p in sesion.Login.paisesRegion.Split(','))
            //        {
            //            var obj=entities.TBL_Objetivo_Pais.Where(t => t.Pais_Id == Convert.ToInt32(p) && t.TBL_Periodo.Activo == true).ToList();
            //            objetivos.AddRange(obj);
            //        }
            //    }
            //    else
            //    {
            //        objetivos = entities.TBL_Objetivo_Pais.Where(t => t.Pais_Id == (int)sesion.Login.PaisId && t.TBL_Periodo.Activo==true).ToList();
            //    }
            //}
            if (periodo != null)
            {
                sesion.PeriodoActual = periodo;
                ViewBag.Pais = sesion.Pais.descripcion;
                if (sesion.Login.perfil == (int)EnumPerfil.Sistemas || sesion.Login.perfil == (int)EnumPerfil.Local || sesion.Login.perfil == (int)EnumPerfil.Regional)
                {
                    objetivos = Utilidades.negocio.RecuperaObjetivosPais(sesion.Login.Pais, sesion.PeriodoActual.id);
                    if (objetivos.Count>0)
                        if(objetivos.First().Objetivo_Cerrado)
                        ViewBag.Cerrados = true;
                }
                ViewBag.PesoTotal = objetivos.Sum(t => t.Objetivo_Peso);
                ViewBag.EtapaPeriodo = periodo.Etapa;
                return PartialView(objetivos);
            }
            else
            {
                return PartialView("Respuesta", "No hay periodos abiertos para su país");
            }
        }

        [Autentificado]
        // GET: ObjetivosPais/Create
        public ActionResult Nuevo(int id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            CSession sesion = (CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " ingreso a nuevo usuario", "Usuario/NuevoUsuario ");
            int pesototal = Utilidades.negocio.RecuperaObjetivosPais(sesion.Login.Pais, sesion.PeriodoActual.id).Sum(t => t.Objetivo_Peso);
            EObjetivoPais modelo = new EObjetivoPais();
            if (id == 0)
            {
                modelo.Pais_Id = sesion.Login.Pais;
                modelo.Periodo_Id = sesion.PeriodoActual.id;
            }
            else
            {
                modelo = Utilidades.negocio.RecuperaObjetivoPais(id);
                pesototal -= modelo.Objetivo_Peso;
            }
            ViewBag.PesoTotal = pesototal;
            return PartialView(modelo);
        }

        // POST: ObjetivosPais/Create
        [HttpPost]
        [Autentificado]
        public ActionResult Guardar(EObjetivoPais modelo, FormCollection form)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");
            CSession sesion = (CSession)Session[Utilidades.session];
            try
            {
                Utilidades.negocio.GuardaObjetivoPais(modelo);
                TempData["Mensaje"] = "El objetivo ha sido guardado correctamente";
                //if(modelo.ID==0)
                //{
                //    entities.TBL_Objetivo_Pais.Add(modelo);
                //    entities.SaveChanges();
                //}
                //else
                //{
                //    var objetivo = entities.TBL_Objetivo_Pais.Find(modelo.ID);
                //    objetivo.Objetivo_Cerrado = false;
                //    objetivo.Objetivo_Cumplimiento = modelo.Objetivo_Cumplimiento;
                //    objetivo.Objetivo_Dsc = modelo.Objetivo_Dsc;
                //    objetivo.Objetivo_Peso = modelo.Objetivo_Peso;
                //    entities.SaveChanges();
                //}
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                    return PartialView("Respuesta", "Ocurrió un error");
            }
        }

        // GET: ObjetivosPais/Delete/5
        public ActionResult Eliminar(int id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");

            CSession sesion = (CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " Elimino el usuario" + id.ToString(), "Usuario/Usuarios ");

            Utilidades.negocio.EliminarObjetivoPais(id);
            TempData["Mensaje"] = "El objetivo ha sido eliminado correctamente";
            return RedirectToAction("Index");
        }
        public ActionResult Cerrar()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            CSession sesion = (CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " ingreso a nuevo usuario", "Usuario/NuevoUsuario ");
            List<EObjetivoPais> modelo = Utilidades.negocio.RecuperaObjetivosPais(sesion.Login.Pais, sesion.PeriodoActual.id);
            foreach(var obj in modelo)
            {
                obj.Objetivo_Cerrado = true;
                Utilidades.negocio.GuardaObjetivoPais(obj);
            }
            TempData["Mensaje"] = "Los objetivos para el periodo actual se han cerrado.";
            return RedirectToAction("Index");
        }
    }
}
