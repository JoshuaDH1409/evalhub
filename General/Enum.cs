using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General
{
    public enum SQLTypeBasic
    {
        INTEGER,
        TEXT,
        BLOB,
        REAL,
        NUMERIC
    }

    public enum TipoOperacion
    {
        Nuevo,
        Lectura,
        Modificar,
        Borrar,
        BajaLogica,
    }
   
}
