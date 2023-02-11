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

namespace CapaLogica.Status
{
    class ClsStatus
    {
        public static List<EStatus> GetStatus()
        {
            try
            {
                return AccesoDB.ReadAll(new EStatus());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        public static EStatus GetStatusById(int Id)
        {
            try
            {
                EStatus status = new EStatus();

                List<EStatus> listEvento = AccesoDB.Read(new EStatus(), new List<Cliterio>()
            {
                new Cliterio(typeof(EStatus).GetProperty("Id"), OperadoresRelacionales.IGUAL, Id, TipoValor.Numero)

            });

                if (listEvento.Count > 0)
                {
                    return listEvento[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                new string[] { string.Format("Error: {0}", ex.Message),
                                                        string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        public static bool SetStatus(EStatus modelo)
        {
            try
            {
                if (modelo.Id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        modelo.Operacion = TipoOperacion.Modificar;
                        AccesoDB.Update(tran, modelo, new List<Cliterio>()
                        {
                            new Cliterio(typeof(EStatus).GetProperty("Id"), OperadoresRelacionales.IGUAL, modelo.Id, TipoValor.Numero)
                        });
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        modelo.Operacion = TipoOperacion.Nuevo;

                        object tmp = AccesoDB.MaxId(tran, new EStatus(), typeof(EStatus).GetProperty("Id"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        modelo.Id = max;
                        AccesoDB.Save(tran, modelo);
                        tran.Commit();
                    }

                    return true;
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
    }
}
