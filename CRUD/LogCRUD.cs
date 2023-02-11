using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CRUD
{
    internal class LogCRUD
    {
        public static void GuardaLog(List<string> errores)
        {
            try
            {
                string path = General.Utilidades.ObtenerAppSettings(General.Utilidades.RutaLog);
                path = string.Format("{0}{2}{1}", path, Path.DirectorySeparatorChar, "CRUD");

                if (!Singleton.Instance.CRUDTxt.ExiteDirectorio(path))
                    throw new Exception("No existe la ruta para guardar el log del CRUD");

                path = string.Format("{0}{1}.txt", path, string.Format("CRUD{0:yyyyMMdd}",DateTime.Now.Date));

                Singleton.Instance.CRUDTxt.EscribeInformacion(path, errores);
            }
            catch (Exception)
            {}
        }
    }
}
