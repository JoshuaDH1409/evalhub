using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General;
using System.IO;
using CRUD;


namespace CapaLogica.Log
{
    internal class LogErrores
    {
        public static void GuardaBitacora() {


        }

        public static void GuardaLogCapaLogica(List<string> errores)
        {
            try
            {
                GuardaLog(errores, "CapaLogica");
            }
            catch (Exception)
            { }
        }

        public static void GuardaLogVsta(List<string> errores)
        {
            try
            {
                GuardaLog(errores, "Vista");
            }
            catch (Exception)
            { }
        }

        public static void GuardaLogBitacora(List<string> errores, string bitacora)
        {
            try
            {
                GuardaLog(errores, bitacora);
            }
            catch (Exception)
            { }
        }

        public static void GuardaLogControlador(List<string> errores)
        {
            try
            {
                GuardaLog(errores, "Controlador");
            }
            catch (Exception)
            { }
        }

        public static void GuardaLog(List<string> errores, string nombreRuta)
        {
            try
            {
                string path = General.Utilidades.ObtenerAppSettings(General.Utilidades.RutaLog);
                path = string.Format("{0}{2}{1}", path, Path.DirectorySeparatorChar, nombreRuta);

                if (!Singleton.Instance.CRUDTxt.ExiteDirectorio(path))
                    throw new Exception();

                path = string.Format("{0}{1}.txt", path, string.Format("{1}{0:yyyyMMdd}", DateTime.Now.Date, nombreRuta));

                Singleton.Instance.CRUDTxt.EscribeInformacion(path, errores);
            }
            catch (Exception)
            { }
        }
    }
}
