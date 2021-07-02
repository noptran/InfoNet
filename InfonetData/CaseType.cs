using System;
using Infonet.Data.Looking;

namespace Infonet.Data {
	[Flags]
	public enum CaseType {
		None = 0,

		/* Client Types */
		Adult = 1,
		Child = 2,
		Victim = 4,
		SignificantOtherSA = 8,
		SignificantOtherCAC = 16,
		ChildNonVictim = 32,
		ChildVictim = 64,
		NonOffendingCaretaker = 128,

		/* Provider Types */
		DV = Adult | Child,
		SA = Victim | SignificantOtherSA,
		CAC = SignificantOtherCAC | ChildNonVictim | ChildVictim | NonOffendingCaretaker,

		Any = DV | SA | CAC
	}

	public static class CaseTypes {
		public static CaseType For(int? clientTypeId) {
			switch (clientTypeId) {
				case null:
					return CaseType.None;
				case 1:
					return CaseType.Adult;
				case 2:
					return CaseType.Child;
				case 3:
					return CaseType.Victim;
				case 4:
					return CaseType.SignificantOtherSA;
				case 5:
					return CaseType.SignificantOtherCAC;
				case 6:
					return CaseType.ChildNonVictim;
				case 7:
					return CaseType.ChildVictim;
				case 8:
					return CaseType.NonOffendingCaretaker;
				default:
					throw new ArgumentOutOfRangeException(nameof(clientTypeId), $"Unrecognized {nameof(clientTypeId)}: {clientTypeId}");
			}
		}

		// ReSharper disable once UnusedMember.Global
		public static CaseType ToProvider(this CaseType caseType) {
			var result = CaseType.None;
			if ((caseType & CaseType.DV) != CaseType.None)
				result |= CaseType.DV;
			if ((caseType & CaseType.SA) != CaseType.None)
				result |= CaseType.SA;
			if ((caseType & CaseType.CAC) != CaseType.None)
				result |= CaseType.CAC;
			return result;
		}

		public static CaseType For(Provider provider) {
			switch (provider) {
				case Provider.None:
					return CaseType.None;
				case Provider.DV:
					return CaseType.DV;
				case Provider.SA:
					return CaseType.SA;
				case Provider.CAC:
					return CaseType.CAC;
				default:
					throw new ArgumentOutOfRangeException(nameof(provider), $"Unrecognized {nameof(provider)}: {provider}");
			}
		}
	}
}