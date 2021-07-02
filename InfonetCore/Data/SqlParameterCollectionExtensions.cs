using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infonet.Core.Data {
	public static class SqlParameterCollectionExtensions {
		public static void AddRange(this SqlParameterCollection self, IEnumerable<SqlParameter> parameters) {
			foreach (var each in parameters)
				self.Add(each);
		}
	}
}