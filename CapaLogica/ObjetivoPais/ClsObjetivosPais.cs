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




namespace CapaLogica.ObjetivosPais
{
    class ClsObjetivosPais
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

        public static EObjetivoPais RecuperaObjetivo(int id)
        {
            try
            {
                List<EObjetivoPais> LiObjetivos = AccesoDB.Read(new EObjetivoPais(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EObjetivoPais).GetProperty("ID"), OperadoresRelacionales.IGUAL,id, TipoValor.Numero)

                });

                if (LiObjetivos.Count > 0)
                    return LiObjetivos[0];
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

        public static List<EObjetivoPais> RecuperaObjetivos(int pais, int periodo)
        {
            List<EObjetivoPais> LiObjetivos = new List<EObjetivoPais>();
            try
            {
                LiObjetivos = AccesoDB.Read(new EObjetivoPais(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EObjetivoPais).GetProperty("Periodo_Id"), OperadoresRelacionales.IGUAL, periodo, TipoValor.Numero),
                    new Cliterio(OperadoresLogicos.AND,typeof(EObjetivoPais).GetProperty("Pais_Id"), OperadoresRelacionales.IGUAL, pais, TipoValor.Numero)

                });
                return LiObjetivos;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }

        public static bool GuardaObjetivo(EObjetivoPais objetivoPais)
        {
            try
            {


                if (objetivoPais.ID != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        objetivoPais.Operacion = TipoOperacion.Modificar;
                        OperacionObjetivos(objetivoPais, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        objetivoPais.Operacion = TipoOperacion.Nuevo;
                        object tmp = AccesoDB.MaxId(tran, new EObjetivoPais(), typeof(EObjetivoPais).GetProperty("ID"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        objetivoPais.ID = max;
                        OperacionObjetivos(objetivoPais, tran);
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

        public static bool EliminaObjetivo(int idObj) {

            try
            {
                EObjetivoPais objetivo = RecuperaObjetivo(idObj);
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

        private static void OperacionObjetivos(EObjetivoPais objetivos, ITransactionCRUD tran)
        {
            try
            {
                switch (objetivos.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        AccesoDB.Save(tran, objetivos);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, objetivos, new List<Cliterio>()
                        {
                            new Cliterio(typeof(EObjetivoPais).GetProperty("ID"), OperadoresRelacionales.IGUAL, objetivos.ID, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.Borrar:
                        AccesoDB.delete(tran, new EObjetivoPais(), new List<Cliterio>(){
                            new Cliterio(typeof(EObjetivoPais).GetProperty("ID"), OperadoresRelacionales.IGUAL,objetivos.ID, TipoValor.Numero)
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

    }
}
