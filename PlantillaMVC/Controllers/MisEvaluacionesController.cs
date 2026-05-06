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
using CapaLogica;
using PlantillaMVC.Filters;
using System.Web.Security;
using System.IO;
using DocumentFormat.OpenXml.Wordprocessing;
using static Spire.Pdf.General.Render.Decode.Jpeg2000.j2k.codestream.HeaderInfo;

namespace PlantillaMVC.Controllers
{
    public class MisEvaluacionesController : Controller
    {
        // GET: MisEvaluaciones
        public ActionResult Index()
        {
            return View();
        }

        [Autentificado]
        public ActionResult MiEvaluacion(int user, int id)
        {

            //usuario
            //0-> usuario Propietario
            //1-> usuario Jefe
            //2-> usuario jefe lv2
            // Al inicio del método MiEvaluacion, después de cargar eval:
           

            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a MiEvaluacion", "MisEvaluaciones/MiEvaluacion ");

            ViewBag.usuario = user;
            Modelo.Clases.CEvaluacion eval = new Modelo.Clases.CEvaluacion();
            if (id == 0)
            {
                eval.sesion = Utilidades.negocio.RecuperaUsuarioEval(sesion.Login.id);
            }
            else
            {
                eval.sesion = Utilidades.negocio.RecuperaUsuarioEval(id);
            }

            if (eval.Liobjetivos == null)
            {
                eval.Liobjetivos = new List<Modelo.EObjetives>(); // o el tipo correcto
            }

            if (eval.sesion.Evaluacion == null)
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso sin tener evaluación ", "MisEvaluaciones/MiEvaluacion ");
                return PartialView("Respuesta", "No se tiene una evaluación asignada actualmente");
            }
            else
            {
                eval.sesion.Login.FechaAntiguedadTemp = new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(eval.sesion.Login.FechaIngresoTemp));
                eval.Jefedir = Utilidades.negocio.RecuperaUnUsuarioSap(eval.sesion.Login.EvaluadorIdSap);
                if (eval.Jefedir == null)
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso sin información del jefe ", "MisEvaluaciones/MiEvaluacion ");
                    return PartialView("Respuesta", "Sin información");
                }

                if (eval.Jefedir.EvaluadorIdSap == null || eval.Jefedir.EvaluadorIdSap == "")
                {
                    eval.EvalL2 = false;
                }
                else
                {
                    eval.EvalL2 = true;
                }

                eval.periodo = Utilidades.negocio.RecuperaPeriodopais(eval.sesion.Login.Pais);
                eval.Liobjetivos = Utilidades.negocio.RecuperaListaObjetivos(eval.sesion.Evaluacion.id);
                ViewBag.PesoTotal = eval.Liobjetivos != null ? eval.Liobjetivos.Sum(t => t.ponderado) : 0;
                ViewBag.nObjetivos = eval.Liobjetivos != null ? eval.Liobjetivos.Count() : 0;
                //eval.ListaCompetencias = Utilidades.negocio.RecuperaLiCompetencias(eval.sesion.Evaluacion.id);

                if (eval.ListaCompetencias != null)
                {
                    foreach (ECompetemces item in eval.ListaCompetencias)
                    {
                        ECatComp Comp = Utilidades.negocio.RecuperaUnaCatCompetencias(item.titulo);
                        ECatSubComp subComp = new ECatSubComp();
                        if (item.Subtitulo != 0)
                            subComp = Utilidades.negocio.RecuperaCatSubCompetencia(item.Subtitulo);
                        subComp.SubCompetencia = subComp.SubCompetencia != null ? subComp.SubCompetencia : string.Empty;
                        item.tituloTemp = $"{Comp.descripcion} | {subComp.SubCompetencia}";

                    }
                }

                //Recuperando los objetivos Personal PTP
                eval.ListaPersonalPTP = Utilidades.negocio.RecuperaListaObjetivosPTP(eval.sesion.Evaluacion.id);
                eval.ListaPersonalPDP = Utilidades.negocio.RecuperaListaObjetivosPDP(eval.sesion.Evaluacion.id);


                eval.Jefedir.FechaAntiguedadTemp = new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(eval.Jefedir.FechaIngresoTemp));
                if (eval.periodo != null)
                {

                    //objetivos
                    eval.periodo.StartDateObjTemp = TiksToDate(eval.periodo.StartDateObj);
                    eval.periodo.FinishDateObjTemp = TiksToDate(eval.periodo.FinishDateObj);
                    //objetivos

                    //evaluacion 1er periodo
                    eval.periodo.StartDateEvaTemp = TiksToDate(eval.periodo.StartDateEva);
                    eval.periodo.FinishDateEvaTemp = TiksToDate(eval.periodo.FinishDateEva);
                    //evaluacion 1er periodo

                    //Calibracion 1er periodo
                    eval.periodo.StartDateCaliTemp = TiksToDate(eval.periodo.StartDateCali);
                    eval.periodo.FinishDateCaliTemp = TiksToDate(eval.periodo.FinishDateCali);
                    //Calibracion 1er periodo

                    //Calibracion 1er periodo
                    eval.periodo.StartDateCaliTemp = TiksToDate(eval.periodo.StartDateCali);
                    eval.periodo.FinishDateCaliTemp = TiksToDate(eval.periodo.FinishDateCali);
                    //Calibracion 1er periodo

                    //Calibracion 2do periodo   StartDateEva2
                    eval.periodo.StartDateEva2Temp = TiksToDate(eval.periodo.StartDateEva2);
                    eval.periodo.FinishDateEva2Temp = TiksToDate(eval.periodo.FinishDateEva2);
                    //Calibracion 1er periodo

                    //Calibracion 1er periodo
                    eval.periodo.StartDateCali2Temp = TiksToDate(eval.periodo.StartDateCali2);
                    eval.periodo.FinishDateCali2Temp = TiksToDate(eval.periodo.FinishDateCali2);
                    //Calibracion 1er periodo

                    EPais temp = Utilidades.negocio.RecuperaUnPais(eval.periodo.Country);
                    eval.periodo.PaisTempral = temp.descripcion;

                }
                string panel1 = "";
                string panel2 = "";
                if (eval.periodo != null)
                {
                    if (eval.periodo.Etapa <= 2)
                    {
                        panel1 = "in active";
                    }

                    if (eval.periodo.Etapa > 2)
                    {
                        panel2 = "in active";
                    }
                }
                else
                {
                    panel1 = "in active";
                }

                EStatus statusEval = Utilidades.negocio.GetStatusById(eval.sesion.Evaluacion.Status);
                ViewBag.status = statusEval.Id;
                ViewBag.statusName = statusEval.Name;
                ViewBag.statusDesc = statusEval.Descript;
                ViewBag.porc = statusEval.Porc;
                ViewBag.panel1 = panel1;
                ViewBag.panel2 = panel2;
                if (eval.sesion.Evaluacion.Status >= 11)
                {
                    eval.Escaleta = Utilidades.negocio.RecuperaEscaleta();

                    // ←←← CORRECCIÓN PRINCIPAL
                    double sum_ponderado = 0;

                    if (eval.Liobjetivos != null && eval.Liobjetivos.Any())
                    {
                        sum_ponderado = eval.Liobjetivos.Sum(t => t.ValorObj);
                    }
                    else
                    {
                        // Opcional: loguear para debug
                        // System.Diagnostics.Debug.WriteLine("Liobjetivos es null o vacío - Status: " + eval.sesion.Evaluacion.Status);
                    }

                    ViewBag.sum_ponderado = sum_ponderado;

                    double finalScore = sum_ponderado;
                    if (finalScore < 1) finalScore = 1;
                    if (finalScore > 5) finalScore = 5;

                    // Guardamos tanto entero como decimal
                    ViewBag.CalFinTemp = new EEscaleta
                    {
                        Valor = Convert.ToInt32(Math.Round(finalScore, MidpointRounding.AwayFromZero))
                    };

                    ViewBag.CalFinalDecimal = finalScore;   // ← Útil para mostrar con decimales

                    // Autoevaluación 2
                    double autoeval2 = 0;
                    if (eval.Liobjetivos != null && eval.Liobjetivos.Any())
                    {
                        autoeval2 = eval.Liobjetivos.Sum(e => e.AutoEval2);
                    }
                    ViewBag.CalAutoEval = autoeval2;
                }
            }
                return PartialView(eval);
        }

        [Autentificado]
        public ActionResult OperacionObjetivos(int id, int eval, int status, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " ingreso a crear/modificar un objetivo ", "MisEvaluaciones/OperacionObjetivos ");


            Modelo.EObjetives model = new EObjetives();
            var objetivos = Utilidades.negocio.RecuperaListaObjetivos(eval);
            ViewBag.status = status;
            if (id > 0)
            {
                model = Utilidades.negocio.RecuperaUnObjetivoid(id);
            }
            model.idEval = eval;

            ViewBag.Eval = eval;
            ViewBag.Usr = Usr;

            if (status == 0)
            {
                model.Etapa = 0; //creado desde el inicio
            }
            else if (status == 4 && model.id == 0)
            {
                model.Etapa = 1; //creado en medio año usuario
            }
            else if (status == 5 && model.id == 0)
            {
                model.Etapa = 2; //creado en medio año jefe
            }
            else if (status == 4 && model.id > 0)
            {
                if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                //model.Etapa = 3; editado por usuario
            }
            else if (status == 5 && model.id > 0)
            {
                if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                else if (model.Etapa == 2)
                {
                    model.Etapa = 2;
                }
                //model.Etapa = 4; editado por jefe
            }
            else if (status == 20)
            {
                model.Etapa = 5;//eliminado por usuario
            }
            else if (status == 10 && model.id == 0)
            {
                model.Etapa = 6; //creado por usuario fin de año
            }
            else if (status == 11 && model.id == 0)
            {
                model.Etapa = 7; //creado por jefe
            }
            else if (status == 10 && model.id > 0)
            {
                if (model.Etapa == 6)
                {
                    model.Etapa = 6;
                }
                else if (model.Etapa == 7)
                {
                    model.Etapa = 7;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                else if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 2)
                {
                    model.Etapa = 2;
                }
            }
            else if (status == 11 && model.id > 0)
            {
                if (model.Etapa == 6)
                {
                    model.Etapa = 6;
                }
                else if (model.Etapa == 7)
                {
                    model.Etapa = 7;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                else if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 2)
                {
                    model.Etapa = 2;
                }
            }
            else if (status == 21)
            {
                model.Etapa = 10;//eliminado por usuario 2
            }
            ViewData["Calificaciones"] = Calificaciones();
            ViewBag.PesoTotal = objetivos != null ? objetivos.Sum(t => t.ponderado) - model.ponderado : 0;
            return PartialView(model);
        }

        [Autentificado]
        public ActionResult OperacionCompetencias(int id, int eval, int status, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " ingreso a cargar/modificar una competencia ", "MisEvaluaciones/OperacionCompetencias ");

            Modelo.ECompetemces model = new ECompetemces();
            model.Eval = eval;
            if (id > 0)
            {
                model = Utilidades.negocio.RecuperaUnaCompetencia(id);
            }
            ECompetemces temp = Utilidades.negocio.RecuperaUnaCompetencia(id);

            //Moldeo.EPersonalDP modelObjetive = Utilidades.negocio.

            model.Eval = eval;
            ViewBag.Eval = eval;

            ViewBag.status = status;
            ViewBag.Usr = Usr;

            if (status == 0)
            {
                model.Etapa = 0; //creado desde el inicio
            }
            else if (status == 4 && model.id == 0)
            {
                model.Etapa = 1; //creado en medio año usuario
            }
            else if (status == 5 && model.id == 0)
            {
                model.Etapa = 2; //creado en medio año jefe
            }
            else if (status == 4 && model.id > 0)
            {
                if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                //model.Etapa = 3; editado por usuario
            }
            else if (status == 5 && model.id > 0)
            {
                if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                else if (model.Etapa == 2)
                {
                    model.Etapa = 2;
                }
                //model.Etapa = 4; editado por jefe
            }
            else if (status == 20)
            {
                model.Etapa = 5;//eliminado por usuario
            }
            else if (status == 10 && model.id == 0)
            {
                model.Etapa = 6; //creado por usuario fin de año
            }
            else if (status == 11 && model.id == 0)
            {
                model.Etapa = 7; //creado por jefe
            }
            else if (status == 10 && model.id > 0)
            {
                if (model.Etapa == 6)
                {
                    model.Etapa = 6;
                }
                else if (model.Etapa == 7)
                {
                    model.Etapa = 7;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                else if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 2)
                {
                    model.Etapa = 2;
                }
            }
            else if (status == 11 && model.id > 0)
            {
                if (model.Etapa == 6)
                {
                    model.Etapa = 6;
                }
                else if (model.Etapa == 7)
                {
                    model.Etapa = 7;
                }
                else if (model.Etapa == 0)
                {
                    model.Etapa = 0;
                }
                else if (model.Etapa == 1)
                {
                    model.Etapa = 1;
                }
                else if (model.Etapa == 2)
                {
                    model.Etapa = 2;
                }
            }
            else if (status == 21)
            {
                model.Etapa = 10;//eliminado por usuario 2
            }

            if (temp != null)
            {
                ViewData["Competencias"] = DropCatComp(eval, temp.titulo);
            }
            else
            {
                ViewData["Competencias"] = DropCatComp(eval, 0);
            }
            if (id != 0)
                ViewData["Editar"] = true;
            else
                ViewData["Editar"] = false;
            ViewData["Subcompetencias"] = SubCompsCat();
            return PartialView(model);
        }

        #region Objetivos PTP y PDP


        [Autentificado]
        public ActionResult RecargaObjetivosPTP(int Eval, int status, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Actualizó la vista de objetivos PTP " + Eval.ToString(), "Gestion/RecargaObjetivosPTP ");

            List<EPersonalTP> liObjetivos = Utilidades.negocio.RecuperaListaObjetivosPTP(Eval);
            EEval evaluacion = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
            EPeriodos periodo = null;
            if (evaluacion != null)
            {
                periodo = Utilidades.negocio.RecuperaUnPeriodo(evaluacion.periodo);
            }

            ViewBag.status = status;
            ViewBag.usuario = Usr;
            ViewBag.StatusPer = periodo.Etapa;
            ViewBag.Eval = Eval;
            ViewBag.nObjetivos = liObjetivos.Count();
            return PartialView(liObjetivos);
        }

        [Autentificado]
        public ActionResult RecargaObjetivosPDP(int Eval, int status, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Actualizó la vista de objetivos PDP " + Eval.ToString(), "Gestion/RecargaObjetivosPDP ");

            List<EPersonalDP> liObjetivos = Utilidades.negocio.RecuperaListaObjetivosPDP(Eval);
            EEval evaluacion = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
            EPeriodos periodo = null;
            if (evaluacion != null)
            {
                periodo = Utilidades.negocio.RecuperaUnPeriodo(evaluacion.periodo);
            }

            ViewBag.status = status;
            ViewBag.usuario = Usr;
            ViewBag.StatusPer = periodo.Etapa;
            ViewBag.Eval = Eval;
            ViewBag.nObjetivos = liObjetivos.Count();
            return PartialView(liObjetivos);
        }


        [Autentificado]
        public ActionResult OperacionObjetivosPTP(int id, int eval, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " ingreso a cargar/Modificar un objetipo PT", "MisEvaluaciones/OperacionObjetivosPTP ");

            Modelo.EPersonalTP model = new EPersonalTP();
            model.Eval = eval;
            if (id > 0)
            {
                model = Utilidades.negocio.RecuperaUnObjetivoPTPid(id);
            }

            model.Eval = eval;
            ViewBag.Eval = eval;

            //ViewBag.status = status;
            ViewBag.Usr = Usr;

            
            if (id != 0)
                ViewData["Editar"] = true;
            else
                ViewData["Editar"] = false;

            return PartialView(model);
        }

        [Autentificado]
        public ActionResult OperacionObjetivosPDP(int id, int eval, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " ingreso a cargar/modificar un objetivo DP", "MisEvaluaciones/OperacionObjetivosPDP ");

            Modelo.EPersonalDP model = new EPersonalDP();
            model.Eval = eval;
            if (id > 0)
            {
                model = Utilidades.negocio.RecuperaUnObjetivoPDPid(id);
            }

            model.Eval = eval;
            ViewBag.Eval = eval;

            //ViewBag.status = status;
            ViewBag.Usr = Usr;


            if (id != 0)
                ViewData["Editar"] = true;
            else
                ViewData["Editar"] = false;

            return PartialView(model);
        }

        [HttpPost]
        [Autentificado]
        public ActionResult GuardaObjetivosPTP(Modelo.EPersonalTP modelo, int Eval, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];

            //if (modelo.id > 0)

            if (Utilidades.negocio.GuardaObjetivosPTP(modelo))
            {
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                return PartialView("Respuesta", "ocurrió un error");
            }
        }

        [HttpPost]
        [Autentificado]
        public ActionResult GuardaObjetivosPDP(Modelo.EPersonalDP modelo, int Eval, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];

            //if (modelo.id > 0)

            if (Utilidades.negocio.GuardaObjetivosPDP(modelo))
            {
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                return PartialView("Respuesta", "ocurrió un error");
            }
        }

        [Autentificado]
        public ActionResult EliminaObjetivoPTP(int id)
        {

            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            if (Utilidades.negocio.EliminaUnObjetivoPTP(id))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Elimino objetivo " + id.ToString(), "MisEvaluaciones/EliminaUnObjetivoPTP ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo eliminar el objetivo " + id.ToString(), "MisEvaluaciones/EliminaUnObjetivoPTP ");
                return PartialView("Respuesta", "ocurrió un error");
            }

        }

        [Autentificado]
        public ActionResult EliminaObjetivoPDP(int id)
        {

            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            if (Utilidades.negocio.EliminaUnObjetivoPDP(id))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Elimino objetivo " + id.ToString(), "MisEvaluaciones/EliminaUnObjetivoPDP ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo eliminar el objetivo " + id.ToString(), "MisEvaluaciones/EliminaUnObjetivoPDP ");
                return PartialView("Respuesta", "ocurrió un error");
            }

        }
        #endregion

        [Autentificado]
        public ActionResult GuardaCompetencias(Modelo.ECompetemces modelo, int Eval, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            EEval EvalTemp = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
            ECompetemces TempComp = Utilidades.negocio.RecuperaUnaCompetencia(modelo.id);

            if (TempComp != null)
            {
                if ((modelo.titulo != TempComp.titulo || modelo.actividades != TempComp.actividades)&& EvalTemp.Status!=16)
                {
                    if (EvalTemp.Status < 5 && Usr == 0 && modelo.Etapa != 1)
                    {
                        modelo.Etapa = 3;
                    }
                    else if (EvalTemp.Status <= 5 && Usr == 1 && modelo.Etapa != 2)
                    {
                        modelo.Etapa = 4;
                    }

                    if (EvalTemp.Status > 5 && Usr == 0 && modelo.Etapa != 6)
                    {
                        modelo.Etapa = 8;
                    }
                    else if (EvalTemp.Status > 5 && Usr == 1 && modelo.Etapa != 7)
                    {
                        modelo.Etapa = 9;
                    }
                }
                else
                {
                    TempComp.Etapa = modelo.Etapa;
                }
            }
            /*else
            {
                TempComp = new ECompetemces();
                TempComp.Etapa = modelo.Etapa;
                TempComp.Eval = modelo.Eval;
            }
            if (EvalTemp.Status == 4)
            {
                TempComp.resultados = modelo.resultados;
                TempComp.actividades = modelo.actividades;
                TempComp.titulo = modelo.titulo;

            }
            else if (EvalTemp.Status == 5)
            {
                TempComp.resultados = modelo.resultados;
                TempComp.actividades = modelo.actividades;
                TempComp.titulo = modelo.titulo;
            }
            else if (EvalTemp.Status == 10)
            {
                TempComp.resultados = modelo.resultados;
                TempComp.actividades = modelo.actividades;
                TempComp.titulo = modelo.titulo;

            }
            else if (EvalTemp.Status == 11)
            {
                TempComp.resultados = modelo.resultados;
                TempComp.actividades = modelo.actividades;
                TempComp.titulo = modelo.titulo;
            }
            else if (EvalTemp.Status == 12)
            {
                TempComp.resultados = modelo.resultados;
                TempComp.actividades = modelo.actividades;
                TempComp.titulo = modelo.titulo;
            }*/

            modelo.Modificadopor = sesion.Login.NombreCompleto;
            if (Utilidades.negocio.GuardaCompetencias(modelo))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardo la competencia " + modelo.id.ToString(), "MisEvaluaciones/OperacionCompetencias ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo guardar la competencia " + modelo.id.ToString(), "MisEvaluaciones/OperacionCompetencias ");
                return PartialView("Respuesta", "ocurrió un error");
            }
        }

        [HttpPost]
        [Autentificado]
        public ActionResult GuardaEvaluacion(Modelo.Clases.CEvaluacion modelo)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            modelo.sesion.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
            if (Utilidades.negocio.GuardaEvaluacion(modelo.sesion.Evaluacion))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardo la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaEvaluacion ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo guardar la evaluacion " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaEvaluacion ");
                return PartialView("Respuesta", "ocurrió un error");
            }
        }

        [Autentificado]
        public ActionResult GuardaObjetivos(Modelo.EObjetives modelo, int Eval, int Usr)
        {
                if (!Utilidades.ValidaSesion(Session, HttpContext))
                    return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];

                if (modelo.id > 0)
                {
                    EObjetives temporal = Utilidades.negocio.RecuperaUnObjetivoid(modelo.id);
                    EEval EvalTemp = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
                    temporal.Modificadopor = sesion.Login.NombreCompleto;
                    modelo.Modificadopor = sesion.Login.NombreCompleto;
                    if ((modelo.ponderado != temporal.ponderado || modelo.titulo != temporal.titulo || modelo.objMetricas != temporal.objMetricas || modelo.objdesc != temporal.objdesc) && (EvalTemp.Status != 16))
                    {

                        if (EvalTemp.Status < 5 && Usr == 0 && temporal.Etapa != 1)
                        {
                            temporal.Etapa = 3;
                        }
                        else if (EvalTemp.Status <= 5 && Usr == 1 && temporal.Etapa != 2)
                        {
                            temporal.Etapa = 4;
                        }

                        if (EvalTemp.Status > 5 && Usr == 0 && temporal.Etapa != 6)
                        {
                            temporal.Etapa = 8;
                        }
                        else if (EvalTemp.Status > 5 && Usr == 1 && temporal.Etapa != 7)
                        {
                            temporal.Etapa = 9;
                        }

                        temporal.titulo = modelo.titulo;
                        temporal.objMetricas = modelo.objMetricas;
                        temporal.objdesc = modelo.objdesc;
                        temporal.ponderado = modelo.ponderado;
                        temporal.cumplimientoEvaluador2 = modelo.cumplimientoEvaluador2;
                    }
                    else
                    {
                        temporal.Etapa = modelo.Etapa;
                    }

                    if (EvalTemp.Status == 4)
                    {
                        temporal.resultado = modelo.resultado;
                    temporal.cumplimientoEvaluado = modelo.cumplimientoEvaluado;

                    }
                    else if (EvalTemp.Status == 5)
                    {
                        temporal.ComentariosJefeEval = modelo.ComentariosJefeEval;
                    temporal.cumplimientoEvaluador = modelo.cumplimientoEvaluador;

                    }
                    else if (EvalTemp.Status == 10)
                    {
                        //temporal.titulo = modelo.titulo;
                        //temporal.objMetricas = modelo.objMetricas;
                        //temporal.objdesc = modelo.objdesc;
                        temporal.Resultado2 = modelo.Resultado2;
                    temporal.cumplimientoEvaluado2 = modelo.cumplimientoEvaluado2;
                        //temporal.AutoEval2 = modelo.AutoEval2;

                    }
                    else if (EvalTemp.Status == 11)
                    {
                        //temporal.titulo = modelo.titulo;
                        //temporal.objMetricas = modelo.objMetricas;
                        //temporal.objdesc = modelo.objdesc;
                        temporal.cumplimientoEvaluador2 = modelo.cumplimientoEvaluador2;
                        temporal.ComentariosJefeEval2 = modelo.ComentariosJefeEval2;
                        //temporal.Calif2 = modelo.Calif2;
                    }
                    else if (EvalTemp.Status == 12)
                    {
                        temporal.titulo = modelo.titulo;
                        temporal.objMetricas = modelo.objMetricas;
                        temporal.objdesc = modelo.objdesc;
                        temporal.ComentariosJefeEval2 = modelo.ComentariosJefeEval2;
                        temporal.cumplimientoEvaluador2 = modelo.cumplimientoEvaluador2;
                    }
                    else if (EvalTemp.Status == 16)
                    {
                        temporal.titulo = modelo.titulo;
                        temporal.objMetricas = modelo.objMetricas;
                        temporal.objdesc = modelo.objdesc;
                    temporal.ponderado = modelo.ponderado;
                    }

                    if (Utilidades.negocio.GuardaObjetivo(temporal))
                    {
                        Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardo el objetivo" + modelo.id, "MisEvaluaciones/GuardaObjetivos ");
                        return PartialView("Respuesta", "se guardó correctamente");
                    }
                    else
                    {
                        Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo guardar el objetivo" + modelo.id, "MisEvaluaciones/GuardaObjetivos ");

                        return PartialView("Respuesta", "ocurrió un error");
                    }
                }
                if (Utilidades.negocio.GuardaObjetivo(modelo))
                {
                    return PartialView("Respuesta", "se guardó correctamente");
                }
                else
                {
                    return PartialView("Respuesta", "ocurrió un error");
                }
        }

        [Autentificado]
        public ActionResult EliminarObjetivosUsr(int id, int Stat)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " ingreso a eliminar objetivos ", "MisEvaluaciones/EliminarObjetivosUsr ");


            Modelo.EObjetives model = new EObjetives();
            if (id > 0)
            {
                model = Utilidades.negocio.RecuperaUnObjetivoid(id);
                model.Modificadopor = sesion.Login.NombreCompleto;
                model.resultado = "";
                model.Resultado2 = "";
                if (Stat == 4)
                {
                    model.Etapa = 5;
                }
                else
                {
                    model.Etapa = 10;
                }

            }

            if (Utilidades.negocio.GuardaObjetivo(model))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + "elimino el objetivo " + model.id, "MisEvaluaciones/EliminarObjetivosUsr ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo eliminiar el objetivo " + model.id, "MisEvaluaciones/EliminarObjetivosUsr ");
                return PartialView("Respuesta", "ocurrió un error");
            }
        }
           
        [Autentificado]
        public ActionResult EliminaUnObjetivos(int id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            if (Utilidades.negocio.EliminarUnObjetivo(id))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Elimino objetivo " + id.ToString() , "MisEvaluaciones/EliminaUnObjetivos ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo eliminar el objetivo " + id.ToString(), "MisEvaluaciones/EliminaUnObjetivos ");
                return PartialView("Respuesta", "ocurrió un error");
            }
        }

        [Autentificado]
        public ActionResult GuardaClaseEvaluacion(string submitButton, Modelo.Clases.CEvaluacion modelo)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");


            if (submitButton== "CagraObjetivos") {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(sesion.Login.id);

                envio.Evaluacion.Status = 1;
                envio.Evaluacion.Nivel = modelo.sesion.Evaluacion.Nivel;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardó su carga de objetivos " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap);
                    //enviar Correo

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(1);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);

                    MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de la carga de sus objetivos a:" + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó a su jefe para su evaluación");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo Guardar su carga de objetivos " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }
            }
            else if (submitButton == "RechazarObjetivos")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 2;
                envio.Evaluacion.MotivoRechazoObj = modelo.sesion.Evaluacion.MotivoRechazoObj;
                envio.Evaluacion.Rechazos = envio.Evaluacion.Rechazos + 1; /*Cantidad de rechazos */
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    //ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Rechazo los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(3);
                    correo.Mensaje = correo.Mensaje.Replace("#Motivo#", modelo.sesion.Evaluacion.MotivoRechazoObj);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de su rechazo de objetivos a:" + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo Rechazar los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "AceptarObjetivos")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);
                if (envio.Evaluacion.periodo_etapa == 0)//Carga de Objetivos
                    envio.Evaluacion.Status = 3;
                else if (envio.Evaluacion.periodo_etapa == 1)//Evaluacion mitad de año
                    envio.Evaluacion.Status = 4;
                //envio.Evaluacion.MotivoRechazoObj = modelo.Sesion.Evaluacion.MotivoRechazoObj;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Acepto los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(2);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de su aprobación de objetivos a:" + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo aprobar los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "EnviarAutoeval")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 5;
                envio.Evaluacion.ComEvaluado = modelo.sesion.Evaluacion.ComEvaluado;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su autoevaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(5);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su autoevaliación a: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", " Tus objetivos han sido autoevaluados. \r\nRecuerda dar seguimiento a esta fase del Proceso de Evaluación de Desempeño ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo enviar su autoevaluación" + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "RechazarObjetivosMitad")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 4;
                envio.Evaluacion.MotivoRechazoObj2 = modelo.sesion.Evaluacion.MotivoRechazoObj2;
                envio.Evaluacion.RechazosMitad = envio.Evaluacion.RechazosMitad + 1; /*Cantidad de rechazos */
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Rechazo los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(42);
                    correo.Mensaje = correo.Mensaje.Replace("#Motivo#", envio.Evaluacion.MotivoRechazoObj2);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de su rechazo de objetivos a:" + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo Rechazar los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "EnviarAutoevalJefe")//se cierra la evaluacion ya que se elimina la calificación
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);
                if (envio.Evaluacion.periodo_etapa == 1)
                    envio.Evaluacion.Status = 8;
                else if (envio.Evaluacion.periodo_etapa == 3)
                    envio.Evaluacion.Status = 10;
                envio.Evaluacion.ComEvaluador = modelo.sesion.Evaluacion.ComEvaluador;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su Calificación final de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(9);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);

                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su calificacion final a " + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "En esta fase deberás calificar cada uno de los objetivos de tu colaborador con base en el cumplimiento de estos y agregar comentarios de retroalimentación. ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo enviar su calificacion final " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "EnvioSuperior")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);//usuario

                envio.Evaluacion.Status = 7;
                envio.Evaluacion.CaliFinal = modelo.sesion.Evaluacion.CaliFinal;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su evaluacion y envio a superior " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ELogin jefeNivel2 = Utilidades.negocio.RecuperaUnUsuarioSap(jefe.EvaluadorIdSap);

                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(8);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);
                    MandarCorreo.SendMail("Soporte", jefeNivel2.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su verificación a " + jefeNivel2.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo enviar su evaluación a superior" + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "CerrarEval")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 8;
                envio.Evaluacion.CaliFinal = modelo.sesion.Evaluacion.CaliFinal;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su Calificación final de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(9);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);

                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su calificacion final a " + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo enviar su calificacion final " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "RechazarCalificacion")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 6;
                envio.Evaluacion.ComentariosSegundoNivel = modelo.sesion.Evaluacion.ComentariosSegundoNivel;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " rechazo su Calificación de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(4);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                   
                    MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su rechazo de calificacion a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo rechazar la calificación de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "AceptarCalificacion")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 8;
                envio.Evaluacion.ComentariosSegundoNivel = modelo.sesion.Evaluacion.ComentariosSegundoNivel;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Acepto la calificacion de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(9);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);                    
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su confirmación de su calificacion a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo aceptar la calificación de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "Calibracion")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 9;
                envio.Evaluacion.MotivoCalibracion = modelo.sesion.Evaluacion.MotivoCalibracion;
                envio.Evaluacion.CaliFinal = modelo.sesion.Evaluacion.CaliFinal;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " calibro la calificacion de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(18);
                    
                    correo.Mensaje = correo.Mensaje.Replace("#Motivo#", modelo.sesion.Evaluacion.MotivoCalibracion);
                    correo.Mensaje = correo.Mensaje.Replace("#Calif#", modelo.sesion.Evaluacion.CaliFinal.ToString());
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);

                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su calibracion a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo calobrar a " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "EnviarAutoeval2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 11;
                envio.Evaluacion.ComEvaluado2 = modelo.sesion.Evaluacion.ComEvaluado2;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su autoevaluacion2 " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(41);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notificó de su autoevaluación de segundo semestre a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Tus objetivos han sido autoevaluados. Recuerda dar seguimiento a esta fase del Proceso de Evaluación de Desempeño");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo enviar autoevaluación de segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "RechazarEvalFinal")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 10;
                envio.Evaluacion.MotivoRechazoObjFin = modelo.sesion.Evaluacion.MotivoRechazoObjFin;
                envio.Evaluacion.RechazosFin = envio.Evaluacion.RechazosFin + 1; /*Cantidad de rechazos */
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    //ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Rechazo los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(42);
                    correo.Mensaje = correo.Mensaje.Replace("#Motivo#", modelo.sesion.Evaluacion.MotivoRechazoObjFin);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de su rechazo de objetivos a:" + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo Rechazar los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "EnviarAutoevalJefe2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);
                modelo.Liobjetivos = Utilidades.negocio.RecuperaListaObjetivos(modelo.sesion.Evaluacion.id);
                modelo.Escaleta = Utilidades.negocio.RecuperaEscaleta();
                envio.Evaluacion.Status = 13;
                envio.Evaluacion.ComEvaluador2 = modelo.sesion.Evaluacion.ComEvaluador2;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                var suma_ponderados = modelo.Liobjetivos.Sum(t => t.ValorObj);

                int finalScore = Convert.ToInt32(Math.Round(suma_ponderados, MidpointRounding.AwayFromZero));
                if (finalScore < 1) finalScore = 1;
                if (finalScore > 5) finalScore = 5;
                envio.Evaluacion.CaliFinal2 = finalScore;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su evaluacion de segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    //ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    //ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(17);
                    //CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    //correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);

                    ////MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    //MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    //Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notificó de su evaluación a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    //Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notificó de su evaluación a " + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente, no se notificó al evaluado");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo evaluar a " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "EnvioSuperior2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 13;
                envio.Evaluacion.CaliFinal2 = modelo.sesion.Evaluacion.CaliFinal2;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su Envio a superior2 " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ELogin jefeNivel2 = Utilidades.negocio.RecuperaUnUsuarioSap(jefe.EvaluadorIdSap);

                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(8);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", jefeNivel2.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su evaluación a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo enviar a superior en segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "CerrarEval2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);
                modelo.Liobjetivos = Utilidades.negocio.RecuperaListaObjetivos(modelo.sesion.Evaluacion.id);
                modelo.Escaleta = Utilidades.negocio.RecuperaEscaleta();
                envio.Evaluacion.Status = 14;
                envio.Evaluacion.ComEvaluador2 = modelo.sesion.Evaluacion.ComEvaluador2;
                //envio.Evaluacion.CaliFinal2 = modelo.sesion.Evaluacion.CaliFinal2;
                var suma_ponderados = modelo.Liobjetivos.Sum(t => t.ValorObj);

                int finalScore = Convert.ToInt32(Math.Round(suma_ponderados, MidpointRounding.AwayFromZero));
                if (finalScore < 1) finalScore = 1;
                if (finalScore > 5) finalScore = 5;
                envio.Evaluacion.CaliFinal2 = finalScore;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " guardo su calificacion final2 " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(9);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);                

                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su evaluación2 a " + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo evaluar en segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "RechazarCalificacion2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);



                envio.Evaluacion.Status = 12;
                envio.Evaluacion.ComentariosSegundoNivel2 = modelo.sesion.Evaluacion.ComentariosSegundoNivel2;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " rechazó la calificacion de " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(6);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);                

                    MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su evaluación a " + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo rechazar calificación. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "AceptarCalificacion2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 14;
                envio.Evaluacion.ComentariosSegundoNivel2 = modelo.sesion.Evaluacion.ComentariosSegundoNivel2;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Aceptó la calificacion en segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(9);
                 
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notificó de su calificación a " + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    
                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo aceptar calificación en segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "Calibracion2")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 15;
                envio.Evaluacion.MotivoCalibracion2 = modelo.sesion.Evaluacion.MotivoCalibracion2;
                envio.Evaluacion.CaliFinalCalibracion = modelo.sesion.Evaluacion.CaliFinal2;

                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " realizó la calibración en segundo semestre. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(envio.Login.EvaluadorIdSap);
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(18);
                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();

                    //correo.Mensaje = correo.Mensaje.Replace("#Motivo#", modelo.sesion.Evaluacion.MotivoCalibracion2);
                    correo.Mensaje = correo.Mensaje.Replace("#Calif#", modelo.sesion.Evaluacion.CaliFinal2.ToString());
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);                   
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " notifico de su calibracion en segundo semestre a " + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo calibrar. Evaluación: " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            if (submitButton == "CagraObjetivosEx")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(sesion.Login.id);

                envio.Evaluacion.Status = 17;
                envio.Evaluacion.Nivel = modelo.sesion.Evaluacion.Nivel;
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Guardo su carga de objetivos " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap);
                    //enviar Correo

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(1);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);

                    MandarCorreo.SendMail("Soporte", jefe.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de la carga de sus objetivos a:" + jefe.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó a su jefe para su evaluación");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo Guardar su carga de objetivos " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }
            }
            else if (submitButton == "RechazarObjetivosEx")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);

                envio.Evaluacion.Status = 16;
                envio.Evaluacion.MotivoRechazoObj = modelo.sesion.Evaluacion.MotivoRechazoObj;
                envio.Evaluacion.Rechazos = envio.Evaluacion.Rechazos + 1; /*Cantidad de rechazos */
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    //ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Rechazo los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(3);
                    correo.Mensaje = correo.Mensaje.Replace("#Motivo#", modelo.sesion.Evaluacion.MotivoRechazoObj);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, sesion.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de su rechazo de objetivos a:" + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo Rechazar los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }
            else if (submitButton == "AceptarObjetivosEx")
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Modelo.Clases.CSessionEval envio = Utilidades.negocio.RecuperaUsuarioEval(modelo.sesion.Login.id);
                EPeriodos PeriodoAux = Utilidades.negocio.RecuperaUnPeriodo(envio.Evaluacion.periodo);

                #region logicaExtemporaneos
                if (PeriodoAux.Etapa == 0)//verificamos la etapa del periodo
                {
                    if (PeriodoAux.StartDateObj <= DateTime.Now.Ticks && PeriodoAux.FinishDateObj >= DateTime.Now.Ticks)//evaluacion carga de objetivos
                    {
                        envio.Evaluacion.Status = 4;
                    }
                    else //Calibración
                    {
                        envio.Evaluacion.Status = 3;
                    }
                }
                else if (PeriodoAux.Etapa == 1)//verificamos la etapa del periodo
                {
                    if (PeriodoAux.StartDateEva <= DateTime.Now.Ticks && PeriodoAux.FinishDateEva >= DateTime.Now.Ticks)//evaluacion medio año
                    {
                        envio.Evaluacion.Status = 4;
                    }
                    else //Calibración
                    {
                        envio.Evaluacion.Status = 9;
                    }
                }
                else if (PeriodoAux.Etapa == 2)
                {
                    if (PeriodoAux.StartDateCali <= DateTime.Now.Ticks && PeriodoAux.FinishDateCali >= DateTime.Now.Ticks)
                    {
                        envio.Evaluacion.Status = 9; //calibración 
                    }
                    else
                    {
                        envio.Evaluacion.Status = 10;//evaluacion final
                    }
                }
                else if (PeriodoAux.Etapa == 3)//evaluacion final
                {
                    if (PeriodoAux.StartDateEva2 <= DateTime.Now.Ticks && PeriodoAux.FinishDateEva2 >= DateTime.Now.Ticks)
                    {
                        envio.Evaluacion.Status = 10; //calibración 
                    }
                    else
                    {
                        envio.Evaluacion.Status = 14;//evaluacion final
                    }
                }
                else
                {
                    envio.Evaluacion.Status = 14;
                }
                //Verificar el estado del Periodo
                #endregion
                envio.Evaluacion.MotivoRechazoObj = "";
                envio.Evaluacion.Modificadopor = sesion.Login.NombreCompleto;
                if (Utilidades.negocio.GuardaEvaluacion(envio.Evaluacion))
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Acepto los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");

                    CapaLogica.Correo.EnvioCorreo MandarCorreo = new CapaLogica.Correo.EnvioCorreo();
                    ECorreos correo = Utilidades.negocio.RecuperaUnCorreo(2);
                    correo.Mensaje = MandarCorreo.ProcesarMsg(correo.Mensaje, envio.Login);
                    MandarCorreo.SendMail("Soporte", envio.Login.Email, " ", correo.Mensaje + " <br><br><br> saludos<br>Atentamente Sistema de Evaluación de Desempeño  <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Notifico de su aprobación de objetivos a:" + envio.Login.Email, "MisEvaluaciones/GuardaClaseEvaluacion ");


                    return PartialView("Respuesta", "Se guardó correctamente y se notificó al evaluado ");
                }
                else
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo aprobar los objetivos de la evaluación " + modelo.sesion.Evaluacion.id, "MisEvaluaciones/GuardaClaseEvaluacion ");
                    return PartialView("Respuesta", "Ocurrió un error verifique los datos");
                }

            }


            return PartialView("Respuesta", "Ocurrió un error verifique los datos");
        }

        [Autentificado]
        public ActionResult RecargaObjetivos(int Eval, int status, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Actualizó la vista de objetivos en la evaluación " + Eval.ToString(), "Gestion/RecargaObjetivos ");

            List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Eval);
            EEval evaluacion = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
            EPeriodos periodo = null;
            if (evaluacion != null)
            {
                periodo = Utilidades.negocio.RecuperaUnPeriodo(evaluacion.periodo);
            }


            string panel1 = "";
            string panel2 = "";
            if (periodo != null)
            {
                if (periodo.Etapa <= 2)
                {
                    panel1 = "in active";
                }

                if (periodo.Etapa > 2)
                {
                    panel2 = "in active";
                }
            }
            else
            {
                panel1 = "in active";
            }

            ViewBag.panel1 = panel1;
            ViewBag.panel2 = panel2;

            
            ViewBag.status = status;
            ViewBag.usuario = Usr;
            ViewBag.StatusPer = periodo.Etapa;
            ViewBag.Eval = Eval;
            ViewBag.nObjetivos = liObjetivos.Count();
            ViewBag.PesoTotal = liObjetivos.Sum(t => t.ponderado);
            var sum_ponderados = liObjetivos.Sum(t => t.ValorObj);

            int finalScore = Convert.ToInt32(Math.Round(sum_ponderados, MidpointRounding.AwayFromZero));
            if (finalScore < 1) finalScore = 1;
            if (finalScore > 5) finalScore = 5;
            ViewBag.CalFinalTemp = finalScore;

            return PartialView(liObjetivos);
        }

        [Autentificado]
        public ActionResult RecargaCompetencias(int Eval, int status, int Usr)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Actualizó las competencias en la evaluación " + Eval.ToString(), "Gestion/RecargaCompetencias ");
            EEval evaluacion = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
            List<ECompetemces> liCompetencias = Utilidades.negocio.RecuperaLiCompetencias(Eval);
            EPeriodos periodo = null;
            if (evaluacion != null)
            {
                periodo = Utilidades.negocio.RecuperaUnPeriodo(evaluacion.periodo);
            }

            if (liCompetencias != null)
            {
                foreach (ECompetemces item in liCompetencias)
                {
                    ECatComp temp = Utilidades.negocio.RecuperaUnaCatCompetencias(item.titulo);
                    ECatSubComp subComp = new ECatSubComp();
                    if (item.Subtitulo != 0)
                        subComp = Utilidades.negocio.RecuperaCatSubCompetencia(item.Subtitulo);
                    subComp.SubCompetencia = subComp.SubCompetencia != null ? subComp.SubCompetencia : string.Empty;
                    item.tituloTemp = $"{temp.descripcion} | {subComp.SubCompetencia}";
                    //item.tituloTemp = temp.descripcion;
                }
            }
            ViewBag.status = status;
            ViewBag.usuario = Usr;
            ViewBag.StatusPer = periodo.Etapa;
            ViewBag.Eval = Eval;
            return PartialView(liCompetencias);
        }

        [Autentificado]
        public ActionResult EliminaUnaCompetencia(int id)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a eliminar objetivos", "Gestion/EliminaUnaCompetencia ");

            if (Utilidades.negocio.EliminaUnaCompetencia(id))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Elimino el objetivo "+id, "Gestion/EliminaUnaCompetencia ");
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo eliminar el objetivo " + id, "Gestion/EliminaUnaCompetencia ");
                return PartialView("Respuesta", "ocurrió un error");
            }
        }

        [Autentificado]
        public ActionResult EliminarCompetenciasUsr(int id, int Stat)
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " ingreso a eliminar objetivos ", "MisEvaluaciones/EliminarObjetivosUsr ");
            
            if (Utilidades.negocio.EliminaUnaCompetencia(id))
            {
                return PartialView("Respuesta", "se guardó correctamente");
            }
            else
            {
                return PartialView("Respuesta", "ocurrió un error");
            }
            /*Modelo.ECompetemces model = new ECompetemces();
            if (id > 0)
            {
                model = Utilidades.negocio.RecuperaUnaCompetencia(id);
                model.resultados = "";

                if (Stat == 4)
                {
                    model.Etapa = 5;
                }
                else
                {
                    model.Etapa = 10;
                }

            }
            if (Utilidades.negocio.GuardaCompetencias(model))
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + "elimino el objetivo " + model.id, "MisEvaluaciones/EliminarObjetivosUsr ");
                return PartialView("Respuesta", "se guardo correctamente");
            }
            else
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo eliminiar el objetivo " + model.id, "MisEvaluaciones/EliminarObjetivosUsr ");
                return PartialView("Respuesta", "ocurrio un error");
            }*/
        }


        public ActionResult DetalleEscaleta()
        {
            List<EEscaleta> escaleta = Utilidades.negocio.RecuperaEscaleta();
            return PartialView(escaleta);
        }

        public ActionResult DetalleObj(int eval)
        {
            List<EObjetives> listObj = Utilidades.negocio.RecuperaListaObjetivos(eval);

            return PartialView(listObj);
        }
        
        public ActionResult DownloadPdf(int Id)
        {
            string path = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "DocTest.docx");
            string pathResp = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");
            string pathpdf = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.pdf");
            //string pathLicencia = Path.Combine(Server.MapPath("~/bin/"), "license.lic");
          
            try
            {
                Modelo.Clases.CSession session = Session[Utilidades.session] as Modelo.Clases.CSession;
                Spire.Doc.Document document = new Spire.Doc.Document();
                Modelo.Clases.CSessionEval Evaluado = Utilidades.negocio.RecuperaUsuarioEval(Id);
                ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(Evaluado.Login.EvaluadorIdSap);
                EPeriodos perido = Utilidades.negocio.RecuperaPeriodopais(Evaluado.Login.Pais);
                List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Evaluado.Evaluacion.id);
                List<ECompetemces> liCompetencias = Utilidades.negocio.RecuperaLiCompetencias(Evaluado.Evaluacion.id);

                System.IO.File.Copy(path, pathResp, true);//copiamos el documento original al temporal
                object fileName = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");//obtenemos la ruta del archivo copiado
                document.LoadFromFile(fileName.ToString());//se carga el documento que se le hara el replace


                document.Replace("#SapEmpleado#", Evaluado.Login.id_sap, true, true);
                document.Replace("#NombreEmp#",String.Concat(Evaluado.Login.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", perido.Llave, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                //document.Replace("#divisionEmp#", Evaluado.Login.Division.Descripcion, true, true);
                //document.Replace("#areaEmp#", Evaluado.login.Area, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#FchEmpini#", TiksToDate(Evaluado.Login.FechaIngreso), true, true);
                document.Replace("#fchEmpAnt#", new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(TiksToDate(Evaluado.Login.FechaAntiguedad))), true, true);

                document.Replace("#SapJefe#", jefe.id_sap, true, true);
                document.Replace("#Nombrejefe#",String.Concat(jefe.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", perido.Llave, true, true);
                document.Replace("#puestoJefe#", jefe.Puesto, true, true);
                //document.Replace("#divisionjefe#", jefe.Division.Descripcion, true, true);
                //document.Replace("#areaJefe#", jefe.Area, true, true);
                document.Replace("#FchJefeini#", jefe.Puesto, true, true);
                document.Replace("#FchJefeAnt#", TiksToDate(jefe.FechaIngreso), true, true);
                
                #region Objetivos
                if (liObjetivos != null)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = bodyObjetivos.Paragraphs.IndexOf(paragraphObjetivos);

                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);

                    
                    String[] Header = { "Objetivo", "Descripción de Objetivo", "Métricas", "Auto evaluación (1-4)", "Calificación Evaluador (1-4)", "Calificación final acordada (1-4)", "Auto evaluación (1-4)", "Calificación Evaluador (1-4)", "Calificación final acordada (1-4)"};
                    string[][] data = new string[liObjetivos.Count][];
                    tableObjetivos.ResetCells(liObjetivos.Count + 1, 9);

                    Spire.Doc.TableRow FRow = tableObjetivos.Rows[0];
                    FRow.IsHeader = true;
                    FRow.Height = 25;
                    
                   
                    for (int h = 0; h < Header.Length; h++)
                    {
                        //Cell Alignment
                        FRow.Cells[h].Width = 25F;
                        Spire.Doc.Documents.Paragraph p = FRow.Cells[h].AddParagraph();
                        FRow.Cells[h].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Top;
                        FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.FromName("#3B20C5");
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);                       
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor= System.Drawing.Color.White;
                        TR.CharacterFormat.FontSize = 10;
                        TR.CharacterFormat.Bold = true;
                        FRow.RowFormat.BackColor = System.Drawing.Color.DeepSkyBlue;
                    }
                    int i = 0;                    
                    foreach (EObjetives objetivo in liObjetivos)
                    {
                        data[i] = new string[9];
                        data[i][0] = objetivo.titulo;
                        data[i][1] = objetivo.objdesc;
                        data[i][2] = objetivo.objMetricas;
                        //data[i][3] = objetivo.autoeval.ToString()+" "+ objetivo.comentariosEvaluado;
                        //data[i][4] = objetivo.calif.ToString() + " " + objetivo.comentariosEvaluador;
                        //data[i][5] = objetivo.califFinal.ToString() + " " + objetivo.comentariosEvaluador2;
                        //data[i][6] = objetivo.AutoEval2.ToString() + " " + objetivo.comentariosEvaluado2;
                        data[i][7] = objetivo.Calif2.ToString() + " " + objetivo.ComentariosJefeEval2;
                        //data[i][8] = objetivo.CalifFinal2.ToString() + " " + objetivo.comentariosEvaluador2;
                        i = i + 1;
                    }

                
                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r+1];
                        DataRow.Height = 20;
                       
                        for (int c = 0; c < data[r].Length; c++)
                        {                           
                            DataRow.Cells[c].Width = 65F;
                            DataRow.Cells[c].CellFormat.Borders.Color= System.Drawing.Color.Aqua;                   
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromName("#004b8d");
                            TR2.CharacterFormat.Border.Color = System.Drawing.Color.Aqua;                            
                        }
                    }
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.Aqua;
                    //bodyObjetivos.ChildObjects.Remove(paragraphObjetivos);
                    document.Replace("#Objetivos#", "", true, true);
                    bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#Objetivos#", "", true, true);
                }
                #endregion

                #region competencias
                if (liCompetencias != null)
                {

                    Spire.Doc.Section sectionCompetencias = document.Sections[4];
                    Spire.Doc.Documents.TextSelection selectionCompetencias = document.FindString("#XXCompetenciasXX#", true, true);
                    Spire.Doc.Fields.TextRange rangeCompetencias = selectionCompetencias.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphCompetencias = rangeCompetencias.OwnerParagraph;
                    Spire.Doc.Body bodyCompetencias = paragraphCompetencias.OwnerTextBody;
                    int index = bodyCompetencias.Paragraphs.IndexOf(paragraphCompetencias);

                    Spire.Doc.Table tableCompetencias = sectionCompetencias.AddTable(true);

                    String[] Header = { "Competencias", "Actividades", "Resultados"};
                    string[][] data = new string[liCompetencias.Count][];
                    tableCompetencias.ResetCells(liCompetencias.Count + 1, 3);

                    Spire.Doc.TableRow FRow = tableCompetencias.Rows[0];
                    FRow.IsHeader = true;
                    FRow.Height = 20;
                    FRow.Cells[0].SetCellWidth(55, Spire.Doc.CellWidthType.Percentage);
                    FRow.Cells[1].SetCellWidth(55, Spire.Doc.CellWidthType.Percentage);
                    FRow.Cells[2].SetCellWidth(55, Spire.Doc.CellWidthType.Percentage);


                    for (int h = 0; h < Header.Length; h++)
                    {
                        
                        
                        Spire.Doc.Documents.Paragraph p = FRow.Cells[h].AddParagraph();
                        FRow.Cells[h].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Top;
                        FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.FromName("#3B20C5");
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        FRow.RowFormat.BackColor = System.Drawing.Color.DeepSkyBlue;
                    }


                    int i = 0;
                   
                    foreach (ECompetemces Competencia in liCompetencias)
                    {
                        ECatComp temp = Utilidades.negocio.RecuperaUnaCatCompetencias(Competencia.titulo);
                        ECatSubComp subComp = new ECatSubComp();
                        if (Competencia.Subtitulo != 0)
                            subComp = Utilidades.negocio.RecuperaCatSubCompetencia(Competencia.Subtitulo);
                        subComp.SubCompetencia = subComp.SubCompetencia != null ? subComp.SubCompetencia : string.Empty;
                        Competencia.tituloTemp = $"{temp.descripcion} | {subComp.SubCompetencia}";
                        //Competencia.tituloTemp = temp.descripcion;
                        data[i] = new string[3];
                        data[i][0] = Competencia.tituloTemp;
                        data[i][1] = Competencia.actividades;
                        data[i][2] = Competencia.resultados;
                        i = i + 1;
                    }


                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableCompetencias.Rows[r+1];

                        for (int c = 0; c < data[r].Length; c++)
                        {
                            tableCompetencias.ColumnWidth[c] =250f;

                            //Cell Alignment
                            DataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                            DataRow.Cells[c].CellFormat.FitText = false; //llenado de la tabla
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.Aqua;
                            //DataRow.Cells[c].CellFormat.BackColor = System.Drawing.Color.SkyBlue;
                            //Fill Data in Rows
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.DarkBlue;
                            DataRow.Cells[c].CellFormat.BackColor = System.Drawing.Color.LightCyan;

                        }
                    }
                    tableCompetencias.TableFormat.Borders.Color = System.Drawing.Color.Aqua;
                    //bodyCompetencias.ChildObjects.Remove(paragraphCompetencias);                    
                    bodyCompetencias.ChildObjects.Insert(index, tableCompetencias);
                    document.Replace("#XXCompetenciasXX#", "", true, true);//ojo poner siempre despues de llenar las tablas
                }
                else
                {
                    document.Replace("#XXCompetenciasXX#", "", true, true);
                }
                #endregion

                #region Calificacion
                if (Evaluado.Evaluacion.CaliFinal > 0)
                {
                    if (Evaluado.Evaluacion.CaliFinal == 4)
                    {
                        document.Replace("#cuatro1#", "4", true, true);
                    }
                    else
                    {
                        document.Replace("#cuatro1#", "", true, true);
                    }
                    if (Evaluado.Evaluacion.CaliFinal == 3)
                    {
                        document.Replace("#tres1#", "3", true, true);
                    }
                    else
                    {
                        document.Replace("#tres1#", "", true, true);
                    }
                    if (Evaluado.Evaluacion.CaliFinal == 2)
                    {
                        document.Replace("#dos1#", "2", true, true);
                    }
                    else
                    {
                        document.Replace("#dos1#", "", true, true);
                    }
                    if (Evaluado.Evaluacion.CaliFinal == 1)
                    {
                        document.Replace("#uno1#", "1", true, true);
                    }
                    else
                    {
                        document.Replace("#uno1#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro1#", "", true, true);
                    document.Replace("#tres1#", "", true, true);
                    document.Replace("#dos1#", "", true, true);
                    document.Replace("#uno1#", "", true, true);
                }

                if (Evaluado.Evaluacion.CaliFinal2 > 0)
                {
                    if (Evaluado.Evaluacion.CaliFinal2 == 4)
                    {
                        document.Replace("#cuatro2#", "4", true, true);
                    }
                    else
                    {
                        document.Replace("#cuatro2#", "", true, true);
                    }
                    if (Evaluado.Evaluacion.CaliFinal2 == 3)
                    {
                        document.Replace("#tres2#", "3", true, true);
                    }
                    else
                    {
                        document.Replace("#tres2#", "", true, true);
                    }
                    if (Evaluado.Evaluacion.CaliFinal2 == 2)
                    {
                        document.Replace("#dos2#", "2", true, true);
                    }
                    else
                    {
                        document.Replace("#dos2#", "", true, true);
                    }
                    if (Evaluado.Evaluacion.CaliFinal2 == 1)
                    {
                        document.Replace("#uno2#", "1", true, true);
                    }
                    else
                    {
                        document.Replace("#uno2#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro2#", "", true, true);
                    document.Replace("#tres2#", "", true, true);
                    document.Replace("#dos2#", "", true, true);
                    document.Replace("#uno2#", "", true, true);
                }
                #endregion

                document.Replace("#ComentariosEval#", Evaluado.Evaluacion.ComEvaluado, true, true);
                document.Replace("#Comentariosjefe#", Evaluado.Evaluacion.ComEvaluador, true, true);
                document.Replace("#ComentariosEval2#", Evaluado.Evaluacion.ComEvaluado2, true, true);
                document.Replace("#ComentariosJefe2#", Evaluado.Evaluacion.ComEvaluador2, true, true);

                document.SaveToFile(pathResp.ToString(), Spire.Doc.FileFormat.PDF);

                //esto es para regresarlo al navegador
                byte[] fileBytes = System.IO.File.ReadAllBytes(pathResp);
                string fileNameResp = "Reporte.pdf";
                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " descargo su evaluacion " + Evaluado.Evaluacion.id, "Gestion/EliminaUnaCompetencia ");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameResp);
            }
            catch (Exception ex)
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " no pudo descargar su evaluación " + Id, "Gestion/EliminaUnaCompetencia ");
                return PartialView("Respuesta", ex.ToString());
            }
            finally
            {
                if (System.IO.File.Exists(pathResp))
                { //eliminar para evitar duplicidades
                    System.IO.File.Delete(pathResp);
                }
            }
        }

        //nandarek
        [Autentificado]
        public ActionResult Evaluacion(int id)
        {

            //usuario
            //0-> usuario Propietario
            //1-> usuario Jefe
            //2-> usuario jefe lv2
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a MiEvaluacion", "MisEvaluaciones/MiEvaluacion ");

            Modelo.Clases.CEvaluacion eval = new Modelo.Clases.CEvaluacion();
            eval.sesion = Utilidades.negocio.RecuperaUsuarioEval(id);

            /*if (id == 0)
            {
                eval.Sesion = Utilidades.negocio.RecuperaUsuarioEval(sesion.Login.id);
            }
            else
            {
                eval.Sesion = Utilidades.negocio.RecuperaUsuarioEval(id);
            }*/

            if (eval.sesion.Evaluacion == null)
            {
                Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso sin tener evaluación ", "MisEvaluaciones/MiEvaluacion ");
                return PartialView("Respuesta", "No se tiene una evaluación asignada actualmente");
            }
            else
            {
                //eval.sesion.Login.FechaIngresoTemp = TiksToDate(eval.sesion.Login.FechaIngreso);
                //eval.sesion.Login.FechaAntiguedadTemp = eval.sesion.Login.FechaAntiguedadTemp = new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(eval.sesion.Login.FechaIngresoTemp));
                eval.Jefedir = Utilidades.negocio.RecuperaUnUsuarioSap(eval.sesion.Login.EvaluadorIdSap);
                if (eval.Jefedir == null)
                {
                    Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso sin información del jefe ", "MisEvaluaciones/MiEvaluacion ");
                    return PartialView("Respuesta", "Sin información");
                }

                if (eval.Jefedir.EvaluadorIdSap == null || eval.Jefedir.EvaluadorIdSap == "")
                {
                    eval.EvalL2 = false;
                }
                else
                {
                    eval.EvalL2 = true;
                }

                eval.periodo = Utilidades.negocio.RecuperaPeriodopais(eval.sesion.Login.Pais);
                eval.Liobjetivos = Utilidades.negocio.RecuperaListaObjetivos(eval.sesion.Evaluacion.id);
                eval.ListaCompetencias = Utilidades.negocio.RecuperaLiCompetencias(eval.sesion.Evaluacion.id);

                if (eval.ListaCompetencias != null)
                {
                    foreach (ECompetemces item in eval.ListaCompetencias)
                    {
                        ECatComp temp = Utilidades.negocio.RecuperaUnaCatCompetencias(item.titulo);
                        ECatSubComp subComp = new ECatSubComp();
                        if (item.Subtitulo != 0)
                            subComp = Utilidades.negocio.RecuperaCatSubCompetencia(item.Subtitulo);
                        subComp.SubCompetencia = subComp.SubCompetencia != null ? subComp.SubCompetencia : string.Empty;
                        item.tituloTemp = $"{temp.descripcion} | {subComp.SubCompetencia}";
                        //item.tituloTemp = temp.descripcion;
                    }
                }

                //eval.Jefedir.FechaIngresoTemp = TiksToDate(eval.Jefedir.FechaIngreso);
                //eval.Jefedir.FechaAntiguedadTemp = eval.sesion.Login.FechaAntiguedadTemp = new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(eval.Jefedir.FechaIngresoTemp));
                if (eval.periodo != null)
                {

                    //objetivos
                    eval.periodo.StartDateObjTemp = TiksToDate(eval.periodo.StartDateObj);
                    eval.periodo.FinishDateObjTemp = TiksToDate(eval.periodo.FinishDateObj);
                    //objetivos

                    //evaluacion 1er periodo
                    eval.periodo.StartDateEvaTemp = TiksToDate(eval.periodo.StartDateEva);
                    eval.periodo.FinishDateEvaTemp = TiksToDate(eval.periodo.FinishDateEva);
                    //evaluacion 1er periodo

                    //Calibracion 1er periodo
                    eval.periodo.StartDateCaliTemp = TiksToDate(eval.periodo.StartDateCali);
                    eval.periodo.FinishDateCaliTemp = TiksToDate(eval.periodo.FinishDateCali);
                    //Calibracion 1er periodo

                    //Calibracion 1er periodo
                    eval.periodo.StartDateCaliTemp = TiksToDate(eval.periodo.StartDateCali);
                    eval.periodo.FinishDateCaliTemp = TiksToDate(eval.periodo.FinishDateCali);
                    //Calibracion 1er periodo

                    //Calibracion 2do periodo   StartDateEva2
                    eval.periodo.StartDateEva2Temp = TiksToDate(eval.periodo.StartDateEva2);
                    eval.periodo.FinishDateEva2Temp = TiksToDate(eval.periodo.FinishDateEva2);
                    //Calibracion 1er periodo

                    //Calibracion 1er periodo
                    eval.periodo.StartDateCali2Temp = TiksToDate(eval.periodo.StartDateCali2);
                    eval.periodo.FinishDateCali2Temp = TiksToDate(eval.periodo.FinishDateCali2);
                    //Calibracion 1er periodo

                    EPais temp = Utilidades.negocio.RecuperaUnPais(eval.periodo.Country);
                    eval.periodo.PaisTempral = temp.descripcion;

                }
                string panel1 = "";
                string panel2 = "";
                if (eval.periodo != null)
                {
                    if (eval.periodo.Etapa <= 2)
                    {
                        panel1 = "in active";
                    }

                    if (eval.periodo.Etapa > 2)
                    {
                        panel2 = "in active";
                    }
                }
                else
                {
                    panel1 = "in active";
                }

                EStatus statusEval = Utilidades.negocio.GetStatusById(eval.sesion.Evaluacion.Status);

                ViewBag.statusName = statusEval.Name;
                ViewBag.statusDesc = statusEval.Descript;
                ViewBag.porc = statusEval.Porc;
                ViewBag.panel1 = panel1;
                ViewBag.panel2 = panel2;

                if (eval.sesion.Evaluacion.Status >= 11)
                {
                    eval.Escaleta = Utilidades.negocio.RecuperaEscaleta();
                    double sum_ponderado = eval.Liobjetivos.Sum(t => t.ValorObj);
                    ViewBag.sum_ponderado = sum_ponderado;

                    double finalScore = sum_ponderado;
                    if (finalScore < 1) finalScore = 1;
                    if (finalScore > 5) finalScore = 5;

                    ViewBag.CalFinTemp = new EEscaleta { Valor = Convert.ToInt32(Math.Round(finalScore, MidpointRounding.AwayFromZero)) };
                    ViewBag.CalFinalDecimal = finalScore;

                    var autoeval2 = eval.Liobjetivos.Sum(e => e.AutoEval2);
                    ViewBag.CalAutoEval = autoeval2;
                }
            }
            return PartialView(eval);
        }

        [Autentificado]
        public ActionResult CloseEval(FormCollection f)
        {
            string com = f["MiEval.Sesion.Evaluacion.ComEvaluador2"];

            //EEval eval = Utilidades.negocio.RecuperaUnaEaluacion(evalId);

            //eval.Status = 14;
            //eval.ComEvaluador2 = evalComment;

            //bool ok = Utilidades.negocio.GuardaEvaluacion(eval);

            string resp = "Sesion.Evaluacion.ComEvaluador2";
            if (true)
                resp = "Datos guardados correctamente";
            else
                resp = "Ha ocurrido un error. Por favor intente más tarde.";

            return PartialView("Respuesta", resp);
        }

        #region drops
        public List<SelectListItem> Calificaciones()
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();

            Respuesta.Add(new SelectListItem { Text = "1", Value = "1" });
            Respuesta.Add(new SelectListItem { Text = "2", Value = "2" });
            Respuesta.Add(new SelectListItem { Text = "3", Value = "3" });
            Respuesta.Add(new SelectListItem { Text = "4", Value = "4" });

            return Respuesta;
        }

        public List<SelectListItem> DropCatComp(int eval, int id)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<ECatComp> ListaCatCom = Utilidades.negocio.RecuperaCatCompetencias();
            List<ECompetemces> ListaCompetencias = Utilidades.negocio.RecuperaLiCompetencias(eval);

            if (ListaCompetencias != null)
            {
                ListaCompetencias = ListaCompetencias.OrderBy(x => x.tituloTemp).ToList();
            }
            
            foreach (ECatComp item in ListaCatCom)
            {
                int temp = 0;
                if (ListaCompetencias != null && item.id!=id)
                {
                    temp = (from ECompetemces in ListaCompetencias
                                where (ECompetemces.titulo == item.id) 
                            select ECompetemces).ToList().Count;
                }
                if (temp == 0)
                {
                    Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
                }
            }
            Respuesta = ((from item in Respuesta where item.Text.Contains("Individuales") select item).ToList()).Union
                        ((from item in Respuesta where item.Text.Contains("Gente") select item).ToList()).Union
                        ((from item in Respuesta where item.Text.Contains("Negocio") select item).ToList()).ToList();

            return Respuesta;
        }

        public List<ECatSubComp> SubCompsCat() {
            var Lista = Utilidades.negocio.RecuperaCatSubCompetencias();
            return Lista;
        }
        #endregion
        
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