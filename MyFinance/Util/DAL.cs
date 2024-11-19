using System.Data;
using System.Data.SqlClient;

namespace MyFinance.Util
{
    public class DAL
    {
        // String de conexão para o SQL Server
        private static string connectionString = "Server=localhost;Database=MyFinanceDB;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        // Conexão com o SQL Server
        private SqlConnection connection;

        public DAL()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();  // Abrir conexão com o SQL Server
        }

        // Executa SELECTs
        public DataTable RetDataTable(string sql)
        {
            DataTable dataTable = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);  // Usando SqlCommand para SQL Server
            SqlDataAdapter da = new SqlDataAdapter(command);  // Usando SqlDataAdapter para SQL Server
            da.Fill(dataTable);
            return dataTable;
        }

        // Executa INSERTs, UPDATEs, DELETEs
        public void ExecutarComandoSQL(string sql)
        {
            SqlCommand command = new SqlCommand(sql, connection);  // Usando SqlCommand para SQL Server
            command.ExecuteNonQuery();
        }
    }
}
