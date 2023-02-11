using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRUD.Interfaz
{
    public interface ICRUDExcel
    {
        List<string> RecuperaNombreHojas(string archivo);

        List<string> RecueraEncabezados(string archivo, string hoja);

        List<Dictionary<string, object>> RecuperaTodosDatosPorNombreDecolumna(string archivo, string hoja);
        
        List<Dictionary<int, object>> RecuperaTodosDatosPorNumeroDecolumna(string archivo, string hoja);

        List<string> RecuperaEncabezadosPrimeraHoja(string archivo);

        List<Dictionary<string, object>> RecuperaTodosDatosPorNombreDecolumnaPrimeraHoja(string archivo);
    }
}
