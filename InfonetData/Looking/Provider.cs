using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Data.Looking {
	/** Enum of named ProviderIds matching subset of those found in TLU_Codes_ProviderID. **/
	public enum Provider {
		None = 0,
		[Display(Name = "Domestic Violence")] DV = 1,
		[Display(Name = "Sexual Assault")] SA = 2,
		[Display(Name = "Child Advocacy")] CAC = 6
	}

	public static class ProviderEnum {
		/** All 'real' Providers (i.e. None is not included) **/
		public static readonly IReadOnlyList<Provider> All = Array.AsReadOnly(new[] { Provider.DV, Provider.SA, Provider.CAC });

		public static Provider For(int? providerId) {
			if (providerId == null)
				throw new ArgumentNullException(nameof(providerId));
			return For(providerId.Value);
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static Provider For(int providerId) {
			var result = (Provider)providerId;
			if (!Enum.IsDefined(typeof(Provider), result))
				throw new ArgumentOutOfRangeException(nameof(providerId));
			return result;
		}

		public static int Id(this Provider self) {
			return (int)self;
		}
	}
}