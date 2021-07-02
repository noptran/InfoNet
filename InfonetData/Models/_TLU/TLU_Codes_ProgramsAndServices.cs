using System.Collections.Generic;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models._TLU {
	public class TLU_Codes_ProgramsAndServices {
		public TLU_Codes_ProgramsAndServices() {
			Cancellations = new List<Cancellation>();
			EventDetails = new List<EventDetail>();
			FundServiceProgramsOfStaff = new List<FundServiceProgramOfStaff>();
			ProgramDetails = new List<ProgramDetail>();
			PublicationDetails = new List<PublicationDetail>();
			ServiceDetailsOfClient = new List<ServiceDetailOfClient>();
		}

		public int CodeID { get; set; }

		public string Code { get; set; }

		public string Description { get; set; }

		[Lookup("FedClass")]
		public int? FedClass { get; set; }

		[Lookup("ServiceClass")]
		public int? StandardClass { get; set; }

		public bool? ShowCancellation { get; set; }

		public bool? IsCommInst { get; set; }

		public bool? IsService { get; set; }

		public bool? IsGroupService { get; set; }

		public bool? IsHotline { get; set; }

		public bool? IsPublication { get; set; }

		public bool? IsEvent { get; set; }

		public bool IsShelter { get; set; }

		public virtual ICollection<Cancellation> Cancellations { get; set; }

		public virtual ICollection<EventDetail> EventDetails { get; set; }

		public virtual ICollection<FundServiceProgramOfStaff> FundServiceProgramsOfStaff { get; set; }

		public virtual ICollection<ProgramDetail> ProgramDetails { get; set; }

		public virtual ICollection<PublicationDetail> PublicationDetails { get; set; }

		public virtual ICollection<ServiceDetailOfClient> ServiceDetailsOfClient { get; set; }

		public virtual ICollection<HudServiceMapping> HudServices { get; set; }
	}
}