using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ViewModels.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public class ReportSpecificRequirementAttribute : ValidationAttribute {
		public SubReportSelection SubReportSelection { get; }

		public ReportSpecificRequirementAttribute(SubReportSelection subReportSelection, string errorMessage) : base(errorMessage) {
			SubReportSelection = subReportSelection;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			var context = validationContext.ObjectInstance as ManagementReportViewModel;
			if (context == null)
				throw new InvalidOperationException("ReportSpecificRequirementAttribute can only be used on Properties of the ManagementReportViewModel class.");

			if (context.ReportSelection == SubReportSelection && value == null)
				return new ValidationResult(ErrorMessage);

			return ValidationResult.Success;
		}
	}
}