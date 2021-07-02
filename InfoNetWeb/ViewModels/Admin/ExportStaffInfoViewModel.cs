using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Admin {
	public class ExportStaffInfoViewModel : PagedListPagination {
		public ExportStaffInfoViewModel() {
			PageSize = 10;
		}

		public StaffStatus? Status { get; set; }

		[Display(Name = "Staff Type")]
		public StaffVolunteer.StaffType? TypeOfStaff { get; set; }

		[Lookup("PersonnelType")]
		[Display(Name = "Personnel Type")]
		public int? PersonnelTypeId { get; set; }

		public IPagedList<StaffVolunteer> SearchResults { get; set; }
	}

	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public enum StaffStatus {
		Active = 1,
		Inactive = 2
	}
}