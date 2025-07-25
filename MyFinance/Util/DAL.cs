using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

namespace MyFinance.Util
{
    public class DAL : IDisposable
    {
        private readonly string connectionString;
        private bool disposed = false;

        public DAL(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
             

        // Executa SELECTs com parâmetros seguros
        public DataTable RetDataTable(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable dataTable = new DataTable();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    using (var da = new SqlDataAdapter(command))
                    {
                        da.Fill(dataTable);
                    }
                }
                return dataTable;
            }
        }

        // Executa INSERTs, UPDATEs, DELETEs com parâmetros seguros
        public int ExecutarComandoSQL(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Executa queries que retornam valor único (scalar)
        public object ExecutarScalar(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    return command.ExecuteScalar();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                // Cleanup managed resources if needed
                disposed = true;
            }
        }
    }
}
