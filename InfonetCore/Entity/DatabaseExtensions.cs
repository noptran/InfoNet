using System;
using System.Data.Entity;

namespace Infonet.Core.Entity {
	public static class DatabaseExtensions {
		public static void LogLine(this Database database, string message = "", params object[] args) {
			if (args == null)
				args = new object[] { null };
			database.Log(string.Format(message, args) + Environment.NewLine);
		}
	}
}