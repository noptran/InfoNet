using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "ClientCode,GenderIdentityId,EthnicityId,RaceHudIds,TwnTshipCountyById,ClientTypeId,RaceId")]
	public class Client : IValidatableObject, IRevisable, INotifyContextSavedChanges, IProvided {
		#region constants
		// ReSharper disable once InconsistentNaming
		private static LookupCode UNKNOWN_RACE_HUD {
			get { return Lookups.RaceHud[6]; }
		}
		#endregion

		public Client() {
			ClientCases = new List<ClientCase>();
			ClientRaces = new List<ClientRace>();
			TwnTshipCounty = new List<TwnTshipCounty>();
			TwnTshipCountyById = new DerivedDictionary<TwnTshipCounty>(() => TwnTshipCounty, true, e => e.LocID?.ToString()) { Template = () => new TwnTshipCounty() };
		}

		public int? ClientId { get; set; }

		[StringTrim]
		[Required]
        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Client ID")]
		[Help("This is the unique identifier assigned to your client to protect their identifying information. This identifier must not contain any personal information about your client, e.g. birth dates, initials, etc.")]
		public string ClientCode { get; set; }

		[Required]
		[Lookup("ClientType")]
		[Display(Name = "Client Type")]
		public int? ClientTypeId { get; set; }

		public bool IsAdult {
			get { return ClientTypeId == 1; }
		}

		public bool IsChild {
			get { return ClientTypeId == 2; }
		}

		public bool IsVictim {
			get { return ClientTypeId == 3; }
		}

		public bool IsSignificantOther {
			get { return ClientTypeId == 4; }
		}

		public bool IsCACSignificantOther {
			get { return ClientTypeId == 5; }
		}

		public bool IsChildNonVictim {
			get { return ClientTypeId == 6; }
		}

		public bool IsChildVictim {
			get { return ClientTypeId == 7; }
		}

		public bool IsNonOffendingCaretaker {
			get { return ClientTypeId == 8; }
		}

		[Required]
		[Lookup("GenderIdentity")]
		[Display(Name = "Gender Identity")]
		public int? GenderIdentityId { get; set; }

		[Lookup("Ethnicity")]
		[Display(Name = "Ethnicity")]
		public int? EthnicityId { get; set; }

		[Lookup("Race")]
		[Display(Name = "Race/Ethnicity")]
		public int? RaceId { get; set; }

		public int? CenterId { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public Provider Provider {
			get {
				if (ClientTypeId == null)
					return Provider.None;
				if (ClientTypeId == 1 || ClientTypeId == 2)
					return Provider.DV;
				if (ClientTypeId == 3 || ClientTypeId == 4)
					return Provider.SA;
				if (ClientTypeId >= 5 && ClientTypeId <= 8)
					return Provider.CAC;
				throw new NotSupportedException("Unrecognized ClientTypeId: " + ClientTypeId);
			}
		}

		public CaseType CaseType {
			get { return CaseTypes.For(ClientTypeId); }
		}

		public virtual Center Center { get; set; }

		public virtual ICollection<ClientCase> ClientCases { get; set; }

		public virtual ICollection<ClientRace> ClientRaces { get; set; }

		//KMS DO rename these
		public virtual IList<TwnTshipCounty> TwnTshipCounty { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<TwnTshipCounty> TwnTshipCountyById { get; }

		[NotMapped]
		[Display(Name = "Race/Ethnicity")]
		public IEnumerable<int> RaceHudIds {
			get { return ClientRaces.Select(r => r.RaceHudId); }
			set {
				foreach (var each in ClientRaces.Where(r => !value.Contains(r.RaceHudId)).ToArray())
					ClientRaces.Remove(each);
				var clientRaceHudIds = ClientRaces.Select(r => r.RaceHudId);
				foreach (int each in value.Where(i => !clientRaceHudIds.Contains(i)))
					ClientRaces.Add(new ClientRace(each));
			}
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			var results = new List<ValidationResult>();

			if (Provider == Provider.CAC && RaceId == null)
				results.Add(new ValidationResult("Race/Ethnicity is required.", new[] { nameof(RaceId) }));
			if (Provider != Provider.CAC && !RaceHudIds.Any())
				results.Add(new ValidationResult("At least one Race/Ethnicity must be selected.", new[] { nameof(RaceHudIds) }));
			if (RaceHudIds.Count() > 1 && RaceHudIds.Contains(UNKNOWN_RACE_HUD.CodeId))
				results.Add(new ValidationResult($"When {UNKNOWN_RACE_HUD.Description} is checked, no other Race/Ethnicity may be chosen.", new[] { nameof(RaceHudIds) }));

			return results;
		}

		public void OnContextSavedChanges(EntityState prior) {
			TwnTshipCountyById.RestorableKeys.Clear();
		}
	}
}