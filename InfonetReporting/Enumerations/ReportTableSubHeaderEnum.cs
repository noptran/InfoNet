using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ReportTableSubHeaderEnum {
		[Display(Name = "Adult")]
		Adult = 1,
		[Display(Name = "Child")]
		Child = 2,
		[Display(Name = "Victim")]
		Victim = 3,
		[Display(Name = "Signifigant Other")]
		SignifigantOther = 4,
		[Display(Name = "CAC Signifigant Other")]
		CACSignifigantOther = 5,
		[Display(Name = "Child Non-Victim")]
		ChildNonVictim = 6,
		[Display(Name = "Child Victim")]
		ChildVictim = 7,
		[Display(Name = "Non-Offending Caretaker")]
		NonOffendingCaretaker = 8,
		[Display(Name = "Total")]
		Total = 999
	}
}
