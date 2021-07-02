using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Infonet.Core.IO;

//KMS DO move this to Core?
namespace Infonet.Web.Mvc.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public class FileExtensionAttribute : ValidationAttribute {
		public FileExtensionAttribute(string fileExtensions) {
			if (fileExtensions == null)
				throw new ArgumentNullException(nameof(fileExtensions));

			var allowed = fileExtensions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (allowed.Length == 0)
				throw new ArgumentException($"must specify 1 or more {nameof(fileExtensions)} separated by commas and/or spaces", nameof(fileExtensions));

			using (var sw = new StringWriter()) {
				sw.Write("{0} must be an ");
				sw.WriteConjoined("or", "*{0}", allowed);
				sw.Write('.');
				ErrorMessageTemplate = sw.ToString();
			}
			AllowedExtensions = allowed; //KMS DO readonly copy?
		}

		public IEnumerable<string> AllowedExtensions { get; }

		public string ErrorMessageTemplate { get; }

		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			string fileExtension = Path.GetExtension(((HttpPostedFileBase)value).FileName);
			if (!AllowedExtensions.Any(ext => StringComparer.OrdinalIgnoreCase.Equals(ext, fileExtension)))
				return new ValidationResult(string.Format(ErrorMessageTemplate, validationContext.DisplayName));

			return ValidationResult.Success;
		}
	}
}