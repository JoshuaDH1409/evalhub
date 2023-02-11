using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRUD;
using CRUD.Interfaz;
using CRUD.Transaction;
using General;
using System.Reflection;
using System.Data;

namespace CapaLogica.Funciones
{
    internal class AccesoDB
    {
        public static ITransactionCRUD BeginsTransaction()
        {
            try
            {
                ITransactionCRUD tran = Singleton.Instance.CRUD.BeginsTransaction();

                if (tran == null)
                    throw new Exception("No se puede inicia la transaccion");

                return tran;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static List<TEntidad> ReadAll<TEntidad>(TEntidad entidad) where TEntidad : EntidadBase
        {
            try
            {
                List<TEntidad> tmp = new List<TEntidad>();

                List<EntidadBase> consulta = Singleton.Instance.CRUD.ReadAll(entidad);

                if (consulta == null)
                    throw new Exception("No se pudo recuperar la información de la DB.");

                foreach (TEntidad item in consulta)
                    tmp.Add(item);

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static List<TEntidad> ReadAll<TEntidad>(ITransactionCRUD tran, TEntidad entidad) where TEntidad : EntidadBase
        {
            try
            {
                List<TEntidad> tmp = new List<TEntidad>();

                List<EntidadBase> consulta = Singleton.Instance.CRUD.ReadAll(tran, entidad);

                if (consulta == null)
                    throw new Exception("No se pudo recuperar la información de la DB.");

                foreach (TEntidad item in consulta)
                    tmp.Add(item);

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static List<TEntidad> Read<TEntidad>(TEntidad entidad, List<Cliterio> cliterios) where TEntidad : EntidadBase
        {
            try
            {
                List<TEntidad> tmp = new List<TEntidad>();
                List<EntidadBase> consulta = Singleton.Instance.CRUD.Read(entidad, cliterios);
                if (consulta == null)
                    throw new Exception("No se pudo recuperar la información de la DB.");

                foreach (TEntidad item in consulta)
                    tmp.Add(item);

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static List<TEntidad> Read<TEntidad>(ITransactionCRUD tran, TEntidad entidad, List<Cliterio> cliterios) where TEntidad : EntidadBase
        {
            try
            {
                List<TEntidad> tmp = new List<TEntidad>();
                List<EntidadBase> consulta = Singleton.Instance.CRUD.Read(tran, entidad, cliterios);

                if (consulta == null)
                    throw new Exception("No se pudo recuperar la información de la DB.");

                foreach (TEntidad item in consulta)
                    tmp.Add(item);

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void Save(ITransactionCRUD tran, EntidadBase entidad)
        {
            try
            {
                if (!Singleton.Instance.CRUD.Save(tran, entidad))
                    throw new Exception("Error al guardar DB.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void Update(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio)
        {
            try
            {
                if (!Singleton.Instance.CRUD.Update(tran, entidad, Cliterio))
                    throw new Exception("Error al actualizar DB.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void delete(ITransactionCRUD tran, EntidadBase entidad)
        {
            try
            {
                if (!Singleton.Instance.CRUD.delete(tran, entidad))
                    throw new Exception("Error al borrar DB.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void delete(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio)
        {
            try
            {
                if (!Singleton.Instance.CRUD.delete(tran, entidad, Cliterio))
                    throw new Exception("Error al borrar DB.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn)
        {
            try
            {
                object tmp = Singleton.Instance.CRUD.MaxId(tran, entidad, propertycolumn);

                if (tmp == null)
                    throw new Exception("No se Puede recupera el maximo de la columna " + propertycolumn.ToString());

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio)
        {
            try
            {
                object tmp = Singleton.Instance.CRUD.MaxId(tran, entidad, propertycolumn, Cliterio);

                if (tmp == null)
                    throw new Exception("No se Puede recupera el maximo de la columna " + propertycolumn.ToString());

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static object MaxId(EntidadBase entidad, PropertyInfo propertycolumn)
        {
            try
            {
                object tmp = Singleton.Instance.CRUD.MaxId(entidad, propertycolumn);

                if (tmp == null)
                    throw new Exception("No se Puede recupera el maximo de la columna " + propertycolumn.ToString());

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static object MaxId(EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio)
        {
            try
            {
                object tmp = Singleton.Instance.CRUD.MaxId(entidad, propertycolumn, Cliterio);

                if (tmp == null)
                    throw new Exception("No se Puede recupera el maximo de la columna " + propertycolumn.ToString());

                return tmp;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void deleteAllData(ITransactionCRUD tran, EntidadBase entidad)
        {
            try
            {
                if (!Singleton.Instance.CRUD.deleteAllData(tran, entidad))
                    throw new Exception("Error al borrar DB.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void BulkCopy<TEntidad>(ITransactionCRUD tran, List<TEntidad> listentidad) where TEntidad : EntidadBase
        {
            try
            {
                if (!Singleton.Instance.CRUD.BulkCopy(tran, new List<EntidadBase>(listentidad)))
                    throw new Exception("Error al realizar bulk copy en DB.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", listentidad.ToString())});
            }
        }

        public static string SaveRecuperaSentencia(EntidadBase entidad)
        {
            try
            {
                string sentencia = Singleton.Instance.CRUD.SaveRecuperaSentencia(entidad);

                if (!string.IsNullOrWhiteSpace(sentencia))
                    return sentencia;
                else
                    throw new Exception("No se pudo recuperar la sentencia.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static string UpdateRecuperaSentencia(EntidadBase entidad, List<Cliterio> Cliterio)
        {
            try
            {
                string sentencia = Singleton.Instance.CRUD.UpdateRecuperaSentencia(entidad, Cliterio);

                if (!string.IsNullOrWhiteSpace(sentencia))
                    return sentencia;
                else
                    throw new Exception("No se pudo recuperar la sentencia.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad)});
            }
        }

        public static void ExecSentencia(ITransactionCRUD tran, string sentencia)
        {
            try
            {
                if (!Singleton.Instance.CRUD.ExecSentencia(tran, sentencia))
                    throw new Exception("No se pudo ejecutar la sentencia.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Sentencia: {1}{0}{1}", sentencia,System.Environment.NewLine)});
            }
        }

        public static void ExecSentencia(string sentencia)
        {
            try
            {
                if (!Singleton.Instance.CRUD.ExecSentencia(sentencia))
                    throw new Exception("No se pudo ejecutar la sentencia.");
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Sentencia: {1}{0}{1}", sentencia,System.Environment.NewLine)});
            }
        }

    }
}
