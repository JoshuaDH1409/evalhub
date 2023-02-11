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
using Spire.Doc;
using Spire.Doc.Documents;
using System.Drawing;
using Spire.Doc.Fields;

namespace PlantillaMVC.Controllers
{
    public class MiHistorialController : Controller
    {
        // GET: MiHistorial
        public ActionResult Index()
        {
            return View();
        }

        [Autentificado]
        public ActionResult Historial()
        {
            if (!Utilidades.ValidaSesion(Session, HttpContext))
                return PartialView("Respuesta", "Su sesión termino favor de re ingresar al sistema");

            

            Modelo.Clases.CSession sesion = (Modelo.Clases.CSession)Session[Utilidades.session];
            Bitacora.NuevaEntrada("El usuario: " + sesion.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

            List<Modelo.Clases.CHistorial> LiHistorial = Utilidades.negocio.RecuperaHistorialUsr(sesion.Login.id);
            string DatosGraf = "";

            if (LiHistorial.Count > 0)
            {
                foreach (Modelo.Clases.CHistorial Item in LiHistorial)
                {
                    if(Item.Evaluacion.CaliFinalCalibracion!=0)
                        DatosGraf = DatosGraf + "['" + Item.Periodo.Llave + "'," + Item.Evaluacion.CaliFinalCalibracion + "],";
                    else
                        DatosGraf = DatosGraf + "['" + Item.Periodo.Llave + "'," + Item.Evaluacion.CaliFinal2 + "],";
                }
                DatosGraf = DatosGraf.Substring(0, DatosGraf.LastIndexOf(","));
            }
            ViewBag.Graf = DatosGraf;
            return PartialView(LiHistorial);
        }

        #region DownloadPdfOld
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
                EPeriodos periodo = Utilidades.negocio.RecuperaUnPeriodo(Evaluacion.periodo);
                List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Evaluacion.id);
                List<ECompetemces> liCompetencias = Utilidades.negocio.RecuperaLiCompetencias(Evaluacion.id);

                System.IO.File.Copy(path, pathResp, true);//copiamos el documento original al temporal
                object fileName = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");//obtenemos la ruta del archivo copiado
                document.LoadFromFile(fileName.ToString());//se carga el documento que se le hara el replace
                Bitacora.NuevaEntrada("se inicia el PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");


                document.Replace("#Periodo#", periodo.Llave, true, true);
                document.Replace("#Pais#", Utilidades.negocio.RecuperaUnPais(periodo.Country).descripcion, true, true);

                document.Replace("#SapEmpleado#", Evaluado.Login.id_sap, true, true);
                document.Replace("#NombreEmp#",String.Concat(Evaluado.Login.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", periodo.Llave, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#divisionEmp#", Evaluado.Login.Division, true, true);
                //document.Replace("#areaEmp#", Evaluado.login.Area, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#FchEmpini#", TiksToDate(Evaluado.Login.FechaIngreso), true, true);
                document.Replace("#fchEmpAnt#", new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(TiksToDate(Evaluado.Login.FechaIngreso))), true, true);

                document.Replace("#SapJefe#", jefe.id_sap, true, true);
                document.Replace("#Nombrejefe#", String.Concat(jefe.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", periodo.Llave, true, true);
                document.Replace("#puestoJefe#", jefe.Puesto, true, true);
                document.Replace("#divisionjefe#", jefe.Division, true, true);
                //document.Replace("#areaJefe#", jefe.Area, true, true);
                document.Replace("#FchJefeini#", TiksToDate(jefe.FechaIngreso), true, true);
                document.Replace("#FchJefeAnt#", new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(TiksToDate(jefe.FechaIngreso))), true, true);



                Bitacora.NuevaEntrada("se llena Cabecera del PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                #region Objetivos Evaluacion medio año
                if (liObjetivos != null)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos1#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = bodyObjetivos.Paragraphs.IndexOf(paragraphObjetivos);

                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);
                    PreferredWidth width = new PreferredWidth(WidthType.Percentage, 112);
                    tableObjetivos.PreferredWidth = width;
                    tableObjetivos.TableFormat.LeftIndent = -22;
                    //tableObjetivos.AutoFitBehavior(AutoFitBehaviorType.wdAutoFitContents);
                    tableObjetivos.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);

                    String[] Header = { "Objetivo", "Descripción", "Métrica (KPI)", "Peso ponderado %","Comentarios","Cumplimiento %", "Comentarios", "Cumplimiento %" };
                    //, "Auto evaluación (1-4)", "Calificación Evaluador (1-4)", "Calificación final acordada (1-4)"
                    string[][] data = new string[liObjetivos.Count][];
                    tableObjetivos.ResetCells(liObjetivos.Count + 2, 8);
                    

                    TableRow F_row = tableObjetivos.Rows[0];
                    F_row.RowFormat.BackColor = Color.FromArgb(0, 171, 239);
                    F_row.IsHeader = true;

                    tableObjetivos.ApplyHorizontalMerge(0, 0, 3);
                    F_row.Cells[0].CellFormat.Borders.Color = Color.White;
                    tableObjetivos.ApplyHorizontalMerge(0, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(0, 6, 7);
                    Paragraph encabezado = F_row.Cells[4].AddParagraph();
                    encabezado.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    F_row.Cells[4].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    F_row.Cells[4].CellFormat.Borders.Color = Color.White;
                    TextRange TRa = encabezado.AppendText("Evaluado");
                    TRa.CharacterFormat.FontName = "Calibri";
                    TRa.CharacterFormat.TextColor = System.Drawing.Color.White;
                    TRa.CharacterFormat.FontSize = 9;
                    TRa.CharacterFormat.Bold = true;
                    Paragraph encabezado2 = F_row.Cells[6].AddParagraph();
                    F_row.Cells[6].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    F_row.Cells[6].CellFormat.Borders.Color = Color.White;
                    encabezado2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    TextRange TRb = encabezado2.AppendText("Evaluador");
                    TRb.CharacterFormat.FontName = "Calibri";
                    TRb.CharacterFormat.TextColor = System.Drawing.Color.White;
                    TRb.CharacterFormat.FontSize = 9;
                    TRb.CharacterFormat.Bold = true;
                    F_row.RowFormat.Borders.Color = System.Drawing.Color.White;
                    
                    Spire.Doc.TableRow FRow = tableObjetivos.Rows[1];
                    FRow.IsHeader = true;
                    FRow.Height = 35;

                    for (int h = 0; h < Header.Length; h++)
                    {
                        //Cell Alignment
                        FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.White;
                        FRow.Cells[h].CellWidthType=CellWidthType.Auto;// = 300F;
                        Spire.Doc.Documents.Paragraph p = FRow.Cells[h].AddParagraph();
                        FRow.Cells[h].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        //FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.White;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        //TR.CharacterFormat.Border.Color = System.Drawing.Color.White;
                        FRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(0, 171, 239);
                    }
                    int i = 0;
                    foreach (EObjetives objetivo in liObjetivos)
                    {
                        data[i] = new string[8];
                        data[i][0] = objetivo.titulo;
                        data[i][1] = objetivo.objdesc;
                        data[i][2] = objetivo.objMetricas;
                        data[i][3] = objetivo.ponderado.ToString();
                        data[i][4] = objetivo.resultado;
                        data[i][5] = objetivo.cumplimientoEvaluado.ToString();
                        data[i][6] = objetivo.ComentariosJefeEval;
                        data[i][7] = objetivo.cumplimientoEvaluador.ToString();
                        //data[i][5] = objetivo.AutoEval2.ToString() + " " + objetivo.Resultado2;
                        //data[i][6] = objetivo.Calif2.ToString() + " " + objetivo.ComentariosJefeEval2;
                        //data[i][7] = objetivo.CalifFinal2.ToString() + " " + objetivo.ComentariosJefeEvalFinall2;
                        i = i + 1;
                    }

                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r + 2];
                        DataRow.Height = 20;

                        DataRow.RowFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        DataRow.RowFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
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
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                            //DataRow.Cells[c].Width = 75F;
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            if (c==3 || c== 5 || c==7)
                                p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);
                            TR2.CharacterFormat.Border.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                        }
                    }
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    //bodyObjetivos.ChildObjects.Remove(paragraphObjetivos);
                    document.Replace("#Objetivos1#", "", true, true);
                    bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#Objetivos1#", "", true, true);
                }
                #endregion


                #region Objetivos Evaluacion fin de año
                if (liObjetivos != null && periodo.Etapa > 2)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos2#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = bodyObjetivos.Paragraphs.IndexOf(paragraphObjetivos);
                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);
                    tableObjetivos.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    PreferredWidth width = new PreferredWidth(WidthType.Percentage, 112);
                    tableObjetivos.PreferredWidth = width;
                    tableObjetivos.TableFormat.LeftIndent = -22;
                    String[] Header = { "Objetivo", "Descripción", "Métrica (KPI)","Peso ponderado %","Comentarios", "Cumplimiento %", "Comentarios","Cumplimiento %" };
                    //, "Auto evaluación (1-4)", "Calificación Evaluador (1-4)", "Calificación final acordada (1-4)"
                    string[][] data = new string[liObjetivos.Count][];
                    tableObjetivos.ResetCells(liObjetivos.Count + 2, 8);

                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);

                    TableRow F_row = tableObjetivos.Rows[0];
                    F_row.RowFormat.Borders.Color = Color.White;
                    F_row.RowFormat.BackColor = Color.FromArgb(0, 171, 239);
                    F_row.IsHeader = true;
                    tableObjetivos.ApplyHorizontalMerge(0, 0, 3);
                    F_row.Cells[0].CellFormat.Borders.Color = Color.White;
                    tableObjetivos.ApplyHorizontalMerge(0, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(0, 6, 7);
                    Paragraph encabezado= F_row.Cells[4].AddParagraph();
                    F_row.Cells[4].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    F_row.Cells[4].CellFormat.Borders.Color = Color.White;
                    encabezado.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    TextRange TRa = encabezado.AppendText("Evaluado");
                    TRa.CharacterFormat.FontName = "Calibri";
                    TRa.CharacterFormat.TextColor = System.Drawing.Color.White;
                    TRa.CharacterFormat.FontSize = 9;
                    TRa.CharacterFormat.Bold = true;
                    Paragraph encabezado2 = F_row.Cells[6].AddParagraph();
                    F_row.Cells[6].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    F_row.Cells[6].CellFormat.Borders.Color = Color.White;
                    encabezado2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    TextRange TRb = encabezado2.AppendText("Evaluador");
                    TRb.CharacterFormat.FontName = "Calibri";
                    TRb.CharacterFormat.TextColor = System.Drawing.Color.White;
                    TRb.CharacterFormat.FontSize = 9;
                    TRb.CharacterFormat.Bold = true;

                    Spire.Doc.TableRow FRow = tableObjetivos.Rows[1];
                    FRow.IsHeader = true;
                    FRow.Height = 35;


                    for (int h = 0; h < Header.Length; h++)
                    {
                        //Cell Alignment

                        FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.White;
                        FRow.Cells[h].CellWidthType = CellWidthType.Auto;
                        Spire.Doc.Documents.Paragraph p = FRow.Cells[h].AddParagraph();
                        FRow.Cells[h].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;

                        TR.CharacterFormat.Border.Color = System.Drawing.Color.White;

                        FRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(0, 171, 239);

                    }
                    int i = 0;
                    foreach (EObjetives objetivo in liObjetivos)
                    {
                        data[i] = new string[8];

                        /*if (objetivo.titulo.Length > 55)
                            data[i][0] = objetivo.titulo.Substring(55) + "...";
                        else*/
                        data[i][0] = objetivo.titulo;

                        /*if (objetivo.objdesc.Length > 55)
                            data[i][1] = objetivo.objdesc.Substring(50)+"...";
                        else*/
                        data[i][1] = objetivo.objdesc;

                        /*if(objetivo.objMetricas.Length>55)
                             data[i][2] = objetivo.objMetricas.Substring(55)+"...";
                        else*/
                        data[i][2] = objetivo.objMetricas;
                        data[i][3] = objetivo.ponderado.ToString();
                        data[i][4] = objetivo.Resultado2.ToString();
                        data[i][5] = objetivo.cumplimientoEvaluado2.ToString();
                        data[i][6] = objetivo.ComentariosJefeEval2.ToString();
                        data[i][7] = objetivo.cumplimientoEvaluador2.ToString();

                        //if (!String.IsNullOrEmpty(objetivo.Resultado2))
                        //    data[i][5] = /*"Calif: " + objetivo.AutoEval2.ToString() +*/ "Comentario: " + objetivo.Resultado2;
                        //else
                        //    data[i][5] = "";

                        //if (objetivo.Calif2 > 0)
                        //    data[i][6] ="Calificación: "+ objetivo.Calif2.ToString() + "\nComentario: " + objetivo.ComentariosJefeEval2;
                        //else
                        //    data[i][6] = "";

                        //if (objetivo.CalifFinal2 > 0)
                        //    data[i][7] = objetivo.CalifFinal2.ToString() + " " + objetivo.ComentariosJefeEvalFinall2;
                        //else
                        //    data[i][7] = "";

                        i = i + 1;
                    }

                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r + 2];
                        DataRow.Height = 20;
                        DataRow.RowFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
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

                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            if (c==3 || c==5 || c==7)
                                p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);
                            TR2.CharacterFormat.Border.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                        }
                    }
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    //bodyObjetivos.ChildObjects.Remove(paragraphObjetivos);
                    document.Replace("#Objetivos2#", "", true, true);
                    bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#Objetivos2#", "", true, true);
                }
                #endregion
                #region Peso ponderado
                document.Replace("#PesoEva#", liObjetivos.Sum(t => t.ValorObj).ToString(), true, true);
                document.Replace("#Peso#", liObjetivos.Sum(t => t.ponderado).ToString(), true, true);
                #endregion

                Bitacora.NuevaEntrada("llenado de objetivos de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

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
                    tableCompetencias.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    tableCompetencias.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    PreferredWidth width = new PreferredWidth(WidthType.Percentage, 112);
                    tableCompetencias.PreferredWidth = width;
                    tableCompetencias.TableFormat.LeftIndent = -22;
                    String[] Header = { "Competencia", "Actividades", "Resultados Mitad Año", "Resultados Fin de Año" };
                    string[][] data = new string[liCompetencias.Count][];
                    tableCompetencias.ResetCells(liCompetencias.Count + 1, 4);

                    Spire.Doc.TableRow FRow = tableCompetencias.Rows[0];
                    FRow.IsHeader = true;
                    FRow.Height = 20;
                    //FRow.Cells[0].SetCellWidth(55, Spire.Doc.CellWidthType.Percentage);
                    //FRow.Cells[1].SetCellWidth(55, Spire.Doc.CellWidthType.Percentage);
                    //FRow.Cells[2].SetCellWidth(55, Spire.Doc.CellWidthType.Percentage);

                    FRow.RowFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    for (int h = 0; h < Header.Length; h++)
                    {
                        FRow.Cells[h].Width = 390F;
                        FRow.Cells[h].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                        Spire.Doc.Documents.Paragraph p = FRow.Cells[h].AddParagraph();
                        FRow.Cells[h].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Top;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        FRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(0, 171, 239);

                    }


                    int i = 0;

                    foreach (ECompetemces Competencia in liCompetencias)
                    {
                        ECatComp temp = Utilidades.negocio.RecuperaUnaCatCompetencias(Competencia.titulo);
                        Competencia.tituloTemp = temp.descripcion;
                        data[i] = new string[4];
                        data[i][0] = Competencia.tituloTemp;
                        data[i][1] = Competencia.actividades;
                        data[i][2] = "Comentario evaluado: "+Competencia.resultados+Environment.NewLine+"Comentario evaluador: "+Competencia.resultadosEvaluador;
                        data[i][3] = "Comentario evaluado: " + Competencia.resultados2 + Environment.NewLine + "Comentario evaluador: " + Competencia.resultados2Evaluador;
                        i = i + 1;
                    }


                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableCompetencias.Rows[r + 1];
                        DataRow.RowFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
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
                            tableCompetencias.ColumnWidth[c] = 270F;

                            //Cell Alignment
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                            DataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                            DataRow.Cells[c].CellFormat.FitText = false; //llenado de la tabla
                            //DataRow.Cells[c].CellFormat.BackColor = System.Drawing.Color.SkyBlue;
                            //Fill Data in Rows
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);

                        }
                    }
                    tableCompetencias.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    //bodyCompetencias.ChildObjects.Remove(paragraphCompetencias);                    
                    bodyCompetencias.ChildObjects.Insert(index, tableCompetencias);
                    document.Replace("#XXCompetenciasXX#", "", true, true);//ojo poner siempre despues de llenar las tablas
                }
                else
                {
                    document.Replace("#XXCompetenciasXX#", "", true, true);
                }
                #endregion

                Bitacora.NuevaEntrada("llenado de competencias de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

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
                    if (Evaluacion.CaliFinal == 0)
                    {
                        document.Replace("#uno0#", "0", true, true);
                    }
                    else
                    {
                        document.Replace("#uno0#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro1#", "", true, true);
                    document.Replace("#tres1#", "", true, true);
                    document.Replace("#dos1#", "", true, true);
                    document.Replace("#uno1#", "", true, true);
                    document.Replace("#cero1#", "", true, true);
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
                    if (Evaluacion.CaliFinal2 == 0)
                    {
                        document.Replace("#cero2#", "0", true, true);
                    }
                    else
                    {
                        document.Replace("#cero2#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro2#", "", true, true);
                    document.Replace("#tres2#", "", true, true);
                    document.Replace("#dos2#", "", true, true);
                    document.Replace("#uno2#", "", true, true);
                    document.Replace("#cero2#", "", true, true);
                }
                if (Evaluado.Login.calibracion)
                {
                    document.Replace("#calibraciontitle#","Calibración",true,true);
                    Paragraph paragraph = document.CreateParagraph();
                    
                    document.Replace("#CalibracionComentarios#", Evaluacion.MotivoCalibracion2, true, true);
                    
                    if (Evaluacion.CaliFinalCalibracion == 4)
                    {
                        document.Replace("#cuatro3#", "4", true, true);
                    }
                    else
                    {
                        document.Replace("#cuatro3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 3)
                    {
                        document.Replace("#tres3#", "3", true, true);
                    }
                    else
                    {
                        document.Replace("#tres3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 2)
                    {
                        document.Replace("#dos3#", "2", true, true);
                    }
                    else
                    {
                        document.Replace("#dos3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 1)
                    {
                        document.Replace("#uno3#", "1", true, true);
                    }
                    else
                    {
                        document.Replace("#uno3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 0)
                    {
                        document.Replace("#cero3#", "0", true, true);
                    }
                    else
                    {
                        document.Replace("#cero3#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro3#", "", true, true);
                    document.Replace("#tres3#", "", true, true);
                    document.Replace("#dos3#", "", true, true);
                    document.Replace("#uno3#", "", true, true);
                    document.Replace("#cero3#", "", true, true);
                    document.Replace("#CalibracionComentarios#", "N/A", true, true);
                    document.Replace("#calibraciontitle#", "", true, true);
                }
                #endregion

                Bitacora.NuevaEntrada("llenado de Calificación de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                document.Replace("#ComentariosEval#", Evaluacion.ComEvaluado, true, true);
                document.Replace("#Comentariosjefe#", Evaluacion.ComEvaluador, true, true);
                document.Replace("#Rechazojefe#", Evaluacion.MotivoRechazoObj2, true, true);
                document.Replace("#ComentariosEval2#", Evaluacion.ComEvaluado2, true, true);
                document.Replace("#ComentariosJefe2#", Evaluacion.ComEvaluador2, true, true);
                document.Replace("#RechazoJefe2#", Evaluacion.MotivoRechazoObjFin, true, true);

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
        #endregion

        #region Formato diferente
        public ActionResult DownloadPdfDetailed(int Id)
        {
            string path = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "DocTest.docx");
            string pathResp = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.docx");
            string pathpdf = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp.pdf");

            try
            {
                Spire.Doc.Document document = new Spire.Doc.Document();
                EEval Evaluacion = Utilidades.negocio.RecuperaEvaluacionIdHistorial(Id);
                Modelo.Clases.CSessionEval Evaluado = Utilidades.negocio.RecuperaUsuarioEval(Evaluacion.id_usuario);

                ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(Evaluado.Login.EvaluadorIdSap);
                EPeriodos periodo = Utilidades.negocio.RecuperaUnPeriodo(Evaluacion.periodo);
                List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Evaluacion.id);
                List<ECompetemces> liCompetencias = Utilidades.negocio.RecuperaLiCompetencias(Evaluacion.id);

                object fileName = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp-2.docx");//obtenemos la ruta del archivo copiado
                document.LoadFromFile(fileName.ToString());//se carga el documento que se le hara el replace
                Bitacora.NuevaEntrada("se inicia el PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                document.Replace("#Periodo#", periodo.Llave, true, true);
                document.Replace("#Pais#", Utilidades.negocio.RecuperaUnPais(periodo.Country).descripcion, true, true);

                document.Replace("#SapEmpleado#", Evaluado.Login.id_sap, true, true);
                document.Replace("#NombreEmp#", String.Concat(Evaluado.Login.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", periodo.Llave, true, true);
                document.Replace("#PuestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#divisionEmp#", Evaluado.Login.Division, true, true);
                //document.Replace("#areaEmp#", Evaluado.login.Area, true, true);
                //document.Replace("#PuestoEmp#", Evaluado.login.Puesto, true, true);
                document.Replace("#FchEmpini#", TiksToDate(Evaluado.Login.FechaIngreso), true, true);
                document.Replace("#fchEmpAnt#", new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(TiksToDate(Evaluado.Login.FechaIngreso))), true, true);

                document.Replace("#SapJefe#", jefe.id_sap, true, true);
                document.Replace("#Nombrejefe#", String.Concat(jefe.NombreCompleto), true, true);
                document.Replace("#llavePeriodo#", periodo.Llave, true, true);
                document.Replace("#puestoJefe#", jefe.Puesto, true, true);
                document.Replace("#divisionjefe#", jefe.Division, true, true);
                //document.Replace("#areaJefe#", jefe.Area, true, true);
                document.Replace("#FchJefeini#", TiksToDate(jefe.FechaIngreso), true, true);
                document.Replace("#FchJefeAnt#", new DateUtils().DateDifference(DateTime.Today, Convert.ToDateTime(TiksToDate(jefe.FechaIngreso))), true, true);

                Bitacora.NuevaEntrada("se llena Cabecera del PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                #region Objetivos
                if (liObjetivos != null)
                {

                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos1#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = bodyObjetivos.Paragraphs.IndexOf(paragraphObjetivos);

                    //Nandarek estilos para agregar
                    ParagraphStyle style = new ParagraphStyle(document);
                    style.Name = "RowHeaderStyle";
                    style.CharacterFormat.FontSize = 12;
                    style.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255);
                    document.Styles.Add(style);

                    //Nandarek estilos para agregar
                    ParagraphStyle styleHead2 = new ParagraphStyle(document);
                    styleHead2.Name = "ColumHeaderStyle";
                    styleHead2.CharacterFormat.FontSize = 12;
                    styleHead2.CharacterFormat.Bold = true;
                    document.Styles.Add(styleHead2);
                    int rows = 6;
                    int cells = 3;


                    foreach (EObjetives objetivo in liObjetivos)
                    {
                        Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);
                        PreferredWidth width = new PreferredWidth(WidthType.Percentage, 112);
                        tableObjetivos.PreferredWidth = width;
                        tableObjetivos.TableFormat.LeftIndent = -22;
                        if (objetivo.ComentariosJefeEval2 != "" || objetivo.Resultado2 != "")
                        {
                            rows = 13;
                        }
                        else if (objetivo.resultado != "")
                        {
                            rows = 9;
                        }
                        tableObjetivos.ResetCells(rows, cells);

                        tableObjetivos[0, 0].AddParagraph().AppendText(objetivo.titulo);
                        tableObjetivos.ApplyHorizontalMerge(0, 0, 2);
                        tableObjetivos.Rows[1].Cells[0].Width = 60F;
                        tableObjetivos.Rows[2].Cells[0].Width = 60F;
                        tableObjetivos.Rows[3].Cells[0].Width = 70F;
                        tableObjetivos.ApplyHorizontalMerge(1, 1, 2);
                        tableObjetivos.ApplyHorizontalMerge(2, 1, 2);
                        tableObjetivos.ApplyHorizontalMerge(3, 1, 2);
                        //tableObjetivos.AutoFitBehavior(AutoFitBehaviorType.wdAutoFitWindow);
                        tableObjetivos.Rows[0].RowFormat.BackColor = Color.FromArgb(0, 75, 140);

                        tableObjetivos.Rows[0].Cells[0].Paragraphs[0].ApplyStyle(style.Name);

                        tableObjetivos[1, 0].AddParagraph().AppendText("Descripción");
                        tableObjetivos[1, 1].AddParagraph().AppendText(objetivo.objdesc.Trim());
                        tableObjetivos[2, 0].AddParagraph().AppendText("Métrica (KPI)");
                        tableObjetivos[2, 1].AddParagraph().AppendText(objetivo.objMetricas.Trim());
                        tableObjetivos[3, 0].AddParagraph().AppendText("Peso ponderado %");
                        tableObjetivos[3, 1].AddParagraph().AppendText(objetivo.ponderado.ToString());


                        tableObjetivos.Rows[1].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                        tableObjetivos.Rows[2].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                        tableObjetivos.Rows[3].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);

                        if (rows > 6)
                        {
                            tableObjetivos[4, 0].AddParagraph().AppendText("Evaluación de Medio Año");
                            tableObjetivos.ApplyHorizontalMerge(4, 0, 2);
                            tableObjetivos.Rows[4].RowFormat.BackColor = Color.FromArgb(216, 216, 216);
                            tableObjetivos.Rows[4].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);

                            tableObjetivos[5, 0].AddParagraph().AppendText("Participantes");
                            tableObjetivos.Rows[5].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos[5, 1].AddParagraph().AppendText("Comentarios");
                            tableObjetivos[5, 2].AddParagraph().AppendText("Cumplimiento %");
                            tableObjetivos.Rows[5].Cells[2].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos.Rows[5].Cells[2].Paragraphs[0].Format.HorizontalAlignment = HorizontalAlignment.Center;
                            tableObjetivos.Rows[5].Cells[1].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos.Rows[5].Cells[1].Paragraphs[0].Format.HorizontalAlignment = HorizontalAlignment.Center;

                            tableObjetivos[6, 0].AddParagraph().AppendText("Evaluado");
                            tableObjetivos[6, 1].AddParagraph().AppendText(objetivo.resultado.Trim());
                            tableObjetivos[6, 2].AddParagraph().AppendText(objetivo.cumplimientoEvaluado.ToString());
                            tableObjetivos.Rows[6].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);

                            tableObjetivos[7, 0].AddParagraph().AppendText("Evaluador");
                            tableObjetivos[7, 1].AddParagraph().AppendText(objetivo.ComentariosJefeEval.Trim());
                            tableObjetivos[7, 2].AddParagraph().AppendText(objetivo.cumplimientoEvaluador.ToString());
                            tableObjetivos.Rows[7].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            //tableObjetivos.ApplyHorizontalMerge(5, 1, 2);
                            //tableObjetivos.ApplyHorizontalMerge(6, 1, 2);
                            //tableObjetivos.Rows[4].Cells[2].Width = 5F;
                            //tableObjetivos.Rows[5].Cells[2].Width = 5F;
                        }
                        if (rows > 10)
                        {
                            tableObjetivos[8, 0].AddParagraph().AppendText("Evaluación de Fin de Año");
                            tableObjetivos.ApplyHorizontalMerge(8, 0, 2);
                            tableObjetivos.Rows[8].RowFormat.BackColor = Color.FromArgb(216, 216, 216);
                            tableObjetivos.Rows[8].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);

                            tableObjetivos[9, 0].AddParagraph().AppendText("Participantes");
                            tableObjetivos.Rows[9].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos[9, 1].AddParagraph().AppendText("Comentarios");
                            tableObjetivos[9, 2].AddParagraph().AppendText("Cumplimiento %");

                            tableObjetivos[10, 0].AddParagraph().AppendText("Evaluado");
                            tableObjetivos.Rows[10].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos[10, 1].AddParagraph().AppendText(objetivo.Resultado2.Trim());
                            tableObjetivos[10, 2].AddParagraph().AppendText(objetivo.cumplimientoEvaluado2.ToString());
                            //tableObjetivos.Rows[8].Cells[2].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            //tableObjetivos.Rows[8].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);

                            tableObjetivos[11, 0].AddParagraph().AppendText("Evaluador");
                            tableObjetivos.Rows[11].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos[11, 1].AddParagraph().AppendText(objetivo.ComentariosJefeEval2.Trim());
                            tableObjetivos[11, 2].AddParagraph().AppendText(objetivo.cumplimientoEvaluador2.ToString());
                            tableObjetivos.Rows[9].Cells[2].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos.Rows[9].Cells[2].Paragraphs[0].Format.HorizontalAlignment = HorizontalAlignment.Center;
                            tableObjetivos.Rows[9].Cells[1].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            tableObjetivos.Rows[9].Cells[1].Paragraphs[0].Format.HorizontalAlignment = HorizontalAlignment.Center;

                            //tableObjetivos[10, 0].AddParagraph().AppendText("Peso ponderado (%)");
                            //tableObjetivos[10, 1].AddParagraph().AppendText(objetivo.cumplimientoEvaluador2.ToString());
                            //tableObjetivos.Rows[10].Cells[0].Paragraphs[0].ApplyStyle(styleHead2.Name);
                            //tableObjetivos.ApplyHorizontalMerge(10, 1, 2);
                            //tableObjetivos.Rows[8].Cells[2].Width = 5F;
                            //tableObjetivos.Rows[9].Cells[2].Width = 5F;
                        }

                        tableObjetivos.Rows[rows - 1].RowFormat.BackColor = System.Drawing.Color.AliceBlue;
                        tableObjetivos.ApplyHorizontalMerge(rows - 1, 0, 2);

                        //tableObjetivos.AutoFitBehavior(AutoFitBehaviorType.wdAutoFitWindow);
                        bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                        index = bodyObjetivos.ChildObjects.IndexOf(tableObjetivos);
                        index++;
                    }
                    //tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                    document.Replace("#Objetivos1#", "", true, true);
                    //bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#Objetivos1#", "", true, true);
                }
                #endregion
                #region Peso ponderado
                document.Replace("#Peso#", liObjetivos.Sum(t => t.ponderado).ToString(), true, true);
                //if(Evaluacion.Status==14)//Evaluacion cerrada
                document.Replace("#PesoEva#", liObjetivos.Sum(t => t.ValorObj).ToString(), true, true);
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

                    //Nandarek estilos para agregar
                    ParagraphStyle styleC = new ParagraphStyle(document);
                    styleC.Name = "RowHeaderCompStyle";
                    styleC.CharacterFormat.FontSize = 12;
                    styleC.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255);
                    document.Styles.Add(styleC);

                    //Nandarek estilos para agregar
                    ParagraphStyle styleHead2C = new ParagraphStyle(document);
                    styleHead2C.Name = "ColumHeaderCompStyle";
                    styleHead2C.CharacterFormat.FontSize = 12;
                    styleHead2C.CharacterFormat.Bold = true;
                    document.Styles.Add(styleHead2C);
                    int rowsC = 2;
                    int cellsC = 2;

                    foreach (ECompetemces Competencia in liCompetencias)
                    {
                        if (!String.IsNullOrEmpty(Competencia.resultados))
                        {
                            rowsC = 5;
                            cellsC = 2;
                        }
                        if (!String.IsNullOrEmpty(Competencia.resultados2))
                        {
                            rowsC = 8;
                            cellsC = 2;
                        }
                        Spire.Doc.Table tableCompetence = sectionCompetencias.AddTable(true);
                        PreferredWidth width = new PreferredWidth(WidthType.Percentage, 112);
                        tableCompetence.PreferredWidth = width;
                        tableCompetence.TableFormat.LeftIndent = -22;

                        tableCompetence.ResetCells(rowsC, cellsC);

                        tableCompetence.Rows[0].RowFormat.BackColor = Color.FromArgb(0, 75, 140);

                        tableCompetence[0, 0].AddParagraph().AppendText(Utilidades.negocio.RecuperaUnaCatCompetencias(Competencia.titulo).descripcion);
                        tableCompetence.ApplyHorizontalMerge(0, 0, 1);
                        tableCompetence.Rows[0].Cells[0].Paragraphs[0].ApplyStyle(styleC.Name);

                        tableCompetence[1, 0].AddParagraph().AppendText("Actividades");
                        tableCompetence.Rows[1].Cells[0].Width = 50F;
                        tableCompetence[1, 1].AddParagraph().AppendText(Competencia.actividades);

                        if (rowsC > 3)
                        {
                            tableCompetence[2, 0].AddParagraph().AppendText("Resultados de medio año");
                            tableCompetence.ApplyHorizontalMerge(2, 0, 1);
                            tableCompetence.Rows[2].RowFormat.BackColor = Color.FromArgb(216, 216, 216);
                            tableCompetence.Rows[2].Cells[0].Paragraphs[0].ApplyStyle(styleHead2C.Name);

                            tableCompetence[3, 0].AddParagraph().AppendText("Comentario evaluado");
                            tableCompetence[3, 1].AddParagraph().AppendText(Competencia.resultados);
                            tableCompetence[4, 0].AddParagraph().AppendText("Comentario evaluador");
                            tableCompetence[4, 1].AddParagraph().AppendText(Competencia.resultadosEvaluador);

                            if (rowsC > 5)
                            {
                                tableCompetence[5, 0].AddParagraph().AppendText("Resultados de fin de año");
                                tableCompetence.ApplyHorizontalMerge(5, 0, 1);
                                tableCompetence.Rows[5].RowFormat.BackColor = Color.FromArgb(216, 216, 216);
                                tableCompetence.Rows[5].Cells[0].Paragraphs[0].ApplyStyle(styleHead2C.Name);

                                tableCompetence[6, 0].AddParagraph().AppendText("Comentario evaluado");
                                tableCompetence[6, 1].AddParagraph().AppendText(Competencia.resultados2);
                                tableCompetence[7, 0].AddParagraph().AppendText("Comentario evaluador");
                                tableCompetence[7, 1].AddParagraph().AppendText(Competencia.resultados2Evaluador);
                            }
                        }

                        bodyCompetencias.ChildObjects.Insert(index, tableCompetence);
                        index = bodyCompetencias.ChildObjects.IndexOf(tableCompetence);
                        index++;
                    }

                    document.Replace("#XXCompetenciasXX#", "", true, true);//ojo poner siempre despues de llenar las tablas
                }
                else
                {
                    document.Replace("#XXCompetenciasXX#", "", true, true);
                }
                #endregion

                Bitacora.NuevaEntrada("llenado de competencias de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

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
                    document.Replace("#cero1#", "", true, true);
                }

                if (Evaluacion.CaliFinal2 > 0 && Evaluacion.CaliFinalCalibracion == 0)
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
                    if (Evaluacion.CaliFinal2 == 0)
                    {
                        document.Replace("#cero2#", "0", true, true);
                    }
                    else
                    {
                        document.Replace("#cero2#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro2#", "", true, true);
                    document.Replace("#tres2#", "", true, true);
                    document.Replace("#dos2#", "", true, true);
                    document.Replace("#uno2#", "", true, true);
                    document.Replace("#cero2#", "", true, true);
                }

                if (Evaluacion.CaliFinalCalibracion > 0)
                {
                    if (Evaluacion.CaliFinalCalibracion == 4)
                    {
                        document.Replace("#cuatro3#", "4", true, true);
                    }
                    else
                    {
                        document.Replace("#cuatro3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 3)
                    {
                        document.Replace("#tres3#", "3", true, true);
                    }
                    else
                    {
                        document.Replace("#tres3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 2)
                    {
                        document.Replace("#dos3#", "2", true, true);
                    }
                    else
                    {
                        document.Replace("#dos3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 1)
                    {
                        document.Replace("#uno3#", "1", true, true);
                    }
                    else
                    {
                        document.Replace("#uno3#", "", true, true);
                    }
                    if (Evaluacion.CaliFinalCalibracion == 0)
                    {
                        document.Replace("#cero3#", "0", true, true);
                    }
                    else
                    {
                        document.Replace("#cero3#", "", true, true);
                    }
                }
                else
                {
                    document.Replace("#cuatro3#", "", true, true);
                    document.Replace("#tres3#", "", true, true);
                    document.Replace("#dos3#", "", true, true);
                    document.Replace("#uno3#", "", true, true);
                    document.Replace("#cero3#", "", true, true);
                }
                #endregion

                Bitacora.NuevaEntrada("llenado de Calificación de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");

                document.Replace("#ComentariosEval#", Evaluacion.ComEvaluado, true, true);
                document.Replace("#Comentariosjefe#", Evaluacion.ComEvaluador, true, true);
                document.Replace("#ComentariosEval2#", Evaluacion.ComEvaluado2, true, true);
                document.Replace("#ComentariosJefe2#", Evaluacion.ComEvaluador2, true, true);
                document.Replace("#RechazoJefe#", Evaluacion.MotivoRechazoObj2, true, true);
                document.Replace("#RechazoJefe2#", Evaluacion.MotivoRechazoObjFin, true, true);
                if(!String.IsNullOrEmpty(Evaluacion.MotivoCalibracion2))
                    document.Replace("#ComentariosCalibracion#", Evaluacion.MotivoCalibracion2, true, true);
                else
                    document.Replace("#ComentariosCalibracion#", "", true, true);

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