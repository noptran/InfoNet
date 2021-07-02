namespace Infonet.Core.Collections {
	public static class HashCode {
		public const int PRIME = 31;

		public static int Compute(params object[] components) {
			int result = 0;
			foreach (var each in components) {
				result *= PRIME;
				result += each?.GetHashCode() ?? 0;
			}
			return result;
		}
	}
}