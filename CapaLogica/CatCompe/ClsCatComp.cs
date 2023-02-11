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




namespace CapaLogica.CatCompe
{
    class ClsCatComp
    {
        public static List<ECatComp> RecuperaCatCompetencias()
        {
            try
            {
                return AccesoDB.ReadAll(new ECatComp());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static ECatComp RecuperaUnaCompetencia(int idComp)
        {
            try
            {
                List<ECatComp> LiCompetencias= AccesoDB.Read(new ECatComp(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ECatComp).GetProperty("id"), OperadoresRelacionales.IGUAL,idComp, TipoValor.Numero)
                });

                if (LiCompetencias.Count > 0)
                    return LiCompetencias[0];
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
