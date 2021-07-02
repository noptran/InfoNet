using System;

namespace Infonet.Reporting.Core {
	public class OrderedTextOutput : IComparable, IComparable<OrderedTextOutput> {
		public string Output { get; set; }
		public int DisplayOrder { get; set; }

		public int CompareTo(object other) {
			return CompareTo((OrderedTextOutput)other);
		}

		public int CompareTo(OrderedTextOutput other) {
			return DisplayOrder.CompareTo(other.DisplayOrder);
		}
	}
}