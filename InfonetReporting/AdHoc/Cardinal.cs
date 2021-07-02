using System;

namespace Infonet.Reporting.AdHoc {
	[Flags]
	public enum Cardinal {
		Zero = 1, /* for bitwise operations only */
		One = 2,
		Many = 4,
		ZeroOrOne = 3,
		ZeroOrMany = 5
	}

	public static class CardinalEnum {
		public static CardinalityExpression To(this Cardinal left, Cardinal right) {
			return new CardinalityExpression(left, right);
		}
	}
}