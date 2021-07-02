using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "PrimaryIncomeId,AFDC,Unknown,GeneralAssistance,SocialSecurity,SSI,AlimonyChildSupport,Employment,OtherIncome,WhatOther")]
	public class ClientIncome : IRevisable {
		public int ClientId { get; set; }

		public int CaseId { get; set; }

		[Display(Name = "Primary Income")]
		[Lookup("IncomeSource")]
		public int? PrimaryIncomeId { get; set; }

		[Display(Name = "TANF/AFDC")]
		public bool AFDC { get; set; }

		public bool Unknown { get; set; }

		[Display(Name = "General Assistance")]
		public bool GeneralAssistance { get; set; }

		[Display(Name = "Social Security")]
		public bool SocialSecurity { get; set; }

		public bool SSI { get; set; }

		[Display(Name = "Alimony/Child Support")]
		public bool AlimonyChildSupport { get; set; }

		public bool Employment { get; set; }

		[Display(Name = "Other Income")]
		public bool OtherIncome { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Other Income")]
		public string WhatOther { get; set; }

		#region obsolete
		[Obsolete]
		public string OtherPrimaryIncome { get; set; }
		
		[Obsolete]
		public decimal? AmountOfPrimaryIncome { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }
	}
}