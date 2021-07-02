using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Models.Centers;

//KMS DO add SubmitterComment(or Description/Message/etc)?
namespace Infonet.Data.Models.Reporting {
	[BindHint(Include = "ScheduledForDate,ExpirationDate,Approvals")]
	public class ReportJob : IRevisable {
		#region constants
		public static readonly IReadOnlyList<Status> ActiveStatuses = Array.AsReadOnly(new[] { Status.Fetching, Status.Running, Status.Deleting });
		public static readonly IReadOnlyList<Status> WaitingStatuses = Array.AsReadOnly(new[] { Status.Ready, Status.Error });
		#endregion

		public ReportJob() {
			Priority = 0;
			RemainingTries = 3;
		}

		public int? Id { get; set; }

		[Required]
		public string SpecificationJson { get; set; }

		[Required]
		public int? StatusId { get; set; }

		[Required]
		public DateTime? StatusDate { get; set; }

		public string StatusLog { get; set; }

		[Required]
		public DateTime? ScheduledForDate { get; set; }

		public DateTime? ExpirationDate { get; set; }

		[Required]
		public int? Priority { get; set; }

		[Required]
		public int? RemainingTries { get; set; }

		public string ActiveThread { get; set; }

		[Required]
		public DateTime? SubmittedDate { get; set; }

		[Required]
		public int? SubmitterCenterId { get; set; }

		[Required]
		public string SubmitterUserName { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public byte[] RowVersion { get; set; }

		public virtual ReportJobApproval Approval { get; set; }

		public virtual Center SubmitterCenter { get; set; }

		public string DisplayName {
			get { return $"{nameof(ReportJob)}[{Id}]"; }
		}

		public string EnterStatus(Status status) {
			StatusId = status.ToInt32();
			StatusDate = DateTime.Now;
			ActiveThread = ActiveStatuses.Contains(status) ? Thread.CurrentThread.Name : null;
			Log($"Status changed to {status}");
			return $"{DisplayName} status changed to {status}";
		}

		public void Log(string message) {
			StatusLog += $"{DateTime.Now}\t{message}{Environment.NewLine}";
		}

		public Ticket ToTicket() {
			return new Ticket { Id = Id, RowVersion = RowVersion };
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public class Ticket {
			public int? Id { get; set; }
			public byte[] RowVersion { get; set; }

			public override string ToString() {
				return $"{nameof(ReportJob)}.{GetType().Name}[{Id}]";
			}

			public override bool Equals(object other) {
				return Equals(other as Ticket);
			}

			// ReSharper disable once MemberCanBePrivate.Global
			public bool Equals(Ticket other) {
				return other != null && other.Id == Id && ((IStructuralEquatable)other.RowVersion).Equals(RowVersion, EqualityComparer<byte>.Default);
			}

			public override int GetHashCode() {
				return HashCode.Compute(Id, RowVersion);
			}

			public static bool operator ==(Ticket a, Ticket b) {
				return ReferenceEquals(a, b) || !ReferenceEquals(a, null) && a.Equals(b);
			}

			public static bool operator !=(Ticket a, Ticket b) {
				return !(a == b);
			}
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public enum Status {
			Hold = 1,
			Ready = 2,
			Fetching = 3,
			Running = 4,
			Error = 5,
			Succeeded = 6,
			Failed = 7,
			Deleting = 8,
			DeleteFailed = 9
		}
	}
}