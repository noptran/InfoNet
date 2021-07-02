﻿using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Infonet.Core.Reflection {
	public static class MemberInfoExtensions {
		public static T GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute {
			var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();
			if (attribute == null && isRequired)
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The {0} attribute must be defined on member {1}", typeof(T).Name, member.Name));
			return (T)attribute;
		}
	}
}