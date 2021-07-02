using System.Data;
using System.Data.SqlClient;

namespace Infonet.Core.Data {
	public static class SqlCommandExtensions {
		public static SqlDataReader ExecuteReader(this SqlCommand command, CommandBehavior behavior, bool openConnectionIfClosed) {
			if (openConnectionIfClosed && command.Connection.State == ConnectionState.Closed)
				command.Connection.Open();
			return command.ExecuteReader(behavior);
		}
	}
}