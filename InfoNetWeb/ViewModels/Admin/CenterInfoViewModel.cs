using System.Collections.Generic;
using Infonet.Data.Models.Centers;
using Infonet.Usps.Data.Models;

namespace Infonet.Web.ViewModels.Admin {
	public class CenterInfoViewModel {
		public CenterInfoViewModel() {
			States = new List<States>();
			Counties = new List<Counties>();
			Cities = new List<Cities>();
		}

		public Center Center { get; set; }
		public IList<States> States { get; set; }
		public IList<Counties> Counties { get; set; }
		public IList<Cities> Cities { get; set; }
	}
}