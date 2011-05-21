using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RemoteSession
{
    public class SqlProcedure
    {
        private readonly string _connectionString;
        private readonly string _name;
        private static readonly ParameterFactory DefaultParameterFactory = new ParameterFactory();

        public SqlProcedure(string connectionString, string name)
        {
            _connectionString = connectionString;
            _name = name;
        }

        public static SqlProcedure Create(string connectionString, string name)
        {
            return new SqlProcedure(connectionString, name);
        }

        public void Execute(params Func<ParameterFactory, Parameter>[] parameters)
        {
            Execute(null, parameters);
        }

        public void Execute(Action<IDataReader> read, params Func<ParameterFactory, Parameter>[] parameterBuilders)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(_name, connection) { CommandType = CommandType.StoredProcedure };
                var parameters = parameterBuilders.Select(x => x(DefaultParameterFactory)).ToList();
                foreach (var parameter in parameters) command.Parameters.Add(parameter.BaseParameter);
                if (read != null) using (var reader = command.ExecuteReader()) read(reader);
                else command.ExecuteNonQuery();
                foreach (var parameter in parameters) parameter.SetOutValue();
            }
        }

        public class Parameter
        {
            private readonly Action<object> _outValue;

            public Parameter(IDbDataParameter parameter)
            {
                BaseParameter = parameter;
            }

            public Parameter(IDbDataParameter parameter, Action<object> outValue)
            {
                BaseParameter = parameter;
                _outValue = outValue;
            }

            public IDbDataParameter BaseParameter { get; set; }

            public void SetOutValue()
            {
                if (_outValue != null) _outValue(Convert.IsDBNull(BaseParameter.Value) ? null : BaseParameter.Value);
            }
        }

        public class ParameterFactory
        {
            public Parameter In(string name, object value)
            {
                return new Parameter(new SqlParameter(name, value));
            }

            public Parameter Out<T>(string name, SqlDbType type, Action<T> outValue)
            {
                return Out(name, type, 0, outValue);
            }

            public Parameter Out<T>(string name, SqlDbType type, int size, Action<T> outValue)
            {
                return new Parameter(new SqlParameter(name, type, size) { Direction = ParameterDirection.Output }, x => outValue((T)x));
            }
        }
    }
}
