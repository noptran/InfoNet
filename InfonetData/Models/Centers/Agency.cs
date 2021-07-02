using System;
using System.Collections.Generic;
using Infonet.Core.Entity;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models.Centers {
	public class Agency : IRevisable {
		public Agency() {
			ClientMDTs = new List<ClientMDT>();
			ProgramDetails = new List<ProgramDetail>();
			ClientCJProcesses = new List<ClientCJProcess>();
			ClientReferralDetails = new List<ClientReferralDetail>();
			ClientReferralSources = new List<ClientReferralSource>();
			VSIObservers = new List<VSIObserver>();
		}

		public int? AgencyID { get; set; }
		public string AgencyName { get; set; }
		public int? CenterID { get; set; }
		public DateTime? RevisionStamp { get; set; }
		public virtual ICollection<ClientMDT> ClientMDTs { get; set; }
		public virtual ICollection<ProgramDetail> ProgramDetails { get; set; }
		public virtual ICollection<ClientCJProcess> ClientCJProcesses { get; set; }
		public virtual ICollection<ClientReferralDetail> ClientReferralDetails { get; set; }
		public virtual ICollection<ClientReferralSource> ClientReferralSources { get; set; }
		public virtual ICollection<VSIObserver> VSIObservers { get; set; }
	}
}