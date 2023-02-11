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




namespace CapaLogica.Objetivos
{
    class ClsObjetivos
    {
        public static List<EObjetives> RecuperaObjetivos()
        {
            try
            {
                return AccesoDB.ReadAll(new EObjetives());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static EObjetives RecuperaUnObjetivoId(int id)
        {
            try
            {
                List<EObjetives> LiPerfiles = AccesoDB.Read(new EObjetives(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EObjetives).GetProperty("id"), OperadoresRelacionales.IGUAL,id, TipoValor.Numero)

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

        public static List<EObjetives> RecuperaListaObjetivos(int Eval)
        {
            List<EObjetives> LiPerfiles = new List<EObjetives>();
            try
            {
                LiPerfiles = AccesoDB.Read(new EObjetives(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EObjetives).GetProperty("Eval"), OperadoresRelacionales.IGUAL,Eval, TipoValor.Numero)

                });
                return LiPerfiles;
                //if (LiPerfiles.Count > 0)
                //    return LiPerfiles;
                //else
                //    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }

        public static bool GuardaObjetivo(EObjetives Evaluacion)
        {
            try
            {


                if (Evaluacion.id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        Evaluacion.Operacion = TipoOperacion.Modificar;
                        OperacionObjetivos(Evaluacion, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        Evaluacion.Operacion = TipoOperacion.Nuevo;
                        //periodo.Status = 0;

                        object tmp = AccesoDB.MaxId(tran, new EObjetives(), typeof(EObjetives).GetProperty("id"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        Evaluacion.id = max;
                        OperacionObjetivos(Evaluacion, tran);
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

        public static bool EliminaUnObjetivo(int idObj) {

            try
            {
                EObjetives objetivo = RecuperaUnObjetivoId(idObj);
                using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                {
                    objetivo.Operacion = TipoOperacion.Borrar;
                    OperacionObjetivos(objetivo, tran);
                    tran.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                throw;
            }
        }

        private static void OperacionObjetivos(EObjetives objetivos, ITransactionCRUD tran)
        {
            try
            {
                switch (objetivos.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        //object tmp = AccesoDB.MaxId(tran, Usuario, typeof(ELogin).GetProperty("id"));
                        //if (DBNull.Value.Equals(tmp))
                        //    tmp = 0;
                        //Usuario.UsuarioID = (int)tmp + 1;
                        AccesoDB.Save(tran, objetivos);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, objetivos, new List<Cliterio>()
                        {
                            new Cliterio(typeof(EObjetives).GetProperty("id"), OperadoresRelacionales.IGUAL, objetivos.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.Borrar:
                        AccesoDB.delete(tran, new EObjetives(), new List<Cliterio>(){
                            new Cliterio(typeof(EObjetives).GetProperty("id"), OperadoresRelacionales.IGUAL,objetivos.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.BajaLogica:
                        break;
                    default:
                        throw new Exception("No se encontro la operación");
                }

                objetivos.Operacion = TipoOperacion.Lectura;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Persona: {0}", objetivos)});
            }
        }
        public static List<EEscaleta> RecuperaEscaleta()
        {
            try
            {
                return AccesoDB.ReadAll(new EEscaleta());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        public static List<EEscaletaEmpresa> RecuperaEscaletaE()
        {
            try
            {
                return AccesoDB.ReadAll(new EEscaletaEmpresa());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

    }
}
