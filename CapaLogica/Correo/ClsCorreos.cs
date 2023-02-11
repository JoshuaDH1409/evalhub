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


namespace CapaLogica.Correo
{
    class ClsCorreos
    {

        public static List<ECorreos> RecuperaCorreos()
        {
            try
            {
                return AccesoDB.ReadAll(new ECorreos());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static ECorreos RecuperaUnCorreo(int idCorreo)
        {
            try
            {
                List<ECorreos> liCorreos = AccesoDB.Read(new ECorreos(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ECorreos).GetProperty("id"), OperadoresRelacionales.IGUAL,idCorreo, TipoValor.Numero)
                });

                if (liCorreos.Count > 0)
                    return liCorreos[0];
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



        public static bool GuardaCorreo(ECorreos correo)
        {
            try
            {


                if (correo.id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        correo.Operacion = TipoOperacion.Modificar;
                        OperacionCorreo(correo, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        correo.Operacion = TipoOperacion.Nuevo;
                        //periodo.Status = 0;

                        object tmp = AccesoDB.MaxId(tran, new EEval(), typeof(ECorreos).GetProperty("id"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        correo.id = max;
                        OperacionCorreo(correo, tran);
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

        private static void OperacionCorreo(ECorreos Correo, ITransactionCRUD tran)
        {
            try
            {
                switch (Correo.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        //object tmp = AccesoDB.MaxId(tran, Usuario, typeof(ELogin).GetProperty("id"));
                        //if (DBNull.Value.Equals(tmp))
                        //    tmp = 0;
                        //Usuario.UsuarioID = (int)tmp + 1;
                        AccesoDB.Save(tran, Correo);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, Correo, new List<Cliterio>()
                        {
                            new Cliterio(typeof(ECorreos).GetProperty("id"), OperadoresRelacionales.IGUAL, Correo.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.Borrar:
                        //AccesoDB.delete(tran, contrato);
                        break;
                    case TipoOperacion.BajaLogica:
                        break;
                    default:
                        throw new Exception("No se encontro la operación");
                }

                Correo.Operacion = TipoOperacion.Lectura;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Persona: {0}", Correo)});
            }
        }


    }
}
