using System;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.Entity.Binding {
	/// <summary>
	///     Denotes a data field that should be trimmed during binding, removing any spaces.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Support for trimming is implmented in the model binder, as currently
	///         Data Annotations provides no mechanism to coerce the value.
	///     </para>
	///     <para>
	///         This attribute does not imply that empty strings should be converted to null.
	///         When that is required you must additionally use the
	///         <see cref="System.ComponentModel.DataAnnotations.DisplayFormatAttribute.ConvertEmptyStringToNull" />
	///         option to control what happens to empty strings.
	///     </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class StringTrimAttribute : Attribute { }
}