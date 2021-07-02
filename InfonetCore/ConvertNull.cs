using System;

namespace Infonet.Core {
	public static class ConvertNull {
		public static int? ToInt32(object o) {
			if (IsNull(o))
				return null;
			return Convert.ToInt32(o);
		}

		public static double? ToDouble(object o) {
			if (IsNull(o))
				return null;
			return Convert.ToDouble(o);
		}

		public static bool? ToBoolean(object o) {
			if (IsNull(o))
				return null;
			return Convert.ToBoolean(o);
		}

		public static DateTime? ToDateTime(object o) {
			if (IsNull(o))
				return null;
			return Convert.ToDateTime(o);
		}

		public static string ToString(object o) {
			if (IsNull(o))
				return null;
			return Convert.ToString(o);
		}

		public static object ToObject(object o) {
			if (IsNull(o))
				return null;
			return o;
		}

		/* returns true if argument is null or DBNull*/
		public static bool IsNull(object o) {
			return o == null || o == Convert.DBNull;
		}
	}
}