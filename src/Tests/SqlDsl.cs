using System.Data.SqlClient;

namespace Tests
{
    public static class SqlDsl
    {
        public static void ExecuteNonQuery(this string connectionString, string format, params object[] args)
        {
            ExecuteNonQuery(connectionString, string.Format(format, args));
        }

        public static void ExecuteNonQuery(this string connectionString, string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                new SqlCommand(commandText, connection).ExecuteNonQuery();   
            }
        }

        public static T ExecuteScalar<T>(this string connectionString, string format, params object[] args)
        {
            return ExecuteScalar<T>(connectionString, string.Format(format, args));
        }

        public static T ExecuteScalar<T>(this string connectionString, string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (T)new SqlCommand(commandText, connection).ExecuteScalar();
            }
        }
    }
}