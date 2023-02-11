using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRUD.Interfaz
{
    public interface ICRUDTxt
    {
        bool ExiteDirectorio(string path);

        void BorrarArchivo(string path);

        void EscribeInformacion(string path, List<string> line);
    }
}
