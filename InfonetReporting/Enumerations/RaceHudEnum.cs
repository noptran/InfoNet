using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum RaceHudEnum {
        AmericanIndian = 1,
        Asian = 2,
        Black = 3,
        NativeHawaiian = 4,
        White = 5,
        HispanicLatino = 7,
        MENA = 8,
        SouthAsian = 9
    }

	public enum RaceHudCompositeEnum {
		[Display(Name = "American Indian or Alaska Native AND White")]
		AmericanIndianOrAlaskaNativeAndWhite = 95,
		[Display(Name = "Asian AND White")]
		AsianAndWhite = 96,
		[Display(Name = "Black or African American AND White")]
		BlackOrAfricanAmericanAndWhite = 97,
		[Display(Name = "American Indian or Alaska Native AND Black or African American")]
		AmericanIndianOrAlaskaNativeAndBlackOrAfricanAmerican = 98,
		[Display(Name = "Other Multiracial (Excludes Hispanic/Latino + White; MENA + White; and Asian + South Asian)")]
		OtherMultiracial = 99,
        [Display(Name = "MENA OR White")]
        MENAORWhite = 100,
        [Display(Name = "Asian OR South Asian")]
        AsianORSouthAsian = 101
    }
    
}