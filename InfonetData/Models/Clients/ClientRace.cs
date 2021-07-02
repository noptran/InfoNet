using System;
using Infonet.Core.Entity;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	public class ClientRace : IRevisable {
		public ClientRace() { }

		public ClientRace(int raceHudId) {
			RaceHudId = raceHudId;
		}

		public int ClientId { get; set; }

		[Lookup("RaceHud")]
		public int RaceHudId { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Client Client { get; set; }
	}
}