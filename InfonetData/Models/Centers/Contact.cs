using System;
using System.Collections.Generic;
using Infonet.Core.Entity;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Models.Centers {
	public class Contact : IRevisable {
		public Contact() {
			ClientMDTs = new List<ClientMDT>();
			VictimSensitiveInterview = new List<VictimSensitiveInterview>();
			VSIObservers = new List<VSIObserver>();
		}

		public int? ContactId { get; set; }
		public string ContactName { get; set; }
		public int? CenterId { get; set; }
		public bool Active { get; set; }
		public DateTime? RevisionStamp { get; set; }
		public virtual Center Center { get; set; }
		public virtual ICollection<ClientMDT> ClientMDTs { get; set; }
		public virtual ICollection<VictimSensitiveInterview> VictimSensitiveInterview { get; set; }
		public virtual ICollection<VSIObserver> VSIObservers { get; set; }
	}
}