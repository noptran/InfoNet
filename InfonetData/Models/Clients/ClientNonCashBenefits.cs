using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "FoodBenefit,SpecSuppNutr,TANFChildCare,TANFTrans,TANFOther,PublicHousing,OtherSource,Medicaid,Medicare,StateChildHealth,VetAdminMed,PrivateIns,NoHealthIns,UnknownHealthIns,NoBenefit,UnknownBenefit")]
	public class ClientNonCashBenefits : IRevisable {
		public ClientNonCashBenefits() {
			FoodBenefit = false;
			SpecSuppNutr = false;
			TANFChildCare = false;
			TANFTrans = false;
			TANFOther = false;
			PublicHousing = false;
			OtherSource = false;
			Medicaid = false;
			Medicare = false;
			StateChildHealth = false;
			VetAdminMed = false;
			PrivateIns = false;
			NoHealthIns = false;
			UnknownHealthIns = false;
			NoBenefit = false;
			UnknownBenefit = false;
		}

		public int ClientID { get; set; }
		public int CaseID { get; set; }

		[Display(Name = "Food stamps/food benefit card (Link Card)")]
		public bool FoodBenefit { get; set; }

		[Display(Name = "Special supplemental nutrition (WIC)")]
		public bool SpecSuppNutr { get; set; }

		[Display(Name = "TANF child care services")]
		public bool TANFChildCare { get; set; }

		[Display(Name = "TANF transportation")]
		public bool TANFTrans { get; set; }

		[Display(Name = "Other TANF funded services")]
		public bool TANFOther { get; set; }

		[Display(Name = "Section 8, public housing, rent assistance")]
		public bool PublicHousing { get; set; }

		[Display(Name = "Other non-cash benefit")]
		public bool OtherSource { get; set; }

		[Display(Name = "Medicaid health insurance (Client 18 or older only)")]
		public bool Medicaid { get; set; }

		[Display(Name = "Medicare health insurance")]
		public bool Medicare { get; set; }

		[Display(Name = "State children's health insurance (Illinois Medicaid)")]
		public bool StateChildHealth { get; set; }

		[Display(Name = "Veteran's Administration Med Services")]
		public bool VetAdminMed { get; set; }

		[Display(Name = "Private Health Insurance")]
		public bool PrivateIns { get; set; }

		[Display(Name = "No Health Insurance")]
		public bool NoHealthIns { get; set; }

		[Display(Name = "Unknown")]
		public bool UnknownHealthIns { get; set; }

		[Display(Name = "Client receives no non-cash benefits")]
		public bool NoBenefit { get; set; }

		[Display(Name = "Unknown")]
		public bool UnknownBenefit { get; set; }

		public DateTime? RevisionStamp { get; set; }
		public virtual ClientCase ClientCases { get; set; }
	}
}