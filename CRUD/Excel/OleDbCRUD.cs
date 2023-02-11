using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRUD.Interfaz;
using System.Data;
using System.Data.OleDb;
using General;
using System.Threading;

namespace CRUD.Excel
{
    internal class OleDbCRUD : ICRUDExcel
    {
        public List<string> RecuperaNombreHojas(string archivo)
        {
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=YES; IMEX=0\";", archivo);
                System.Data.DataTable tmp = null;
                List<string> result = new List<string>();

                using (OleDbConnection conexion = new OleDbConnection(connectionString))
                {
                    conexion.Open();

                    tmp = conexion.GetSchema("Tables");
                }

                foreach (DataRow item in tmp.Rows)
                {
                    if(item[2].ToString().Contains("$"))
                        result.Add(item[2].ToString());
                }
                
                return result;
            }
            catch (Exception ex)
            {
                ListErrores exc = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("archivo: {0}", archivo)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(exc));

                return null;
            }
        }

        public List<string> RecueraEncabezados(string archivo, string hoja)
        {
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=YES; IMEX=0\";", archivo);
                System.Data.DataTable tmp = null;
                List<string> result = new List<string>();

                using (OleDbConnection conexion = new OleDbConnection(connectionString))
                {
                    conexion.Open();

                    tmp = conexion.GetSchema("Columns", new string[] {null, null, hoja});
                }

                foreach (DataRow item in tmp.Rows)
                    result.Add(item[3].ToString());

                return result;
            }
            catch (Exception ex)
            {
                ListErrores exc = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Archivo: {0}", archivo),
                                                            string.Format("Hoja: {0}", hoja)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(exc));

                return null;
            }
 
        }

        public List<Dictionary<string, object>> RecuperaTodosDatosPorNombreDecolumna(string archivo, string hoja)
        {
            OleDbDataReader reader = null;
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=YES; IMEX=0\";", archivo);
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = null;

                string query = string.Format("SELECT * FROM [{0}]", hoja);

                using (OleDbConnection conexion = new OleDbConnection(connectionString))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conexion))
                    {
                        conexion.Open();
                        reader = da.SelectCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                                row[reader.GetName(i)] = reader[i];

                            result.Add(row);
                        }

                        conexion.Close();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ListErrores exc = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Archivo: {0}", archivo),
                                                            string.Format("Hoja: {0}", hoja)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(exc));

                return null;
            }
            finally
            {
                if (reader != null) { reader.Close(); reader.Dispose(); reader = null; }
            }
        }

        public List<Dictionary<int, object>> RecuperaTodosDatosPorNumeroDecolumna(string archivo, string hoja)
        {
            OleDbDataReader reader = null;
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=YES; IMEX=0\";", archivo);
                List<Dictionary<int, object>> result = new List<Dictionary<int, object>>();
                Dictionary<int, object> row = null;

                string query = string.Format("SELECT * FROM [{0}]", hoja);

                using (OleDbConnection conexion = new OleDbConnection(connectionString))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conexion))
                    {
                        conexion.Open();
                        reader = da.SelectCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            row = new Dictionary<int, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                                row[i] = reader[i];

                            result.Add(row);
                        }

                        conexion.Close();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ListErrores exc = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Archivo: {0}", archivo),
                                                            string.Format("Hoja: {0}", hoja)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(exc));

                return null;
            }
            finally
            {
                if (reader != null) { reader.Close(); reader.Dispose(); reader = null; }
            }
        }

        public List<string> RecuperaEncabezadosPrimeraHoja(string archivo)
        {
            OleDbConnection conexion = null;
            try
            {
                List<string> result = new List<string>();

                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=YES; IMEX=0\";", archivo);
                System.Data.DataTable tmp = null;

                conexion = new OleDbConnection(connectionString);

                conexion.Open();

                tmp = conexion.GetSchema("Tables");

                string nombreHoja = string.Empty;

                foreach (DataRow item in tmp.Rows)
                {
                    if (item[2].ToString().Contains("$"))
                    {
                        nombreHoja = item[2].ToString();
                        break;
                    }
                }

                tmp = conexion.GetSchema("Columns", new string[] { null, null, nombreHoja });

                foreach (DataRow item in tmp.Rows)
                    result.Add(item[3].ToString());

                return result;
            }
            catch (Exception ex)
            {
                ListErrores exc = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Archivo: {0}", archivo)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(exc));

                return null;
            }
            finally 
            {
                if (conexion != null)
                {
                    conexion.Close();
                    conexion.Dispose();
                    conexion = null;
                }
            }
        }

        public List<Dictionary<string, object>> RecuperaTodosDatosPorNombreDecolumnaPrimeraHoja(string archivo)
        {
            OleDbConnection conexion = null;
            OleDbDataReader reader = null;
            try
            {
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=YES; IMEX=0\";", archivo);
                System.Data.DataTable tmp = null;
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = null;

                conexion = new OleDbConnection(connectionString);

                //eliminar solo lap josé
                //#if DEBUG
                //Thread.Sleep(2000);
                //#endif
                //eliminar solo lap josé


                conexion.Open();

                tmp = conexion.GetSchema("Tables");

                string nombreHoja = string.Empty;

                foreach (DataRow item in tmp.Rows)
                {
                    if (item[2].ToString().Contains("$"))
                    {
                        nombreHoja = item[2].ToString();
                        break;
                    }
                }

                string query = string.Format("SELECT * FROM [{0}]", nombreHoja);

                using (OleDbDataAdapter da = new OleDbDataAdapter(query, conexion))
                {
#if DEBUG
                    DataTable dt = new DataTable();
                    da.Fill(dt);
#endif
                    reader = da.SelectCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                            row[reader.GetName(i)] = reader[i];

                        result.Add(row);
                    }
                    conexion.Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                ListErrores exc = GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Archivo: {0}", archivo)});

                LogCRUD.GuardaLog(GeneraException.RecuperaErrores(exc));

                return null;
            }
            finally
            {
                if (reader != null) { reader.Close(); reader.Dispose(); reader = null; }
                if (conexion != null){ conexion.Close(); conexion.Dispose(); conexion = null; }
            }
        }
    }
}
