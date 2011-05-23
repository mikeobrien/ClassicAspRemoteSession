using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.Common
{
    public static class SqlClientExtensions
    {
        public static int ExecuteNonQuery(this string connectionString, string commandText, params SqlParameter[] parameters)
        {
            return Execute(connectionString, commandText, parameters, x => x.ExecuteNonQuery());
        }

        public static IDictionary<string, object> ExecuteRecord(this string connectionString, string commandText, params SqlParameter[] parameters)
        {
            return Execute<IDictionary<string, object>>(connectionString, commandText, parameters, 
                                x =>
                                    {
                                        using (var reader = x.ExecuteReader())
                                            return reader.Read() ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, y => reader[y]) : null;
                                    });
        }

        public static T ExecuteScalar<T>(this string connectionString, string commandText, params SqlParameter[] parameters)
        {
            return Execute(connectionString, commandText, parameters, x => (T)x.ExecuteScalar());
        }

        public static object ValueOrNull(this object value)
        {
            return Convert.IsDBNull(value) ? null : value;
        }

        private static T Execute<T>(this string connectionString, string commandText, SqlParameter[] parameters, Func<SqlCommand, T> execute)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(commandText, connection);
                command.Parameters.AddRange(parameters);
                return execute(command);
            }
        }
    }
}