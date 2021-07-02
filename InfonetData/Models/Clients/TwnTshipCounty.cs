using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Usps.Data.Helpers;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "LocID,ClientID,CaseID,CityOrTown,Township,CountyID,Zipcode,MoveDate,CityID,StateID,townshipID,ZipcodeID,ZipcodeSuffix,ResidenceTypeID,LengthOfStayInResidenceID,IsDeleted")]
	public class TwnTshipCounty : IRevisable {
		public TwnTshipCounty() {
			ServiceDetailsOfClient = new List<ServiceDetailOfClient>();
			ClientReferralDetails = new List<ClientReferralDetail>();
			StateID = UspsHelper.IllinoisId;
		}

		public int? LocID { get; set; }

		public int? ClientID { get; set; }

		public int? CaseID { get; set; }

		[StringTrim]
		[Help("Enter the client’s residential City or Town. Make sure client’s city / town is spelled correctly to prevent data errors and inconsistencies.")]
		[Help(Provider.CAC, "Enter the client's residential city or town.")]
		[Display(Name = "City or Town")]
		public string CityOrTown { get; set; }

		[StringTrim]
		[Help("Enter the client’s residential Township. Make sure client’s township is spelled correctly to prevent data errors and inconsistencies.")]
		[Help(Provider.CAC, "Enter the client's residential township.")]
		public string Township { get; set; }

		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[StringTrim]
		[Display(Name = "Zip Code")]
		public string Zipcode { get; set; }

		[Required]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Effective Date")]
		public DateTime MoveDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		#region obsolete
		[Obsolete]
		public int? CityID { get; set; }
		#endregion

		//KMS DO not Required?
		[Display(Name = "State")]
		public int? StateID { get; set; }

		#region obsolete
		[Obsolete]
		public int? TownshipID { get; set; }
		#endregion

		#region obsolete
		[Obsolete]
		public int? ZipcodeID { get; set; }
		
		[Obsolete]
		public int? ZipcodeSuffix { get; set; }
		#endregion

		[Lookup("ResidenceType")]
		[Display(Name = "Residence Type")]
		public int? ResidenceTypeID { get; set; }

		[Lookup("LengthOfStay")]
		[Display(Name = "Length of Stay")]
		public int? LengthOfStayInResidenceID { get; set; }

		public virtual Client Client { get; set; }

		public virtual ICollection<ServiceDetailOfClient> ServiceDetailsOfClient { get; set; }

		public virtual ICollection<ClientReferralDetail> ClientReferralDetails { get; set; }
	}
}