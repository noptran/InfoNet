using System.Data;

namespace Infonet.Core.Data {
	public static class DataRowExtensions {
		public static int? GetInt32(this DataRow row, string columnName) {
			return ConvertNull.ToInt32(row[columnName]);
		}
	}
}