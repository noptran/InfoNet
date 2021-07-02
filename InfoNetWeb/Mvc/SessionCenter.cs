using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Web.Mvc {
	public class SessionCenter {
		public SessionCenter(Center center, SessionCenter parent) {
			Id = center.CenterID;
			Name = center.CenterName;
			Parent = parent;
			Provider = (Provider)center.ProviderID;
			HasShelter = center.ShelterStatus ?? false;

			if (IsSatellite)
				Satellites = Array.Empty<SessionCenter>();
			else
				Satellites = Array.AsReadOnly(center.Satellites.Where(s => s.CenterID != Id).OrderBy(s => s.CenterName).Select(s => new SessionCenter(s, this)).ToArray());
		}

		public int Id { get; }

		public string Name { get; }

		public SessionCenter Parent { get; }

		public IEnumerable<SessionCenter> Satellites { get; }

		public Provider Provider { get; }

		public bool IsDV {
			get { return Provider == Provider.DV; }
		}

		public bool IsSA {
			get { return Provider == Provider.SA; }
		}

		public bool IsCAC {
			get { return Provider == Provider.CAC; }
		}

		public int ProviderId {
			get { return Provider.Id(); }
		}

		public bool HasShelter { get; }

		public bool IsSatellite {
			get { return Parent != null; }
		}

		public SessionCenter Top {
			get { return IsSatellite ? Parent.Top : this; }
		}

		/* Enumerates parent and then its satellites. */
		public IEnumerable<SessionCenter> AllRelated {
			get {
				var top = Top;
				yield return top;
				foreach (var each in top.Satellites)
					yield return each;
			}
		}

		public SessionCenter FindRelated(int centerId) {
			return AllRelated.Single(c => c.Id == centerId);
		}
	}
}