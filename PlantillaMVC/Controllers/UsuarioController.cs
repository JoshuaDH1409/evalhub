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
using System.IO;
using Newtonsoft.Json;
using Modelo.Clases;

namespace PlantillaMVC.Controllers
{
    public class UsuarioController : Controller
    {
     
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }


        [Autentificado]
        public ActionResult Usuarios()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            CSession sesion = (CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " ingreso a usuarios", "Usuario/Usuarios ");
            List<CSession> ListaUsuarios = Utilidades.negocio.RecuperaUsuarios(false);

            if (ListaUsuarios != null)
            {
                if (sesion.Login.perfil == (int)EnumPerfil.Local)
                {
                    ListaUsuarios = (from liuser in ListaUsuarios
                                     where liuser.Login.Pais == sesion.Login.Pais
                                     select liuser).ToList();
                }
                else if(sesion.Login.perfil == (int)EnumPerfil.Regional)
                {
                    ListaUsuarios = (from liuser in ListaUsuarios
                                     where sesion.Login.paisesRegion.Split(',').ToList().Contains( liuser.Login.Pais.ToString())
                                     select liuser).ToList();
                }
                ViewBag.rol = sesion.Login.perfil;
            }

            return PartialView(ListaUsuarios);
        }

        [Autentificado]
        public ActionResult NuevoUsuario(int id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            CSession sesion = (CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " ingreso a nuevo usuario", "Usuario/NuevoUsuario ");

            ELogin modelo = new ELogin();
            ViewData["ListaPerfiles"]= DropPerfil(sesion.Login.perfil);
            ViewData["ListaDivisiones"] = DropDivisiones();
            ViewData["ListaPaises"] = DropPais(sesion.Login.perfil);
            ViewData["Evaluadores"] = LiEvaluadores(sesion.Login.Pais);

            if (id > 0)
            {
                modelo = Utilidades.negocio.RecuperaUnUsuario(id);
                ELogin temporal = Utilidades.negocio.RecuperaUnUsuarioSap(modelo.EvaluadorIdSap);
                if (temporal != null) 
                modelo.EvaluadorNombre = temporal.id_sap + "|" + temporal.NombreCompleto;
                
                modelo.NoAplica = String.IsNullOrEmpty(modelo.ApellidoMat) ? true : false;
                if(modelo.NoAplica)
                    modelo.ApellidoMat = "NA";
            }
            return PartialView( modelo);
        }

        [Autentificado]
        public ActionResult CambiarCon()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Models.CambioContraseña model = new Models.CambioContraseña();
            return PartialView(model);
        }

        [Autentificado]
        [HttpPost]
        public ActionResult GuardarCambioCon(Models.CambioContraseña model)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            if (!ModelState.IsValid)
            {
             
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            }
            else {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                sesion.Login.Password = Encrypt.Cifrado(model.ConContrasena);
                if (Utilidades.negocio.GuardaUsuario(sesion.Login))
                {                   
                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                    MandarCorreo.SendMail("Soporte", sesion.Login.Email, " ", "Estimado:" + sesion.Login.NombreCompleto + "<br><br><br>" + "Se cambió la contraseña correctamente <b>" + "</b> <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", "Cambio de contraseña");

                    return PartialView("Respuesta", "Se guardó correctamente");
                }
                else {
                    return PartialView("Respuesta", "Ocurrió un error al ingresar el dato");                    
                }
            }
        }

        public JsonResult ValidaUsuario(string Correo, string id )
        {
            try
            {
             
                int auxiliar = 0;

                List<Modelo.Clases.CSession> ListaUsuarios = Utilidades.negocio.RecuperaUsuarios();

                List<Modelo.Clases.CSession> AuxUsuarios = (from Usuarios in ListaUsuarios
                                           where Usuarios.Login.Email == Correo
                                           select Usuarios).ToList();

                if (AuxUsuarios.Count > 0) {
                    auxiliar = 1;
                    if (Convert.ToInt32(id) > 0)//verificamos que el usuario no sea el mismo 
                    {
                        ELogin modeloAux = Utilidades.negocio.RecuperaUnUsuario(Convert.ToInt32(id));
                        if (AuxUsuarios[0].Login.id == modeloAux.id)
                        {
                            auxiliar = 0;
                        }
                    }            
                 
                }

                List<object> data = new List<object>();
               
                    data.Add(new
                    {
                        Aux = auxiliar
                    });
          

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                //Utilidades.negocio.GuardaLogView(GeneraException.RecuperaErrores(tmp));
                return Json(new { error = "OK" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ValidaUsuarioSap(string idSap, string id)
        {
            try
            {

                int auxiliar = 0;

                List<Modelo.Clases.CSession> ListaUsuarios = Utilidades.negocio.RecuperaUsuarios();

                List<Modelo.Clases.CSession> AuxUsuarios = (from Usuarios in ListaUsuarios
                                                            where Usuarios.Login.id_sap == idSap
                                                            select Usuarios).ToList();

                if (AuxUsuarios.Count > 0)
                {
                    auxiliar = 1;
                    if (Convert.ToInt32(id) > 0)//verificamos que el usuario no sea el mismo 
                    {
                        ELogin modeloAux = Utilidades.negocio.RecuperaUnUsuario(Convert.ToInt32(id));
                        if (AuxUsuarios[0].Login.id == modeloAux.id)
                        {
                            auxiliar = 0;
                        }
                    }

                }

                List<object> data = new List<object>();

                data.Add(new
                {
                    Aux = auxiliar
                });


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                //Utilidades.negocio.GuardaLogView(GeneraException.RecuperaErrores(tmp));
                return Json(new { error = "OK" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getDivision(string term)
       {
            try { 
            var contexto = Utilidades.negocio.RecuperaDivisiones(term);
                return Json(contexto, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                //Utilidades.negocio.GuardaLogView(GeneraException.RecuperaErrores(tmp));
                return Json( new List<string> { "OK" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Autentificado]
        public ActionResult CargaMasivaUsuarios(string resp)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");

            return PartialView();
        }

        [HttpPost]
        [Autentificado]
        public ActionResult Subir(HttpPostedFileBase file)
        {
            if (file == null)
                return PartialView("RespuestaCarga", "error de carga");

            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                file.SaveAs(path);
                Utilidades.negocio.ProcesaUsuarios(path, fileName.ToString());
                return PartialView("RespuestaCarga", "se guardó correctamente");
            }
            catch (Exception ex)
            {
                return PartialView("RespuestaCarga", "error de carga");
                //throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                //                           new string[] { string.Format("Error: {0}", ex.Message),
                //                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public ActionResult TakeControl(int id)
        {
            ELogin modelo = new ELogin();
            modelo = Utilidades.negocio.RecuperaUnUsuario(id);

            Modelo.Clases.CSession sesion = Utilidades.negocio.obtenSession(modelo.Email, modelo.Password);
            Session["PreviousSession"] = Session[Utilidades.session];
            Session[Utilidades.session] = sesion;
            FormsAuthentication.SetAuthCookie(modelo.Email, false);

            return RedirectToAction("Principal", "Evaluacion");
        }

        public ActionResult GetBackControl()
        {
            //ELogin modelo = new ELogin();
            //modelo = Utilidades.negocio.RecuperaUnUsuario(id);

            if (Session[Utilidades.session] == null)
            {
                Utilidades.CierraSession(Session);
                return RedirectToAction("Account", "Login");
            }

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session["PreviousSession"];
            //Session["PreviousSession"] = Session[Utilidades.session];
            Session[Utilidades.session] = sesion;
            Session["PreviousSession"] = null;

            FormsAuthentication.SetAuthCookie(sesion.Login.Email, false);

            return RedirectToAction("Principal", "Evaluacion");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["PreviousSession"] = null;
            Utilidades.CierraSession(Session);
            TempData.Clear();
            //Modelo.Clases.CSession sesionPru = (Modelo.Clases.CSession)Session[Utilidades.session];
            //return RedirectToAction("Login", "Account");
            return RedirectToAction("Login", "Account");
        }

        public FileResult Download()
        {
            string path = Path.Combine(Server.MapPath("~/PlantillasCargaMasiva/"), "Layout_cargaEmployees.xlsx");
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = "Plantilla.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        
        #region utilidades
        private static long FechaTick(string fecha)
        {
            DateTime temp = DateTime.Parse(fecha);
            long respuesta = temp.Ticks;
            return respuesta;
        }

        public static long DateToTicks(DateTime dtInput)
        {
            long ticks = 0;
            ticks = dtInput.Ticks;
            return ticks;
        }

        //public static string TiksToDate(long fecha)
        //{
        //    DateTime temp = new DateTime(fecha);
        //    string auxday = "", auxMonth = "";

        //    if (temp.Day.ToString().Length < 2)
        //    {
        //        auxday = "0" + temp.Day.ToString();
        //    }
        //    else
        //    {
        //        auxday = temp.Day.ToString();
        //    }

        //    if (temp.Month.ToString().Length < 2)
        //    {
        //        auxMonth = "0" + temp.Month.ToString();
        //    }
        //    else
        //    {
        //        auxMonth = temp.Month.ToString();
        //    }

        //    string res = auxday + "/" + auxMonth + "/" + temp.Year.ToString();
        //    return res;
        //}

        #endregion
        

        #region DropUsuarios
        public List<SelectListItem> DropPerfil(int perfil)
        {
            List<SelectListItem> Respuesta  = new List<SelectListItem>();
            List<EPerfil> ListaRoles = Utilidades.negocio.RecuperaPerfiles();
            if (perfil != 1)
                ListaRoles.Remove(ListaRoles.Where(t => t.id == 1).SingleOrDefault());

            foreach (EPerfil item in ListaRoles)
            {
                Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
            }
            return Respuesta;
        }
        public List<SelectListItem> DropPais(int perfil)
        {
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPais> ListaPaises = Utilidades.negocio.RecuperaPaises();
            if(perfil!=2 && perfil!=1)
            {
                ListaPaises = ListaPaises.Where(t => t.id == sesion.Login.Pais).ToList();
            }
            else if(perfil==2)
            {
                ListaPaises = ListaPaises.Where(t => sesion.Login.paisesRegion.Split(',').ToList().Contains(t.id.ToString())).ToList();
            }

            foreach (EPais item in ListaPaises)
            {
                //if(item.Activo==1)
                Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });

            }
            return Respuesta;
        }
        public List<SelectListItem> DropDivisiones()
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EDivision> ListaDivisiones = Utilidades.negocio.RecuperaDivisiones("");

            foreach (EDivision item in ListaDivisiones)
            {
                Respuesta.Add(new SelectListItem { Text = item.Descripcion, Value = item.id.ToString() });

            }
            return Respuesta;
        }
        public List<string> LiEvaluadores(int pais)
        {
            List<string> Respuesta = new List<string>();
            List<ELogin> ListaEvaluadores = Utilidades.negocio.RecuperaLogsIn().Where(t=>t.Activo).ToList();


            foreach (ELogin item in ListaEvaluadores)
            {
                //item.NombreCompleto = item.Nombre + " " + item.ApellidoPat + " " + item.ApellidoMat;
                Respuesta.Add(item.id_sap+"|"+item.NombreCompleto);
            }
            return Respuesta;
        }


        #endregion

        [Autentificado]
        public ActionResult EliminarUsuario(int Id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " Elimino el usuario" + Id.ToString() , "Usuario/Usuarios ");

            ELogin modelo = Utilidades.negocio.RecuperaUnUsuario(Id);
            modelo.Activo = false;
            modelo.Modificadopor = sesion.Login.NombreCompleto;
            Utilidades.negocio.GuardaUsuario(modelo);

            return PartialView("RespEliminar", "Se eliminó correctamente");
        }

        [Autentificado]
        public ActionResult ActivarUsuario(int Id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El Usuario " + sesion.Login.NombreCompleto + " Activó el usuario" + Id.ToString(), "Usuario/Usuarios ");

            ELogin usuario = Utilidades.negocio.RecuperaUnUsuario(Id);
            if (usuario == null)
            {
                return PartialView("RespEliminar", "No se encontró el usuario");
            }

            usuario.Activo = true;
            usuario.Modificadopor = sesion.Login.NombreCompleto;
            if (!Utilidades.negocio.GuardaUsuario(usuario))
            {
                return PartialView("RespEliminar", "No se pudo activar el usuario");
            }

            string mensaje = "Se activó correctamente";

            EPeriodos per = Utilidades.negocio.RecuperaPeriodopais(usuario.Pais);
            if (per != null && per.Activo)
            {
                EEval evalActiva = Utilidades.negocio.RecuperaEvaluacionActivaUsario(usuario.id);
                if (evalActiva == null || evalActiva.periodo != per.id)
                {
                    EEval evalPeriodo = null;
                    List<Modelo.Clases.CHistorial> historial = Utilidades.negocio.RecuperaHistorialUsr(usuario.id);
                    if (historial != null)
                    {
                        evalPeriodo = historial.Select(t => t.Evaluacion).FirstOrDefault(t => t.periodo == per.id);
                    }

                    if (evalPeriodo == null)
                    {
                        evalPeriodo = new Modelo.EEval
                        {
                            id = 0,
                            periodo = per.id,
                            Status = 0,
                            Activo = true,
                            Modificadopor = sesion.Login.NombreCompleto,
                            id_usuario = usuario.id,
                            Puesto = usuario.Division,
                            Nivel = usuario.perfil,
                            Division = usuario.Division,
                            Area = "",
                            id_evaluador = usuario.EvaluadorIdSap
                        };
                    }
                    else
                    {
                        evalPeriodo.Activo = true;
                        evalPeriodo.Status = 0;
                        evalPeriodo.Modificadopor = sesion.Login.NombreCompleto;
                    }

                    Utilidades.negocio.GuardaEvaluacion(evalPeriodo);
                }
            }
            else
            {
                mensaje = "Usuario activado. No hay periodo activo para asignar evaluación";
            }

            return PartialView("RespEliminar", mensaje);
        }

        [Autentificado]
        public ActionResult GuardaUsuario(ELogin modelo, FormCollection form)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");
            CSession sesion = (CSession)Session[Utilidades.session];
            modelo.Activo = true;
            modelo.Modificadopor = sesion.Login.NombreCompleto;
            if (modelo.NoAplica)
                modelo.ApellidoMat = "";
            if (modelo.EvaluadorNombre.Contains("|")) {
                modelo.EvaluadorIdSap = modelo.EvaluadorNombre.Substring(0,modelo.EvaluadorNombre.IndexOf("|"));
                modelo.FechaIngreso = FechaTick(form["FechaIngresoTemp"]);
                modelo.FechaAntiguedad = modelo.FechaIngreso;
                if (modelo.id == 0) {
                    modelo.Password = "Aspen2017";
                }
                
                if (Utilidades.negocio.GuardaUsuario(modelo))
                {
                    EEval eval = Utilidades.negocio.RecuperaEvaluacionActivaUsario(modelo.id);

                    //caso 1: La evaluación se cambiara si hubo cambio de país y el periodo esta activo y la evaluacion no es nulla
                    //caso 2: La evaluación se agregará si es null y el periodo esta activo
                    if (form["changecountry"] == "1" || eval == null)
                    {
                        //Verifica si el periodo esta activo
                        EPeriodos per = Utilidades.negocio.RecuperaPeriodopais(modelo.Pais);
                        if(per != null)
                        {
                            //verifica si es el caso 1
                            if (form["changecountry"] == "1" && eval != null)
                            {
                               
                                if (per.id != eval.periodo)
                                {
                                    eval.periodo = per.id;
                                    bool saved = Utilidades.negocio.GuardaEvaluacion(eval);
                                }
                            }

                            //sino es el caso 1 por ende es el caso 2
                            else
                            {
                                //INICIALIZANDO NUESTRO EEVAL PARA INSERTAR A LOS USUARIOS
                                Modelo.EEval evalAux = new Modelo.EEval();
                                evalAux.id = 0; //INDICA QUE SERA UNA NUEVA EVALUACIÓN
                                evalAux.periodo = per.id;
                                evalAux.Status = 0;
                                evalAux.Activo = true;
                                evalAux.Modificadopor = sesion.Login.NombreCompleto;

                                evalAux.id_usuario = modelo.id;
                                evalAux.Puesto = modelo.Division;
                                evalAux.Nivel = modelo.perfil;
                                evalAux.Division = modelo.Division;
                                evalAux.Area = "";
                                evalAux.id_evaluador = modelo.EvaluadorIdSap;

                                //SE REALIZA LA INSERSIÓN DE LA EVALUACIÓN
                                Utilidades.negocio.GuardaEvaluacion(evalAux);
                            }
                        }


                    }
        


                    /*if (form["changecountry"] == "1")
                    {
                        EEval eval = Utilidades.negocio.RecuperaEvaluacionActivaUsario(modelo.id);
                        if (eval != null)
                        {
                            EPeriodos per = Utilidades.negocio.RecuperaPeriodopais(modelo.Pais);
                            if (per != null)
                            {
                                if(per.id != eval.periodo)
                                {
                                    eval.periodo = per.id;
                                    bool saved = Utilidades.negocio.GuardaEvaluacion(eval);
                                }
                            }
                        }
                        
                    }*/

                    if (modelo.Notificar) {
                        ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(40);
                        PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                        correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, modelo);
                        MandarCorreo.SendMail("Soporte", modelo.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    }
                    return PartialView("Respuesta", "Se Guardó Correctamente");
                }
                else
                    return PartialView("Respuesta", "Ocurrió un error");
            }
            else {
                return PartialView("Respuesta", "el formato del evaluador no es correcto");
            } 
        }
    }
}