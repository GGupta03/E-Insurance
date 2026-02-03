using System.Data;
using Microsoft.Data.SqlClient;

namespace E_Insurance_App.Helpers
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using SqlConnection conn = GetConnection();
            using SqlCommand cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using SqlConnection conn = GetConnection();
            using SqlCommand cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using SqlConnection conn = GetConnection();
            using SqlCommand cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteScalar();
        }
    }
}
