using System;
using System.Linq;
using System.Reflection;

namespace Infonet.Core.Entity {
	//KMS: Could probably, eventually combine this with DeleteIfNulled
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class RemoveIfTrueAttribute : Attribute {
		private static readonly char[] _PropertyDelimiters = { ',', ' ' };
		private readonly string[] _propertyNames;
		private PropertyInfo[] _properties = null;

		public RemoveIfTrueAttribute(string commaSeparatedProperties) {
			_propertyNames = commaSeparatedProperties.Split(_PropertyDelimiters, StringSplitOptions.RemoveEmptyEntries);
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

			return _properties.All(each => Convert.ToBoolean(each.GetValue(target)));
		}
	}
}