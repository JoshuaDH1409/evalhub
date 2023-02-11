using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CRUD
{
    public class DB
    {
        public static DataTable RecuperaInformacion(string queryString)
        {
            try
            {
                SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder() 
                {
                    Password = "admin",
                    UserID = "admin",
                    DataSource = "HPUSER-PC\\SQLEXPRESS2008",
                    InitialCatalog = "Dev",
                };

                DataTable result = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(
                        queryString, connection);
                    adapter.Fill(result);
                    return result;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }
    }
}
