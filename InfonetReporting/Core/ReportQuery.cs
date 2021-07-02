using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.ViewModels;
using LinqKit;

namespace Infonet.Reporting.Core {
	public class ReportQuery<TEntity> : IReportQuery {
		private readonly Func<ReportContainer, IQueryable<TEntity>> _querySelector;
		private readonly Func<FilterContext, Vertex<TEntity>> _vertexSelector;

		public ReportQuery(Func<ReportContainer, IQueryable<TEntity>> querySelector, Func<FilterContext, Vertex<TEntity>> vertexSelector) {
			_querySelector = querySelector;
			_vertexSelector = vertexSelector;
			Filters = new List<ReportFilter>();
			Orders = new List<IReportOrder<TEntity>>();
			SubReports = new List<ISubReportBuilder<TEntity>>();
		}

		public ReportContainer ReportContainer { get; set; }

		public IList<ReportFilter> Filters { get; }

		public List<IReportOrder<TEntity>> Orders { get; }

		public List<ISubReportBuilder<TEntity>> SubReports { get; }

		public IEnumerable<ReportFilter> VisibleFilters {
			get {
				var visible = new HashSet<ReportFilter>();
				foreach (var each in Filters)
					each.AddVisibleTo(visible);
				return visible.OrderBy(f => f.DisplayOrder).ThenBy(f => f.Label);
			}
		}

		public IEnumerable<ReportOrderDisplay> OrderDisplay {
			get { return Orders.Where(o => !o.HideOrder).OrderBy(o => o.DisplayOrder).Select(o => new ReportOrderDisplay { ReportOrderAsString = o.ReportOrderAsString }); }
		}

		private IOrderedQueryable<TEntity> FilteredOrderedQuery { get; set; }

		//KMS DO eliminate this
		private void StageData() {
			var filteredDataQuery = _querySelector(ReportContainer).AsExpandable();

			var filterContext = new FilterContext();
			foreach (var filter in Filters)
				filter.ApplyTo(filterContext, ReportContainer);
			var predicate = _vertexSelector(filterContext).Build();
			if (predicate != null)
				filteredDataQuery = filteredDataQuery.Where(predicate);

			if (Orders.Any()) {
				var orderedOrders = Orders.OrderBy(o => o.DisplayOrder).ToList();
				FilteredOrderedQuery = orderedOrders.First().ApplyOrder(filteredDataQuery);
				for (int i = 1; i < orderedOrders.Count; i++) //KMS DO why aren't we iterating through orderedOrders?
					FilteredOrderedQuery = Orders.ElementAt(i).ApplyOrder(FilteredOrderedQuery);
			} else {
				//KMS DO to what end?
				FilteredOrderedQuery = filteredDataQuery.OrderBy(x => 0);
			}
		}

		public void Write(List<OrderedTextOutput> html, TextWriter csv) {
			StageData();
			foreach (var sub in SubReports.OrderBy(s => s.DisplayOrder)) {
				sub.ReportContainer = ReportContainer;
				sub.ReportQuery = this;
				using (var sw = html == null ? null : new StringWriter()) {
					sub.Write(FilteredOrderedQuery, sw, csv);
					csv?.WriteLine();
					html?.Add(new OrderedTextOutput { DisplayOrder = sub.DisplayOrder, Output = sw.ToString() });
				}
			}
		}
	}
}