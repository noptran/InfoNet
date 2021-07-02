using System;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.Entity.Binding {
	[AttributeUsage(AttributeTargets.Class)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public sealed class BindHintAttribute : Attribute {
		/// <summary>Gets or sets a comma-delimited list of property names for which binding is not allowed.</summary>
		/// <returns>The exclude list.</returns>
		public string Exclude { get; set; }

		/// <summary>Gets or sets a comma-delimited list of property names for which binding is allowed.</summary>
		/// <returns>The include list.</returns>
		public string Include { get; set; }

		/// <summary>
		///     Gets or sets the prefix to use when markup is rendered for binding to an action argument or to a model
		///     property.
		/// </summary>
		/// <returns>The prefix to use.</returns>
		public string Prefix { get; set; }
	}
}