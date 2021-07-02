using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models.Centers;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Admin {
	public class AgenciesLookupViewModel : PagedListPagination {

		public AgenciesLookupViewModel() {
			PageSize = 10;
		}

		[Display(Name = "Agency Name")]
        [MaxLength(60, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string AgencyName { get; set; }
		[Display(Name = "Is Active?")]
		public bool? IsActive { get; set; }
		[Range(0, 99, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		[Display(Name = "Display Order")]
		public int? DisplayOrder { get; set; }
		public int CenterID { get; set; }
		public IPagedList<AgencyResultList> AgencyList { get; set; }
		public List<AgencyResultList> displayForPaging { get; set; }
		public class AgencyResultList {
			[Display(Name = "Agency Name")]
            [MaxLength(60, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
            public string AgencyName { get; set; }
			public int? CenterID { get; set; }
			public DateTime? RevisionStamp { get; set; }
			public int? AgencyID { get; set; }
			public int? ItemAssignmentID { get; set; }
			public bool IsActive { get; set; }
			[Display(Name = "Display Order")]
			[Range(0, 99, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
			public int? DisplayOrder { get; set; }
			public bool shouldDelete { get; set; }
			public bool shouldEdit { get; set; }
			public bool hasReference { get; set; }
			public bool alreadyExists { get; set; }
            public bool isDuplicate { get; set; }
			public Agency thisAgency { get; set; }
		}
	}
}