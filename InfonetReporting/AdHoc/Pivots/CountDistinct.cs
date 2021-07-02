using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infonet.Reporting.AdHoc.Pivots {
	public class CountDistinct : IAggregate<SqlDataReader, object> {
		private readonly HashSet<object> _distinct = new HashSet<object>();
		private readonly IFieldReader _key;

		public CountDistinct(Field key) {
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			_key = key.CreateReader();
		}

		public object Result {
			get { return _distinct.Count; }
		}

		public void Ingest(SqlDataReader reader) {
			_distinct.Add(_key.Read(reader));
		}

		public override string ToString() {
			return $"{GetType().Name}[{Result}]";
		}
	}
}