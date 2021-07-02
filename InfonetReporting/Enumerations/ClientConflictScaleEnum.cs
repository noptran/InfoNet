using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ClientConflictScaleEnum {
		[Display(Name = "Beat up your victim")]
		BeatUp = 1,
		[Display(Name = "Strangled your victim")]
		Choked = 2,
		[Display(Name = "Hit or tried to hit your victim with something")]
		Hit = 3,
		[Display(Name = "Kicked, bit or hit your victim with a fist")]
		Kicked = 4,
		[Display(Name = "Pushed, grabbed or shoved your victim")]
		Pushed = 5,
		[Display(Name = "Slapped your victim")]
		Slapped = 6,
		[Display(Name = "Threatened your victim with a knife or gun")]
		Threatened = 7,
		[Display(Name = "Threw something at your victim")]
		Threw = 8,
		[Display(Name = "Used a knife or fired a gun")]
		Used = 9
	}
}