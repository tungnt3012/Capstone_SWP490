using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.DAO
{
    public class SqlDataAccess
    {
        public static string connectionName = "Server=103.139.102.5;Database=gocyberx_icpc;User Id=gocyberx_icpc;Password=Wb8yb46$";
        //Scaffold-DbContext "Server=Server=103.139.102.5;Database=gocyberx_icpc;User Id=gocyberx_icpc;Password=Wb8yb46$" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
        public static List<T> LoadData<T>(string sql)
        {
            using (IDbConnection cnn = new SqlConnection(connectionName))
            {
                return cnn.Query<T>(sql).ToList();
            }
        }

        public static int InsertData<T>(string sql, T data)
        {
            using (IDbConnection cnn = new SqlConnection(connectionName))
            {
                return cnn.Execute(sql, data);
            }
        }

        public static T FindData<T>(string sql, T data)
        {
            using (IDbConnection cnn = new SqlConnection(connectionName))
            {
                return cnn.QueryFirstOrDefault<T>(sql, data);
            }
        }
        public static DataTable GetDataBySQL(string sql, params SqlParameter[] Parameters)
        {
            using (SqlConnection cnn = new SqlConnection(connectionName))
            {
                SqlCommand command = new SqlCommand(sql, cnn);
                command.Parameters.AddRange(Parameters);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                command.Connection.Close();
                return dt;
            }
        }
        public static int ExecuteSQL(string sql, params SqlParameter[] Parameters)
        {
            using (SqlConnection cnn = new SqlConnection(connectionName))
            {
                SqlCommand command = new SqlCommand(sql, cnn);
                command.Parameters.AddRange(Parameters);
                command.Connection.Open();
                int count = command.ExecuteNonQuery();
                command.Connection.Close();
                return count;
            }

        }
    }
}