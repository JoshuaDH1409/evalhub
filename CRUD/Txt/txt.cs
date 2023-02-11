using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CRUD;

namespace CRUD.Txt
{
    internal class txt : Interfaz.ICRUDTxt
    {
        private static readonly object padlock = new object();

        public bool ExiteDirectorio(string path)
        {
            try
            {
                if (!Directory.Exists(path))                    
                    Directory.CreateDirectory(path);

                return Directory.Exists(path);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void BorrarArchivo(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception)
            {
                
            }
        }

        public void EscribeInformacion(string path, List<string> line)
        {
            lock (padlock)
            {
                FileStream fs = null;
                StreamWriter sw = null;
                try
                {

                    fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);

                    foreach (string item in line)
                        sw.WriteLine(item);

                    sw.Flush();

                }
                catch (Exception)
                {
                }
                finally
                {
                    if (sw != null) { sw.Close(); sw.Dispose(); }
                    if (fs != null) { fs.Close(); fs.Dispose(); }
                }
            }
        }
    }
}
