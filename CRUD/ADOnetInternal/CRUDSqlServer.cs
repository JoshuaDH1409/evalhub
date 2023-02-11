using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using General;
using System.Data.SqlClient;
using System.Data;
using CRUD.Transaction;

namespace CRUD.ADOnetInternal
{
    internal class CRUDSqlServer : CRUD
    {
        #region Miembros
        List<SqlDataAdapter> mListDataAdapter = null;
        private readonly object Padlock = new object();
        Dictionary<string, object> NombreConexiones = new Dictionary<string, object>();
        #endregion

        #region Constructor
        public CRUDSqlServer(string ConnectionString)
            : base(ConnectionString)
        {
#if DEBUG
            mListDataAdapter = new List<SqlDataAdapter>(10);
#else
            mListDataAdapter = new List<SqlDataAdapter>(50);
#endif
            for (int i = 0; i < mListDataAdapter.Capacity; i++)
                mListDataAdapter.Add(null);
        }
        #endregion

        #region Metodos Privados
        private SqlDataAdapter getSQLDataAdapter()
        {
            try
            {
                bool encontrado = false;
                SqlDataAdapter DataAdapter = null;

                lock (Padlock)
                {
                    do
                    {
                        for (int i = 0; i < mListDataAdapter.Count; i++)
                        {
                            if (mListDataAdapter[i] == null)
                            {
                                SqlConnection Connection = new SqlConnection(mConnectionString);
                                SqlCommand command = new SqlCommand("", Connection);
                                mListDataAdapter[i] = new SqlDataAdapter(command);
                                DataAdapter = mListDataAdapter[i];
                                encontrado = true;
                                break;
                            }
                            else if (mListDataAdapter[i].SelectCommand.Connection.State == ConnectionState.Closed)
                            {
                                DataAdapter = mListDataAdapter[i];
                                encontrado = true;
                                break;
                            }
                        }

                    } while (encontrado == false);

                    if (DataAdapter.SelectCommand.Connection.State == ConnectionState.Closed)
                        DataAdapter.SelectCommand.Connection.Open();
                }

                return DataAdapter;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private SqlDataAdapter getSQLDataAdapter(ITransactionCRUD tran)
        {
            try
            {
                if (!(tran is TransactionCRUD))
                    throw new Exception("La transaccion no corresponde a SqlServer");

                TransactionCRUD tmp = (TransactionCRUD)tran;

                if (!(tmp.DataAdapter is SqlDataAdapter))
                    throw new Exception("La transaccion no corresponde a SqlServer");

                return (SqlDataAdapter)tmp.DataAdapter;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        #endregion

        #region Metodos Publicos

        public override ITransactionCRUD BeginsTransaction()
        {
            try
            {
                TransactionCRUD tran = null;

                lock (Padlock)
                {
                    tran = new TransactionCRUD(getSQLDataAdapter());
                }

                return tran;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                               new string[] { string.Format("Error: {0}", ex.Message),
                                                                              string.Format("InnerException: {0}", ex.InnerException)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
        }

        public override List<EntidadBase> Read(EntidadBase entidad, List<Cliterio> cliterios)
        {
            string sentencia = string.Empty;
            SqlDataReader reader = null;
            SqlDataAdapter dataAdapter = null;
            List<EntidadBase> result = new List<EntidadBase>();
            try
            {
                sentencia = Select(entidad, cliterios);
                dataAdapter = getSQLDataAdapter();
                dataAdapter.SelectCommand.CommandTimeout = 0;
                dataAdapter.SelectCommand.CommandText = sentencia;
                reader = dataAdapter.SelectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                    result.Add(Utilidades.LLenaModel(entidad.NuevoObjeto(), reader));

                return result;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));

                return null;
            }
            finally
            {
                if (reader != null) { reader.Close(); reader.Dispose(); reader = null; }
            }
        }

        public override List<EntidadBase> Read(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> cliterios)
        {
            string sentencia = string.Empty;
            SqlDataReader reader = null; ;
            SqlDataAdapter dataAdapter = null;
            List<EntidadBase> result = new List<EntidadBase>();
            try
            {
                sentencia = Select(entidad, cliterios);
                dataAdapter = getSQLDataAdapter(tran);
                dataAdapter.SelectCommand.CommandTimeout = 0;
                dataAdapter.SelectCommand.CommandText = sentencia;
                reader = dataAdapter.SelectCommand.ExecuteReader();

                while (reader.Read())
                    result.Add(Utilidades.LLenaModel(entidad.NuevoObjeto(), reader));

                return result;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));

                return null;
            }
            finally
            {
                if (reader != null) { reader.Close(); reader.Dispose(); reader = null; }
            }
        }

        public override List<EntidadBase> ReadAll(EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataReader reader = null;
            SqlDataAdapter dataAdapter = null;
            List<EntidadBase> result = new List<EntidadBase>();

            try
            {
                sentencia = Select(entidad);
                dataAdapter = getSQLDataAdapter();
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.CommandTimeout = 0;
                reader = dataAdapter.SelectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                    result.Add(Utilidades.LLenaModel(entidad.NuevoObjeto(), reader));

                return result;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));

                return null;
            }
            finally
            {
                if (reader != null) { reader.Close(); reader.Dispose(); reader = null; }
            }
        }

        public override List<EntidadBase> ReadAll(ITransactionCRUD tran, EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataReader reader;
            SqlDataAdapter dataAdapter = null;
            List<EntidadBase> result = new List<EntidadBase>();

            try
            {
                sentencia = Select(entidad);
                dataAdapter = getSQLDataAdapter(tran);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.CommandTimeout = 0;
                reader = dataAdapter.SelectCommand.ExecuteReader();

                while (reader.Read())
                    result.Add(Utilidades.LLenaModel(entidad.NuevoObjeto(), reader));

                return result;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));

                return null;
            }    
        }

        public override bool Save(EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter();
                sentencia = Insert(entidad);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
            finally
            {
                if (dataAdapter != null)
                    dataAdapter.SelectCommand.Connection.Close();
            }
        }

        public override bool Save(ITransactionCRUD tran, EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;
            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                sentencia = Insert(entidad);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }    
        }

        public override bool Update(EntidadBase entidad, List<Cliterio> Cliterio)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter();
                sentencia = UpdateBase(entidad, Cliterio);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
            finally
            {
                if (dataAdapter != null)
                    dataAdapter.SelectCommand.Connection.Close();
            }
        }

        public override bool Update(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                sentencia = UpdateBase(entidad, Cliterio);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
        }

        public override bool delete(EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter();
                sentencia = Delete(entidad);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
            finally
            {
                if (dataAdapter != null)
                    dataAdapter.SelectCommand.Connection.Close();
            }
        }

        public override bool delete(ITransactionCRUD tran, EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                sentencia = Delete(entidad);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
        }

        public override bool delete(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                sentencia = Delete(entidad, Cliterio);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
        }

        public override object MaxId(EntidadBase entidad, PropertyInfo propertycolumn)
        {
            SqlDataAdapter dataAdapter = null;
            string sentencia = string.Empty;

            try
            {
                int result = 0;

                sentencia = Select(entidad, propertycolumn);
                dataAdapter = getSQLDataAdapter();
                dataAdapter.SelectCommand.CommandText = sentencia;
                object dato = dataAdapter.SelectCommand.ExecuteScalar();

                return int.TryParse(dato.ToString(), out result) ? result : 0;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
            finally
            {
                if (dataAdapter != null)
                    dataAdapter.SelectCommand.Connection.Close();
            }
        }

        public override object MaxId(EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio)
        {
            SqlDataAdapter dataAdapter = null;
            string sentencia = string.Empty;

            try
            {             

                sentencia = Select(entidad, propertycolumn, Cliterio);
                dataAdapter = getSQLDataAdapter();
                dataAdapter.SelectCommand.CommandText = sentencia;
                object dato = dataAdapter.SelectCommand.ExecuteScalar();

                return dato;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
            finally
            {
                if (dataAdapter != null)
                    dataAdapter.SelectCommand.Connection.Close();
            }
        }

        public override object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn)
        {
            SqlDataAdapter dataAdapter = null;
            string sentencia = string.Empty;

            try
            {
                int result = 0;

                sentencia = Select(entidad, propertycolumn);
                dataAdapter = getSQLDataAdapter(tran);
                dataAdapter.SelectCommand.CommandText = sentencia;
                object dato = dataAdapter.SelectCommand.ExecuteScalar();

                return int.TryParse(dato.ToString(), out result) ? result : 0;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
        }

        public override object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio)
        {
            SqlDataAdapter dataAdapter = null;
            string sentencia = string.Empty;

            try
            {

                sentencia = Select(entidad, propertycolumn, Cliterio);
                dataAdapter = getSQLDataAdapter(tran);
                dataAdapter.SelectCommand.CommandText = sentencia;
                object dato = dataAdapter.SelectCommand.ExecuteScalar();

                return dato;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
        }

        public override bool deleteAllData(ITransactionCRUD tran, EntidadBase entidad)
        {
            string sentencia = string.Empty;
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                sentencia = DeleteAll(entidad);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException,
                                                            string.Format("Entidad: {0}", entidad.ToString()))});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
        }

        public override bool BulkCopy(ITransactionCRUD tran, List<EntidadBase> listentidad)
        {
            SqlDataAdapter dataAdapter = null;
            SqlBulkCopy bulkcopy = null;
            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                bulkcopy = new SqlBulkCopy(dataAdapter.SelectCommand.Connection);

                bulkcopy.DestinationTableName = NombreTabla(listentidad[0]);
                bulkcopy.BulkCopyTimeout = 0;

                DataTable datasource = GeneraDatatable(listentidad);

                foreach (DataColumn column in datasource.Columns)
                    bulkcopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);

                bulkcopy.WriteToServer(datasource);

                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Lista: {0}", listentidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
            finally
            {
                if (bulkcopy != null)
                {
                    bulkcopy.Close();
                    bulkcopy = null;
                }
            }
        }

        public override string SaveRecuperaSentencia(EntidadBase entidad) 
        {
            try
            {
                return Insert(entidad);
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
        }

        public override string UpdateRecuperaSentencia(EntidadBase entidad, List<Cliterio> Cliterio) 
        {
            try
            {
                return UpdateBase(entidad, Cliterio);
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Entidad: {0}", entidad.ToString())});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return null;
            }
        }

        public override bool ExecSentencia(ITransactionCRUD tran, string sentencia)
        {
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter(tran);
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Sentencia: {1}{0}{1}", sentencia,System.Environment.NewLine)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
        }

        public override bool ExecSentencia(string sentencia)
        {
            SqlDataAdapter dataAdapter = null;

            try
            {
                dataAdapter = getSQLDataAdapter();
                dataAdapter.SelectCommand.CommandText = sentencia;
                dataAdapter.SelectCommand.CommandTimeout = 0;
                dataAdapter.SelectCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ListErrores tmp = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Sentencia: {1}{0}{1}", sentencia,System.Environment.NewLine)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(tmp));
                return false;
            }
            finally
            {
                if (dataAdapter != null)
                    dataAdapter.SelectCommand.Connection.Close();
            }
        }
        #endregion
    }
}