using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Admin {
	public class PersonnelSearchViewModel : PagedListPagination {
		public PersonnelSearchViewModel() {
			StatusList = new List<SimpleListItem> { new SimpleListItem("1", "Active"), new SimpleListItem("2", "Inactive") };
			PageSize = 10;
		}

		[Display(Name = "Staff Type")]
		public int? TypeID { get; set; }

		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Lookup("PersonnelType")]
		[Display(Name = "Personnel Type")]
		public int? PersonnelTypeID { get; set; }

		[Lookup("YesNo2")]
		[Display(Name = "Is Student?")]
		public int? CollegeUnivStudent { get; set; }

		[Lookup("Race")]
		[Display(Name = "Race/Ethnicity")]
		public int? RaceId { get; set; }

		[Lookup("Sex")]
		[Display(Name = "Gender Identity")]
		public int? SexId { get; set; }

		[Display(Name = "Status")]
		public int? Status { get; set; }

		public List<SimpleListItem> StatusList { get; set; }

		public IPagedList<MyStaffVolunteer> staffList { get; set; }

		public class MyStaffVolunteer {
			public int SvId { get; set; }
			public int CenterId { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public int? SexId { get; set; }
			public int? RaceId { get; set; }
			public int? PersonnelTypeId { get; set; }
			public bool CollegeUnivStudent { get; set; }
			public DateTime? StartDate { get; set; }
			public DateTime? TerminationDate { get; set; }
			public int TypeId { get; set; }

			public string Type {
				get { return ((StaffVolunteer.StaffType)TypeId).ToString(); }
			}
		}

		public class SimpleListItem {
			public SimpleListItem(string id, string name) {
				ID = id;
				Name = name;
			}

			public string ID { get; }
			public string Name { get; }
		}
	}
}