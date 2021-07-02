using System;
using System.Collections.Generic;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models.Centers {
	[BindHint(Include = "FundDateID,CenterID,FundingDate")]
	public class FundingDate : IRevisable {
		public FundingDate() {
			FundServiceProgramsOfStaff = new List<FundServiceProgramOfStaff>();
			ServiceDetailsOfClients = new List<ServiceDetailOfClient>();
		}

		public int FundDateID { get; set; }

		public int CenterID { get; set; }

		public DateTime Date { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Center Center { get; set; }

		public virtual ICollection<FundServiceProgramOfStaff> FundServiceProgramsOfStaff { get; set; }

		public virtual ICollection<ServiceDetailOfClient> ServiceDetailsOfClients { get; set; }

		public virtual ICollection<PhoneHotline> PhoneHotlines { get; set; }

		public virtual ICollection<EventDetail> EventsDetail { get; set; }

		public virtual ICollection<ProgramDetail> ProgramsDetail { get; set; }

		public virtual ICollection<PublicationDetail> PublicationsDetail { get; set; }
	}
}