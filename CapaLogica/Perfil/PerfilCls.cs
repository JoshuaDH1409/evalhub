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


namespace CapaLogica.Perfil
{
    class PerfilCls
    {
        public static List<EPerfil> RecuperaPerfil()
        {
            try
            {
                return AccesoDB.ReadAll(new EPerfil());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static EPerfil RecuperaUnPerfil(int idPerfil)
        {
            try
            {
                List<EPerfil> LiPerfiles = AccesoDB.Read(new EPerfil(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EPerfil).GetProperty("id"), OperadoresRelacionales.IGUAL,idPerfil, TipoValor.Numero)

                });

                if (LiPerfiles.Count > 0)
                    return LiPerfiles[0];
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
