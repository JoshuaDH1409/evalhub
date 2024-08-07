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
using System.Data;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using CapaLogica.Log;

namespace PlantillaMVC.Controllers
{
    public class PeriodosController : Controller
    {
        // GET: Periodos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Periodos()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: "+ sesion.Login.NombreCompleto + " Ingreso a periodos", "Periodos/Periodos ");

            List<EPeriodos> listaPeriodos = Utilidades.negocio.RecuperaPeriodos().Where(t=>t.Etapa!=5 && t.Activo).ToList();
            if (listaPeriodos != null)
            {
                if (sesion.Login.perfil != 1)
                {
                    if (sesion.Login.perfil == 2)
                    {
                        var paises = sesion.Login.paisesRegion.Split(',').ToList();
                        listaPeriodos = (from liperiodo in listaPeriodos
                                         where paises.Contains(liperiodo.Country.ToString())
                                         select liperiodo).ToList();
                    }
                    else
                        listaPeriodos = (from liperiodo in listaPeriodos
                                         where liperiodo.Country == sesion.Login.Pais
                                         select liperiodo).ToList();
                }
                foreach (EPeriodos item in listaPeriodos)
                {
                    EPais temp = Utilidades.negocio.RecuperaUnPais(item.Country);
                    if (item.Country == 9 || item.Country == 1)
                        item.Grupo = "México";
                    //else
                      //  item.Grupo = "Latam";
                    item.PaisTempral = temp.descripcion;
                    item.EtapaDsc = Utilidades.negocio.RecuperaEtapa(item.Etapa).EtapaDsc;
                    int total = 0;
                    List <Modelo.Clases.CSessionEval> listaAux = null;
                    item.StartDateObjTemp = TiksToDate(item.StartDateObj);
                    item.FinishDateObjTemp = TiksToDate(item.FinishDateObj);
                    List<Modelo.Clases.CSessionEval> temporal = Utilidades.negocio.RecuperaLiUsuariosPais(item.Country);
                    double aux = 0;

                    if (temporal != null)
                    {
                        listaAux = (from CSessionEval in temporal
                                    where CSessionEval.Evaluacion != null && CSessionEval.Login.Activo==true
                                    select CSessionEval).ToList();
                        total = listaAux.Count;
                    }
                    switch (item.Etapa)
                    {
                        case 0:
                            if (listaAux != null)
                            {
                                int t1 = (from CSessionEval in listaAux
                                          where CSessionEval.Evaluacion.Status == 3
                                          select CSessionEval).ToList().Count;
                                
                                if (total != 0) {
                                    aux = Convert.ToDouble( t1) / Convert.ToDouble( total);
                                    aux = Math.Truncate((aux * 100));
                                }                                
                                item.porcentaje = aux.ToString();
                            }
                            else
                            {
                                item.porcentaje = "0";
                            }                            
                            break;
                        case 1:
                            if (temporal != null)
                            {
                                int t1 = (from CSessionEval in listaAux
                                          where CSessionEval.Evaluacion.Status == 8
                                          select CSessionEval).ToList().Count;
                               
                                if (total != 0)
                                {
                                    aux = Convert.ToDouble(t1) / Convert.ToDouble(total);
                                    aux = Math.Truncate((aux * 100));
                                }
                                item.porcentaje = aux.ToString();
                            }
                            else
                            {
                                item.porcentaje = "0";
                            }
                            break;
                        case 2:
                            if (temporal != null)
                            {
                                int t1 = (from CSessionEval in listaAux
                                          where CSessionEval.Evaluacion.Status == 9
                                          select CSessionEval).ToList().Count;
                             
                                if (total != 0)
                                {
                                    aux = Convert.ToDouble(t1) / Convert.ToDouble(total);
                                    aux = Math.Truncate((aux * 100));
                                }
                                item.porcentaje = aux.ToString();
                            }
                            else
                            {
                                item.porcentaje = "0";
                            }
                            break;
                        case 3:
                            if (temporal != null)
                            {
                                int t1 = (from CSessionEval in listaAux
                                          where CSessionEval.Evaluacion.Status == 14
                                          select CSessionEval).ToList().Count;
                            
                                if (total != 0)
                                {
                                    aux = Convert.ToDouble(t1) / Convert.ToDouble(total);
                                    aux = Math.Truncate((aux * 100));
                                }
                                item.porcentaje = aux.ToString();
                            }
                            else
                            {
                                item.porcentaje = "0";
                            }
                            break;
                        case 4:
                            if (temporal != null)
                            {                              
                                int t1 = (from CSessionEval in listaAux
                                          where CSessionEval.Evaluacion.Status == 15
                                          select CSessionEval).ToList().Count;
                              
                                if (total != 0)
                                {
                                    aux = Convert.ToDouble(t1) / Convert.ToDouble(total);
                                    aux = Math.Truncate((aux * 100));
                                }
                                item.porcentaje = aux.ToString();
                            }
                            else
                            {
                                item.porcentaje = "0";
                            }
                            break;
                    }
                }
            }
            return PartialView(listaPeriodos);
        }

    
        //se tiene que hacer el detalle
        public PartialViewResult EjecutarRecordatorio(int Id)
        {
            Utilidades.negocio.EjecutarRecordatorios(Id);
            return PartialView("Respuesta", "Se notificó correctamente a los usuarios del periodo");
        }

        [Autentificado]
        public ActionResult NuevoPeriodo(int id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");           
            
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a nuevo periodo", "Periodos/NuevoPeriodo ");

            EPeriodos modelo = new EPeriodos();
            modelo.EtapaDsc = Utilidades.negocio.RecuperaEtapa(modelo.Etapa).EtapaDsc;
            if (id > 0)
            {
                modelo = Utilidades.negocio.RecuperaUnPeriodo(id);
                //objetivos
                modelo.StartDateObjTemp = TiksToDate(modelo.StartDateObj);
                modelo.FinishDateObjTemp = TiksToDate(modelo.FinishDateObj);
                //objetivos

                //evaluacion 1er periodo
                modelo.StartDateEvaTemp = TiksToDate(modelo.StartDateEva);
                modelo.FinishDateEvaTemp = TiksToDate(modelo.FinishDateEva);
                //evaluacion 1er periodo

                //Calibracion 1er periodo
                modelo.StartDateCaliTemp = TiksToDate(modelo.StartDateCali);
                modelo.FinishDateCaliTemp = TiksToDate(modelo.FinishDateCali);
                //Calibracion 1er periodo

                //Calibracion 1er periodo
                modelo.StartDateCaliTemp = TiksToDate(modelo.StartDateCali);
                modelo.FinishDateCaliTemp = TiksToDate(modelo.FinishDateCali);
                //Calibracion 1er periodo

                //Calibracion 2do periodo   StartDateEva2
                modelo.StartDateEva2Temp = TiksToDate(modelo.StartDateEva2);
                modelo.FinishDateEva2Temp = TiksToDate(modelo.FinishDateEva2);
                //Calibracion 1er periodo

                //Calibracion 1er periodo
                modelo.StartDateCali2Temp = TiksToDate(modelo.StartDateCali2);
                modelo.FinishDateCali2Temp = TiksToDate(modelo.FinishDateCali2);
                //Calibracion 1er periodo

            }
            List<SelectListItem> listaDePaises = DropPais();
            if (sesion.Login.perfil != 1 &&sesion.Login.perfil != 2)
                listaDePaises = listaDePaises.Where(t => t.Value == sesion.Login.Pais.ToString()).ToList();
            else if(sesion.Login.perfil == 2)
                listaDePaises = listaDePaises.Where(t => sesion.Login.paisesRegion.Split(',').ToList().Contains(t.Value)).ToList();
            ViewData["ListaPaises"] = listaDePaises;

            return PartialView(modelo);
        }
        [HttpPost]
        [Autentificado]
        public ActionResult GuardaPeriodo(EPeriodos Periodo)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingresó a nuevo periodo", "Periodos/GuardaPeriodo ");

            List<string> errores = new List<string>();

            errores.Add("\n\n entra controlador \n");
            Utilidades.negocio.GuardaLogVista(errores);
            errores.Clear();
           
            if (Periodo.id > 0) {
                EPeriodos aux = Utilidades.negocio.RecuperaUnPeriodo(Periodo.id);
                Periodo.Country = aux.Country;
                Periodo.Etapa = aux.Etapa;
                Periodo.PaisTempral = Utilidades.negocio.RecuperaUnPais(Periodo.Country).descripcion;
            }
            Periodo.Modificadopor = sesion.Login.NombreCompleto;
            //objetivos
            Periodo.StartDateObj = FechaTick(Periodo.StartDateObjTemp);
            Periodo.FinishDateObj = FechaTick(Periodo.FinishDateObjTemp);
            //objetivos

            //evaluacion 1er periodo
            Periodo.StartDateEva = FechaTick(Periodo.StartDateEvaTemp);
            Periodo.FinishDateEva = FechaTick(Periodo.FinishDateEvaTemp);
            //evaluacion 1er periodo

            //Calibracion 1er periodo
            Periodo.StartDateCali  = FechaTick(Periodo.StartDateCaliTemp);
            Periodo.FinishDateCali = FechaTick(Periodo.FinishDateCaliTemp);
            //Calibracion 1er periodo

            //Calibracion 1er periodo
            Periodo.StartDateCali = FechaTick(Periodo.StartDateCaliTemp);
            Periodo.FinishDateCali = FechaTick(Periodo.FinishDateCaliTemp);
            //Calibracion 1er periodo


            //Calibracion 2do periodo
            Periodo.StartDateEva2 = FechaTick(Periodo.StartDateEva2Temp);
            Periodo.FinishDateEva2 = FechaTick(Periodo.FinishDateEva2Temp);
            //Calibracion 2do periodo

            //Calibracion 2do periodo
            Periodo.StartDateCali2 = FechaTick(Periodo.StartDateCali2Temp);
            Periodo.FinishDateCali2 = FechaTick(Periodo.FinishDateCali2Temp);
            //Calibracion 2do periodo

            errores.Add("\n\n Conversion de fechas \n");
            Utilidades.negocio.GuardaLogVista(errores);
            errores.Clear();

            Periodo.Activo = true;

            if (Periodo.id > 0)
            {
                if (Utilidades.negocio.GuardaPeriodo(Periodo))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardó  el nuevo periodo" + Periodo.Llave, "Periodos/GuardaPeriodo ");
                    TempData["Message"]= "Se guardó correctamente. Periodo: " + Periodo.Llave + ", país: " + Periodo.PaisTempral;
                    return RedirectToAction("Periodos");
                    //return PartialView("Respuesta", "Se guardó correctamente. Periodo: "+Periodo.Llave+", país: "+Periodo.PaisTempral);
                   
                }
                else
                {
                    return PartialView("Respuesta", "Ocurrió un error favor de contactar a su administrador");
                }
            }
            if (Periodo.Country == 0)
            {
                errores.Add("\n\n condicional cero \n");
                Utilidades.negocio.GuardaLogVista(errores);
                errores.Clear();

                List<EPais> ListaPaises = Utilidades.negocio.RecuperaPaises();
                string error = "";
                Periodo.Llave = Periodo.Llave.Trim();
                
                Periodo.id = 0;
                Periodo.Etapa = 0;
              
                foreach (EPais item in ListaPaises)
                {
                    EPeriodos PerAux = new EPeriodos();
                    PerAux = Periodo;
                    PerAux.Country = item.id;
                    Periodo.id = 0;
                    EPeriodos temp = Utilidades.negocio.RecuperaPeriodopais(item.id);
                    

                    if (temp == null)//verificamos que no traiga periodos activos
                    {
                        try
                        {
                            if (Periodo.Llave != "")
                            {
                                if (!Utilidades.negocio.GuardaPeriodo(PerAux))
                                {
                                    error = error + "el pais" + item.descripcion + "no pudo cargarse correctamente  \n";
                                }
                                else
                                {
                                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardó  el nuevo periodo " + Periodo.Llave + " Del pais" + item.descripcion, "Periodos/GuardaPeriodo ");
                                }
                            }
                            else
                            {
                                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " No pudo insertar el periodo " + Periodo.Llave + " Del pais" + item.descripcion, "Periodos/GuardaPeriodo ");
                                error = error + "\n\n" + item.descripcion + "la llave se perdio\n";
                            }
                        }
                        catch (Exception ex) {
                            throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                     new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),PerAux.Llave});

                            throw;
                        }
                    }
                }
                if (error == "")
                {
                    return PartialView("Respuesta", "Se Guardó Correctamente");
                }
                else
                {
                    return PartialView("Respuesta", error + "\n\n  Ocurrió un error favor de contactar a su administrador");
                }
            }
            else {
                EPeriodos listaPeriodos = Utilidades.negocio.RecuperaPeriodopais(Periodo.Country);

                if (listaPeriodos != null)
                {
                    if (Periodo.id == 0)
                    {
                        return PartialView("Respuesta", "No se puede ingresar un nuevo periodo sin terminar el anterior");
                    }
                    else
                    {
                        if (Utilidades.negocio.GuardaPeriodo(Periodo))
                        {
                            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardó el periodo ", "Gestion/GuardaPeriodo ");
                            return PartialView("Respuesta", "Se Guardó Correctamente");
                        }
                        else
                        {
                            return PartialView("Respuesta", "Ocurrió un error favor de contactar a su administrador");
                        }
                    }
                }
                else
                {
                    Periodo.Etapa = 0;
                    Periodo.PaisTempral = Utilidades.negocio.RecuperaUnPais(Periodo.Country).descripcion;
                    //Se guarda el NUEVO PERIODO
                    bool savePeriodo = Utilidades.negocio.GuardaPeriodo(Periodo);
                    if (savePeriodo)
                    {
                        //GUARDAR LAS EVALUACIONES DE LOS USUARIOS
                        Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();

                        //SE RECUPERA LA LISTA DE USUARIOS POR PAÍS
                        Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(Periodo.Country);
                        //SE FILTRAN LOS USUARIOS QUE ESTAN ACTIVOS EN EL SISTEMA
                        Respuesta.listSesion = (from item in Respuesta.listSesion
                                                where item.Login.Activo
                                                select item).ToList();
                        //SE RECUPERA EL NUEVO PERIODO AGREGADO CON LA FINALIDAD DE SABER SU ID
                        Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(Periodo.Country);
                        //SE RECUPERA EL PAIS DEL CUAL ES EL PERIODO
                        EPais paisTemp = Utilidades.negocio.RecuperaUnPais(Periodo.Country);

                        //INICIALIZANDO NUESTRO EEVAL PARA INSERTAR A LOS USUARIOS
                        Modelo.EEval eval = new Modelo.EEval();
                        eval.id = 0; //INDICA QUE SERA UNA NUEVA EVALUACIÓN
                        eval.periodo = Respuesta.periodo.id; //id del periodo
                        eval.Status = 0;
                        eval.Activo = true;
                        eval.Modificadopor = sesion.Login.NombreCompleto;

                        foreach (var user in Respuesta.listSesion)
                        {
                            eval.id = 0;
                            eval.id_usuario = user.Login.id;
                            eval.Puesto = user.Login.Division;
                            eval.Nivel = user.Login.perfil;
                            eval.Division = user.Login.Division;
                            eval.Area = "";
                            eval.id_evaluador = user.Login.EvaluadorIdSap;

                            //SE REALIZA LA INSERSIÓN DE LA EVALUACIÓN
                            Utilidades.negocio.GuardaEvaluacion(eval);
                        
                        }



                        TempData["Message"]= "Se creó el periodo: " + Periodo.Llave + ", país: " + Periodo.PaisTempral;
                        return RedirectToAction("Periodos");
                        //return PartialView("Respuesta", "Se creó el periodo: "+Periodo.Llave+", país: "+Periodo.PaisTempral);
                    }
                    else
                    {
                        return PartialView("Respuesta", "Ocurrió un error favor de contactar a su administrador");
                    }
                }
            }       
        }

        [Autentificado]
        public ActionResult EliminarPeriodo(int Id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return View("Login");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardó el periodo ", "Gestion/GuardaPeriodo ");

            EPeriodos modelo = Utilidades.negocio.RecuperaUnPeriodo(Id);
            modelo.Activo = false;
            modelo.Modificadopor = sesion.Login.NombreCompleto;
            modelo.PaisTempral = Utilidades.negocio.RecuperaUnPais(modelo.Country).descripcion;
            List<Modelo.Clases.CSessionEval> lista = Utilidades.negocio.RecuperaLiUsuariosPais(modelo.Country);

            
            foreach (Modelo.Clases.CSessionEval item in lista)
            {
                if (item.Evaluacion != null)
                {
                    item.Evaluacion.Activo = false;
                    item.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                    Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                }
            }

            if (Utilidades.negocio.GuardaPeriodo(modelo))
            {
                TempData["Message"]= "Se eliminó correctamente. Periodo: " + modelo.Llave + ", País: " + modelo.PaisTempral;
                return RedirectToAction("Periodos");
                //return PartialView("Respuesta", "Se eliminó correctamente. Periodo: "+modelo.Llave+", País: "+modelo.PaisTempral);
            }
            else {
                return PartialView("Respuesta", "Ocurrió un error favor de contactar a su administrador");
            }
        }

        [Autentificado]
        public ActionResult TerminarPeriodo(int Id, int status)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            int emailtemplate = 0;
            List<string> listToMail = new List<string>();

            EPeriodos modelo = Utilidades.negocio.RecuperaUnPeriodo(Id);
            modelo.Etapa = status;
            modelo.Modificadopor = sesion.Login.NombreCompleto;
            modelo.PaisTempral = Utilidades.negocio.RecuperaUnPais(modelo.Country).descripcion;
            if (status == 5) {
                modelo.Activo = false;
            }
            if (Utilidades.negocio.GuardaPeriodo(modelo))
            {
                if (status == 1)
                {
                 List<Modelo.Clases.CSessionEval> lista =  Utilidades.negocio.RecuperaLiUsuariosPais(modelo.Country);
                    if (lista != null)
                    {
                        foreach (Modelo.Clases.CSessionEval item in lista)
                        {
                            if (item.Evaluacion != null)
                            {
                                if (item.Evaluacion.Status == 3)
                                {
                                    item.Evaluacion.Status = 4;
                                    item.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                                    Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(15);
                                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                                    correo.Mensaje=MandarCorreo.ProcesarMsg(correo.Mensaje,item.Login);
                                    MandarCorreo.SendMail("Soporte", item.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                                    Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                                    Bitacora.NuevaEntrada("El usuario: " + item.Login.NombreCompleto + " Finaliza etapa de carga de objetivos del periodo " + modelo.Llave, "Periodos/TerminarPeriodo ");
                                }
                            }
                        }
                    }
                }//hay que cambiar esta sección o eliminarla y cambiar el estatus 2 en donde aparece estatus = 3 abajo
                //else if (status == 2)
                //{
                //    List<Modelo.Clases.CSessionEval> lista = Utilidades.negocio.RecuperaLiUsuariosPais(modelo.Country);
                //    if (lista != null)
                //    {
                //        foreach (Modelo.Clases.CSessionEval item in lista)
                //        {
                //            if (item.Evaluacion != null)
                //            {
                //                if (item.Evaluacion.Status == 8)
                //                {
                //                    item.Evaluacion.Status = 9;
                //                    Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                //                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(17);                                  
                //                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                //                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, item.login);
                //                    MandarCorreo.SendMail("Soporte", item.login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente sistema de evaluación de desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                //                    Bitacora.NuevaEntrada("El usuario: " + item.login.NombreCompleto + " Finaliza etapa de evaluación primer semestre del periodo " + modelo.Llave, "Periodos/TerminarPeriodo ");
                //                }
                //            }
                //        }
                //    }
                //}
                else if (status == 3)
                {
                    List<Modelo.Clases.CSessionEval> lista = Utilidades.negocio.RecuperaLiUsuariosPais(modelo.Country);
                    if (lista != null)
                    {
                        foreach (Modelo.Clases.CSessionEval item in lista)
                        {
                            if (item.Evaluacion != null)
                            {
                                if (item.Evaluacion.Status == 8)
                                {
                                    item.Evaluacion.Status = 10;
                                    item.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                                    Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(15);                            
                                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, item.Login);
                                    MandarCorreo.SendMail("Soporte", item.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                                    Bitacora.NuevaEntrada("El usuario: " + item.Login.NombreCompleto + " Finaliza etapa de calibración primer semestre del periodo " + modelo.Llave, "Periodos/TerminarPeriodo ");
                                }
                            }
                        }
                    }
                }
                else if (status == 4)
                {
                    List<Modelo.Clases.CSessionEval> lista = Utilidades.negocio.RecuperaLiUsuariosPais(modelo.Country);
                    if (lista != null)
                    {
                        foreach (Modelo.Clases.CSessionEval item in lista)
                        {
                            if (item.Evaluacion != null)
                            {
                                if (item.Evaluacion.Status == 14)
                                {
                                    item.Evaluacion.Status = 15;
                                    item.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                                    Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(17);
                                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, item.Login);
                                    MandarCorreo.SendMail("Soporte", item.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                                    Bitacora.NuevaEntrada("El usuario: " + item.Login.NombreCompleto + " Finaliza etapa de Evaluación segundo semestre del periodo " + modelo.Llave, "Periodos/TerminarPeriodo ");
                                }
                            }
                        }
                    }
                }
                else if (status == 5)
                {
                    List<Modelo.Clases.CSessionEval> lista = Utilidades.negocio.RecuperaLiUsuariosPais(modelo.Country);
                    if (lista != null)
                    {
                        foreach (Modelo.Clases.CSessionEval item in lista)
                        {
                            if (item.Evaluacion != null)
                            {
                                    item.Evaluacion.Status = 15;
                                    item.Evaluacion.Activo = false;
                                item.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                                Utilidades.negocio.GuardaEvaluacion(item.Evaluacion);
                                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(20);
                                    correo.Mensaje = correo.Mensaje.Replace("#Periodo#", modelo.Llave);
                                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, item.Login);
                                    MandarCorreo.SendMail("Soporte", item.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                                    Bitacora.NuevaEntrada("El usuario: " + item.Login.NombreCompleto + " Finaliza etapa de calibración del periodo " + modelo.Llave, "Periodos/TerminarPeriodo ");
                            }
                        }
                    }
                }
                TempData["Message"]= "Se guardó correctamente, periodo: " + modelo.Llave + ", país: " + modelo.PaisTempral;
                return RedirectToAction("Periodos");
                //return PartialView("Respuesta", "Se guardó correctamente, periodo: "+modelo.Llave+", país: "+modelo.PaisTempral);
            }
            else
            {
                return PartialView("Respuesta", "Ocurrió un error favor de contactar a su administrador");
            }

        }
        
        public ActionResult Download(int pais)
        {
            string path = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Temp.xlsx");
            try
            {
                Create(pais);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string fileName = "Reporte.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return PartialView("Respuesta", ex.ToString() + " Phat->" + path);
            }
        }

        public ActionResult Detalle(int pais)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");

            Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();
           
            Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(pais);

            if (Respuesta.listSesion != null)
            {
                Respuesta.listSesion = (from usr in Respuesta.listSesion
                                        where (usr.Evaluacion != null) && (usr.Login.Activo == true )
                                        select usr).ToList();
            }

            Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(pais);
            EPais paisTemp = Utilidades.negocio.RecuperaUnPais(pais);

            if (Respuesta.listSesion == null)
            {
                return PartialView("Respuesta", "Ocurrió un error favor de contactar a su administrador");
            }
            else
            {
                if (Respuesta.listSesion.Count > 0)
                {
                    return PartialView(Respuesta);
                }
                else
                {
                    return PartialView("Respuesta", "No tiene usuarios activos en este periodo");
                }
               
            }
        }
        
        public void Create(int pais)
        {
          
            Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();

            Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(pais);
            Respuesta.listSesion = (from item in Respuesta.listSesion
                                    where item.Login.Activo
                                    select item).ToList();

           Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(pais);
            EPais paisTemp = Utilidades.negocio.RecuperaUnPais(pais);


            string filePath = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Temp.xlsx");

            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Column Cells");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
              

                ws.Cell(1, 1).Value = "ID SAP";
                ws.Cell(1, 2).Value = "Nombre completo";
                ws.Cell(1, 3).Value = "Clave de Periodo";
                ws.Cell(1, 4).Value = "Dirección";
                ws.Cell(1, 5).Value = "Puesto";
                ws.Cell(1, 6).Value = "País";
                ws.Cell(1, 7).Value = "Estatus";
                //ws.Cell(1, 8).Value = "Calificación Primer Semestre";
                ws.Cell(1, 8).Value = "Calificación Segundo Semestre";
                ws.Cell(1, 9).Value = "Calificación calibración";
                int i = 0;


                foreach(Modelo.Clases.CSessionEval item in Respuesta.listSesion)
                {
                    ws.Cell(i + 2, 1).Value = item.Login.id_sap;
                    ws.Cell(i + 2, 2).Value = item.Login.NombreCompleto;
                    ws.Cell(i + 2, 3).Value = Respuesta.periodo.Llave;
                    ws.Cell(i + 2, 4).Value = item.Login.Division;
                    ws.Cell(i + 2, 5).Value = item.Login.Puesto;
                    ws.Cell(i + 2, 6).Value = paisTemp.descripcion;

                    if (item.Evaluacion != null)
                    {
                        ws.Cell(i + 2, 7).Value = item.Evaluacion.StatusDsc;
                        //switch (item.Evaluacion.Status)
                        //{
                        //    case 0:
                        //        ws.Cell(i + 2, 7).Value = "Iniciado";
                        //        break;
                        //    case 1:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos cargados";
                        //        break;
                        //    case 2:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos rechazados";
                        //        break;
                        //    case 3:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos aprobados";
                        //        break;
                        //    case 4:
                        //        ws.Cell(i + 2, 7).Value = "Inicio evaluación";
                        //        break;
                        //    case 5:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos autoevaluados";
                        //        break;
                        //    case 6:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos evaluados";
                        //        break;
                        //    case 7:
                        //        ws.Cell(i + 2, 7).Value = "Evaluación segundo nivel";
                        //        break;
                        //    case 8:
                        //        ws.Cell(i + 2, 7).Value = "Cerrada";
                        //        break;
                        //    case 9:
                        //        ws.Cell(i + 2, 7).Value = "Calibración";

                        //        break;
                        //    case 10:
                        //        ws.Cell(i + 2, 7).Value = "Inicio evaluación";

                        //        break;
                        //    case 11:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos autoevaluados";

                        //        break;
                        //    case 12:
                        //        ws.Cell(i + 2, 7).Value = "Objetivos evaluados";
                        //        break;
                        //    case 13:
                        //        ws.Cell(i + 2, 7).Value = "Evaluación segundo nivel";
                        //        break;
                        //    case 14:
                        //        ws.Cell(i + 2, 7).Value = "Cerrada";
                        //        break;
                        //    case 15:
                        //        ws.Cell(i + 2, 7).Value = "Calibración";
                        //        break;
                        //}

                        //if (item.Evaluacion.CaliFinal > 0)
                        //{
                        //    ws.Cell(i + 2, 8).Value = item.Evaluacion.CaliFinal.ToString();
                        //}
                        //else
                        //{
                        //    ws.Cell(i + 2, 8).Value = " ";
                        //}

                        if (item.Evaluacion.CaliFinal2 > 0)
                        {
                            ws.Cell(i + 2, 8).Value = item.Evaluacion.CaliFinal2.ToString();
                        }
                        else
                        {
                            ws.Cell(i + 2, 8).Value = " ";
                        }
                        if (item.Evaluacion.CaliFinalCalibracion > 0)
                        {
                            ws.Cell(i + 2, 9).Value = item.Evaluacion.CaliFinalCalibracion.ToString();
                        }
                        else
                        {
                            ws.Cell(i + 2, 9).Value = " ";
                        }

                    }
                    else
                    {
                        ws.Cell(i + 2, 5).Value = "";
                        ws.Cell(i + 2, 6).Value = " ";
                        ws.Cell(i + 2, 7).Value = " ";
                    }
                    
                    i = i + 1;
                }
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {

            }

        }
        
        #region Drops
        public List<SelectListItem> DropPais()
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPais> ListaRoles = Utilidades.negocio.RecuperaPaises().Where(t=>t.Activo).ToList();

            foreach (EPais item in ListaRoles)
            {
                //if(item.Activo==1)
                Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });

            }
            return Respuesta;
        }

        #endregion

        #region utilidades
        private static long FechaTick(string fecha)
        {
            long respuesta = 0;
            if (fecha != null)
            {
                DateTime temp = DateTime.Parse(fecha);
                respuesta = temp.Ticks;
            }      
            return respuesta;
        }

        public static long DateToTicks(DateTime dtInput)
        {
            long ticks = 0;
            ticks = dtInput.Ticks;
            return ticks;
        }

        public static string TiksToDate(long fecha)
        {
            string res = "";
            if (fecha>0)
            {
                DateTime temp = new DateTime(fecha);
                string auxday = "", auxMonth = "";

                if (temp.Day.ToString().Length < 2)
                {
                    auxday = "0" + temp.Day.ToString();
                }
                else
                {
                    auxday = temp.Day.ToString();
                }

                if (temp.Month.ToString().Length < 2)
                {
                    auxMonth = "0" + temp.Month.ToString();
                }
                else
                {
                    auxMonth = temp.Month.ToString();
                }

                res = auxday + "/" + auxMonth + "/" + temp.Year.ToString();
            }
            return res;
        }

        #endregion


    }
}