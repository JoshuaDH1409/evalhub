using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Modelo;
using System.Web.SessionState;
using General;
using Modelo.Clases;
using System.Web.Mvc;

namespace PlantillaMVC
{
    public class Utilidades
    {
        internal static readonly CapaLogica.Interface.Interface1 negocio = new CapaLogica.CapaLogica();
 

        public const string ErrorInesperado = "Ocurrió error inesperado.\nIntentelo más tarde";
        public const string InicioSession = "Inicio Sesión";
        public const string CerroSession = "Cerró Sesión";
        public const string VencioSession = "Venció sesión ";
        public const string Seleccione = "Seleccione";

        public const string session = "Session";
        public const string TodosUsuarios = "TodosUsuarios";
        public const string Grupos = "Grupos";
        public const string Perfiles = "Perfiles";
        public const string Catalogos = "Catalogos";
        public const string Archivos = "Archivos";
        public const string NombreArchivo = "NombreArchivo";
        public const string TMPArchivo = "TMPArchivo";
        public const string ColumnasExcel = "ColumnasExcel";
        public const string ColumnasMisXml = "ColumnasMisXml";
        public const string Pagina = "Pagina";
        public const string NumElemento = "NumElemento";
        public const string NumeroPaginas = "NumeroPaginas";

        public const string TamanoBloque = "TamanoBloque";
        public const string RutaDocumentos = "RutaDocumentos";
        public const string RutaXML = "RutaXML";
        public const string NumeroElementosPaginacion = "NumeroElementosPaginacion";
        public const string RutaTmpServer = "RutaTmpServer";
        public const string EmailEnvio = "EmailEnvio";
        public const string PasswordEnvio = "PasswordEnvio";
        public const string Port = "Port";
        public const string Host = "Host";

        public const string PermisoCapturar = "PermisoCapturar";
        public const string PermisoConsultar = "PermisoConsultar";
        public const string PermisoEditar = "PermisoEditar";
        public const string PermisoEliminar = "PermisoEliminar";
        public const string PermisoVerTodosCreditos = "PermisoVerTodosCreditos";

        internal static bool ValidaSesion(HttpSessionStateBase Session, HttpContextBase HttpContext)
        {
            if (Session[session] == null)
                return false;

            if (Session[session] is CSession)
                return true;
            else
                return false;
        }

        internal static bool ValidaSesion(HttpSessionState Session)
        {
            if (Session[session] == null)
                return false;

            if (Session[session] is CSession)
                return true;
            else
                return false;
        }

        internal static void CierraSession(HttpSessionStateBase Session)
        {
            Session[session] = null;
            if(Session["PreviousSession"] != null)
            {
                Session["PreviousSession"] = null;
            }
            Session.Clear();
        }

        public static List<Modelo.Clases.CSessionEval> AgregaDatosSession(List<Modelo.Clases.CSessionEval> LiPrincipal, List<Modelo.Clases.CSessionEval> LiSecundaria)
        {

            foreach (Modelo.Clases.CSessionEval item in LiSecundaria)
            { 
                LiPrincipal.Add(item);
            }
            return LiPrincipal;
        }
        public List<SelectListItem> DropPais(int perfilId, List<int> paisId)
        {
            List<SelectListItem> Respuesta = new List<SelectListItem>();
            List<EPais> ListaPaises = Utilidades.negocio.RecuperaPaises();
            if (perfilId ==(int)EnumPerfil.Local)
            {
                ListaPaises = ListaPaises.Where(t => t.id == paisId.First()).ToList();
            }
            else if (perfilId == (int)EnumPerfil.Regional)
            {
                ListaPaises = ListaPaises.Where(t => paisId.ToList().Contains(t.id)).ToList();
            }

            foreach (EPais item in ListaPaises)
            {
                Respuesta.Add(new SelectListItem { Text = item.descripcion, Value = item.id.ToString() });

            }
            return Respuesta;
        }
        public static Results GetResultKPI(List<EObjetivoPais> objetivospais, List<EObjetives> objetivos_ind, int periodoetapa)
        {
            Results results = new Results();           
            results.CompB_Weighting = 50;
            results.CompC_Weighting = 50;
            results.Total_Weighting = 100;
            results.CompB_SumaCumplimiento = objetivospais.Sum(t => t.Objetivo_Cumplimiento);
            results.CompB_SumaPonderados = objetivospais.Sum(t => t.Objetivo_Peso);
            results.CompB_SumaValor = objetivospais.Sum(t=>t.Valor);
            if (periodoetapa <= 2)
            {
                results.CompC_SumaCumplimiento = objetivos_ind.Sum(t => t.cumplimientoEvaluador);
                results.CompC_SumaValor = objetivos_ind.Sum(t => t.ValorObj_MA);
            }
            else
            {
                results.CompC_SumaCumplimiento = objetivos_ind.Sum(t => t.cumplimientoEvaluador2);
                results.CompC_SumaValor = objetivos_ind.Sum(t => t.ValorObj);
            }
            results.CompC_SumaPonderados = objetivos_ind.Sum(t => t.ponderado);
            results.CompB_Score = getScoreEmpresa(results.CompB_SumaValor).Score;
            results.CompB_FinalScore =Decimal.Divide(results.CompB_Score * results.CompB_Weighting,100);
            results.CompC_Score = getScore(results.CompC_SumaValor).Bono;
            results.CompC_FinalScore = Decimal.Divide(results.CompC_Score * results.CompC_Weighting, 100);
            results.Total_Score = results.CompB_FinalScore + results.CompC_FinalScore;
            return results;
        }
        public static EEscaletaEmpresa getScoreEmpresa(decimal sumponderados)
        {
            List<EEscaletaEmpresa> esc_empresa = Utilidades.negocio.RecuperaEscaletaE();
            var score = esc_empresa.Where(t => t.Min <= sumponderados && t.Max > sumponderados).SingleOrDefault();
            return score;
        }
        public static EEscaleta getScore(decimal sumponderados)
        {
            List<EEscaleta> esc_empresa = Utilidades.negocio.RecuperaEscaleta();
            var score = esc_empresa.Where(t => t.Cal_Min <= sumponderados && t.Cal_Max > sumponderados).SingleOrDefault();
            return score;
        }
    }

    enum EnumPerfil
    {
        Sistemas = 1,
        Regional,
        Local,
        Usuario
    }
}