using System;
using System.Collections.Generic;
using Modelo;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace CapaLogica.Excel
{
    class ClsExcel
    {


        string errores = "";
        public string ProcesaUsuarios(string rutaArchivo, string nombreArchivo)
        {
            string error = "";
            DataSet respuesta = new DataSet();
            try
            {
                respuesta = LeeArchivo(rutaArchivo, nombreArchivo);
                List<ELogin> listaUsuarios = RecuperaListaUsuarios(respuesta);
                error = Seguridad.Seguridad.GuardaListaUsuarios(listaUsuarios);
                return errores;                
            }
            catch
            {
                return errores;
            }
            finally
            {
                if (System.IO.File.Exists(rutaArchivo))
                {
                    // Use a try block to catch IOExceptions, to
                    // handle the case of the file already being
                    // opened by another process.
                    try
                    {
                        System.IO.File.Delete(rutaArchivo);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);

                    }
                }
            }

        }





        public List<ELogin> RecuperaListaUsuarios(DataSet ds)
        {
            List<ELogin> resultado = new List<ELogin>();
            int temp = 0;
            foreach (DataRow Fila in ds.Tables[0].Rows)
            {

                ELogin temporal = new ELogin();
                try
                {//se busca el dato por nombre de la cabecera
                    
                 

                    temporal.id_sap = Fila["NumEmpSAP"].ToString();
                    //temporal.NombreCompleto = Fila["NombreCompleto"].ToString();
                    temporal.Puesto = Fila["Puesto"].ToString();
                    temporal.Division =Fila["Division"].ToString();
                    //temporal.Division = Fila["Direccion"].ToString();
                    temporal.FechaIngreso = FechaTick(Fila["FechaAntiguedad"].ToString());
                    temporal.FechaAntiguedad = FechaTick(Fila["FechaAntiguedad"].ToString());
                    temporal.EvaluadorIdSap =Fila["EvaluadorDirecto"].ToString();
                    string temporalPais= Fila["Pais"].ToString();


                    if (temporalPais == "México")
                    {
                        temporal.Pais = 1;
                    }
                    else if (temporalPais == "Argentina")
                    {
                        temporal.Pais = 2;
                    }
                    else if (temporalPais == "Colombia")
                    {
                        temporal.Pais = 3;
                    }
                    else if (temporalPais == "Venezuela")
                    {
                        temporal.Pais = 4;
                    }
                    else if (temporalPais == "Ecuador")
                    {
                        temporal.Pais = 5;
                    }
                    else if (temporalPais == "Chile")
                    {
                        temporal.Pais = 6;
                    }
                    else if (temporalPais == "Perú")
                    {
                        temporal.Pais = 7;
                    }
                    else if (temporalPais == "Costa rica")
                    {
                        temporal.Pais = 8;
                    }
                    else if (temporalPais == "Vallejo")
                    {
                        temporal.Pais = 9;
                    }
                    else if (temporalPais == "LATAM")
                    {
                        temporal.Pais = 9;
                    }
                    else if (temporalPais == "Caricam") {
                        temporal.Pais = 10;
                    }
                    else
                    {
                        temporal.Pais = 10;
                    } 

                    if (Fila["Perfil"] == null || Fila["Perfil"].ToString() == "")
                    {
                        temporal.perfil = 4;
                    }
                    else
                    {
                        temporal.perfil = Convert.ToInt32(Fila["Perfil"].ToString());
                    }

                    temporal.Email = Fila["Email"].ToString();

                    temporal.Password = "Aspen2017";
                    temporal.Activo = true;

                    resultado.Add(temporal);
                }
                catch (Exception ex)
                {
                    errores = errores + "\n fila-> " + temp;

                }
                finally
                {
                    temp = temp + 1;
                }
            }
            return resultado;
        }



        private DataSet LeeArchivo(string rutaArchivo, string nombreArchivo)
        {
            if (File.Exists(rutaArchivo))
            {
                string connectionString = nombreArchivo.ToLower().EndsWith(".csv") ? string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Text;HDR=YES;FMT=Delimited\"", (object)Path.GetDirectoryName(rutaArchivo)) : "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + rutaArchivo + ";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=2\"";
                OleDbConnection ObjConn = new OleDbConnection(connectionString);
                ObjConn.Open();
                string strSheetName = getSheetName(ObjConn);
                ObjConn.Close();

                string cmdText = nombreArchivo.ToLower().EndsWith(".csv") ? "SELECT * FROM [" + nombreArchivo + "]" : "SELECT * FROM [" + strSheetName + "]"; //va sin signo de $ por que ya lo trae en el nuevo metodo
                OleDbConnection connection = new OleDbConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        return (DataSet)null;
                    }
                }
                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(new OleDbCommand(cmdText, connection));
                DataSet dataSet = new DataSet();
                try
                {
                    oleDbDataAdapter.Fill(dataSet);
                }
                catch (Exception ex)
                {
                    return (DataSet)null;
                }
                finally
                {
                    oleDbDataAdapter.Dispose();
                    connection.Close();
                    connection.Dispose();
                }
                return dataSet;
            }
            return (DataSet)null;
        }


        private string getSheetName(OleDbConnection ObjConn)
        {
            string strSheetName = String.Empty;
            try
            {
                System.Data.DataTable dtSheetNames = ObjConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dtSheetNames.Rows.Count > 0)
                {
                    strSheetName = dtSheetNames.Rows[0]["TABLE_NAME"].ToString();
                }
                return strSheetName;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get the sheet name", ex);
            }
        }

        #region utilidades
        private static long FechaTick(string fecha)
        {
            DateTime temp = DateTime.Parse(fecha);
            long respuesta = temp.Ticks;
            return respuesta;
        }

        public static long DateToTicks(DateTime dtInput)
        {
            long ticks = 0;
            ticks = dtInput.Ticks;
            return ticks;
        }

        public static string TiksToDate(long fecha)
        {
            DateTime temp = new DateTime(fecha);
            string auxday = "", auxMonth = "";

            if (temp.Day.ToString().Length < 2)
            {
                auxday = "0" + temp.Day.ToString();
            }
            else
            {
                auxday = temp.Day.ToString();
            }

            if (temp.Month.ToString().Length < 2)
            {
                auxMonth = "0" + temp.Month.ToString();
            }
            else
            {
                auxMonth = temp.Month.ToString();
            }

            string res = auxday + "/" + auxMonth + "/" + temp.Year.ToString();
            return res;
        }

        #endregion



    }
}
