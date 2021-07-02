using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.AdHoc {
	public enum Perspective {
		[Display(Name="Centers")]
		Center,
		[Display(Name="Clients")]
		Client,
		Staff
	}
}