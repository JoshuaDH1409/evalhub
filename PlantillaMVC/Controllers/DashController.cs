using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Modelo;
using CRUD;
using General;
using CRUD.Transaction;
using CapaLogica.Funciones;
using System.IO;
using ClosedXML.Excel;
using System.Web.Script.Serialization;
using Modelo.Clases;

namespace PlantillaMVC.Controllers
{
    public class DashController : Controller
    {
        List<CSessionEval> listSesion = new List<CSessionEval>();
        // GET: Dash
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult DashBoard()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];

            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a dash board", "DashBoard/DashBoard ");

            List<SelectListItem> ListaPaises = DropPais(sesion.Login.perfil, sesion.Login.Pais);
            ViewData["ListaPaises"] = ListaPaises;
            //List<SelectListItem> ListAreas = DropAreas(sesion.Login.Pais);
            //ViewData["ListaAreas"] = ListAreas;

            return PartialView();  //ver carga de datos
        }

        //detalle de la consulta Periodo
        public ActionResult _DetallePeriodo(string IdPais)
        {
            if (IdPais != "")
            {
                PlantillaMVC.Models.EFlitrosDash Modelo = new Models.EFlitrosDash();

                List<SelectListItem> ListaPeriodos = DropPeriodos(Convert.ToInt32(IdPais.Split(',')[0]));
                ViewData["ListaPeriodos"] = ListaPeriodos;

                return PartialView(Modelo);
            }           
            return null;                
        }
        //detalle de la consulta área
        public ActionResult _DetalleArea(int IdPeriodo)
        {
            
            if (IdPeriodo > 0)
            {
                PlantillaMVC.Models.EFlitrosDash Modelo = new Models.EFlitrosDash();

                EPeriodos PeriodoAux = Utilidades.negocio.RecuperaUnPeriodo(IdPeriodo); 
                List<SelectListItem> ListAreas = DropAreas(PeriodoAux.Country);
                ViewData["ListaAreas"] = ListAreas;
                ViewBag.PerActivo = PeriodoAux.Activo;
               return PartialView(Modelo);
            }
            return null;
        }


        public ActionResult Consultar(Models.EFlitrosDash filtros)
        {

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a dash board", "DashBoard/Consultar ");

            string resultado = "";
            int total;
            List<Modelo.Clases.CSessionEval> ListaEvalSession = new List<Modelo.Clases.CSessionEval>();
            var periodo = Utilidades.negocio.RecuperaUnPeriodo(filtros.Periodo);
            List<int> periodos = new List<int>();
            foreach(var p in filtros.Pais)
            {
                periodos.AddRange(Utilidades.negocio.RecuperaTodosPeriodosPais(p).Where(t=>t.Llave==periodo.Llave).Select(t=>t.id).ToList());
            }
            if (filtros.PeriodoActivo)
            {
                ListaEvalSession = Utilidades.negocio.RecuperaEvaluacionesPeriodoUsuarios(filtros.Periodo).Where(t=>t.Login.Activo).ToList();

                ListaEvalSession = (from item in ListaEvalSession
                                    where item.Login.Activo
                                    select item).ToList();

                if (filtros.Division == null)
                {
                    if (ListaEvalSession.Count > 0)
                    {
                        //EPeriodos periodo = Utilidades.negocio.RecuperaUnPeriodo(listaEval[0].periodo);

                        total = ListaEvalSession.Count();

                        int pendientes = ListaEvalSession.Count(x => x.Evaluacion.Status == 0);
                        int ObjCargados = ListaEvalSession.Count(x => x.Evaluacion.Status == 1);
                        int ObjRechazados = ListaEvalSession.Count(x => x.Evaluacion.Status == 2);
                        int ObjAprobados = ListaEvalSession.Count(x => x.Evaluacion.Status == 3);
                        int InEval = ListaEvalSession.Count(x => x.Evaluacion.Status == 4);
                        int AutoEval = ListaEvalSession.Count(x => x.Evaluacion.Status == 5);
                        int EvalJefe = ListaEvalSession.Count(x => x.Evaluacion.Status == 6);
                        int EvaLevD = ListaEvalSession.Count(x => x.Evaluacion.Status == 7);
                        int Cerrado = ListaEvalSession.Count(x => x.Evaluacion.Status == 8);
                        int IniEval2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 10);
                        int AutoEval2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 11);
                        int EvalJefe2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 12);
                        int EvaLevD2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 13);
                        int Cerrada = ListaEvalSession.Count(x => x.Evaluacion.Status == 14);
                        int Calibrada = ListaEvalSession.Count(x => x.Evaluacion.Status == 15);


                        resultado = "['Etapa', 'Cantidad ', { role: \"style\" }],";
                        resultado = resultado + "[\"Pendiente\"," + pendientes.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Objetivos cargados\"," + ObjCargados.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Objetivos rechazados\"," + ObjRechazados.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Objetivos aprobados\"," + ObjAprobados.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Inicio de evaluación\"," + InEval.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Comentario evaluado\"," + AutoEval.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Comentario jefe\"," + EvalJefe.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Evaluación de segundo nivel\"," + EvaLevD.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Calificación de medio año cerrada\"," + Cerrado.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Inicio de evaluación fin de año\"," + IniEval2.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Comentario evaluado fin de año\"," + AutoEval2.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Comentario jefe fin de año\"," + EvalJefe2.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Evaluación de segundo nivel fin de año\"," + EvaLevD2.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Calificación de medio año cerrada fin de año \"," + Cerrada.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Calibración de la evaluación\"," + Calibrada.ToString() + ", \"#727ABA\"],";
                    }
                    else
                    {
                        return PartialView("Respuesta", "No se cuenta con suficiente  información ");
                    }
                }
                else
                {
                    ListaEvalSession = (from aux in ListaEvalSession
                                        where aux.Login.Division == filtros.Division && aux.Login.Activo
                                        select aux).ToList();

                    if (ListaEvalSession.Count > 0)
                    {
                        //EPeriodos periodo = Utilidades.negocio.RecuperaUnPeriodo(listaEval[0].periodo);
                        total = ListaEvalSession.Count();
                        int pendientes = ListaEvalSession.Count(x => x.Evaluacion.Status == 0);
                        int ObjCargados = ListaEvalSession.Count(x => x.Evaluacion.Status == 1);
                        int ObjRechazados = ListaEvalSession.Count(x => x.Evaluacion.Status == 2);
                        int ObjAprobados = ListaEvalSession.Count(x => x.Evaluacion.Status == 3);
                        int InEval = ListaEvalSession.Count(x => x.Evaluacion.Status == 4);
                        int AutoEval = ListaEvalSession.Count(x => x.Evaluacion.Status == 5);
                        int EvalJefe = ListaEvalSession.Count(x => x.Evaluacion.Status == 6);
                        int EvaLevD = ListaEvalSession.Count(x => x.Evaluacion.Status == 7);
                        int Cerrado = ListaEvalSession.Count(x => x.Evaluacion.Status == 8);
                        int IniEval2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 10);
                        int AutoEval2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 11);
                        int EvalJefe2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 12);
                        int EvaLevD2 = ListaEvalSession.Count(x => x.Evaluacion.Status == 13);
                        int Cerrada = ListaEvalSession.Count(x => x.Evaluacion.Status == 14);
                        int Calibrada = ListaEvalSession.Count(x => x.Evaluacion.Status == 15);


                        resultado = "['Etapa', 'Cantidad ', { role: \"style\" }],";
                        resultado = resultado + "[\"Pendiente\"," + pendientes.ToString() + ", \"#F2BC36\"],";
                        resultado = resultado + "[\"Objetivos cargados\"," + ObjCargados.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Objetivos rechazados\"," + ObjRechazados.ToString() + ", \"#E56F6B\"],";
                        resultado = resultado + "[\"Objetivos aprobados\"," + ObjAprobados.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Inicio de evaluación\"," + InEval.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Comentario evaluado\"," + AutoEval.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Comentario jefe\"," + EvalJefe.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Evaluación de segundo nivel\"," + EvaLevD.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Calificación de medio año cerrada\"," + Cerrado.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Inicio de evaluación fin de año\"," + IniEval2.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Comentario evaluado fin de año\"," + AutoEval2.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Comentario jefe fin de año\"," + EvalJefe2.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Evaluación de segundo nivel fin de año\"," + EvaLevD2.ToString() + ", \"#727ABA\"],";
                        resultado = resultado + "[\"Calificación de medio año cerrada fin de año \"," + Cerrada.ToString() + ", \"#0019FB\"],";
                        resultado = resultado + "[\"Calibración de la evaluación\"," + Calibrada.ToString() + ", \"#727ABA\"],";
                    }
                    else
                    {
                        return PartialView("Respuesta", "No se cuenta con suficiente  información ");
                    }

                }
            }
            else
            {
                foreach (var per in periodos)
                {
                    ListaEvalSession.AddRange(Utilidades.negocio.RecuperaEvaluacionesPeriodoUsuarios(per).Where(t=>t.Login.Activo));
                    //ListaEvalSession = (from item in ListaEvalSession
                      //                  where item.login.Activo
                        //                select item).ToList();
                }

                if (filtros.Division == null)
                {
                    List<string> ListAreas = ObtenListaAreas(filtros.Pais);  
                    resultado = "['Promedio por área', 'Periodo', { role: \"style\" }],";
                    if (ListaEvalSession.Count > 0)
                    {
                        foreach (string item in ListAreas)
                        {
                            List<Modelo.Clases.CSessionEval> ListaEvalSessionTemp = (from aux in ListaEvalSession
                                                                                    where aux.Login.Division.ToString() == item
                                                                                     select aux).ToList();

                            int NoEval = ListaEvalSessionTemp.Count;
                            int TotalCalif = 0;
                            if (ListaEvalSessionTemp.Count > 0)
                            {
                                foreach (Modelo.Clases.CSessionEval eval in ListaEvalSessionTemp)
                                {
                                    TotalCalif = TotalCalif + eval.Evaluacion.CaliFinal2;
                                }
                                int promedio = TotalCalif / ListaEvalSessionTemp.Count;
                                resultado = resultado + "[\"" + item + "\"," + promedio.ToString() + ", \"#884DE6\"],";
                            }                          
                       
                        }
                    }
                    else
                    {
                        return PartialView("Respuesta", "No se cuenta con suficiente  información ");
                    }
                }
                else
                {
                    resultado = "['Nombre', 'Calificación', { role: \"style\" }],";

                    ListaEvalSession = (from aux in ListaEvalSession
                                        where aux.Login.Division == filtros.Division && aux.Login.Activo
                                        select aux).ToList();

                    if (ListaEvalSession.Count > 0)
                    {
                        foreach (Modelo.Clases.CSessionEval eval in ListaEvalSession)
                        {
                            resultado = resultado + "[\"" + eval.Login.NombreCompleto + "\"," + eval.Evaluacion.CaliFinal2.ToString() + ", \"#97CB9B\"],";
                        }
                    }
                    else
                    {
                        return PartialView("Respuesta", "No se cuenta con suficiente  información ");
                    }
                }
            }
            resultado = resultado.Substring(0, resultado.LastIndexOf(","));            
            ViewBag.Resultado = resultado;
            ViewBag.Pais = filtros.Pais;
            ViewBag.Periodo = periodos;
            ViewBag.Area = filtros.Division;
            ViewBag.Actual = filtros.PeriodoActivo;
            return PartialView(ListaEvalSession);
        }
        
        public ActionResult Download(string Pais, string Periodo, string Area, bool Actual)
        {
            var paises = Pais.Split(',').Select(s => int.Parse(s)).ToList();
            var periodos = Periodo.Split(',').Select(s => int.Parse(s)).ToList();
            string path = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");
            try
            {
                Create(paises,periodos,Area,Actual);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string fileName = "Reporte.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return PartialView("Respuesta", ex.ToString() + " Phat->" + path);
            }
        }

        public void Create(List<int> Pais, List<int> Periodo, string Area, bool Actual)
        {

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a crear documento", "DashBoard/Create ");

            List<VEvaluacion> evals = new List<VEvaluacion>();
            var index = 0;
            foreach (var p in Pais)
            {
                if (Area == null)
                {
                    evals.AddRange(Utilidades.negocio.GetEvalViewByPeriodCountry(Periodo[index], p));
                }
                else
                {
                    evals.AddRange(Utilidades.negocio.GetEvalViewByPeriodCountry(Periodo[index], p).Where(t => t.Division == Area));

                }
                index++;
            }

            string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");

            /*List<Modelo.Clases.CSessionEval> ListaEvalSession = new List<Modelo.Clases.CSessionEval>();

            if (Actual)
            {
                ListaEvalSession = Utilidades.negocio.RecuperaEvaluacionesPeriodoUsuarios(Periodo);
            
                if(Area!=null)
                {
                    ListaEvalSession = (from aux in ListaEvalSession
                                        where aux.login.Area == Area  && aux.login.Activo && aux.login.Pais==Pais
                                        select aux).ToList();                 
                }
            }
            else
            {
                ListaEvalSession = Utilidades.negocio.RecuperaEvaluacionesPeriodoUsuarios(Periodo);
                if (Area != null)
                {
                    ListaEvalSession = (from aux in ListaEvalSession
                                                where aux.login.Area == Area && aux.login.Activo && aux.login.Pais==Pais
                                        select aux).ToList();
                }
            }
            
            string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");

            ListaEvalSession = (from aux in ListaEvalSession
                                where aux.login.Pais == Pais
                                select aux).ToList();
                                
            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Column Cells");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                ws.Rows(1, 1).Style.Fill.BackgroundColor = XLColor.AliceBlue;
                //ws.Rows(1, 1).Style.Font.FontColor = XLColor.White;
                ws.Rows(1, 1).Style.Font.Bold = true;
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 40;
                ws.Column(3).Width = 40;
                ws.Column(4).Width = 40;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 20;
                ws.Column(8).Width = 40;

                ws.Cell(1, 1).Value = "Numero Empleado SAP";
                ws.Cell(1, 2).Value = "Nombre completo";
                ws.Cell(1, 3).Value = "División";
                ws.Cell(1, 4).Value = "Puesto";
                ws.Cell(1, 5).Value = "Evaluador";
                ws.Cell(1, 6).Value = "Fecha de ingreso";
                ws.Cell(1, 7).Value = "Pais";
                ws.Cell(1, 8).Value = "Estatus";
                ws.Cell(1, 9).Value = "Calificación fin de año";
                int i = 0;

                List<EStatus> estatus = Utilidades.negocio.GetStatus();

                foreach (Modelo.Clases.CSessionEval item in ListaEvalSession)
                {
                    if (item.login.Activo)
                    {
                        ELogin Evaluador = new ELogin();
                        Evaluador = Utilidades.negocio.RecuperaUnUsuarioSap(item.login.EvaluadorIdSap);

                        EPeriodos PeriodoAux = Utilidades.negocio.RecuperaUnPeriodo(item.Evaluacion.periodo);
                        EPais PaisAux = Utilidades.negocio.RecuperaUnPais(item.login.Pais);
                        ws.Cell(i + 2, 1).Value = item.login.id_sap;
                        ws.Cell(i + 2, 2).Value = item.login.NombreCompleto;
                        ws.Cell(i + 2, 3).Value = item.login.Division;
                        ws.Cell(i + 2, 4).Value = item.login.Puesto;
                        ws.Cell(i + 2, 5).Value = Evaluador.NombreCompleto;
                        DateTime dt = new DateTime(item.login.FechaIngreso);
                        ws.Cell(i + 2, 6).Value = dt.ToString("dd/MM/yyyy");
                        ws.Cell(i + 2, 7).Value = PaisAux.descripcion;

                        if (item.Evaluacion != null)
                        {
                            //EStatus estatusId = new EStatus();
                            //estatusId = (EStatus)(from aux in estatus
                            //                      where aux.Id == item.Evaluacion.Status
                            //                      select aux).First().Name;
                            ws.Cell(i + 2, 8).Value = (from aux in estatus
                                                                where aux.Id == item.Evaluacion.Status
                                                                select aux).First().Name;

                            if (item.Evaluacion.CaliFinal > 0)
                            {
                                ws.Cell(i + 2, 10).Value = item.Evaluacion.CaliFinal.ToString();
                            }
                            else
                            {
                                ws.Cell(i + 2, 10).Value = " ";
                            }

                            if (item.Evaluacion.CaliFinal2 > 0)
                            {
                                ws.Cell(i + 2, 9).Value = item.Evaluacion.CaliFinal2.ToString();
                            }
                            else
                            {
                                ws.Cell(i + 2, 9).Value = " ";
                            }

                        }
                        else
                        {
                            ws.Cell(i + 2, 8).Value = " ";
                            ws.Cell(i + 2, 9).Value = " ";
                            //ws.Cell(i + 2, 11).Value = " ";
                        }

                        i = i + 1;
                    }
                }
                workbook.SaveAs(filePath);
            }*/
            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Column Cells");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                ws.Rows(1, 1).Style.Fill.BackgroundColor = XLColor.AliceBlue;
                ws.Rows(1, 1).Style.Font.Bold = true;
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 40;
                ws.Column(3).Width = 40;
                ws.Column(4).Width = 40;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 20;
                ws.Column(7).Width = 25;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 20;
                //ws.Column(10).Width = 20;
                //ws.Column(11).Width = 20;

                ws.Cell(1, 1).Value = "Numero Empleado SAP";
                ws.Cell(1, 2).Value = "Nombre";
                //ws.Cell(1, 3).Value = "A. Paterno";
                //ws.Cell(1, 4).Value = "A. Materno";
                ws.Cell(1, 3).Value = "Dirección";
                ws.Cell(1, 4).Value = "Puesto";
                ws.Cell(1, 5).Value = "Evaluador";
                ws.Cell(1, 6).Value = "Fecha de ingreso";
                ws.Cell(1, 7).Value = "País";
                ws.Cell(1, 8).Value = "Estatus";
                ws.Cell(1, 9).Value = "Calificación fin de año";
                ws.Cell(1, 10).Value = "Porcentaje de Cumplimiento";
                ws.Cell(1, 11).Value = "Calificación Calibración";
                int i = 0;

                List<EStatus> estatus = Utilidades.negocio.GetStatus();

                foreach (VEvaluacion item in evals)
                {
                    CEvaluacion eval = new CEvaluacion();
                    eval.Liobjetivos = Utilidades.negocio.RecuperaListaObjetivos(item.UsuarioId);
                    eval.Liobjetivos = Utilidades.negocio.RecuperaListaObjetivos(item.Id);
                    var sum_ponderado = eval.Liobjetivos.Sum(t => t.ValorObj);

                    ws.Cell(i + 2, 1).Value = item.UsuarioSap;
                    ws.Cell(i + 2, 2).Value = item.UsuarioNombre;
                    //ws.Cell(i + 2, 3).Value = item.UsuarioNombre;
                    //ws.Cell(i + 2, 4).Value = item.UsuarioNombre;
                    ws.Cell(i + 2, 3).Value = item.Division;
                    ws.Cell(i + 2, 4).Value = item.Puesto;
                    ws.Cell(i + 2, 5).Value = item.EvaluadorNombre;
                    DateTime dt = new DateTime(item.FechaIngreso);
                    ws.Cell(i + 2, 6).Value = dt.ToString("dd/MM/yyyy");
                    ws.Cell(i + 2, 7).Value = item.PaisName;
                    ws.Cell(i + 2, 8).Value = item.statusName;
                    ws.Cell(i + 2, 9).Value = item.CalifFinal2;
                    ws.Cell(i + 2, 10).Value = sum_ponderado + "%";
                    ws.Cell(i + 2, 11).Value = item.CaliFinalCalibracion;

                    i = i + 1;
                }
                 workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Bitacora.NuevaEntrada(ex.Message, "MisEvaluaciones/OperacionCompetencias ");
            }

        }

        public ActionResult Comments(string Pais, string Periodo)
        {
            //string path = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");
            string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");
            try
            {
                ExcelSheetComments(filePath, Pais, Periodo);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = "Reporte" + DateTime.Now.ToString("yyyy_MM_dd hh-MM-ss") + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return PartialView("Respuesta", ex.ToString() + " Phat->" + filePath);
            }
        }

        public void ExcelSheetComments(string filePath, string Pais, string Periodo)
        {
            var paises = Pais.Split(',').Select(s => int.Parse(s)).ToList();
            var periodos = Periodo.Split(',').Select(s => int.Parse(s)).ToList();
            List<VEvaluacion> listEval = new List<VEvaluacion>();
            var index = 0;
            foreach (var p in paises)
            {
                listEval.AddRange(Utilidades.negocio.GetEvalViewByPeriodCountry(periodos[index], p));
                index++;
            }
            //List<VSeguimiento> ListaDev = Utilidades.negocio.GetVSeguimiento();
            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Column Cells");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                ws.Rows(1, 1).Style.Fill.BackgroundColor = XLColor.GreenRyb;
                ws.Rows(1, 1).Style.Font.FontColor = XLColor.White;
                ws.Column(1).Width = 32;
                ws.Column(2).Width = 80;
                ws.Column(3).Width = 80;

                ws.Cell(1, 1).Value = "Evaluado";
                ws.Cell(1, 2).Value = "Comentario";
                ws.Cell(1, 3).Value = "Comentario Evaluador";
                ws.Cell(1, 4).Value = "Evaluador";

                int i = 0;

                foreach (VEvaluacion item in listEval)
                {
                    ws.Cell(i + 2, 1).Value = item.UsuarioNombre;
                    ws.Cell(i + 2, 2).Value = item.ComentarioEvaluado;
                    ws.Cell(i + 2, 2).Style.Alignment.WrapText = true;
                    ws.Cell(i + 2, 2).DataType = XLCellValues.Text;
                    ws.Cell(i + 2, 3).Value = item.ComentarioEvaluador;
                    ws.Cell(i + 2, 3).DataType = XLCellValues.Text;
                    ws.Cell(i + 2, 3).Style.Alignment.WrapText = true;
                    ws.Cell(i + 2, 4).Value = item.EvaluadorNombre;

                    i = i + 1;
                }
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {

            }
        }

        public ActionResult Competences(string Pais, string Periodo)
        {
            //string path = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");
            string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp2.xlsx");
            try
            {
                ExcelSheetCompetences(filePath, Pais, Periodo);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = "Reporte" + DateTime.Now.ToString("yyyy_MM_dd HH-mm-ss") + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return PartialView("Respuesta", ex.ToString() + " Phat->" + filePath);
            }
        }

        public void ExcelSheetCompetences(string filePath, string Pais, string Periodo)
        {
            var paises = Pais.Split(',').Select(s => int.Parse(s)).ToList();
            var periodos = Periodo.Split(',').Select(s => int.Parse(s)).ToList();
            List<VEvalCompetences> listEval = new List<VEvalCompetences>();
            var index = 0;
            foreach (var p in paises)
            {
                listEval.AddRange(Utilidades.negocio.GetEvalCompViewByPeriodCountry(periodos[index], p));
                index++;
            }
            //List<VSeguimiento> ListaDev = Utilidades.negocio.GetVSeguimiento();
            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Competencias");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                ws.Rows(1, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Rows(1, 1).Style.Font.Bold = true;
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 25;
                ws.Column(3).Width = 28;
                ws.Column(4).Width = 45;
                ws.Column(5).Width = 80;
                ws.Column(6).Width = 15;

                ws.Cell(1, 1).Value = "Num SAP";
                ws.Cell(1, 2).Value = "Nombre";
                ws.Cell(1, 3).Value = "Dirección";
                ws.Cell(1, 4).Value = "Competencia";
                ws.Cell(1, 5).Value = "Actividades";
                ws.Cell(1, 6).Value = "Pais";

                int i = 0;

                foreach (VEvalCompetences item in listEval)
                {
                    ws.Cell(i + 2, 1).Value = item.UserSapId;
                    ws.Cell(i + 2, 2).Value = item.UserName;
                    ws.Cell(i + 2, 3).Value = item.Division;
                    ws.Cell(i + 2, 4).Value = item.NombreCatCompetencia;
                    ws.Cell(i + 2, 5).Value = item.ActividadesCompetence;
                    ws.Cell(i + 2, 5).DataType = XLCellValues.Text;
                    ws.Cell(i + 2, 5).Style.Alignment.WrapText = true;
                    ws.Cell(i + 2, 6).Value = item.PaisNombre;

                    i = i + 1;
                }

                var ws2 = workbook.Worksheets.Add("Resumen");

                //var wordList = new List<String> { "test", "one", "test", "two" };
                IEnumerable<CompetenceResutls> grouped = listEval
                    .GroupBy(jh => jh.CompCatId) //Group the words
                    .Select(jh => new CompetenceResutls { Comp = jh.Key, CantComp = jh.Count() }); //get a count for each

                ws2.Cell(3, 1).Value = "Competencia";
                ws2.Cell(3, 2).Value = "";

                int j = 0;
                List<ECatComp> comps = Utilidades.negocio.RecuperaCatCompetencias();
                grouped.ForEach(item2 =>
                   {
                       //ECatComp comp = Utilidades.negocio.RecuperaUnaCatCompetencias(item2.Comp);
                       var comp = comps.Where(x => x.id.Equals(item2.Comp)).First();
                       ws2.Cell(j + 4, 1).Value = comp.descripcion;
                       ws2.Cell(j + 4, 2).Value = item2.CantComp;
                       j += 1;
                   });
                ws2.Cell(j + 5, 2).Value = listEval.Count();

                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {

            }
        }

        public JsonResult GetListPeriods(int cou)
        {
            List<SelectListItem> periodsList = DropPeriodos(cou);

            return Json(new SelectList(periodsList, "Value", "Text"));
        }

        [AllowAnonymous]
        public JsonResult GetListDivisions(int? cou, string prefix)
        {
            List<ELogin> users = new List<ELogin>();
            if (cou == null)
            {
                users = new List<ELogin>();
            }
            else
            {
                users = Utilidades.negocio.RecuperaLogInsPais((int)cou);
            }
            var usrfilters = users.Where(x => x.Division.ToString().ToLower().Contains(prefix.ToLower()));

            return Json(usrfilters.Select(x => x.Division).Distinct(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetListObjetives(int cou, int per, string div)
        {
            List<VEvalObj> evals = Utilidades.negocio.GetEvalObjViewByPeriodCountryDiv(per, cou, div);

            return Json(evals, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchObjetives()
        {
            List<SelectListItem> ListaPeriodos = DropPeriodos(1);
            
            return PartialView();
        }

        public ActionResult DashObjetivesView()
        {
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            List<SelectListItem> ListaPeriodos = DropPeriodos(1);
            ViewBag.Pais = DropPais(sesion.Login.perfil, sesion.Login.Pais);
            //ViewBag.Pais = new SelectList(Utilidades.negocio.RecuperaPaises().Where(t=>t.Activo).ToList(), "id", "descripcion");
            ViewBag.Periodo = DropPeriodos(sesion.Login.Pais);
            return PartialView();
        }

        [HttpPost]
        public ActionResult DashObjetivesView(Models.EFlitrosDash obj)
        {
            string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp3.xlsx");
            try
            {
                DashObjetives(filePath, obj.Pais, obj.Periodo, obj.Division);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = "Reporte" + DateTime.Now.ToString("yyyy_MM_dd HH-mm-ss") + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return PartialView("Respuesta", ex.ToString() + " Phat->" + filePath);
            }

            //return PartialView();
        }

        public ActionResult DashObjetivesEmpleados()
        {
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            //List<SelectListItem> ListaPeriodos = DropPeriodos(sesion.Login.Pais);
            //ViewBag.Pais = DropPais(sesion.Login.perfil, sesion.Login.Pais);
            //ViewBag.Pais = new SelectList(Utilidades.negocio.RecuperaPaises().Where(t=>t.Activo).ToList(), "id", "descripcion");
            ViewBag.Periodo = DropPeriodos(sesion.Login.Pais);
            return PartialView();
        }

        [HttpPost]
        public ActionResult DashObjetivesEmpleados(Models.EFlitrosDash obj)
        {
            string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp3.xlsx");
            try
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                DashObjetivesRecursivo(filePath, obj.Periodo, sesion.Login.id_sap);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string fileName = "Reporte" + DateTime.Now.ToString("yyyy_MM_dd HH-mm-ss") + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return PartialView("Respuesta", ex.ToString() + " Phat->" + filePath);
            }

            //return PartialView();
        }
        public void DashObjetives(string filePath, List<int> Pais, int Periodo, string Area)
        {

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a crear documento", "DashBoard/DashObjetives ");
            List<VEvalObj> evals = new List<VEvalObj>();
            foreach (var p in Pais)
            {
                evals.AddRange(Utilidades.negocio.GetEvalObjViewByPeriodCountryDiv(Periodo,p, Area));
            }
            //string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");

            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Objetivos");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                ws.Rows(1, 1).Style.Fill.BackgroundColor = XLColor.AliceBlue;
                ws.Rows(1, 1).Style.Font.Bold = true;
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 35;
                ws.Column(3).Width = 35;
                ws.Column(4).Width = 35;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 40;
                ws.Column(7).Width = 40;
                ws.Column(8).Width = 45;
                ws.Column(9).Width = 45;
                ws.Column(10).Width = 45;
                ws.Column(11).Width = 45;

                ws.Cell(1, 1).Value = "Numero Empleado SAP";
                ws.Cell(1, 2).Value = "Nombre completo";
                ws.Cell(1, 3).Value = "Dirección";
                ws.Cell(1, 4).Value = "Puesto";
                ws.Cell(1, 5).Value = "Evaluador";
                ws.Cell(1, 6).Value = "Objetivo";
                ws.Cell(1, 7).Value = "Descripción";
                ws.Cell(1, 8).Value = "Métrica";
                ws.Cell(1, 9).Value = "Resultado";
                ws.Cell(1, 10).Value = "Comentarios jefe";
                ws.Cell(1, 11).Value = "Peso ponderado %";
                int i = 0;

                foreach (VEvalObj item in evals)
                {
                    if((i % 2) == 0 && i!=0)
                        ws.Rows(i, i).Style.Fill.BackgroundColor = XLColor.AliceBlue;

                    ws.Cell(i + 2, 1).Value = item.EvaluatedNoEmp;
                    ws.Cell(i + 2, 2).Value = item.Evaluated;
                    ws.Cell(i + 2, 3).Value = item.Division;
                    ws.Cell(i + 2, 4).Value = item.Puesto;
                    ws.Cell(i + 2, 5).Value = item.Evaluator;
                    ws.Cell(i + 2, 6).Value = item.Objetive;
                    ws.Cell(i + 2, 7).Value = item.objdesc;
                    ws.Cell(i + 2, 8).Value = item.objMetricas;
                    ws.Cell(i + 2, 9).Value = item.ObjResult;
                    ws.Cell(i + 2, 10).Value = item.ComentariosJefeEval;
                    ws.Cell(i + 2, 11).Value = item.ObjetivePeso;
                    ws.Row(i+1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    i = i + 1;
                }
                ws.Column(7).Style.Alignment.WrapText = true;
                ws.Column(8).Style.Alignment.WrapText = true;
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Bitacora.NuevaEntrada(ex.Message, "DashBoard/DashObjetives ");
            }

        }
        public void DashObjetivesRecursivo(string filePath, int Periodo, string idsap)
        {

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a crear documento", "DashBoard/DashObjetives ");
            //Obtener los evaluados por cada evaluado, hacer la funcion recursivamente
            //List<CSessionEval> listSesion = new List<CSessionEval>();// Utilidades.negocio.RecuperaLiSubordinados(sesion.Login.id_sap);
            BuscarSubordinados(idsap, Periodo);
            //foreach (var item in listSesion)
            //  listSesion.AddRange(BuscarSubordinados(item.Login.id_sap));
            //List<VEvalObj> evals = new List<VEvalObj>();
            //  evals.AddRange(Utilidades.negocio.GetEvalObjViewByPeriodCountryDiv(Periodo));

            //string filePath = Path.Combine(Server.MapPath("~/Export/"), "Temp.xlsx");

            try
            {
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Objetivos");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                ws.Rows(1, 1).Style.Fill.BackgroundColor = XLColor.AliceBlue;
                ws.Rows(1, 1).Style.Font.Bold = true;
                ws.Column(1).Width = 15;
                ws.Column(2).Width = 35;
                ws.Column(3).Width = 35;
                ws.Column(4).Width = 35;
                ws.Column(5).Width = 40;
                ws.Column(6).Width = 40;
                ws.Column(7).Width = 40;
                ws.Column(8).Width = 45;
                ws.Column(9).Width = 45;
                ws.Column(10).Width = 45;
                ws.Column(11).Width = 45;

                ws.Cell(1, 1).Value = "Numero Empleado SAP";
                ws.Cell(1, 2).Value = "Nombre completo";
                ws.Cell(1, 3).Value = "Dirección";
                ws.Cell(1, 4).Value = "Puesto";
                ws.Cell(1, 5).Value = "Evaluador";
                ws.Cell(1, 6).Value = "Objetivo";
                ws.Cell(1, 7).Value = "Descripción";
                ws.Cell(1, 8).Value = "Métrica";
                ws.Cell(1, 9).Value = "Peso del objetivo(%)";
                ws.Cell(1, 10).Value = "Resultado";
                ws.Cell(1, 11).Value = "Comentarios jefe";
                //ws.Cell(1, 12).Value = "Peso ponderado";
                int i = 0;
                foreach (var usuario in listSesion)
                {
                    if (usuario.Evaluacion != null)
                    {
                        var objetivos = Utilidades.negocio.RecuperaListaObjetivos(usuario.Evaluacion.id);
                        if(objetivos.Count>0)
                        foreach (var item in objetivos)
                        {
                            //if ((i % 2) == 0 && i != 0)
                              //  ws.Rows(i, i).Style.Fill.BackgroundColor = XLColor.AliceBlue;

                            ws.Cell(i + 2, 1).Value = usuario.Login.id_sap;
                            ws.Cell(i + 2, 2).Value = usuario.Login.NombreCompleto;
                            ws.Cell(i + 2, 3).Value = usuario.Login.Division;
                            ws.Cell(i + 2, 4).Value = usuario.Login.Puesto;
                            ws.Cell(i + 2, 5).Value = Utilidades.negocio.RecuperaUnUsuarioSap(usuario.Login.EvaluadorIdSap).NombreCompleto;
                            ws.Cell(i + 2, 6).Value = item.titulo;
                            ws.Cell(i + 2, 7).Value = item.objdesc;
                            ws.Cell(i + 2, 8).Value = item.objMetricas;
                            ws.Cell(i + 2, 9).Value = item.ponderado;
                            ws.Cell(i + 2, 10).Value = item.Resultado2;
                            ws.Cell(i + 2, 11).Value = item.ComentariosJefeEval2;
                            //ws.Cell(i + 2, 11).Value = item.ObjetivePeso;
                            ws.Row(i + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            i++;
                        }
                        else
                        {
                            ws.Row(i + 2).Style.Fill.BackgroundColor = XLColor.LightYellow;
                            ws.Cell(i + 2, 1).Value = usuario.Login.id_sap;
                            ws.Cell(i + 2, 2).Value = usuario.Login.NombreCompleto;
                            ws.Cell(i + 2, 3).Value = usuario.Login.Division;
                            ws.Cell(i + 2, 4).Value = usuario.Login.Puesto;
                            ws.Cell(i + 2, 5).Value = Utilidades.negocio.RecuperaUnUsuarioSap(usuario.Login.EvaluadorIdSap).NombreCompleto;
                            ws.Cell(i + 2, 6).Value = "Sin objetivos en el periodo";
                            i++;
                        }
                    }
                    else
                    {
                        ws.Row(i+2).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell(i + 2, 1).Value = usuario.Login.id_sap;
                        ws.Cell(i + 2, 2).Value = usuario.Login.NombreCompleto;
                        ws.Cell(i + 2, 3).Value = usuario.Login.Division;
                        ws.Cell(i + 2, 4).Value = usuario.Login.Puesto;
                        ws.Cell(i + 2, 5).Value = Utilidades.negocio.RecuperaUnUsuarioSap(usuario.Login.EvaluadorIdSap).NombreCompleto;
                        ws.Cell(i + 2, 6).Value = "Sin evaluación en el periodo";
                        i++;
                    }
                }
                ws.Column(7).Style.Alignment.WrapText = true;
                ws.Column(8).Style.Alignment.WrapText = true;
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Bitacora.NuevaEntrada(ex.Message, "DashBoard/DashObjetives ");
            }

        }
        public void BuscarSubordinados(string idsap, int Periodo)
        {
            var users=Utilidades.negocio.RecuperaLiSubordinadosPeriodo(idsap,Periodo).Where(t=>t.Login.Activo).ToList();
            listSesion.AddRange(users);
            if(users.Count>0)
            {
                foreach (var u in users)
                    BuscarSubordinados(u.Login.id_sap, Periodo);
            }
        }
        public class CompetenceResutls
        {
            public int Comp;
            public int CompId;
            public int CantComp;
        }

        #region DropsFiltros

        public List<SelectListItem> DropPais(int perfil, int pais)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPais> ListaPeriodos = Utilidades.negocio.RecuperaPaises().Where(t => t.Activo).ToList();
            if (perfil == 1)
            {        
                foreach (EPais item in ListaPeriodos)
                {
                    Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
                }
            }
            else if(perfil==2)
            {
                Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
                foreach (EPais item in ListaPeriodos)
                {
                    if(sesion.Login.paisesRegion.Split(',').ToList().Contains(item.id.ToString()))
                    Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });
                }
            }
            else
            {
                EPais PaisTemp = Utilidades.negocio.RecuperaUnPais(pais);
                Respuesta.Add(new SelectListItem { Text = PaisTemp.descripcion, Value = PaisTemp.id.ToString() });    
            }
            return Respuesta;
        }


        public List<SelectListItem> DropPeriodos(int pais)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPeriodos> ListaPeriodos = Utilidades.negocio.RecuperaTodosPeriodosPais(pais);
                        
            foreach (EPeriodos item in ListaPeriodos)
            {
                Respuesta.Add(new SelectListItem { Text = item.Llave, Value = item.id.ToString() });
            }
            return Respuesta;
        }

        public List<SelectListItem> DropAreas(int pais)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<ELogin> ListaUsuarios = Utilidades.negocio.RecuperaLogInsPais(pais).Where(t=>t.Activo).ToList();        

            ListaUsuarios = ListaUsuarios.GroupBy(x => x.Division).Select(x => x.First()).ToList();


            foreach (ELogin item in ListaUsuarios)
            {
                //if(item.Activo==1)
                Respuesta.Add(new SelectListItem { Text = item.Division, Value = item.Division });

            }
            return Respuesta;
        }


        public static List<string> ObtenListaAreas(List<int> pais)
        {
            List<ELogin> ListaUsuarios = new List<ELogin>();
            foreach (var p in pais)
            {
                ListaUsuarios.AddRange(Utilidades.negocio.RecuperaLogInsPais(p));
            }
            ListaUsuarios = ListaUsuarios.GroupBy(x => x.Division).Select(x => x.First()).ToList();
            List<string> RespuestaLista = new List<string>();

            foreach (ELogin item in ListaUsuarios)
            {
                RespuestaLista.Add(item.Division);
            }
            return RespuestaLista;
        }

        #endregion
    }
}