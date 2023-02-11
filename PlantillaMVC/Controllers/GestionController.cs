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
using System.Web.Security;
using System.IO;
using Modelo;
using System.Threading.Tasks;
using System.Net.Mail;

namespace PlantillaMVC.Controllers
{
    public class GestionController : Controller
    {
        // GET: Gestion
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [Autentificado]
        public ActionResult GestionEvaluacion()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a gestion de evaluaciones", "Gestion/GestionEvaluacion ");

            ViewBag.Perfil = sesion.Login.perfil;
            Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();
            Respuesta.listSesion = new List<Modelo.Clases.CSessionEval>();
            //nandarek agregando opción para que si es nivel administrador/sistemas, te aparezcan todos
            if (sesion.Login.perfil == (int)EnumPerfil.Sistemas)
            {
                Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(0);
            }
            /*!nandarek termina*/
            //Respuesta.listSesion = ListaUsuarios;
            else
            {
                if(sesion.Login.perfil == (int)EnumPerfil.Regional)
                    foreach(var pais in sesion.Login.paisesRegion.Split(',').ToList())
                    Respuesta.listSesion.AddRange(Utilidades.negocio.RecuperaLiUsuariosPais(Convert.ToInt32(pais)));
                else
                Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(sesion.Login.Pais);
            }
            Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);

            if (Respuesta.periodo != null)
            {

                //objetivos
                Respuesta.periodo.StartDateObjTemp = TiksToDate(Respuesta.periodo.StartDateObj);
                Respuesta.periodo.FinishDateObjTemp = TiksToDate(Respuesta.periodo.FinishDateObj);
                //objetivos

                //evaluacion 1er periodo
                Respuesta.periodo.StartDateEvaTemp = TiksToDate(Respuesta.periodo.StartDateEva);
                Respuesta.periodo.FinishDateEvaTemp = TiksToDate(Respuesta.periodo.FinishDateEva);
                //evaluacion 1er periodo

                //Calibracion 1er periodo
                Respuesta.periodo.StartDateCaliTemp = TiksToDate(Respuesta.periodo.StartDateCali);
                Respuesta.periodo.FinishDateCaliTemp = TiksToDate(Respuesta.periodo.FinishDateCali);
                //Calibracion 1er periodo

                //Calibracion 1er periodo
                Respuesta.periodo.StartDateCaliTemp = TiksToDate(Respuesta.periodo.StartDateCali);
                Respuesta.periodo.FinishDateCaliTemp = TiksToDate(Respuesta.periodo.FinishDateCali);
                //Calibracion 1er periodo

                //Calibracion 2do periodo   StartDateEva2
                Respuesta.periodo.StartDateEva2Temp = TiksToDate(Respuesta.periodo.StartDateEva2);
                Respuesta.periodo.FinishDateEva2Temp = TiksToDate(Respuesta.periodo.FinishDateEva2);
                //Calibracion 1er periodo

                //Calibracion 1er periodo
                Respuesta.periodo.StartDateCali2Temp = TiksToDate(Respuesta.periodo.StartDateCali2);
                Respuesta.periodo.FinishDateCali2Temp = TiksToDate(Respuesta.periodo.FinishDateCali2);
                //Calibracion 1er periodo

                EPais temp = Utilidades.negocio.RecuperaUnPais(Respuesta.periodo.Country);
                Respuesta.periodo.PaisTempral = temp.descripcion;

            }
            return PartialView(Respuesta);
        }

        [Autentificado]
        public ActionResult NuevaGestion(int status, int proceso, int Id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            int email = 0;
            List<string> listToMail = new List<string>();

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            if (status == 0 && proceso == 0 && Id > 0)
            { //inicio evaluacion
                EEval eval = new EEval();
                ELogin temp = Utilidades.negocio.RecuperaUnUsuario(Id);
                EPeriodos Periodo = Utilidades.negocio.RecuperaPeriodopais(temp.Pais);

                eval.Puesto = temp.Puesto;
                eval.Division = temp.Division;
                eval.periodo = Periodo.id;
                eval.id_usuario = Id;
                eval.id_evaluador = temp.EvaluadorIdSap;
                eval.Activo = true;
                //temp.NombreCompleto = String.Concat(temp.Nombre, " ", temp.ApellidoPat, " ", temp.ApellidoMat);
                eval.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(eval))
                {
                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                    if (temp.Email != "")
                    {
                        //listToMail.Add(temp.Email);
                        ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(13);
                        correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, temp);
                        MandarCorreo.SendMail("Soporte", temp.Email, " ", "Estimado:" + temp.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                        Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " envió el correo 13 a " + temp.Email, "Gestion/NuevaGestion ");

                    }
                    return PartialView("Respuesta", "Se notificó al colaborador que tiene que iniciar su carga de objetivos");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo ingresar el periodo", "Gestion/NuevaGestion ");
                    return PartialView("Respuesta", "Ocurrió un error favor de notificar al administrador");
                }
            }
            if (status == 0 && proceso == 0 && Id == 0)
            {
                string error = "";
                ViewBag.Perfil = sesion.Login.perfil;
                Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();

                Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(sesion.Login.Pais);
                
                Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);

                foreach (Modelo.Clases.CSessionEval item in Respuesta.listSesion)
                {
                    if (item.Evaluacion == null)
                    {
                        EEval eval = new EEval();
                        eval.periodo = Respuesta.periodo.id;
                        eval.id_usuario = item.Login.id;
                        eval.id_evaluador = item.Login.EvaluadorIdSap;
                        eval.Activo = true;
                        eval.Modificadopor = sesion.Login.NombreCompleto;
                        if (Utilidades.negocio.GuardaEvaluacion(eval))
                        {
                            PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                            if (item.Login.Email != "")
                            {
                                ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(13);
                                correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, item.Login);
                                //await MandarCorreo.SendEmailAsync("Soporte", item.login.Email," ","", "Estimado:" + item.login.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                                MandarCorreo.SendMail("Soporte", item.Login.Email, " ", "Estimado:" + item.Login.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                                //MandarCorreo.SendEmail(item.login.Email, correo.asunto, "Estimado:" + item.login.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*");
                            }
                        }
                        else
                        {
                            error = error + "\n error al enviar a" + item.Login.NombreCompleto;
                        }
                    }
                }
                if (error != "")
                {
                    return PartialView("Respuesta", error);
                }
                else
                {
                    return PartialView("Respuesta", "Se notificó al colaborador que tiene que iniciar su carga de objetivos");
                }
            }
            if (status == 0 && proceso == 1 && Id > 0)
            { //inicio evaluacion
                EEval temp = Utilidades.negocio.RecuperaEvaluacionActivaUsario(Id);
                temp.Activo = false;
                temp.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(temp))
                {
                    return PartialView("Respuesta", "Se eliminó correctamente");
                }
                else
                {
                    return PartialView("Respuesta", "Ocurrió un error favor de notificar al administrador");
                }
            }
            if (status == 0 && proceso == 99 && Id > 0)//reiniciar objetivos
            { //inicio evaluacion
                EEval temp = Utilidades.negocio.RecuperaEvaluacionActivaUsario(Id);
                temp.Status = 0;
                temp.Activo = true;
                temp.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(temp))
                {
                    return PartialView("Respuesta", "Se guardó correctamente");
                }
                else
                {
                    return PartialView("Respuesta", "Ocurrió un error favor de notificar al administrador");
                }
            }
            if (status == 0 && proceso == 200 && Id > 0)//
            { //inicio evaluacion
                EEval eval = new EEval();
                ELogin temp = Utilidades.negocio.RecuperaUnUsuario(Id);
                EPeriodos Periodo = Utilidades.negocio.RecuperaPeriodopais(temp.Pais);

                eval.Status = 16;
                eval.Puesto = temp.Puesto;
                eval.Division = temp.Division;
                eval.periodo = Periodo.id;
                eval.id_usuario = Id;
                eval.id_evaluador = temp.EvaluadorIdSap;
                eval.Activo = true;
                eval.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(eval))
                {
                    PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                    if (temp.Email != "")
                    {
                        ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(13);
                        correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, temp);
                        //await MandarCorreo.SendEmailAsync("Soporte", temp.Email, " ", "", "Estimado:" + temp.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                        MandarCorreo.SendMail("Soporte", temp.Email, " ", "Estimado:" + temp.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                        Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " envió el correo 13 a " + temp.Email, "Gestion/NuevaGestion ");

                    }
                    return PartialView("Respuesta", "Se notificó al colaborador que tiene que iniciar su carga de objetivos");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo ingresar el periodo", "Gestion/NuevaGestion ");
                    return PartialView("Respuesta", "Ocurrió un error favor de notificar al administrador");
                }
            }

            return PartialView();
        }


        [Autentificado]
        public ActionResult Recordatorio(int status, int Id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];

            if (status == 0)
            { //inicio evaluacion              
                ELogin temp = Utilidades.negocio.RecuperaUnUsuario(Id);               
                PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(21);
                correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, temp);
                MandarCorreo.SendMail("Soporte", temp.Email, " ", "Estimado:" + temp.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Envió recordatorio de inicio de evaluación a " + temp.Email, "Gestion/Recordatorio ");
                return PartialView("Respuesta", "Se notificó al colaborador que tiene que iniciar su carga de objetivos");          
            }
            if (status == 4)
            { //inicio evaluacion              
                ELogin temp = Utilidades.negocio.RecuperaUnUsuario(Id);
                PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(22);
                correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, temp);
                MandarCorreo.SendMail("Soporte", temp.Email, " ", "Estimado:" + temp.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Envió recordatorio de inicio de evaluación a " + temp.Email, "Gestion/Recordatorio ");
                return PartialView("Respuesta", "Se notificó al colaborador que tiene que iniciar su carga de objetivos");
            }
            if (status == 10)
            { //inicio evaluacion              
                ELogin temp = Utilidades.negocio.RecuperaUnUsuario(Id);
                PlantillaMVC.EnvioCorreo MandarCorreo = new PlantillaMVC.EnvioCorreo();
                ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(22);
                correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, temp);
                MandarCorreo.SendMail("Soporte", temp.Email, " ", "Estimado:" + temp.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Envió recordatorio de inicio de evaluación a " + temp.Email, "Gestion/Recordatorio ");
                return PartialView("Respuesta", "Se notificó al colaborador que tiene que iniciar su carga de objetivos");
            }
            return PartialView();
        }

        public ActionResult RegresarEvaluacion(int Id, int lvl)
        {
            EEval eval = new EEval();
            eval = Utilidades.negocio.RecuperaUnaEaluacion(Id);
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            eval.Modificadopor = sesion.Login.NombreCompleto;
            if((eval.Status <= 9 && eval.Status >= 4) || eval.Status > 16)
            {
                if (lvl == 0)
                    eval.Status = 4;
                else
                    eval.Status = 5;
            }else if (eval.Status <= 15)
            {
                if (lvl == 0)
                    eval.Status = 10;
                else
                    eval.Status = 11;
            }
            
            bool ok = Utilidades.negocio.GuardaEvaluacion(eval);

            string resp = "";
            if (ok)
                resp = "El estatus fue cambiado correctamente";
            else
                resp = "ha ocurrido un error. Por favor intente más tarde.";

            return PartialView("Respuesta", resp);
        }

        public ActionResult CambiarStatus(int Id, int status)
        {
            EEval eval = new EEval();
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            eval = Utilidades.negocio.RecuperaUnaEaluacion(Id);
            eval.Status = status;
            if(eval.Status==0 && eval.Rechazos==3)
                eval.Rechazos -= 1;
            else if (eval.Status == 4 && eval.RechazosMitad == 3)
                eval.RechazosMitad -= 1;
            else if (eval.Status == 10 && eval.RechazosFin == 3)
                eval.RechazosFin -= 1;
            eval.Modificadopor = sesion.Login.NombreCompleto;
            bool ok = Utilidades.negocio.GuardaEvaluacion(eval);

            string resp = "";
            if (ok)
                resp = "El estatus fue cambiado correctamente";
            else
                resp = "ha ocurrido un error. Por favor intente más tarde.";

            //if (!Utilidades.ValidaSesion(Session, HttpContext))
            //    return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a gestion de evaluaciones", "Gestion/GestionEvaluacion ");

            ViewBag.Perfil = sesion.Login.perfil;
            Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();

            Respuesta.listSesion = Utilidades.negocio.RecuperaLiUsuariosPais(sesion.Login.Pais);
            Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);

            if (Respuesta.periodo != null)
            {

                //objetivos
                Respuesta.periodo.StartDateObjTemp = TiksToDate(Respuesta.periodo.StartDateObj);
                Respuesta.periodo.FinishDateObjTemp = TiksToDate(Respuesta.periodo.FinishDateObj);
                //objetivos

                //evaluacion 1er periodo
                Respuesta.periodo.StartDateEvaTemp = TiksToDate(Respuesta.periodo.StartDateEva);
                Respuesta.periodo.FinishDateEvaTemp = TiksToDate(Respuesta.periodo.FinishDateEva);
                //evaluacion 1er periodo

                //Calibracion 1er periodo
                Respuesta.periodo.StartDateCaliTemp = TiksToDate(Respuesta.periodo.StartDateCali);
                Respuesta.periodo.FinishDateCaliTemp = TiksToDate(Respuesta.periodo.FinishDateCali);
                //Calibracion 1er periodo

                //Calibracion 1er periodo
                Respuesta.periodo.StartDateCaliTemp = TiksToDate(Respuesta.periodo.StartDateCali);
                Respuesta.periodo.FinishDateCaliTemp = TiksToDate(Respuesta.periodo.FinishDateCali);
                //Calibracion 1er periodo

                //Calibracion 2do periodo   StartDateEva2
                Respuesta.periodo.StartDateEva2Temp = TiksToDate(Respuesta.periodo.StartDateEva2);
                Respuesta.periodo.FinishDateEva2Temp = TiksToDate(Respuesta.periodo.FinishDateEva2);
                //Calibracion 1er periodo

                //Calibracion 1er periodo
                Respuesta.periodo.StartDateCali2Temp = TiksToDate(Respuesta.periodo.StartDateCali2);
                Respuesta.periodo.FinishDateCali2Temp = TiksToDate(Respuesta.periodo.FinishDateCali2);
                //Calibracion 1er periodo

                EPais temp = Utilidades.negocio.RecuperaUnPais(Respuesta.periodo.Country);
                Respuesta.periodo.PaisTempral = temp.descripcion;
            }

            //return PartialView("Respuesta", resp);
            return PartialView("GestionEvaluacion", Respuesta);
        }

        //-------------------------------
        public ActionResult Index()
        {
            List<VEvaluacion> eval = new List<VEvaluacion>();

            eval = Utilidades.negocio.GetEvalViewByPeriodActive();

            return PartialView(eval);
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

        public static string TiksToDate(long fecha)
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

            string res = auxday + "/" + auxMonth + "/" + temp.Year.ToString();
            return res;
        }

        #endregion


    }
}