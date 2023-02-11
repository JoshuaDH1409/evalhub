using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecordatoriosService
{
    class Bitacora
    {
        public static void NuevaEntrada(string Entrada, string Controlador)
        {
          
            string path = "C:\\Log\\Service\\";
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
                path = path + DateTime.Now.ToLongDateString().Trim() + ".txt";
                string mensaje = Controlador + ": " + Entrada;
                using (StreamWriter w = File.AppendText(path))
                {
                    Log(mensaje, w);
                }

                using (StreamReader r = File.OpenText(path))
                {
                    DumpLog(r);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            }
        }


        public static void NuevaEntradaService(string Entrada, string Controlador)
        {

            string path = "C:\\Log\\Service\\";
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
                path = path + "Service" + DateTime.Now.ToLongDateString().Trim() + ".txt";
                string mensaje = Controlador + ": " + Entrada;
                using (StreamWriter w = File.AppendText(path))
                {
                    Log(mensaje, w);
                }

                using (StreamReader r = File.OpenText(path))
                {
                    DumpLog(r);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            }
        }


        private static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("{0}", DateTime.Now.ToLongTimeString());
            w.WriteLine("{0}", logMessage);
            w.WriteLine("-------------------------------");
        }

        private static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
