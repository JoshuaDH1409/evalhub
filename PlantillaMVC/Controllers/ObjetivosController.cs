using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using General;
using CRUD.Transaction;
using CapaLogica.Funciones;
using PlantillaMVC.Filters;
using System.Web.Security;
using System.IO;
using Modelo;
using Modelo.Clases;
using Spire.Doc;
using Spire.Doc.Documents;

namespace PlantillaMVC.Controllers
{
    public class ObjetivosController : Controller
    {
        // GET: Objetivos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RevisarObjetivos()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");


            Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Respuesta.listSesion = Utilidades.negocio.RecuperaLiSubordinados(sesion.Login.id_sap);
            Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);
            
            List<VEvaluacion> vistaeval = new List<VEvaluacion>();
            if(Respuesta.periodo!=null)
                vistaeval = Utilidades.negocio.GetEvalViewByPeriodoEvaluator(Respuesta.periodo.id, sesion.Login.id_sap);

            if (Respuesta.listSesion == null)
            {
                return PartialView("Respuesta", "No se tiene una evaluación asignada actualmente.");
            }
            else
            {
                ViewBag.IDSAP = sesion.Login.id_sap;
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
                ViewBag.listaEvals= vistaeval;
                return PartialView(Respuesta);
            }
        }
        public ActionResult SetKPI()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");


            Modelo.Clases.CPeriodo Respuesta = new Modelo.Clases.CPeriodo();
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Respuesta.listSesion = Utilidades.negocio.RecuperaLiSubordinados(sesion.Login.id_sap);
            Respuesta.periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);

            List<VEvaluacion> vistaeval = Utilidades.negocio.GetEvalViewByPeriodoEvaluator(Respuesta.periodo.id, sesion.Login.id_sap);

            if (Respuesta.listSesion == null)
            {
                return PartialView("Respuesta", "No se tiene una evaluación asignada actualmente");
            }
            else
            {
                ViewBag.IDSAP = sesion.Login.id_sap;
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
                ViewBag.listaEvals = vistaeval;
                return PartialView();
            }
        }
        [Autentificado]
        public ActionResult GetKPI()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");
            return PartialView();
        }
        [Autentificado]
        public ActionResult Resumen()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return RedirectToAction("Login", "Account");

            CPeriodoUserKPI modelo = new CPeriodoUserKPI();
            CSession sesion = (CSession)Session[Utilidades.session];
            modelo.Periodo = Utilidades.negocio.RecuperaPeriodopais(sesion.Login.Pais);
            modelo.Periodo.PaisTempral = sesion.Pais.descripcion;
            modelo.ObjetivoPais = Utilidades.negocio.RecuperaObjetivosPais(sesion.Pais.id, modelo.Periodo.id);
            modelo.Login = sesion.Login;
            sesion.Evaluacion = Utilidades.negocio.RecuperaEvaluacionActivaUsario(sesion.Login.id);
            sesion.Login.EvaluadorNombre = Utilidades.negocio.RecuperaUnUsuarioSap(sesion.Login.EvaluadorIdSap).NombreCompleto;
            modelo.Objetives = Utilidades.negocio.RecuperaListaObjetivos(sesion.Evaluacion.id);
            if (modelo.ObjetivoPais.Count > 0)
                if (modelo.ObjetivoPais.First().Objetivo_Cerrado)
                    ViewBag.Cerrados = true;
            modelo.Results = Utilidades.GetResultKPI(modelo.ObjetivoPais,
                modelo.Objetives, modelo.Periodo.Etapa);
            ViewBag.IdEval = sesion.Evaluacion.id;
            return PartialView(modelo);
        }
        public ActionResult EscaletaIndividual()
        {
            List<EEscaleta> escaleta = Utilidades.negocio.RecuperaEscaleta();
            return PartialView(escaleta);
        }
        public ActionResult EscaletaEmpresa()
        {
            List<EEscaletaEmpresa> escaleta = Utilidades.negocio.RecuperaEscaletaE();
            return PartialView(escaleta);
        }
        public ActionResult DownloadResumenPdf(int id)
        {
            //Recuperamos informacion
            CSession sesion = (CSession)Session[Utilidades.session];
            //
            string path = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "_DocTest.docx");
            string pathResp = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");
            string pathpdf = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.pdf");
            try
            {

                Spire.Doc.Document document = new Spire.Doc.Document();
                EEval Evaluacion = Utilidades.negocio.RecuperaEvaluacionIdHistorial(sesion.Evaluacion.id);
                Modelo.Clases.CSessionEval Evaluado = Utilidades.negocio.RecuperaUsuarioEval(Evaluacion.id_usuario);

                ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(Evaluado.Login.EvaluadorIdSap);
                EPeriodos periodo = Utilidades.negocio.RecuperaUnPeriodo(Evaluacion.periodo);
                List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Evaluacion.id);
                var ObjetivoPais = Utilidades.negocio.RecuperaObjetivosPais(sesion.Pais.id, periodo.id);

                var Results = Utilidades.GetResultKPI(ObjetivoPais,
                    liObjetivos, periodo.Etapa);
                Results.CompB_SumaPonderados = ObjetivoPais.Sum(t => t.Objetivo_Peso);
                Results.CompB_SumaCumplimiento = ObjetivoPais.Sum(t => t.Objetivo_Cumplimiento);
                Results.CompC_SumaPonderados = liObjetivos.Sum(t => t.ponderado);
                Results.CompC_SumaCumplimiento = liObjetivos.Sum(t => t.cumplimientoEvaluador2);

                System.IO.File.Copy(path, pathResp, true);//copiamos el documento original al temporal
                object fileName = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");//obtenemos la ruta del archivo copiado
                document.LoadFromFile(fileName.ToString());//se carga el documento que se le hara el replace
                Bitacora.NuevaEntrada("se inicia el PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");


                document.Replace("#Periodo#", periodo.Llave, true, true);
                document.Replace("#Pais#", Utilidades.negocio.RecuperaUnPais(periodo.Country).descripcion, true, true);

                document.Replace("#NombreEmp#", String.Concat(Evaluado.Login.NombreCompleto), true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#divisionEmp#", Evaluado.Login.Division, true, true);

                document.Replace("#Nombrejefe#", String.Concat(jefe.NombreCompleto), true, true);
                //document.Replace("#puestoJefe#", jefe.Puesto, true, true);
                //document.Replace("#divisionjefe#", jefe.Division, true, true);
                document.Replace("#sumpond_empresa#",String.Concat(Results.CompB_SumaPonderados.ToString(),"%"), true, true);
                document.Replace("#sumcump_empresa#", String.Concat(Results.CompB_SumaCumplimiento.ToString(),"%"), true, true);
                document.Replace("#sumvalor_empresa#", String.Concat(Results.CompB_SumaValor.ToString(),"%"), true, true);
                document.Replace("#compscore_b#", String.Concat(Results.CompB_Score.ToString(),"%"), true, true);
                document.Replace("#weigcomp_b#", String.Concat(Results.CompB_Weighting.ToString(),"%"), true, true);
                document.Replace("#finalscorecomp_b#", String.Concat(Results.CompB_FinalScore.ToString(),"%"), true, true);

                #region Objetivos Pais
                if (ObjetivoPais != null)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[1];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#ObjetivosEmpresa#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = sectionObjetivos.Body.ChildObjects.IndexOf(paragraphObjetivos);

                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);
                    PreferredWidth width = new PreferredWidth(WidthType.Percentage, 100);
                    tableObjetivos.PreferredWidth = width;
                    //tableObjetivos.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    //tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 0, 0);

                    String[] Header = { "30.8", "11", "12.6", "11", "12.6", "11", "11" };
                    string[][] data = new string[ObjetivoPais.Count][];
                    tableObjetivos.ResetCells(ObjetivoPais.Count, 7);

                    int i = 0;
                    foreach (EObjetivoPais objetivo in ObjetivoPais)
                    {
                        data[i] = new string[7];
                        data[i][0] = objetivo.Objetivo_Dsc;
                        data[i][1] = objetivo.Objetivo_Peso.ToString();
                        data[i][2] = objetivo.Objetivo_Cumplimiento.ToString();
                        data[i][3] = objetivo.Valor.ToString();
                        data[i][4] = ""; data[i][5] = ""; data[i][6] = "";
                        //for (int r = 0; r < data.Length; r++)
                        //{
                            Spire.Doc.TableRow DataRow = tableObjetivos.Rows[i];
                            DataRow.Height = 22;

                        //    DataRow.RowFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        //    DataRow.RowFormat.Borders.Color = System.Drawing.Color.FromArgb(0,0,0);
                        if ((i % 2) == 0)
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(222, 234, 246);
                        }
                        else
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                        }

                        for (int c = 0; c < data[i].Length; c++)
                            {
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Cleared;
                            DataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                                DataRow.Cells[c].SetCellWidth((float)Convert.ToDouble(Header[c]), CellWidthType.Percentage);
                                DataRow.Cells[c].CellFormat.Borders.Bottom.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                                DataRow.Cells[c].CellFormat.Borders.Bottom.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                                Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                                Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[i][c]);
                                //Format Cells
                                p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            p2.Format.TextAlignment = TextAlignment.Center;
                            if (c == 1 || c == 2 || c == 3)
                                    p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                                TR2.CharacterFormat.FontName = "Calibri";
                                TR2.CharacterFormat.FontSize = 9;
                                TR2.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);
                                //TR2.CharacterFormat.Border.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                            }
                        //}
                        i++;
                    }
                    //tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    bodyObjetivos.ChildObjects.Remove(paragraphObjetivos);
                    document.Replace("#ObjetivosEmpresa#", "", true, true);
                    bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#ObjetivosEmpresa#", "", true, true);
                }
                #endregion                

                document.Replace("#sumpond_ind#", String.Concat(Results.CompC_SumaPonderados.ToString(),"%"), true, true);
                document.Replace("#sumcump_ind#", String.Concat(Results.CompC_SumaCumplimiento.ToString(),"%"), true, true);
                document.Replace("#sumvalor_ind#", String.Concat(Results.CompC_SumaValor.ToString(),"%"), true, true);
                document.Replace("#compscore_c#", String.Concat(Results.CompC_Score.ToString(),"%"), true, true);
                document.Replace("#weigcomp_c#", String.Concat(Results.CompC_Weighting.ToString(),"%"), true, true);
                document.Replace("#finalscorecomp_c#", String.Concat(Results.CompC_FinalScore.ToString(),"%"), true, true);

                document.Replace("#totalweig#", String.Concat(Results.Total_Weighting.ToString(),"%"), true, true);
                document.Replace("#total_finalscore#", String.Concat(Results.Total_Score.ToString(),"%"), true, true);
                Bitacora.NuevaEntrada("se llena Cabecera del PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                #region Objetivos Evaluacion medio año
                if (liObjetivos != null)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[1];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#ObjetivosIndividual#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = sectionObjetivos.Body.ChildObjects.IndexOf(paragraphObjetivos);

                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);
                    PreferredWidth width = new PreferredWidth(WidthType.Percentage, 100);
                    tableObjetivos.PreferredWidth = width;
                    //tableObjetivos.AutoFitBehavior(AutoFitBehaviorType.wdAutoFitContents);
                    //tableObjetivos.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    //tableObjetivos.TableFormat.Borders.Bottom.Color = System.Drawing.Color.FromArgb(0, 171, 239);

                    String[] Header = { "30.8", "11", "12.6", "11", "12.6", "11", "11" };
                    string[][] data = new string[liObjetivos.Count][];
                    tableObjetivos.ResetCells(liObjetivos.Count, 7);

                    int i = 0;
                    foreach (EObjetives objetivo in liObjetivos)
                    {
                        data[i] = new string[7];
                        data[i][0] = objetivo.titulo;
                        data[i][1] = objetivo.ponderado.ToString();
                        data[i][2] = objetivo.cumplimientoEvaluador2.ToString();
                        data[i][3] = objetivo.ValorObj.ToString();
                        data[i][4] = ""; data[i][5] = ""; data[i][6] = "";
                        i = i + 1;
                    }

                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r];
                        DataRow.Height = 22;

                        //DataRow.RowFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        //DataRow.RowFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                        if ((r % 2) == 0)
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(222, 234, 246);
                        }
                        else
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                        }

                        for (int c = 0; c < data[r].Length; c++)
                        {
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Cleared;
                            DataRow.Cells[c].CellFormat.Borders.Bottom.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                            DataRow.Cells[c].CellFormat.Borders.Bottom.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                            DataRow.Cells[c].SetCellWidth((float)Convert.ToDouble(Header[c]), CellWidthType.Percentage);
                            DataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;

                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            p2.Format.TextAlignment = TextAlignment.Center;
                            if (c == 3 || c == 2 || c == 1)
                                p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);
                            //TR2.CharacterFormat.Border.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                        }
                    }
                    //tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    bodyObjetivos.ChildObjects.Remove(paragraphObjetivos);
                    document.Replace("#ObjetivosIndividual#", "", true, true);
                    bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#ObjetivosIndividual#", "", true, true);
                }
                #endregion

                Bitacora.NuevaEntrada("llenado de objetivos de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");


                Bitacora.NuevaEntrada("llenado de competencias de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");


                Bitacora.NuevaEntrada("llenado de Calificación de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                

                Bitacora.NuevaEntrada("fin de Calificación de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto.Trim() + " Ingreso a Historial", "MiHistorial/Historial ");
                Bitacora.NuevaEntrada("path:" + pathResp.ToString() + " Ingreso a Historial", "MiHistorial/Historial ");

                pathResp = pathResp.Replace(".docx", ".pdf");
                document.SaveToFile(pathResp.ToString(), Spire.Doc.FileFormat.PDF);


                Bitacora.NuevaEntrada("Se guardo el archivo:" + pathResp.ToString() + " Ingreso a Historial", "MiHistorial/Historial ");
                //esto es para regresarlo al navegador
                byte[] fileBytes = System.IO.File.ReadAllBytes(pathResp);
                string fileNameResp = Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto.Trim().Replace(" ", "") + ".pdf";
                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Logro descargar su  evaluación del periodo " + periodo.Llave, "MiHistorial/DownloadPdf ");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameResp);
            }
            catch (Exception ex)
            {
                Bitacora.NuevaEntrada("*****exception:" + ex.ToString(), "MiHistorial/Historial ");
                return PartialView("Respuesta", ex.ToString());
            }
            finally
            {
                if (System.IO.File.Exists(pathResp))
                { //eliminar para evitar duplicidades
                    //System.IO.File.Delete(pathResp);
                }
            }
        }
        public FileResult DownloadKPI()
        {
            string path = Path.Combine(Server.MapPath("~/PlantillasCargaMasiva/"), "BU_KPI_Template.xlsx");
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = "KPI_Template.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public ActionResult BackStatus(int Eval, int Period)
        {
            EEval eval = new EEval();
            eval = Utilidades.negocio.RecuperaUnaEaluacion(Eval);
            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            eval.Modificadopor = sesion.Login.NombreCompleto;
            if (Period == 1)
            {
                eval.Status = 0;
            }
            else if (Period == 2)
            {
                eval.Status = 4;
            }
            else if (Period == 3)
            {
                eval.Status = 10;
            }

            bool ok = Utilidades.negocio.GuardaEvaluacion(eval);

            string resp = "";
            if (ok)
                resp = "El estatus fue cambiado correctamente";
            else
                resp = "ha ocurrido un error. Por favor intente más tarde.";

            return PartialView("Respuesta", resp);
            //return null;
        }

        public ActionResult DownloadPdf(int Id)
        {
            string path = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "DocTest.docx");
            string pathResp = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");
            string pathpdf = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.pdf");
            //string pathLicencia = Path.Combine(Server.MapPath("~/bin/"), "license.lic");
            try
            {

                Spire.Doc.Document document = new Spire.Doc.Document();
                EEval Evaluacion = Utilidades.negocio.RecuperaEvaluacionIdHistorial(Id);
                Modelo.Clases.CSessionEval Evaluado = Utilidades.negocio.RecuperaUsuarioEval(Evaluacion.id_usuario);

                ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(Evaluado.Login.EvaluadorIdSap);
                EPeriodos perido = Utilidades.negocio.RecuperaUnPeriodo(Evaluacion.periodo);
                List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Evaluacion.id);
                List<ECompetemces> liCompetencias = Utilidades.negocio.RecuperaLiCompetencias(Evaluacion.id);

                System.IO.File.Copy(path, pathResp, true);//copiamos el documento original al temporal
                object fileName = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");//obtenemos la ruta del archivo copiado
                document.LoadFromFile(fileName.ToString());//se carga el documento que se le hara el replace

                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Inicio la descarga de su evaluación " + perido.Llave, "Objetivos/DownloadPdf ");


                document.Replace("#SapEmpleado#", Evaluado.Login.id_sap, true, true);
                document.Replace("#NombreEmp#", String.Concat(Evaluado.Login.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", perido.Llave, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#divisionEmp#", Evaluado.Login.Division, true, true);
                //document.Replace("#areaEmp#", Evaluado.login.Area, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#FchEmpini#", TiksToDate(Evaluado.Login.FechaIngreso), true, true);
                document.Replace("#fchEmpAnt#", TiksToDate(Evaluado.Login.FechaAntiguedad), true, true);

                document.Replace("#SapJefe#", jefe.id_sap, true, true);
                document.Replace("#Nombrejefe#",String.Concat(jefe.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", perido.Llave, true, true);
                document.Replace("#puestoJefe#", jefe.Puesto, true, true);
                document.Replace("#divisionjefe#", jefe.Division, true, true);
                //document.Replace("#areaJefe#", jefe.Area, true, true);
                document.Replace("#FchJefeini#", jefe.Puesto, true, true);
                document.Replace("#FchJefeAnt#", TiksToDate(jefe.FechaIngreso), true, true);

                #region Objetivos
                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Inicio El procesamiento de su pdf- objetivos ", "Objetivos/DownloadPdf ");

                if (liObjetivos != null)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = bodyObjetivos.Paragraphs.IndexOf(paragraphObjetivos);

                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);


                    String[] Header = { "Objetivo", "Descripción de Objetivo", "Métricas", "Auto evaluación (1-4)", "Calificación Evaluador (1-4)", "Calificación final acordada (1-4)", "Auto evaluación (1-4)", "Calificación Evaluador (1-4)", "Calificación final acordada (1-4)" };
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
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
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

                        if (objetivo.resultado != null)
                        {
                            data[i][3] = objetivo.cumplimientoEvaluado.ToString() + " " + objetivo.resultado;
                        }
                        else {
                            data[i][3] = objetivo.cumplimientoEvaluado.ToString() + " ";
                        }

                        if (objetivo.ComentariosJefeEval != null)
                        {
                            data[i][4] = objetivo.cumplimientoEvaluador.ToString() + " " + objetivo.ComentariosJefeEval;
                        }
                        else
                        {
                            data[i][4] = objetivo.cumplimientoEvaluador.ToString() + " ";
                        }

                        if (objetivo.ComentariosJefeEval2 != null)
                        {
                            data[i][5] = objetivo.cumplimientoEvaluador2.ToString() + " " + objetivo.ComentariosJefeEval2;
                        }
                        else
                        {
                            data[i][5] = objetivo.cumplimientoEvaluador2.ToString() + " " ;
                        }

                        if (objetivo.Resultado2 != null)
                        {
                            data[i][6] = objetivo.cumplimientoEvaluado2.ToString() + " " + objetivo.Resultado2;
                        }
                        else
                        {
                            data[i][6] = objetivo.cumplimientoEvaluado2.ToString() + " ";
                        }

                        if (objetivo.ComentariosJefeEval2 != null)
                        {
                            data[i][7] = objetivo.Calif2.ToString() + " " + objetivo.ComentariosJefeEval2;
                        }
                        else
                        {
                            data[i][7] = objetivo.Calif2.ToString() + " ";
                        }

                        //if (objetivo.ComentariosJefeEvalFinall2 != null)
                        //{
                        //    data[i][8] = objetivo.CalifFinal2.ToString() + " " + objetivo.ComentariosJefeEvalFinall2;
                        //}
                        //else
                        //{
                        //    data[i][8] = objetivo.CalifFinal2.ToString() + " ";
                        //}
                        i = i + 1;
                    }


                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r + 1];
                        DataRow.Height = 20;

                        for (int c = 0; c < data[r].Length; c++)
                        {
                            DataRow.Cells[c].Width = 65F;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.Aqua;
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromName("#004b8d");
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

                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Inicio El procesamiento de su pdf- competencias ", "Objetivos/DownloadPdf ");
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

                    String[] Header = { "Competencias", "Actividades", "Resultados" };
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
                        Competencia.tituloTemp = temp.descripcion;
                        data[i] = new string[3];
                        data[i][0] = Competencia.tituloTemp;
                        data[i][1] = Competencia.actividades;
                        data[i][2] = Competencia.resultados;
                        i = i + 1;
                    }


                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableCompetencias.Rows[r + 1];

                        for (int c = 0; c < data[r].Length; c++)
                        {
                            tableCompetencias.ColumnWidth[c] = 250f;

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

                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Inicio El procesamiento de su pdf- Calificación ", "Objetivos/DownloadPdf ");

                #region Calificacion
                if (Evaluacion.CaliFinal > 0)
                {
                    if (Evaluacion.CaliFinal == 4)
                    {
                        document.Replace("#cuatro1#", "4", true, true);
                    }
                    else
                    {
                        document.Replace("#cuatro1#", "", true, true);
                    }
                    if (Evaluacion.CaliFinal == 3)
                    {
                        document.Replace("#tres1#", "3", true, true);
                    }
                    else
                    {
                        document.Replace("#tres1#", "", true, true);
                    }
                    if (Evaluacion.CaliFinal == 2)
                    {
                        document.Replace("#dos1#", "2", true, true);
                    }
                    else
                    {
                        document.Replace("#dos1#", "", true, true);
                    }
                    if (Evaluacion.CaliFinal == 1)
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

                if (Evaluacion.CaliFinal2 > 0)
                {
                    if (Evaluacion.CaliFinal2 == 4)
                    {
                        document.Replace("#cuatro2#", "4", true, true);
                    }
                    else
                    {
                        document.Replace("#cuatro2#", "", true, true);
                    }
                    if (Evaluacion.CaliFinal2 == 3)
                    {
                        document.Replace("#tres2#", "3", true, true);
                    }
                    else
                    {
                        document.Replace("#tres2#", "", true, true);
                    }
                    if (Evaluacion.CaliFinal2 == 2)
                    {
                        document.Replace("#dos2#", "2", true, true);
                    }
                    else
                    {
                        document.Replace("#dos2#", "", true, true);
                    }
                    if (Evaluacion.CaliFinal2 == 1)
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

                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Inicio El procesamiento de su pdf- Comentarios ", "Objetivos/DownloadPdf ");

                document.Replace("#ComentariosEval#", Evaluacion.ComEvaluado, true, true);
                document.Replace("#Comentariosjefe#", Evaluacion.ComEvaluador, true, true);
                document.Replace("#ComentariosEval2#", Evaluacion.ComEvaluado2, true, true);
                document.Replace("#ComentariosJefe2#", Evaluacion.ComEvaluador2, true, true);

                document.SaveToFile(pathResp.ToString(), Spire.Doc.FileFormat.PDF);

                //esto es para regresarlo al navegador
                byte[] fileBytes = System.IO.File.ReadAllBytes(pathResp);
                string fileNameResp = "Reporte_"+ Evaluado.Login.NombreCompleto.Trim() +".pdf";
                Bitacora.NuevaEntrada("El usuario: " + Evaluado.Login.NombreCompleto + " Logro descargar su  evaluación del periodo " + perido.Llave, "MiHistorial/DownloadPdf ");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameResp);
            }
            catch (Exception ex)
            {

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