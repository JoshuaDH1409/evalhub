using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRUD
{
    public enum OperadoresRelacionales
    {
        LIKE,
        IGUAL,
        MAYORQUE,
        MENORQUE,
        MAYORIGUAL,
        MENORIGUAL,
        IN,
        NOTIN,
        DIFERENTE,
        ANDLOGICO,
    }

    public enum OperadoresLogicos
    {
        Noaplica = 0,
        AND,
        OR,
    }

    public enum TipoValor
    {
        Texto,
        Numero,
        Boleano,
        Fecha,
        ParaIN,
    }
}
