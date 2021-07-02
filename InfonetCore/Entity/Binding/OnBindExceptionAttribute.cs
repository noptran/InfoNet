using System;
using System.Linq;

namespace Infonet.Core.Entity.Binding {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class OnBindExceptionAttribute : Attribute {
		public OnBindExceptionAttribute(string messageTemplate, params Type[] exceptionTypes) {
			if (exceptionTypes == null || exceptionTypes.Length == 0)
				exceptionTypes = new[] { typeof(Exception) };
			MessageTemplate = messageTemplate;
			ExceptionTypes = exceptionTypes;
		}

		/** {0} refers to AttemptedValue and {1} refers to DisplayName. **/
		public string MessageTemplate { get; }

		/** exceptionTypes default to { typeof(Exception) } if none specified **/
		public Type[] ExceptionTypes { get; }

		public bool AppliesTo(Exception e, bool checkInners) {
			for (var each = e; each != null; each = each.InnerException) {
				var eachType = each.GetType();
				if (ExceptionTypes.Any(t => t.IsAssignableFrom(eachType)))
					return true;
			}
			return false;
		}
	}
}