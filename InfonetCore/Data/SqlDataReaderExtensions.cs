using System;
using System.Data.SqlClient;

namespace Infonet.Core.Data {
	public static class SqlDataReaderExtensions {
		/* returns 'default' if 'name' not found unless 'default' out of range (then returns -1) */
		public static int OrdinalOf(this SqlDataReader reader, string name, int @default = -1) {
			try {
				return reader.GetOrdinal(name);
			} catch (IndexOutOfRangeException) {
				return @default < 0 || @default >= reader.VisibleFieldCount ? -1 : @default;
			}
		}

		public static string[] Headers(this SqlDataReader reader) {
			var result = new string[reader.VisibleFieldCount];
			for (int i = 0; i < result.Length; i++)
				result[i] = reader.GetName(i);
			return result;
		}
	}
}