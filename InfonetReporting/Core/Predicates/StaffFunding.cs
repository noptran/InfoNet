using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;

namespace Infonet.Reporting.Core.Predicates {
	public class StaffFunding {
		#region constants
		public static readonly IReadOnlyCollection<int> HotlineServiceIds = Array.AsReadOnly(ProviderEnum.All.SelectMany(p => Lookups.HotlineService[p].Select(lc => lc.CodeId)).ToArray());
		#endregion

		public int? SvId { get; set; }
		public int? FundingSourceId { get; set; }
		public short? PercentFund { get; set; }

		#region selectors
		public static readonly Expression<Func<ServiceDetailOfClient, IEnumerable<StaffFunding>>> ServiceDetail = sd =>
			sd.Tl_ProgramDetail.ProgramDetailStaff
				.Select(pds => new StaffService { SvId = pds.SVID, ServiceProgramId = sd.ServiceID, FundDateId = sd.FundDateID, IsGroupWithoutStaff = false })
				.DefaultIfEmpty(new StaffService { SvId = sd.SVID, ServiceProgramId = sd.ServiceID, FundDateId = sd.FundDateID, IsGroupWithoutStaff = sd.ICS_ID != null })
				.Where(ss => !ss.IsGroupWithoutStaff)
				.GroupJoin(
					sd.FundingDate.FundServiceProgramsOfStaff,
					outer => outer,
					inner => new StaffService { SvId = inner.SVID, ServiceProgramId = inner.ServiceProgramID, FundDateId = inner.FundDateID, IsGroupWithoutStaff = false },
					(outer, inner) => new { SVID = outer.SvId, outer.ServiceProgramId, FundDateID = outer.FundDateId, SourceAndPercent = inner.Select(fspos => new { fspos.FundingSourceID, fspos.PercentFund }).DefaultIfEmpty() })
				.SelectMany(group => group.SourceAndPercent.Select(sourceAndPercent => new StaffFunding { SvId = group.SVID, FundingSourceId = sourceAndPercent.FundingSourceID, PercentFund = sourceAndPercent.PercentFund }));

		public static readonly Expression<Func<ProgramDetailStaff, IEnumerable<StaffFunding>>> ProgramDetailStaff = pds => pds.ProgramDetail.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == pds.SVID && fs.ServiceProgramID == pds.ProgramDetail.ProgramID).Select(fs => new StaffFunding { SvId = fs.SVID, FundingSourceId = fs.FundingSourceID, PercentFund = fs.PercentFund });

		public static readonly Expression<Func<EventDetailStaff, IEnumerable<StaffFunding>>> EventDetailStaff = eds => eds.EventDetail.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == eds.SVID && fs.ServiceProgramID == eds.EventDetail.ProgramID).Select(fs => new StaffFunding { SvId = fs.SVID, FundingSourceId = fs.FundingSourceID, PercentFund = fs.PercentFund });

		public static readonly Expression<Func<PublicationDetailStaff, IEnumerable<StaffFunding>>> PublicationDetailStaff = eds => eds.PublicationDetail.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == eds.SVID && fs.ServiceProgramID == eds.PublicationDetail.ProgramID).Select(fs => new StaffFunding { SvId = fs.SVID, FundingSourceId = fs.FundingSourceID, PercentFund = fs.PercentFund });

		public static readonly Expression<Func<PhoneHotline, IEnumerable<StaffFunding>>> Hotline = h => h.FundingDate.FundServiceProgramsOfStaff.Where(fs => fs.SVID == h.SVID && HotlineServiceIds.Contains(fs.ServiceProgramID)).Select(fs => new StaffFunding { SvId = fs.SVID, FundingSourceId = fs.FundingSourceID, PercentFund = fs.PercentFund });
		#endregion

		#region inner
		private class StaffService {
			public int? SvId { get; set; }
			public int ServiceProgramId { get; set; }
			public int? FundDateId { get; set; }
			public bool IsGroupWithoutStaff { get; set; }
		}
		#endregion
	}
}