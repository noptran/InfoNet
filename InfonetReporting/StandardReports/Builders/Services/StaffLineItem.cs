using System.Collections.Generic;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class StaffLineItem {
		public int SvId { get; set; }
		public double ConductHours { get; set; }
		public double PrepHours { get; set; }
		public double TravelHours { get; set; }
		public IEnumerable<StaffFunding> Funding { get; set; }
	}
}