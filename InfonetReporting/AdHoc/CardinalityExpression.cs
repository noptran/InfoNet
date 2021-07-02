namespace Infonet.Reporting.AdHoc {
	public struct CardinalityExpression {
		public CardinalityExpression(Cardinal left, Cardinal right) {
			Left = left;
			Right = right;
		}

		public Cardinal Left { get; }

		public Cardinal Right { get; }

		public bool AllowsZero {
			get { return ((Left | Right) & Cardinal.Zero) > 0; }
		}

		public CardinalityExpression Reverse() {
			return new CardinalityExpression(Right, Left);
		}
	}
}