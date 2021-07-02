using System.Text;
using System.Text.RegularExpressions;

namespace Infonet.Core {
	public static class CamelCase {
		private static readonly Regex _SpaceAfter = new Regex("([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))");

		public static string ToProper(string camelCase) {
			if (string.IsNullOrEmpty(camelCase))
				return camelCase;

			var sb = new StringBuilder(_SpaceAfter.Replace(camelCase, "$1 "));
			sb[0] = char.ToUpper(sb[0]);
			return sb.ToString();
		}

		public static string FromPascal(string pascalCase) {
			if (string.IsNullOrEmpty(pascalCase))
				return pascalCase;

			int firstLower = IndexOfLower(pascalCase);
			if (firstLower == 0)
				return pascalCase;

			if (firstLower == -1)
				return pascalCase.ToLower();

			var sb = new StringBuilder(pascalCase);
			if (firstLower == 1)
				sb[0] = char.ToLower(sb[0]);
			else
				for (int i = 0; i < firstLower - 1; i++)
					sb[i] = char.ToLower(sb[i]);

			return sb.ToString();
		}

		#region private
		private static int IndexOfLower(string s) {
			if (string.IsNullOrEmpty(s))
				return -1;

			int i = 0;
			for (; i < s.Length; i++)
				if (char.IsLower(s[i]))
					break;
			return i < s.Length ? i : -1;
		}
		#endregion
	}
}