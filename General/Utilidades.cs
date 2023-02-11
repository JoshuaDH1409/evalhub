using System;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Web;
using System.Globalization;

namespace General
{
    public class Utilidades
    {
        //^[A-Za-z0-9._-]+@(?:[A-Za-z0-9])+\.[A-Za-z](?([A-Za-z])[A-Za-z]|\.[A-z]{2,})+$ PattEmali primero//
        public const string PattCurp = @"^[a-zA-Z]{4}\d{6}[a-zA-Z]{6}\d{2}$";
        public const string PattEmail = @"^[A-Za-z0-9._-]+@(?:[A-Za-z0-9])+(\.[A-Za-z]{2,})+$";
        public const string PattRFC = "^[a-zA-Z]{3,4}[0-9]{6}(?:[a-zA-z0-9]{3}){0,1}$";
        public const string PattTelofono = "^[0-9]+$";
        public const string PattTextoValido = "^[A-Za-z 'ÑñáéíóúÁÉÍÓÚ,.]{1,}$";
        public const string PattComentarios = "^[A-Za-z 'ÑñáéíóúÁÉÍÓÚ0-9.,\n]{1,}$";
        public const string PattAlphaNumerico = "^[A-Za-z 'ÑñáéíóúÁÉÍÓÚ0-9]{1,}$";
        public const string PattNumero = "^[0-9]+$";
        public const string PattDecimalPrecision4 = @"^[0-9]+\.?([0-9]{1,4})?$";
        public const string PattDecimalPrecision2 = @"^[0-9]+\.?([0-9]{1,2})?$";
        public const string PattDecimalConsignoPesos = @"^\$[0-9]{1,3}([,][0-9]{3})*[\.][0-9]{2}$";
        public const string PattBRPoolMiss = "^BR[0-9]{1,}$";
        public const string PattINFPoolMiss = "^INF[0-9]{1,}$";
        public const string PattSPollMiss = "^S[0-9]{1,}$";
        public const string pattDate = @"^\d{2}/\d{2}/\d{4}$";
        public const string pattTime = @"^\d{2}:\d{2}$";
        public const string RutaLog = "RutaLog";
        public const string EmailAdmin = "EmailAdmin";
        public const string PasswordAdmin = "PasswordAdmin";
        public const string Port = "Port";
        public const string Host = "Host";
        public const string validaMail = "validaMail";
        public const string TextoBitacora = "TextoBitacora";
        public const string HojaReportePago = "HojaReportePago";
        public const string HojaEJCSI = "HojaEJCSI";
        public const string HojaEJRE = "HojaEJRE";
        public const string HojaEJINFOVIEW = "HojaEJINFOVIEW";
        public const string HojaPagosSimulados = "HojaPagosSimulados";

        public const string CREDITO = "ColumnaReportePago1";
        public const string TIPO = "ColumnaReportePago2";
        public const string CORREO = "ColumnaReportePago3";
        public const string TIPOCORREO = "ColumnaReportePago4";
        public const string FECHAENVIO = "ColumnaReportePago5";
        public const string FECHADEPAGO = "ColumnaReportePago6";
        public const string MONTODEPAGO = "ColumnaReportePago7";
        public const string FOLIO = "ColumnaReportePago8";
        public const string ASESOR = "ColumnaReportePago9";



        public static string getHashFromString(string text)
        {
            try
            {
                byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(text.Trim()));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ObtenerSQLServerConexion()
        {
            string keyconexion = ObtenerAppSettings("DefaultConexionBaseDatos");
            if (ConfigurationManager.ConnectionStrings[keyconexion] != null)
                return ConfigurationManager.ConnectionStrings[keyconexion].ConnectionString;
            else
                return string.Empty;
        }

        public static string ObtenerAppSettings(string key)
        {
//#if DEBUG
            if (key == "RutaDocumentos")
                return HttpContext.Current.Server.MapPath("~/tmp/");
            if (key == "RutaLog")
                return HttpContext.Current.Server.MapPath("~/Log/");
            else
                return ConfigurationManager.AppSettings[key];
//#else
//            return ConfigurationManager.AppSettings[key];
//#endif
        }

        public static void PonerCulturaMx()
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "es-MX")
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-MX");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");
            }
        }

        public static string RecuperaMesDsc(int month)
        {
            DateTimeFormatInfo dtinfo = new System.Globalization.CultureInfo("es-MX").DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }

        public static string HTMLToPlainText(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {
                return source;
            }
        }







    } 
}
