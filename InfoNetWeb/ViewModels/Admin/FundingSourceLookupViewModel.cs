using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models._TLU;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Admin
{
    public class FundingSourceLookupViewModel : PagedListPagination
    {
        public FundingSourceLookupViewModel()
        {
            PageSize = 10;
        }
        [Display(Name = "Funding Source")]
        [MaxLength(60, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string FundingSource { get; set; }
        [Display(Name = "Is Active?")]
        public bool? IsActive { get; set; }
        [Display(Name = "Display Order")]
        [Range(0, 99, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? DisplayOrder { get; set; }
        public int CenterID { get; set; }
        public IPagedList<FundingSourceResult> FundingSourceList { get; set; }
        public List<FundingSourceResult> displayForPaging { get; set; }

        public class FundingSourceResult
        {
            [Display(Name = "Funding Source")]
            [MaxLength(60, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
            public string FundingSourceName { get; set; }
            public int? CenterID { get; set; }
            public int? FundingSourceID { get; set; }
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
            public bool? ICADVAdmin { get; set; }
            public bool? ICASAAdmin { get; set; }
            public DateTime? BeginDate { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
            [Display(Name = "End Date")]
            public DateTime? EndDate { get; set; }
            public TLU_Codes_FundingSource thisFundingSource { get; set; }
        }
    }
}