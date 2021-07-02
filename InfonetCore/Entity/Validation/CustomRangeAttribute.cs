using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	public class CustomRangeAttribute : RangeAttribute {
		// NEH DO Fix for comma delimited Population. Should we keep?

		public CustomRangeAttribute(double min, double max) : base(min, max) { }

		public override bool IsValid(object value) {
			if (value == null)
				return base.IsValid(null);

			string strVal = value.ToString();
			int index = strVal.IndexOf(',');
			while (index > -1) {
				strVal = strVal.Remove(index, 1);
				index = strVal.IndexOf(',');
			}
			double newValue;
			return double.TryParse(strVal, out newValue) && base.IsValid(newValue);
		}
	}
}