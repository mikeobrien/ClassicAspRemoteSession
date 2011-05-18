using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RemoteSession
{
    public class SqlSessionStore
    {
        private readonly string _connectionString;

        public SqlSessionStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetApplicationId(string applicationName)
        {
            int applicationId = 0;
            ExecuteProcedure("TempGetAppID",
                x => x(new SqlParameter("@appName", applicationName), y => { }),
                x => x(new SqlParameter("@appId", SqlDbType.Int) { Direction = ParameterDirection.Output }, y => applicationId = (int)y));
            return applicationId;
        }

        private void ExecuteProcedure(string name, params Action<Action<SqlParameter, Action<object>>>[] initParameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(name, connection) { CommandType = CommandType.StoredProcedure };
                var parameters = new List<Tuple<SqlParameter, Action<object>>>();
                foreach (var initParameter in initParameters)
                {
                    SqlParameter parameter = null;
                    Action<object> value = null;
                    initParameter((p, v) => { parameter = p; value = v; });
                    parameters.Add(Tuple.Create(parameter, value));
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
                foreach (var parameter in parameters.Where(x => x.Item2 != null)) 
                    parameter.Item2(parameter.Item1.Value);
            }
        }
    }
}