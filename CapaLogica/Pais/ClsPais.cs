using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo;
using CRUD.Transaction;
using CapaLogica.Funciones;
using System.Reflection;
using CRUD;
using General;



namespace CapaLogica.Pais
{
    class ClsPais
    {
        public static List<EPais> RecuperaPaises()
        {
            try
            {
                return AccesoDB.Read(new EPais(), new List<Cliterio> { new Cliterio(typeof(EPais).GetProperty("Activo"),
                    OperadoresRelacionales.IGUAL,true, TipoValor.Boleano) });
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        public static List<EDivision> RecuperaDivisiones(string term)
        {
            try
            {
                if (!String.IsNullOrEmpty(term))
                {
                    List<EDivision> divisiones = AccesoDB.Read(new EDivision(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EDivision).GetProperty("Descripcion"), OperadoresRelacionales.LIKE,term,TipoValor.Texto)
                });
                    return divisiones;
                }
                else
                {
                    List<EDivision> divisiones = AccesoDB.ReadAll(new EDivision());
                    return divisiones;
                }
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static EPais RecuperaUnPais(int idPais)
        {
            try
            {
                List<EPais> LiPaises= AccesoDB.Read(new EPais(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EPerfil).GetProperty("id"), OperadoresRelacionales.IGUAL,idPais, TipoValor.Numero)
                });

                if (LiPaises.Count > 0)
                    return LiPaises[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }

    }
}
