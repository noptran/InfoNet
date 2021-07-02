using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Infonet.Core.Entity {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class DeleteIfNulledAttribute : Attribute {
		private readonly string[] _propertyNames;
		private PropertyInfo[] _properties = null;

		public DeleteIfNulledAttribute(string commaSeparatedProperties) {
			_propertyNames = Regex.Split(commaSeparatedProperties, @"\s*,\s*");
		}

		public bool AppliesTo(object target) {
			if (_properties == null) {
				var properties = new PropertyInfo[_propertyNames.Length];
				for (int i = 0; i < _propertyNames.Length; i++) {
					properties[i] = target.GetType().GetProperty(_propertyNames[i]);
					if (properties[i] == null)
						throw new InvalidOperationException("No such property: " + target.GetType().FullName + "." + _propertyNames[i]);
				}
				_properties = properties;
			}

			foreach (var each in _properties)
				if (each.GetValue(target) != null)
					return false;

			return true;
		}
	}
}