using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Infonet.Core.Reflection {
	public static class AttributeCollectionExtensions {
		public static IEnumerable<TDesired> Filter<TDesired>(this AttributeCollection self) where TDesired : Attribute {
			var desiredType = typeof(TDesired);
			var result = new List<TDesired>();
			foreach (var each in self)
				if (desiredType.IsInstanceOfType(each))
					result.Add((TDesired)each);
			return result.AsReadOnly();
		}
	}
}