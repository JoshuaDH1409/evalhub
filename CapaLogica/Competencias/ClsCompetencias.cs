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

namespace CapaLogica.Competencias
{
    class ClsCompetencias
    {
        public static List<ECompetemces> RecuperaCompetencias()
        {
            try
            {
                return AccesoDB.ReadAll(new ECompetemces());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static ECompetemces RecuperaUnaCompetencia(int id)
        {
            try
            {
                List<ECompetemces> LiPerfiles = AccesoDB.Read(new ECompetemces(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ECompetemces).GetProperty("id"), OperadoresRelacionales.IGUAL,id, TipoValor.Numero)
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

        public static List<ECompetemces> RecuperaLiCompetencias(int Eval)
        {
            try
            {
                List<ECompetemces> LiPerfiles = AccesoDB.Read(new ECompetemces(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ECompetemces).GetProperty("Eval"), OperadoresRelacionales.IGUAL,Eval, TipoValor.Numero)
                });

                if (LiPerfiles.Count > 0)
                    return LiPerfiles;
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
        
        public static bool GuardaCompetencias(ECompetemces Competencia)
        {
            try
            {


                if (Competencia.id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        Competencia.Operacion = TipoOperacion.Modificar;
                        OperacionCompetencias(Competencia, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        Competencia.Operacion = TipoOperacion.Nuevo;
                        //periodo.Status = 0;

                        object tmp = AccesoDB.MaxId(tran, new ECompetemces(), typeof(ECompetemces).GetProperty("id"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        Competencia.id = max;
                        OperacionCompetencias(Competencia, tran);
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

        private static void OperacionCompetencias(ECompetemces Competencias, ITransactionCRUD tran)
        {
            try
            {
                switch (Competencias.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        //object tmp = AccesoDB.MaxId(tran, Usuario, typeof(ELogin).GetProperty("id"));
                        //if (DBNull.Value.Equals(tmp))
                        //    tmp = 0;
                        //Usuario.UsuarioID = (int)tmp + 1;
                        AccesoDB.Save(tran, Competencias);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, Competencias, new List<Cliterio>()
                        {
                            new Cliterio(typeof(ECompetemces).GetProperty("id"), OperadoresRelacionales.IGUAL, Competencias.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.Borrar:
                        AccesoDB.delete(tran, new ECompetemces(), new List<Cliterio>(){
                            new Cliterio(typeof(ECompetemces).GetProperty("id"), OperadoresRelacionales.IGUAL,Competencias.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.BajaLogica:
                        break;
                    default:
                        throw new Exception("No se encontro la operación");
                }

                Competencias.Operacion = TipoOperacion.Lectura;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Persona: {0}", Competencias)});
            }
        }

        public static bool EliminaUnaCompetencia(int idCompetencia)
        {

            try
            {
                ECompetemces Competencia = RecuperaUnaCompetencia(idCompetencia);
                using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                {
                    Competencia.Operacion = TipoOperacion.Borrar;
                    OperacionCompetencias(Competencia, tran);
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


    }
}
