using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.ViewModels {
	public interface IReportCenterSelectionBase {
		[Required(ErrorMessage = "You must specify at least one Center.")]
		int?[] CenterIds { get; set; }
	}
}