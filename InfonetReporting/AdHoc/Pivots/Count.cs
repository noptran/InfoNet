using System.Data.SqlClient;

namespace Infonet.Reporting.AdHoc.Pivots {
	public class Count : IAggregate<SqlDataReader, object> {
		private int _count = 0;

		public object Result {
			get { return _count; }
		}

		public void Ingest(SqlDataReader reader) {
			_count++;
		}

		public override string ToString() {
			return $"{GetType().Name}[{Result}]";
		}
	}
}