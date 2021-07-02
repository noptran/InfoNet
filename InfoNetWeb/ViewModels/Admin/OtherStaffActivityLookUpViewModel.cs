using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models._TLU;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Admin {
	public class OtherStaffActivityLookUpViewModel : PagedListPagination {

		public OtherStaffActivityLookUpViewModel() {
			PageSize = 10;
		}

		[Display(Name = "Activity Name")]
        [MaxLength(60, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string ActivityName { get; set; }
		[Display(Name = "Is Active?")]
		public bool? IsActive { get; set; }
		[Range(0, 99, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		[Display(Name = "Display Order")]
		public int? DisplayOrder { get; set; }
		public int CenterID { get; set; }
		public IPagedList<OtherStaffActivityResultList> OtherStaffActivityList { get; set; }
		public List<OtherStaffActivityResultList> displayForPaging { get; set; }
		public class OtherStaffActivityResultList {
			public int? CodeID { get; set; }
			public int? ItemAssignmentID { get; set; }
			[Display(Name = "Activity Name")]
            [MaxLength(60, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
            public string ActivityName { get; set; }
			public int? CenterID { get; set; }
			public bool IsActive { get; set; }
			[Display(Name = "Display Order")]
			[Range(0, 99, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
			public int? DisplayOrder { get; set; }
			public bool shouldDelete { get; set; }
			public bool shouldEdit { get; set; }
			public bool hasReference { get; set; }
			public bool alreadyExists { get; set; }
            public bool isDuplicate { get; set; }
            public TLU_Codes_OtherStaffActivity Activity { get; set; }
		}
	}
}