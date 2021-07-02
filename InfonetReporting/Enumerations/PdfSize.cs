using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum PdfSize {
		[Display(Name = "A0 (Largest)")] A0 = 0,
		A1 = 1,
		A2 = 2,
		A3 = 3,
		A4 = 4,
		A5 = 5,
		A6 = 6,
		A7 = 7,
		A8 = 8,
		[Display(Name = "A9 (Smallest)")] A9 = 9,
		Legal = 27,
		[Display(Name = "Letter (Default)")] Letter = 28,
		[Display(Name = "Tabloid/Ledger")] Tabloid = 29
	}

	public static class PdfSizeEnum {
		public static IEnumerable<PdfSize> BySize {
			get {
				yield return PdfSize.A9;
				yield return PdfSize.A8;
				yield return PdfSize.A7;
				yield return PdfSize.A6;
				yield return PdfSize.A5;
				yield return PdfSize.Letter;
				yield return PdfSize.A4;
				yield return PdfSize.Legal;
				yield return PdfSize.Tabloid;
				yield return PdfSize.A3;
				yield return PdfSize.A2;
				yield return PdfSize.A1;
				yield return PdfSize.A0;
			}
		}
	}
}