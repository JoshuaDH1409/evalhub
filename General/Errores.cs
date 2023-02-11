using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace General
{
    public class Errores : System.Exception
    {
        public string[] DatosErrores { get; set; }

        public string Clase { get; set; }

        public string Metodo { get; set; }

        internal DateTime Fecha { get; set; }
    }

    public class ListErrores : System.Exception
    {
        private List<Errores> mListaErrores = null;

        public List<Errores> ListaErrores { get { return mListaErrores;} }

        public ListErrores()
        {
            mListaErrores = new List<Errores>();
        }
    }

    public class GeneraException
    {
        public static ListErrores AddException(Exception exception, MethodBase metodo , string[] datoserrores)
        {
            ListErrores exec = null;
            if(exception is ListErrores)
            {
                exec = (ListErrores)exception;
                exec.ListaErrores.Add(new Errores()
                {
                    Clase = metodo.ReflectedType.ToString(), 
                    Metodo = metodo.Name, 
                    DatosErrores = datoserrores,
                    Fecha = DateTime.Now 
                });
            }
            else
            {
                exec = new ListErrores();
                exec.ListaErrores.Add(new Errores() 
                {
                    Clase = metodo.ReflectedType.ToString(),
                    Metodo = metodo.Name, 
                    DatosErrores = datoserrores, 
                    Fecha = DateTime.Now 
                });
            }

            return exec;
        }

        public static List<string> RecuperaErrores(ListErrores exe)
        {
            try
            {
                List<string> result = new List<string>();
                result.Add("********************************************************************************************************" + System.Environment.NewLine);
                foreach (Errores item in exe.ListaErrores)
                {
                    string tmp = string.Format("Fecha: {0}{1}Clase: {2}{1}Metodo: {3}{1}", item.Fecha, System.Environment.NewLine, item.Clase, item.Metodo);

                    if (item.DatosErrores != null)
                        foreach (string err in item.DatosErrores)
                            tmp = string.Format("{0}{2}{1}", tmp, System.Environment.NewLine, err);

                    tmp = string.Format("{0}{1}", tmp, System.Environment.NewLine);

                    result.Add(tmp);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new List<string>(){string.Format("Ocurrió un error miestras se recuperaban la lista de errores.{2}Error: {0}{2}InnerException:{1}{2}{2}",
                                                        ex.Message, ex.InnerException, System.Environment.NewLine)};
            }
        }
    }
}
