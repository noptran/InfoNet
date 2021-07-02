using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ShelterServiceEnum {
		[Display(Name = "Off-Site Shelter")] OffsiteShelter = 65,

		[Display(Name = "On-Site Shelter")] OnsiteShelter = 66,

		[Display(Name = "Transitional Housing")] TransitionalHousing = 118,

		[Display(Name = "Walk-In")] Walkin = -1
	}
}