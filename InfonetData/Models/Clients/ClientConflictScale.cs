using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "Threw, Pushed, Slapped, Kicked, Hit, BeatUp, Choked, Threatened, Used")]
	public class ClientConflictScale : IRevisable {
		public int ClientID { get; set; }

		public int CaseID { get; set; }

		[Display(Name = "Threw something at your victim")]
		public bool Threw { get; set; }

		[Display(Name = "Pushed, grabbed or shoved your victim")]
		public bool Pushed { get; set; }

		[Display(Name = "Slapped your victim")]
		public bool Slapped { get; set; }

		[Display(Name = "Kicked, bit or hit your victim with a fist")]
		public bool Kicked { get; set; }

		[Display(Name = "Hit or tried to hit your victim with something")]
		public bool Hit { get; set; }

		[Display(Name = "Beat up your victim")]
		public bool BeatUp { get; set; }

		[Display(Name = "Strangled your victim")]
		public bool Choked { get; set; }

		[Display(Name = "Threatened your victim with a knife or gun")]
		public bool Threatened { get; set; }

		[Display(Name = "Used a knife or fired a gun")]
		public bool Used { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }
	}
}