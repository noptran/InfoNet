using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Infonet.Core.Data;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Entity;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Centers {
	[BindHint(Include = "ProviderIdsString,ProviderIds,ProviderIdArray,CenterIdsString,LocationIdsString,ModeId,Title,Message,Details,LinkUrl,LinkText,IsDownload,IsHot,IsJumbo,PostedDate,ExpirationDate")]
	public class SystemMessage : IRevisable {
		private readonly DelimitedField<int> _providerIds = new DelimitedField<int>("|", "|", "|");
		private readonly DelimitedField<int> _centerIds = new DelimitedField<int>("|", "|", "|");
		private readonly DelimitedField<int> _locationIds = new DelimitedField<int>("|", "|", "|");

		public static DateTime OldestNewPostedDate {
			get { return DateTime.Today.AddMonths(-3); }
		}

		public SystemMessage() {
			ModeId = 1;
		}

		public int? Id { get; set; }

		[MaxLength(25, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string ProviderIdsString {
			get { return _providerIds.Value; }
			set { _providerIds.Value = value; }
		}

		[NotMapped]
		[Display(Name = "Providers")]
		[Help("Select the Providers who should see this message.  NOTE: If ALL Providers below are checked, then SYSADMIN users will see this message also.")]
		public IEnumerable<int> ProviderIds {
			get { return _providerIds.Items; }
			set { _providerIds.Items = value; }
		}

		[MaxLength(4000, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string CenterIdsString {
			get { return _centerIds.Value; }
			set { _centerIds.Value = value; }
		}

		[NotMapped]
		public IEnumerable<int> CenterIds {
			get { return _centerIds.Items; }
			set { _centerIds.Items = value; }
		}

		[MaxLength(4000, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string LocationIdsString {
			get { return _locationIds.Value; }
			set { _locationIds.Value = value; }
		}

		[NotMapped]
		public IEnumerable<int> LocationIds {
			get { return _locationIds.Items; }
			set { _locationIds.Items = value; }
		}

		[Required]
		[Lookup("SystemMessageMode")]
		[Display(Name = "Display Mode")]
		[Help("Cards are the standard messages shown on the welcome page.  Carousel messages are larger and rotate in the first spot on the welcome page.  Marquee messages scroll across the top of every page.")]
		public int ModeId { get; set; }

		[MaxLength(250, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string Title { get; set; }

		[MaxLength(500, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string Message { get; set; }

		[MaxLength(500, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[Help("Optional extra details shown in smaller font at the bottom of Carousel messages.")]
		public string Details { get; set; }

		[MaxLength(250, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[Display(Name = "Link URL")]
		[Help("Optional URL provided by Card messages for downloads or internal/external links.")]
		public string LinkUrl { get; set; }

		[MaxLength(25, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[Display(Name = "Link Text")]
		[Help("Optional text used to replace default Link URL text.")]
		public string LinkText { get; set; }

		[Display(Name = "Link is Download?")]
		[Help("Should the link (if specified) be displayed as a download or internal/external link?")]
		public bool IsDownload { get; set; }

		[Display(Name = "Hot")]
		[Help("Should this message given extra priority?  Hot messages are styled to get users' attention.  They are also displayed before all non-hot messages.")]
		public bool IsHot { get; set; }

		[Display(Name = "Posted Date and Time")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
		[Help("Date and time this message should appear to users.  If blank, the message will appear immediately.  Messages with no Posted Date and Time are sorted after after New messages and before Old messages.  Messages are considered New for 3 months after Posted.")]
		public DateTime? PostedDate { get; set; }

		[Display(Name = "Expiration Date and TIme")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
		[Help("Date and time this message should no longer appear to users.  If blank, the message will never expire.")]
		public DateTime? ExpirationDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		[NotMapped]
		public bool IsNew {
			get { return PostedDate >= OldestNewPostedDate; }
		}

		[NotMapped]
		public bool IsCurrent {
			get {
				var now = DateTime.Now;
				return (PostedDate == null || PostedDate <= now) && (ExpirationDate == null || ExpirationDate >= now);
			}
		}

		[NotMapped]
		public bool IsExpired {
			get { return ExpirationDate < DateTime.Now; }
		}

		#region queryable
		public static IQueryable<SystemMessage> WhereAvailable(IQueryable<SystemMessage> q, int providerId, int centerId, int locationId) {
			string provider = $"|{providerId}|";
			string center = $"|{centerId}|";
			string location = $"|{locationId}|";
			var now = DateTime.Now;
			return q.Where(m => (m.ProviderIdsString == null || m.ProviderIdsString.Contains(provider)) &&
								(m.CenterIdsString == null || m.CenterIdsString.Contains(center)) &&
								(m.LocationIdsString == null || m.LocationIdsString.Contains(location)) &&
								(m.PostedDate == null || m.PostedDate <= now) &&
								(m.ExpirationDate == null || m.ExpirationDate >= now));
		}

		public static IOrderedQueryable<SystemMessage> OrderForDisplay(IQueryable<SystemMessage> q) {
			var justOlderThanNew = OldestNewPostedDate.AddTicks(-1);
			return q
				.OrderByDescending(m => m.IsHot)
				.ThenByDescending(m => m.PostedDate ?? justOlderThanNew)
				.ThenBy(m => m.ExpirationDate == null)
				.ThenBy(m => m.ExpirationDate)
				.ThenBy(m => m.Id);
		}
		#endregion

		#region inner
		public enum Mode {
			Card = 1,
			Carousel = 2,
			Marquee = 3
		}
		#endregion
	}
}