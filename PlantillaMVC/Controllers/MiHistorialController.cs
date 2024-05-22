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
using static Spire.Pdf.General.Render.Decode.Jpeg2000.j2k.codestream.HeaderInfo;
using DocumentFormat.OpenXml.Drawing.Charts;

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
                        ECatSubComp subComp = new ECatSubComp();
                        if (Competencia.Subtitulo != 0)
                            subComp = Utilidades.negocio.RecuperaCatSubCompetencia(Competencia.Subtitulo);
                        subComp.SubCompetencia = subComp.SubCompetencia != null ? subComp.SubCompetencia : string.Empty;
                        Competencia.tituloTemp = $"{temp.descripcion} | {subComp.SubCompetencia}";
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


        /// <summary>
        /// Nuevo requerimiento 29-04-2024
        /// Descargar en formato PDF la plantilla en excel => formato_excel_ejemplo_
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        #region DounLoadPDFNew
        public ActionResult DownloadPdfNew(int Id, int typeLanguaje)
        {

            //IDIOMA
            int idiomaSP = 0;
            int idiomaEN = 1;

            int idiomaDOC = typeLanguaje;

            string firmaJefe = "";
            string firmaEmpleado = "";
            if(idiomaDOC == idiomaSP)
            {
                firmaJefe = "Firma Jefe";
                firmaEmpleado = "Firma Empleado";
            }
            else
            {
                firmaJefe = "Line Manager Signature";
                firmaEmpleado = "Employee Signature";
            }

            string path = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "DocTest_new.docx");
            string pathResp = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp_new.docx");
            string pathpdf = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp_new.pdf");

            try
            {

                Spire.Doc.Document document = new Spire.Doc.Document();
                EEval Evaluacion = Utilidades.negocio.RecuperaEvaluacionIdHistorial(Id);
                Modelo.Clases.CSessionEval Evaluado = Utilidades.negocio.RecuperaUsuarioEval(Evaluacion.id_usuario);

                ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(Evaluado.Login.EvaluadorIdSap);
                EPeriodos periodo = Utilidades.negocio.RecuperaUnPeriodo(Evaluacion.periodo);

                //SOLO PARA PRUEBAS ESPECIFICAS
                //Evaluacion.id = 3357;

                List<EObjetives> liObjetivos = Utilidades.negocio.RecuperaListaObjetivos(Evaluacion.id);
                List<EPersonalTP> liPersonalPTP = Utilidades.negocio.RecuperaListaObjetivosPTP(Evaluacion.id);
                List<EPersonalDP> liPersonalPDP = Utilidades.negocio.RecuperaListaObjetivosPDP(Evaluacion.id);

                System.IO.File.Copy(path, pathResp, true);//copiamos el documento original al temporal
                //System.IO.File.Copy(path, pathRespNew, true);//copiamos el documento original al temporal

                object fileName = Path.Combine(Server.MapPath("~/PlantillasReportes/"), "Resp_new.docx");//obtenemos la ruta del archivo copiado


                document.LoadFromFile(fileName.ToString());//se carga el documento que se le hara el replace
                Bitacora.NuevaEntrada("se inicia el PDF de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto + " Ingreso a Historial", "MiHistorial/Historial ");


                //IDIOMA INFORMACIÓN



                //PRUBAS NUEVO CÓDIGO

                #region DatosUsuario
                string label_Titulo = "";
                string label_claveUnidad = "";
                string label_nombreEmpleado = "";
                string label_apellidoEmpleado = "";
                string label_numEmp = "";
                string label_puestoEmp = "";
                string label_departamento = "";
                string label_nombreJefe = "";

                if(idiomaDOC == idiomaSP)
                {
                    label_Titulo = "FORMATO INDIVIDUAL DE REVISIÓN DE DESEMPEÑO";
                    label_claveUnidad = "Unidad de Negocio";
                    label_nombreEmpleado = "Nombre";
                    label_apellidoEmpleado = "Apellido";
                    label_numEmp = "Numero de Empleado";
                    label_puestoEmp = "Nombre del Puesto";
                    label_departamento = "Área";
                    label_nombreJefe = "Nombre del Jefe Inmediato";
                }
                else
                {
                    label_Titulo = "INDIVIDUAL PERFORMANCE REVIEW";
                    label_claveUnidad = "Business Unit:";
                    label_nombreEmpleado = "Employee Name";
                    label_apellidoEmpleado = "Employee Surname";
                    label_numEmp = "Employee Number";
                    label_puestoEmp = "Job Title";
                    label_departamento = "Department";
                    label_nombreJefe = "Line Manager";
                }



                document.Replace("#label_Titulo#", label_Titulo, true, true);
                document.Replace("#label_claveUnidad#", label_claveUnidad, true, true);
                document.Replace("#label_nombreEmpleado#", label_nombreEmpleado, true, true);
                document.Replace("#label_apellidoEmpleado#", label_apellidoEmpleado, true, true);
                document.Replace("#label_numEmp#", label_numEmp, true, true);
                document.Replace("#label_puestoEmp#", label_puestoEmp, true, true);
                document.Replace("#label_departamento#", label_departamento, true, true);
                document.Replace("#label_nombreJefe#", label_nombreJefe, true, true);


                document.Replace("#claveUnidad#", "S-LATAM", true, true);
                document.Replace("#nombreEmpleado#", Evaluado.Login.Nombre, true, true);
                document.Replace("#apellidoEmpleado#", Evaluado.Login.ApellidoPat + " " + Evaluado.Login.ApellidoMat, true, true);
                document.Replace("#numEmpleado#", Evaluado.Login.id_sap, true, true);
                document.Replace("#puestoEmp#", Evaluado.Login.Puesto, true, true);
                document.Replace("#departamento#", Evaluado.Login.Division, true, true);
                document.Replace("#nombreJefe#", String.Concat(jefe.NombreCompleto), true, true);
                #endregion

                ////
                #region Objetivos Evaluacion medio año
                if (liObjetivos != null)
                {
                    #region DECLARACION
                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos1#", true, true);
                    Spire.Doc.Fields.TextRange rangeObjetivos = selectionObjetivos.GetAsOneRange();
                    Spire.Doc.Documents.Paragraph paragraphObjetivos = rangeObjetivos.OwnerParagraph;
                    Spire.Doc.Body bodyObjetivos = paragraphObjetivos.OwnerTextBody;
                    int index = bodyObjetivos.Paragraphs.IndexOf(paragraphObjetivos);

                    Spire.Doc.Table tableObjetivos = sectionObjetivos.AddTable(true);
                    PreferredWidth width = new PreferredWidth(WidthType.Percentage, 112);
                    tableObjetivos.PreferredWidth = width;
                    tableObjetivos.TableFormat.LeftIndent = -42;
                    //tableObjetivos.AutoFitBehavior(AutoFitBehaviorType.wdAutoFitContents);
                    tableObjetivos.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);

                    #region IDIOMA ENCABEZADOS
                    //Configuracion de idioma

                    //INICIO ENCABEZADOS
                    String[] Header;
                    String[] HeaderEN = {
                        "#", "KEYPERFORMANCEAREA (KPA)", "KEY PERFORMANCE INDICATOR (KPI) Specific objective(s) to be achieved per each KPA",
                        "WEIGHTING Total of 100%", "Self Assessment Comments Completed by: Employee", "Mid-Year Comments Completed by: Manager %",
                        "Self Assessment Rating Completed by: Employee", "Justification of Rating Completed by: Employee", "Final Rating Completed by: Manager", "Final Justification of Rating Completed by: Manager" };
                    String[] HeaderSP = {
                        "#", "ÁREA DE RENDIMIENTO CLAVE (KPA)", "INDICADOR DE RENDIMIENTO CLAVE (KPI) Objetivo(s) específico(s) a lograr por cada KPA",
                        "PONDERACIÓN Total de 100%", "Comentarios de Autoevaluación Completado por: Empleado", "Comentarios de Medio Año Completado por: Gerente %",
                        "Calificación de Autoevaluación Completado por: Empleado", "Justificación de la Calificación Completado por: Empleado", "Calificación Final Completado por: Gerente", "Justificación Final de la Calificación Completado por: Gerente"
                    };
                    
                    String[] HeaderUno;
                    String[] HeaderUnoSP = { "Configuración de Objetivos", "Revisión de Medio Año", "Revisión Anual Final" };
                    String[] HeaderUnoEN = { "Objetivos Settings", "Mid-Year Review", "Final Annual Review" };

                    String[] HeaderDos;
                    String[] HeaderDosSP = { "Periodo KPI", "Julio 2023 a Junio 2024", "Fecha de Revisión de Medio Año (DD/MMM/AA)", "Fecha de Revisión Anual Final (DD/MMM/AA)" };
                    String[] HeaderDosEN = { "KPI Period", "July 2023 to June 2024", "Mid-Year Review Date (DD/MMM/YY)", "Final Annual Review Date (DD/MMM/YY)" };

                    if (idiomaDOC == idiomaSP)
                    {
                        Header = HeaderSP;
                        HeaderUno = HeaderUnoSP;
                        HeaderDos = HeaderDosSP;
                    }
                    else
                    {
                        Header = HeaderEN;
                        HeaderUno = HeaderUnoEN;
                        HeaderDos = HeaderDosEN;
                    }
                    #endregion



                    //tamaño de los paquetes de datos
                    int SIZE_HEADER = 10;//TAMAÑO DE LA LISTA Header
                    int ENCABEZADOS = 3;
                    int FOOTER = 4;

                    //LISTA QUE ALMACENARA LOS DATOS DE LOS OBJETIVOS
                    string[][] data = new string[liObjetivos.Count][];

                    //SE DEFINE EL NUMERO DE FILAS QUE HABRA EN LA TABLA. EL NUMERO DE OBJETIVOS MAS LOS TRES ENCABEZADOS MAS LOS FOOTER
                    //SE DEFINE EL NUMERO DE COLUMNAS CON EL NUMERO DE HEADERS
                    tableObjetivos.ResetCells(liObjetivos.Count + ENCABEZADOS + FOOTER, SIZE_HEADER);
                    #endregion

                    #region HEADERS
                    //PARA EL PRIMER HEADER SE REALIZA LO SIGUIENTE
                    //String[] HeaderUno = { "Objetivos Settings", "Mid-Year Review", "Final Annual Review" };
                    int[] IndexHeaderUno = { 0, 4, 6 };//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    System.Drawing.Color[] columnColors = { System.Drawing.Color.FromArgb(0, 171, 239), System.Drawing.Color.FromArgb(0, 128, 0), System.Drawing.Color.FromArgb(128, 128, 128) };
                    //COLORES 
                    TableRow F_Header1_row = tableObjetivos.Rows[0];
                    //F_Header1_row.RowFormat.BackColor = Color.FromArgb(0, 128, 0);
                    F_Header1_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(0, 0, 3);
                    tableObjetivos.ApplyHorizontalMerge(0, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(0, 6, 9);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < HeaderUno.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexHeaderUno[h];
                        // Configurar el color de fondo de la celda según la columna
                        F_Header1_row.Cells[columnIndex].CellFormat.BackColor = columnColors[h];

                        //Cell Alignment
                        F_Header1_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black
                            ;
                        F_Header1_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Header1_row.Cells[columnIndex].AddParagraph();
                        F_Header1_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(HeaderUno[h]);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                    
                    }

                    //PARA EL SEGUNDO HEADER
                    //String[] HeaderDos = { "KPI Period", "July 2023 to June 2024", "Mid-Year Review Date (DD/MMM/YY)", "Final Annual Review Date (DD/MMM/YY)" };
                    int[] IndexHeaderDos = { 0, 2, 4, 6 };//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    //COLORES 
                    TableRow S_row = tableObjetivos.Rows[1];
                    S_row.RowFormat.BackColor = Color.FromArgb(0, 171, 239);
                    S_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(1, 0, 1);
                    tableObjetivos.ApplyHorizontalMerge(1, 2, 3);
                    tableObjetivos.ApplyHorizontalMerge(1, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(1, 6, 9);


                    // Agregar los encabezados y configurar su formato
                    //int celdaS_row = 0;
                    for (int h = 0; h < HeaderDos.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexHeaderDos[h];
                        //Cell Alignment
                        S_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        S_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;
                        Spire.Doc.Documents.Paragraph p = S_row.Cells[columnIndex].AddParagraph();
                        S_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(HeaderDos[h]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor = System.Drawing.Color.Black;
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        S_row.RowFormat.BackColor = System.Drawing.Color.FromArgb(173, 216, 230);
                    }

                    //PARA EL TERCER HEADER
                    Spire.Doc.TableRow FRow = tableObjetivos.Rows[2];
                    FRow.IsHeader = false;
                    FRow.Height = 35;

                    for (int h = 0; h < Header.Length; h++)
                    {
                        //Cell Alignment
                        FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        FRow.Cells[h].CellWidthType = CellWidthType.Auto;// = 300F;
                        Spire.Doc.Documents.Paragraph p = FRow.Cells[h].AddParagraph();
                        FRow.Cells[h].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        //FRow.Cells[h].CellFormat.Borders.Color = System.Drawing.Color.White;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                        TR.CharacterFormat.FontSize = 9;
                        if (h == 1)
                        {
                            TR.CharacterFormat.FontSize = 8;
                        }
                        
                        TR.CharacterFormat.Bold = true;
                        //TR.CharacterFormat.Border.Color = System.Drawing.Color.White;
                        FRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(0, 171, 239);
                    }
                    #endregion

                    #region DATOS DE LA TABLA
                    ////DATOS
                    int i = 0;
                    int totalPonderado = 0;
                    float totalEvaluado = 0.0f;
                    float totalEvaluador = 0.0f;
                    foreach (EObjetives objetivo in liObjetivos)
                    {
                        int numObj = i + 1;
                        String titulo = objetivo.titulo;
                        //Objetivos
                        String descripcion = objetivo.objdesc + "\n" + objetivo.objMetricas;
                        //Ponderado
                        totalPonderado += objetivo.ponderado;
                        String ponderado = objetivo.ponderado.ToString();
                        //definicion del tamaño
                        data[i] = new string[SIZE_HEADER];
                        //primer apartado
                        data[i][0] = numObj.ToString();
                        data[i][1] = titulo;
                        data[i][2] = descripcion;
                        data[i][3] = ponderado;
                        //segundo apartado
                        data[i][4] = objetivo.resultado;
                        data[i][5] = objetivo.ComentariosJefeEval;
                        //tercer apartdo
                        data[i][6] = objetivo.cumplimientoEvaluado2.ToString();
                        data[i][7] = objetivo.Resultado2;
                        data[i][8] = objetivo.cumplimientoEvaluador2.ToString();
                        data[i][9] = objetivo.ComentariosJefeEval2;

                        totalEvaluado += (( objetivo.ponderado/100f) * objetivo.cumplimientoEvaluado2);
                        totalEvaluador += ((objetivo.ponderado/100f) * objetivo.cumplimientoEvaluador2);
                        
                        i = i + 1;
                    }

                    //rellenando la tabla
                    for (int r = 0; r < data.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r + ENCABEZADOS];
                        DataRow.Height = 20;                      
                        
                        if ((r % 2) == 0)
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(222, 234, 246);//AZUL
                        }
                        else
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);//BLANCO
                        }

                        for (int c = 0; c < data[r].Length; c++)
                        {
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                            
                            //DataRow.Cells[c].Width = 75F;
                            Spire.Doc.Documents.Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TR2 = p2.AppendText(data[r][c]);
                            
                            if(c == 2)
                            {
                                DataRow.Cells[c].Width = 50F;
                            }


                            //Format Cells
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            if (c == 3 || c == 5 || c == 7)
                            {
                                p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            }

                            //LETRA
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 9;
                            if(c == 2 || c == 1)
                            {
                                TR2.CharacterFormat.FontSize = 7;

                            }
                            TR2.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);
                            TR2.CharacterFormat.Border.Color = System.Drawing.Color.FromArgb(0, 171, 239);
                        }
                    }
                    #endregion

                    #region FOOTER

                    #region IDIOMA FOOTER
                    //FOOTER
                    String[] FooterUno;
                    String[] FooterUnoSP = { "PUNTUACIÓN GENERAL DE DESEMPEÑO INDIVIDUAL", totalPonderado.ToString() + "%", "", totalEvaluado.ToString("F2"), Evaluado.Evaluacion.ComEvaluado2, totalEvaluador.ToString("F2"), Evaluado.Evaluacion.ComEvaluador2 };
                    String[] FooterUnoEN = { "OVERALL INDIVIDUAL PERFORMANCE SCORE", totalPonderado.ToString() + "%", "", totalEvaluado.ToString("F2"), Evaluado.Evaluacion.ComEvaluado2, totalEvaluador.ToString("F2"), Evaluado.Evaluacion.ComEvaluador2 };

                    String[] FooterDos;
                    String[] FooterDosSP = { "APROBACIÓN DE LA FASE ESTABLECIMIENTO DE OBJETIVOS", "FIRMA DE REVISIÓN DE MEDIO AÑO", "FIRMA DE REVISIÓN ANUAL FINAL" };
                    String[] FooterDosEN = { "OBJECTIVE SETTING SIGN-OFF", "MID-YEAR REVIEW SIGN-OFF", "FINAL ANNUAL REVIEW SIGN-OFF" };

                    if (idiomaDOC == idiomaSP)
                    {
                        FooterUno = FooterUnoSP;
                        FooterDos = FooterDosSP;
                    }
                    else
                    {
                        FooterUno = FooterUnoEN;
                        FooterDos = FooterDosEN;
                    }

                    #endregion
                    //PARA EL PRIMER footer SE REALIZA LO SIGUIENTE
                    //String[] FooterUno = { "OVERALL INDIVIDUAL PERFORMANCE SCORE", totalPonderado.ToString() + "%", "", totalEvaluado.ToString("F2"), "",totalEvaluador.ToString("F2"), "" };

                    int[] IndexFooterUno = { 0, 3, 4, 6, 7, 8, 9};//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    //COLORES 
                    System.Drawing.Color[] columnColorsFooter = { System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(254, 254, 254), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(254, 254, 254), System.Drawing.Color.FromArgb(128, 128, 128), System.Drawing.Color.FromArgb(254, 254, 254), System.Drawing.Color.FromArgb(128, 128, 128) };
                    System.Drawing.Color[] columnColorsTextFooter = { System.Drawing.Color.FromArgb(254, 254, 254), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0) };
                    
                    int inicioFooter = ENCABEZADOS + liObjetivos.Count;//suma para saber donde comienza el footer
                    TableRow F_Footer1_row = tableObjetivos.Rows[inicioFooter];
                    //F_Header1_row.RowFormat.BackColor = Color.FromArgb(0, 128, 0);
                    F_Footer1_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(inicioFooter, 0, 2);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooter, 4, 5);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < FooterUno.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexFooterUno[h];
                        // Configurar el color de fondo de la celda según la columna
                        F_Footer1_row.Cells[columnIndex].CellFormat.BackColor = columnColorsFooter[h];

                        //Cell Alignment
                        F_Footer1_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Footer1_row.Cells[columnIndex].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        F_Footer1_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Footer1_row.Cells[columnIndex].AddParagraph();
                        F_Footer1_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(FooterUno[h]);
                        TR.CharacterFormat.TextColor = columnColorsTextFooter[h];
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                    }

                    //PARA EL SEGUNDO FOOTER SE REALIZA LO SIGUIENTE
                    //String[] FooterDos = { "OVERALL INDIVIDUAL PERFORMANCE SCORE", "MID-YEAR REVIEW SIGN-OFF", "FINAL ANNUAL REVIEW SIGN-OFF" };
                    int[] IndexFooterDos = { 0, 4, 6};//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    //COLORES 
                    int inicioFooterDos = inicioFooter + 1;//suma para saber donde comienza el footer dos
                    TableRow F_Footer2_row = tableObjetivos.Rows[inicioFooterDos];
                    F_Footer2_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Footer2_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterDos, 0, 3);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterDos, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterDos, 6, 9);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < FooterDos.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexFooterDos[h];

                        //Cell Alignment
                        F_Footer2_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Footer2_row.Cells[columnIndex].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        F_Footer2_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Footer2_row.Cells[columnIndex].AddParagraph();
                        F_Footer2_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(FooterDos[h]);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 0, 0);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                    }

                    //PARA EL tercer FOOTER SE REALIZA LO SIGUIENTE
                    int[] IndexFooterTres = { 0, 4, 6 };//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    int inicioFooterTres = inicioFooterDos + 1;//suma para saber donde comienza el footer dos
                    TableRow F_Footer3_row = tableObjetivos.Rows[inicioFooterTres];
                    F_Footer3_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Footer3_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterTres, 0, 3);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterTres, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterTres, 6, 9);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < IndexFooterTres.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexFooterTres[h];

                        //Cell Alignment
                        F_Footer3_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Footer3_row.Cells[columnIndex].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        F_Footer3_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Footer3_row.Cells[columnIndex].AddParagraph();
                        F_Footer3_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        String textFirma = firmaEmpleado + "\n\n\n\n-------------------------\n" + Evaluado.Login.NombreCompleto;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(textFirma);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 0, 0);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 10;
                        TR.CharacterFormat.Bold = true;
                    }

                    //PARA EL CUARTO FOOTER SE REALIZA LO SIGUIENTE
                    int inicioFooterCuatro = inicioFooterTres + 1;//suma para saber donde comienza el footer dos
                    F_Footer3_row = tableObjetivos.Rows[inicioFooterCuatro];
                    F_Footer3_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Footer3_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterCuatro, 0, 3);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterCuatro, 4, 5);
                    tableObjetivos.ApplyHorizontalMerge(inicioFooterCuatro, 6, 9);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < IndexFooterTres.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexFooterTres[h];

                        //Cell Alignment
                        F_Footer3_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Footer3_row.Cells[columnIndex].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        F_Footer3_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Footer3_row.Cells[columnIndex].AddParagraph();
                        F_Footer3_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        String textFirma = firmaJefe + "\r\n\n\n\n\n-------------------------\n" + jefe.NombreCompleto;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(textFirma);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 0, 0);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 10;
                        TR.CharacterFormat.Bold = true;
                    }




                    #endregion

                    #region ALINEACION
                    for (int r = 0; r < 3; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r];

                        for (int c = 0; c < SIZE_HEADER; c++)
                        {
                            //DataRow.Cells[c].CellFormat.Borders.Right.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                            //DataRow.Cells[c].CellFormat.Borders.Right.BorderType = Spire.Doc.Documents.BorderStyle.Double;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                        }
                    }
                    #endregion

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

                #region SEGUNDA TABLA

                if(liPersonalPTP != null && liPersonalPTP.Count > 0 || liPersonalPDP != null && liPersonalPDP.Count > 0)
                {
                    #region DECLARACION
                    Spire.Doc.Section sectionObjetivos = document.Sections[0];
                    Spire.Doc.Documents.TextSelection selectionObjetivos = document.FindString("#Objetivos2#", true, true);
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
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                    
                    //INICIO ENCABEZADOS

                    #region IDIOMA ENCABEZADOS
                    String[] Header;
                    String[] HeaderSP = { "Objetivos", "Acción", "Para cuándo", "Objetivos", "Propósito", "¿Cómo puede ayudar?" };
                    String[] HeaderEN = {"Goals", "Action", "By when ", "Goals", "Purpose", "How can help?"};

                    String textHeader1= "";
                    String textHeader1SP = "PLAN DE FORMACIÓN Y DESARROLLO PERSONAL";
                    String textHeader1EN = "PERSONAL TRAINING AND DEVELOPMENT PLAN";

                    String[] HeaderDos;
                    String[] HeaderDosSP = { "PLAN DE FORMACIÓN PERSONAL - ROL ACTUAL", "PLAN DE DESARROLLO PERSONAL - ENFOQUE EN LA CARRERA" };
                    String[] HeaderDosEN = { "PERSONAL TRAINING PLAN - CURRENT ROLE ", "PERSONAL DEVELOPMENT PLAN - CAREER FOCUSED" };

                    String[] HeaderTres;

                    String[] HeaderTresSP = {
                        "¿Qué entrenamiento necesitas? (por ejemplo, desarrollar tus habilidades, conocimientos, capacidades para desempeñar tu trabajo actual)",
                        "Tu primera prioridad es tu rol actual. Si eres nuevo en el rol, o si tu desempeño actual necesita mejorar, entonces concéntrate primero en tu plan de entrenamiento. Piensa en tus aspiraciones profesionales y utiliza a tu gerente para ayudarte a establecer metas realistas si es necesario. Crea un plan de desarrollo de desempeño para cómo vas a lograr tus objetivos."
                    };
                    String[] HeaderTresEN = {
                        "What training do you need? (i.e. developing your skills, knowledge, capabilities to do your currentjob)",
                        "Your first priority is your current role, if you are new to role, or if your current performance needs to be improved then focus on your training plan first. Think about your career aspirations, using you manager to assist with establishing realistic goals if required. Create a performance development plan for how you are going to achieve your goals"
                    };

                    String[] HeaderCinco;
                    String[] HeaderCincoSP = {
                        "Lista de apoyo que necesitas en tu rol actual",
                        "Lista de pasos que tomarás para alcanzar el objetivo (Aprendizaje autodirigido, aprendizaje en clase, coaching o aprendizaje en el trabajo)",
                        "Fecha límite para lograr tu objetivo",
                        "¿Cuáles son los objetivos de crecimiento, desafío y aspiraciones más allá del rol actual?",
                        "¿Cuál es tu motivación para lograr estas aspiraciones?",
                        "¿Qué necesitas hacer y/o qué apoyo necesitas para lograr tus aspiraciones profesionales? (estudios, proyectos específicos, formación, etc.)"
                    };
                    String[] HeaderCincoEN = {
                        "List support you require in your current role",
                        "List steps you will take to achieve the goal? (Self-directed learning, classroom learning, coaching or on-the job learning)",
                        "Date by when you want to achieve your goa",
                        "What are the growth, stretch goals and aspirations beyond current role?",
                        "What is your motivation to achieve these aspirations?",
                        "What do you need to do and/or what support do you need in order to achieve your career aspirations? (studies, specific projects, training, etc.)"
                    };

                    if (idiomaDOC == idiomaSP)
                    {
                        Header = HeaderSP;
                        textHeader1 = textHeader1SP;
                        HeaderDos = HeaderDosSP;
                        HeaderTres = HeaderTresSP;
                        HeaderCinco = HeaderCincoSP;
                    }
                    else
                    {
                        Header = HeaderEN;
                        textHeader1 = textHeader1EN;
                        HeaderDos = HeaderDosEN;
                        HeaderTres = HeaderTresEN;
                        HeaderCinco = HeaderCincoEN;
                    }


                    #endregion



                    //DEFINIR QUIEN TIENE MAS DATOS PARA LA CREACIÓN DE LA TABLA
                    int numeroDatosPTP = 0;
                    int numeroDatosPDP = 0;

                    if (liPersonalPTP != null && liPersonalPTP.Count > 0)
                    {
                        numeroDatosPTP = liPersonalPTP.Count;
                    }
                    if (liPersonalPDP != null && liPersonalPDP.Count > 0)
                    {
                        numeroDatosPDP = liPersonalPDP.Count;
                    }

                    bool isPDPmayor = true; // si son iguales tambien es true
                    isPDPmayor = (numeroDatosPDP >= numeroDatosPTP);


                    //tamaño de los paquetes de datos
                    int SIZE_HEADER = 6;//TAMAÑO DE LA LISTA Header
                    int ENCABEZADOS = 5;//POR DEFECTO PARA LA TABLA SE CREARON 5 ENCABEZADOS
                    int FOOTER = 3;// POR DEFECTO PARA LA TABLA SE CREARON 3 ENCABEZADOS

                    //LISTA QUE ALMACENARA LOS DATOS DE LOS OBJETIVOS PTP
                    //Para este caso se crearan dos listas y se revisará cual es la mas larga
                    string[][] dataGeneral;


                    //SE DEFINE EL NUMERO DE FILAS QUE HABRA EN LA TABLA. EL NUMERO DE OBJETIVOS MAS LOS TRES ENCABEZADOS MAS LOS FOOTER
                    //SE DEFINE EL NUMERO DE COLUMNAS CON EL NUMERO DE HEADERS
                    //SE DEFINE EL TAMAÑO DEL APARTADO DE DATOS DE ACUERDO A LA LISTA QUE TENGA MAS DATOS 
                    int numMayor = 0;
                    if (isPDPmayor)
                    {
                        dataGeneral = new string[numeroDatosPDP][];
                        tableObjetivos.ResetCells(numeroDatosPDP + ENCABEZADOS + FOOTER, SIZE_HEADER);
                        numMayor = numeroDatosPDP;
                    }
                    else
                    {
                        dataGeneral = new string[numeroDatosPTP][];
                        tableObjetivos.ResetCells(numeroDatosPTP + ENCABEZADOS + FOOTER, SIZE_HEADER);
                        numMayor = numeroDatosPTP;
                    }

                    //tableObjetivos.ResetCells(liPersonalPTP.Count + ENCABEZADOS + FOOTER, SIZE_HEADER);

                    #endregion

                    #region ENCABEZADOS
                    //PARA EL PRIMER HEADER SE REALIZA LO SIGUIENTE
                    
                    TableRow F_Header1_row = tableObjetivos.Rows[0];
                    F_Header1_row.RowFormat.BackColor = Color.FromArgb(255, 69, 0);
                    F_Header1_row.IsHeader = false;
                    // Fusionar las celdas para crear UN encabezados
                    tableObjetivos.ApplyHorizontalMerge(0, 0, 5);

                    // Obtener el índice de la columna actual
                    int columnIndex1 = 0;

                    //Cell Alignment
                    F_Header1_row.Cells[columnIndex1].CellFormat.Borders.Color = System.Drawing.Color.Black;
                    F_Header1_row.Cells[columnIndex1].CellWidthType = CellWidthType.Auto;

                    Spire.Doc.Documents.Paragraph p1 = F_Header1_row.Cells[columnIndex1].AddParagraph();
                    F_Header1_row.Cells[columnIndex1].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                    p1.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    //String textHeader1 = "PERSONAL TRAINING AND DEVELOPMENT PLAN";
                    
                    Spire.Doc.Fields.TextRange TR1 = p1.AppendText(textHeader1);
                    TR1.CharacterFormat.TextColor = System.Drawing.Color.White;
                    TR1.CharacterFormat.FontName = "Calibri";
                    TR1.CharacterFormat.FontSize = 15;
                    TR1.CharacterFormat.Bold = true;


                    //PARA EL SEGUNDO HEADER SE REALIZA LO SIGUIENTE
                    //String[] HeaderDos = { "PERSONAL TRAINING PLAN - CURRENT ROLE ", "PERSONAL DEVELOPMENT PLAN - CAREER FOCUSED"};
                    int[] IndexHeaderDos = { 0, 3};//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    
                    TableRow F_Header2_row = tableObjetivos.Rows[1];
                    F_Header2_row.RowFormat.BackColor = Color.FromArgb(255, 165, 0);
                    F_Header2_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(1, 0, 2);
                    tableObjetivos.ApplyHorizontalMerge(1, 3, 5);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < HeaderDos.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexHeaderDos[h];

                        //Cell Alignment
                        F_Header2_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Header2_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Header2_row.Cells[columnIndex].AddParagraph();
                        F_Header2_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(HeaderDos[h]);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.Black;
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;

                    }

                    //PARA EL TERCER HEADER SE REALIZA LO SIGUIENTE
                    //String[] HeaderTres = {};
                    int[] IndexHeaderTres = { 0, 3 };//Guarda las columnas donde se encontraran las nuevas celdas despues de la fución
                    
                    TableRow F_Header3_row = tableObjetivos.Rows[2];
                    F_Header3_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Header3_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(2, 0, 2);
                    tableObjetivos.ApplyHorizontalMerge(2, 3, 5);

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < HeaderTres.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = IndexHeaderTres[h];

                        //Cell Alignment
                        F_Header3_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Header3_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Header3_row.Cells[columnIndex].AddParagraph();
                        F_Header3_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(HeaderTres[h]);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.Black;
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                    }

                    //PARA EL CINCO HEADER SE REALIZA LO SIGUIENTE
                    //String[] HeaderCinco = {};
                     
                    TableRow F_Header5_row = tableObjetivos.Rows[4];
                    F_Header5_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Header5_row.IsHeader = false;

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < HeaderCinco.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = h;

                        //Cell Alignment
                        F_Header5_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Header5_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Header5_row.Cells[columnIndex].AddParagraph();
                        F_Header5_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(HeaderCinco[h]);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.Black;
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                    }

                    //PARA EL CUATRO HEADER SE REALIZA LO SIGUIENTE
                    TableRow F_Header4_row = tableObjetivos.Rows[3];
                    F_Header4_row.RowFormat.BackColor = Color.FromArgb(255, 165, 0);
                    F_Header4_row.IsHeader = false;

                    // Agregar los encabezados y configurar su formato
                    for (int h = 0; h < Header.Length; h++)
                    {
                        // Obtener el índice de la columna actual
                        int columnIndex = h;

                        //Cell Alignment
                        F_Header4_row.Cells[columnIndex].CellFormat.Borders.Color = System.Drawing.Color.Black;
                        F_Header4_row.Cells[columnIndex].CellWidthType = CellWidthType.Auto;

                        Spire.Doc.Documents.Paragraph p = F_Header4_row.Cells[columnIndex].AddParagraph();
                        F_Header4_row.Cells[columnIndex].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        Spire.Doc.Fields.TextRange TR = p.AppendText(Header[h]);
                        TR.CharacterFormat.TextColor = System.Drawing.Color.Black;
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                    }



                    #endregion

                    #region DATOS
                    ////DATOS

                    //LLENADO DE LA TABLA DE OBJETIVOS PERSONALES
                    //PRIMERO SE LLENARAN LOS PTP Y DESPUES LOS PDP

                    for (int h = 0; h < numMayor; h++)
                    {
                        //Definimos el tamaño de cada fila que es la suma de las columnas de ambas listas
                        //de cada una son tres columnas, en total son 6
                        dataGeneral[h] = new string[6];


                        //llenamos PTP
                        string tituloPTP = "";
                        string descripcionPTP = "";
                        string datePTP = "";

                        if (liPersonalPTP != null && numeroDatosPTP > 0 &&numeroDatosPTP > h)
                        {
                            tituloPTP = liPersonalPTP[h].ObjetivePTP;
                            descripcionPTP = liPersonalPTP[h].ActionPTP;
                            datePTP = liPersonalPTP[h].DateFin;
                        }

                        dataGeneral[h][0] = tituloPTP;
                        dataGeneral[h][1] = descripcionPTP;
                        dataGeneral[h][2] = datePTP;

                        //llenamos PDP
                        string tituloPDP = "";
                        string descripcionPDP = "";
                        string datePDP = "";

                        if (liPersonalPDP != null && numeroDatosPDP > 0 && numeroDatosPDP > h)
                        {
                            tituloPDP = liPersonalPDP[h].ObjetivePDP;
                            descripcionPDP = liPersonalPDP[h].actionPDP;
                            datePDP = liPersonalPDP[h].dateFinish;
                        }

                        dataGeneral[h][3] = tituloPDP;
                        dataGeneral[h][4] = descripcionPDP;
                        dataGeneral[h][5] = datePDP;

                    }

                    //rellenando la tabla
                    //SE LLENA PRIMERO LA PARTE DE LOS OBJETIVOS PTP
                    for (int r = 0; r < dataGeneral.Length; r++)
                    {
                        Spire.Doc.TableRow DataRow = tableObjetivos.Rows[r + ENCABEZADOS];
                        DataRow.Height = 20;

                        if ((r % 2) == 0)
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(222, 234, 246);//AZUL
                        }
                        else
                        {
                            DataRow.RowFormat.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);//BLANCO
                        }

                        for (int c = 0; c < dataGeneral[r].Length; c++)
                        {
                            DataRow.Cells[c].CellFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                            DataRow.Cells[c].CellFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 0, 0);

                            //DataRow.Cells[c].Width = 75F;
                            Spire.Doc.Documents.Paragraph pPTP = DataRow.Cells[c].AddParagraph();
                            Spire.Doc.Fields.TextRange TRPTP = pPTP.AppendText(dataGeneral[r][c]);

                            if (c == 2)
                            {
                                DataRow.Cells[c].Width = 50F;
                            }


                            //Format Cells
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            pPTP.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                            //COLUMAS EN LAS CUALES EL TEXTO ESTARA EN MEDIO
                            if (c == 2)
                            {
                                pPTP.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            }

                            //LETRA
                            TRPTP.CharacterFormat.FontName = "Calibri";
                            TRPTP.CharacterFormat.FontSize = 9;
                            //CAMBIAR LA LETRA DE LAS COLUMNAS
                            if (c == 1)
                            {
                                TRPTP.CharacterFormat.FontSize = 7;
                            }
                            TRPTP.CharacterFormat.TextColor = System.Drawing.Color.FromArgb(0, 32, 96);
                            TRPTP.CharacterFormat.Border.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                        }
                    }
                    #endregion

                    #region FOOTER
                    //PARA EL PRIMER FOOTER SE REALIZA LO SIGUIENTE
                    int indexFooter1 = SIZE_HEADER + numMayor - 1 ;

                    TableRow F_Footer1_row = tableObjetivos.Rows[indexFooter1];
                    F_Footer1_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Footer1_row.Cells[columnIndex1].CellFormat.Borders.Color = System.Drawing.Color.Black;

                    tableObjetivos.TableFormat.Borders.BorderType = Spire.Doc.Documents.BorderStyle.Single;
                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 0, 0);

                    //DataRow.Cells[c].CellFormat.Borders.Right.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                    //DataRow.Cells[c].CellFormat.Borders.Right.BorderType = Spire.Doc.Documents.BorderStyle.Double;

                    F_Footer1_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(indexFooter1, 0, 5);

                    // Obtener el índice de la columna actual
                    columnIndex1 = 0;

                    //Cell Alignment
                    F_Footer1_row.Cells[columnIndex1].CellFormat.Borders.Color = System.Drawing.Color.Black;
                    F_Footer1_row.Cells[columnIndex1].CellWidthType = CellWidthType.Auto;

                    Spire.Doc.Documents.Paragraph p2 = F_Footer1_row.Cells[columnIndex1].AddParagraph();
                    F_Footer1_row.Cells[columnIndex1].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                    p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    String titleFooter1 = "";
                    if(idiomaDOC == idiomaSP)
                    {
                        titleFooter1 = "FIRMA DE APROBACIÓN DEL PLAN DE FORMACIÓN Y DESARROLLO PERSONAL";
                    }
                    else
                    {
                        titleFooter1 = "PERSONAL TRAINING AND DEVELOPMENT SIGN-OFF";

                    }
                    Spire.Doc.Fields.TextRange TR2 = p2.AppendText(titleFooter1);
                    TR2.CharacterFormat.TextColor = System.Drawing.Color.Black;
                    TR2.CharacterFormat.FontName = "Calibri";
                    TR2.CharacterFormat.FontSize = 9;
                    TR2.CharacterFormat.Bold = true;


                    //PARA EL SEGUNDO FOOTER SE REALIZA LO SIGUIENTE
                    int indexFooter2 = indexFooter1 + 1;

                    F_Footer1_row = tableObjetivos.Rows[indexFooter2];
                    F_Footer1_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Footer1_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(indexFooter2, 0, 5);

                    // Obtener el índice de la columna actual
                    columnIndex1 = 0;

                    //Cell Alignment
                    F_Footer1_row.Cells[columnIndex1].CellFormat.Borders.Color = System.Drawing.Color.Black;
                    F_Footer1_row.Cells[columnIndex1].CellWidthType = CellWidthType.Auto;

                    p2 = F_Footer1_row.Cells[columnIndex1].AddParagraph();
                    F_Footer1_row.Cells[columnIndex1].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Top;
                    p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                    String textFirmaEmpleado = firmaEmpleado+"\n\n\t\t\t-------------------------\n\t\t\t" + Evaluado.Login.NombreCompleto;
                    TR2 = p2.AppendText(textFirmaEmpleado);
                    TR2.CharacterFormat.TextColor = System.Drawing.Color.Black;
                    TR2.CharacterFormat.FontName = "Calibri";
                    TR2.CharacterFormat.FontSize = 9;
                    TR2.CharacterFormat.Bold = true;


                    //PARA EL TERCER FOOTER SE REALIZA LO SIGUIENTE
                    int indexFooter3 = indexFooter2 + 1;

                    F_Footer1_row = tableObjetivos.Rows[indexFooter3];
                    F_Footer1_row.RowFormat.BackColor = Color.FromArgb(254, 254, 254);
                    F_Footer1_row.IsHeader = false;
                    // Fusionar las celdas para crear tres encabezados
                    tableObjetivos.ApplyHorizontalMerge(indexFooter3, 0, 5);

                    // Obtener el índice de la columna actual
                    columnIndex1 = 0;

                    //Cell Alignment
                    F_Footer1_row.Cells[columnIndex1].CellFormat.Borders.Color = System.Drawing.Color.Black;
                    F_Footer1_row.Cells[columnIndex1].CellWidthType = CellWidthType.Auto;

                    p2 = F_Footer1_row.Cells[columnIndex1].AddParagraph();
                    F_Footer1_row.Cells[columnIndex1].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Top;
                    p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                    String textFirmaJefe = firmaJefe+"\n\n\t\t\t-------------------------\n\t\t\t" + jefe.NombreCompleto;
                    TR2 = p2.AppendText(textFirmaJefe);
                    TR2.CharacterFormat.TextColor = System.Drawing.Color.Black;
                    TR2.CharacterFormat.FontName = "Calibri";
                    TR2.CharacterFormat.FontSize = 9;
                    TR2.CharacterFormat.Bold = true;

                    #endregion

                    tableObjetivos.TableFormat.Borders.Color = System.Drawing.Color.FromArgb(0, 0, 0);
                    //bodyObjetivos.ChildObjects.Remove(paragraphObjetivos);
                    document.Replace("#Objetivos2#", "", true, true);
                    bodyObjetivos.ChildObjects.Insert(index, tableObjetivos);
                }
                else
                {
                    document.Replace("#Objetivos2#", "", true, true);
                }

                #endregion


                Bitacora.NuevaEntrada("fin de Calificación de: " + Evaluado.Login.id_sap + "_" + Evaluado.Login.NombreCompleto.Trim() + " Ingreso a Historial", "MiHistorial/Historial ");
                Bitacora.NuevaEntrada("path:" + pathResp.ToString() + " Ingreso a Historial", "MiHistorial/Historial ");

                //SE REALIZA EL GUARDADO DEL ARCHIVO
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
                { //eliminar para evitar duplicid
                  //ades
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