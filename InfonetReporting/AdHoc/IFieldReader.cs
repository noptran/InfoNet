using System.Data.SqlClient;

namespace Infonet.Reporting.AdHoc {
	public interface IFieldReader {
		Field Field { get; }

		/* must return null instead of DBNull */
		object Read(SqlDataReader reader);
	}
}