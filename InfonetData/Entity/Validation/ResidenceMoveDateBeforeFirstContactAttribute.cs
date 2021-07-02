using System.ComponentModel.DataAnnotations;

namespace Infonet.Data.Entity.Validation {
	public class ResidenceMoveDateBeforeFirstContactAttribute : ValidationAttribute {
		public override bool IsValid(object value) {
			//return Approval.IsNumberReasonable(value.ToString());
			return false;
		}
	}
}