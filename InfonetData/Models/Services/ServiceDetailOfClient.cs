using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models._TLU;
using LinqKit;

namespace Infonet.Data.Models.Services {
	public class ServiceDetailOfClient : IRevisable, IValidatableObject {
		#region constants
		public static readonly IReadOnlyList<int> AllShelterIds = Array.AsReadOnly(new[] { 65, 66, 118 });
		#endregion

		public int? ServiceDetailID { get; set; }

		[Display(Name = "Client ID")]
		public int? ClientID { get; set; }

		[Display(Name = "Case ID")]
		public int? CaseID { get; set; }

		public int ServiceID { get; set; }

		public int? SVID { get; set; }

		public DateTime? ServiceDate { get; set; }

		public int? LocationID { get; set; }

		[Display(Name = "Received Hours")]
		[Range(0, 999)]
		[QuarterIncrement]
		public double? ReceivedHours { get; set; }

		public int? FundDateID { get; set; }
		public DateTime? ShelterBegDate { get; set; }

		public DateTime? ShelterEndDate { get; set; }

		public int? CityTownTownshpID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public int? AgencyRecID { get; set; }

		#region derived by trigger
		public DateTime? ServiceBegDate { get; set; }

		public DateTime? ServiceEndDate { get; set; }
		#endregion

		public int? ICS_ID { get; set; }

		public virtual Center Center { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual StaffVolunteer StaffVolunteer { get; set; }

		public virtual ProgramDetail Tl_ProgramDetail { get; set; }

		public virtual TwnTshipCounty TwnTshipCounty { get; set; }

		public virtual TLU_Codes_ProgramsAndServices TLU_Codes_ProgramsAndServices { get; set; }

		public virtual FundingDate FundingDate { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			var results = new List<ValidationResult>();
			if (ClientID != null) {
				if (ReceivedHours == null)
					results.Add(new ValidationResult("The Field Received Hours can not be blank.", new[] { "ReceivedHours" }));
				//if (ReceivedHours > )
			}
			return results;
		}

		public bool IsUnchanged(ServiceDetailOfClient other) {
			return ICS_ID == other.ICS_ID &&
					ServiceDate == other.ServiceDate &&
					ServiceID == other.ServiceID &&
					LocationID == other.LocationID &&
					ClientID == other.ClientID &&
					CaseID == other.CaseID &&
					CityTownTownshpID == other.CityTownTownshpID &&
					FundDateID == other.FundDateID &&
					ReceivedHours == other.ReceivedHours;
		}

		#region predicates
		public static Expression<Func<ServiceDetailOfClient, bool>> ServiceDateBetween(DateTime? minServiceDate, DateTime? maxServiceDate) {
			var predicate = PredicateBuilder.New<ServiceDetailOfClient>(true);
			predicate.And(sd => !AllShelterIds.Contains(sd.ServiceID));
			if (minServiceDate != null)
				predicate.And(sd => sd.ServiceDate >= minServiceDate);
			if (maxServiceDate != null)
				predicate.And(sd => sd.ServiceDate <= maxServiceDate);
			return predicate;
		}

		public static Expression<Func<ServiceDetailOfClient, bool>> ShelterDatesIntersect(DateTime? minShelterDate, DateTime? maxShelterDate) {
			var predicate = PredicateBuilder.New<ServiceDetailOfClient>(true);
			if (minShelterDate != null)
				predicate.And(sd => sd.ShelterEndDate == null || sd.ShelterEndDate >= minShelterDate);
			if (maxShelterDate != null)
				predicate.And(sd => sd.ShelterBegDate <= maxShelterDate);
			return predicate;
		}

		public static Expression<Func<ServiceDetailOfClient, bool>> IsShelter() {
			return sd => AllShelterIds.Contains(sd.ServiceID);
		}

		public static Expression<Func<ServiceDetailOfClient, bool>> IsNotShelter() {
			return sd => !AllShelterIds.Contains(sd.ServiceID);
		}
		#endregion
	}
}