using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Data.Importing {
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public class SqlCommandFactory {
		public SqlCommandFactory(SqlConnection connection, int timeout) {
			Connection = connection;
			Timeout = timeout;
		}

		public SqlConnection Connection { get; }

		public int Timeout { get; }

		public SqlCommand Sql(string sql, params object[] parametersOrValues) {
			var result = new SqlCommand {
				CommandType = CommandType.Text,
				Connection = Connection,
				CommandTimeout = Timeout,
				CommandText = sql
			};
			if (parametersOrValues == null) {
				result.Parameters.AddWithValue("p0", null);
			} else if (parametersOrValues.Length > 0) {
				var parameters = new SqlParameter[parametersOrValues.Length];
				for (int i = 0; i < parameters.Length; i++)
					parameters[i] = parametersOrValues[i] as SqlParameter ?? new SqlParameter("p" + i, parametersOrValues[i]);
				result.Parameters.AddRange(parameters);
			}
			return result;
		}

		public SqlCommand StoredProcedure(string name, SqlParameter[] parameters = null) {
			var result = new SqlCommand {
				CommandType = CommandType.StoredProcedure,
				Connection = Connection,
				CommandTimeout = Timeout,
				CommandText = name
			};
			if (parameters != null)
				result.Parameters.AddRange(parameters);
			return result;
		}
	}
}