using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Collections;

namespace Infonet.Data.Importing {
	public class SqlParameters : Parameters {
		public SqlParameters() { }

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public SqlParameters(IDictionary<string, object> dictionary) : base(dictionary) { }

		public static implicit operator SqlParameter[](SqlParameters parameters) {
			var result = new SqlParameter[parameters.Count];
			int i = 0;
			foreach (var each in parameters)
				result[i++] = new SqlParameter(each.Key, each.Value);
			return result;
		}
	}
}