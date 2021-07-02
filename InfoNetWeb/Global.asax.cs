using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Infonet.Core;
using Infonet.Core.Entity.Validation;
using Infonet.Core.Reflection;
using Infonet.Data.Entity.Validation;
using Infonet.Reporting.Core;
using Infonet.Web.Models.Identity;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Binding;
using Infonet.Web.Mvc.Validation;
using Infonet.Web.Utilities;
using Serilog;
using Serilog.Events;

namespace Infonet.Web {
	public class MvcApplication : HttpApplication {
		public static readonly string Copyright = typeof(MvcApplication).Assembly.GetCustomAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright) ?? "© 2018";
		public static readonly string Company = typeof(MvcApplication).Assembly.GetCustomAttributeValue<AssemblyCompanyAttribute>(a => a.Company) ?? "Illinois Criminal Justice Information Authority";
		public static readonly string InformationalVersion = typeof(MvcApplication).Assembly.GetCustomAttributeValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion) ?? "v#.#.#.#-N-gXXXXXXX";
		public static readonly string FileVersion = typeof(MvcApplication).Assembly.GetCustomAttributeValue<AssemblyFileVersionAttribute>(a => a.Version) ?? "#.#.#.#";
		public static readonly string FileVersionMajorMinor = string.Join(".", FileVersion.Split('.').Take(2));

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			ModelBinders.Binders.DefaultBinder = new ModelBindingRuleSet(ModelBinders.Binders.DefaultBinder, new DerivedDictionaryRule(), new StringTrimModelBindingRule());
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

			//add client-side validation adapters for our custom validation attributes
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(BetweenJulyTwoThousandEightTodayAttribute), typeof(BetweenJulyTwoThousandEightTodayAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(BetweenNineteenFiftyTodayAttribute), typeof(BetweenNineteenFiftyTodayAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(BetweenNineteenNinetyTodayAttribute), typeof(BetweenNineteenNinetyTodayAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(BetweenNineteenSeventyTodayAttribute), typeof(BetweenNineteenSeventyTodayAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(BetweenNineteenThirtyTodayAttribute), typeof(BetweenNineteenThirtyTodayAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(NotGreaterThanTodayAttribute), typeof(NotGreaterThanTodayAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(NotLessThanNineteenSeventyAttribute), typeof(NotLessThanNineteenSeventyAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(QuarterIncrementAttribute), typeof(QuarterIncrementAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(WholeNumberAttribute), typeof(WholeNumberAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(CompareWithDateAttribute), typeof(CompareWithDateAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(FileExtensionAttribute), typeof(FileExtensionAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ResidenceMoveDateBeforeFirstContactAttribute), typeof(ResidenceMoveDateBeforeFirstContactAttributeAdapter));

			string logFileName = ConfigurationManager.AppSettings["Logging:FileName"];
			if (logFileName != null)
				Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Verbose()
					.WriteTo.Async(c => c.RollingFile(logFileName,
						(LogEventLevel)Enum.Parse(typeof(LogEventLevel), ConfigurationManager.AppSettings["Logging:FileMinimumLevel"] ?? "Information"),
						retainedFileCountLimit: ConvertNull.ToInt32(ConfigurationManager.AppSettings["Logging:FilesRetainedCount"]),
						fileSizeLimitBytes: ConvertNull.ToInt32(ConfigurationManager.AppSettings["Logging:FileSizeMaximumBytes"]),
						shared: true)) /* Shared will allow multiple or overlapping worker procesess to share the log file. Async should nullify the performance penalty. */
					.CreateLogger();
			Log.Information("InfonetWeb Started");

			//add Binds for all BindHints
			BindHints.GenerateBindAttributes();

			ReportContainer.InitializeRazor();

			ApplicationDbInitializer.InitializeIdentityForEF(new ApplicationDbContext());
		}

		protected void Application_Error(object sender, EventArgs e) {
			var request = HttpContext.Current.Request;
			var parms = new Dictionary<string, object>();
			foreach (string key in request.Params.Keys) {
				if (key == "__RequestVerificationToken")
					continue;
				if (key == "ASP.NET_SessionId")
					continue;
				if (key == ".AspNet.ApplicationCookie")
					continue;
				if (key.All(c => c == '_' || char.IsUpper(c)))
					continue;
				var values = request.Params.GetValues(key);
				parms.Add(key, values?.Length == 1 ? (object)values[0] : values);
			}
			Log.Error(Server.GetLastError(), "An unhandled Application error occurred during {Method:l} request by {UserName:l} for {Url:l}\r\n{Params}", request.HttpMethod, User.Identity.Name, request.Url.AbsoluteUri, parms);
		}

		protected void Application_End(object sender, EventArgs e) {
			ReportContainer.DisposeRazor();
			Log.Information("InfonetWeb Stopped");
			Log.CloseAndFlush();
		}

		protected void Application_PostRequestHandlerExecute(object sender, EventArgs e) {
			if (string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) || HttpContext.Current.Session == null)
				return;
			var center = (SessionCenter)HttpContext.Current.Session["_Center"];
			if (center == null)
				return;

			UsersActivityList.Update(new UserActivity {
				CenterId = center.Id,
				CenterName = center.Name,
				LastAccessed = DateTime.Now,
				SessionId = HttpContext.Current.Session.SessionID,
				UserName = HttpContext.Current.User.Identity.Name
			});
		}

		protected void Session_Start(object sender, EventArgs e) {
			var now = DateTime.Now;
			UsersActivityList.Update(new UserActivity {
				LastAccessed = now,
				SessionId = HttpContext.Current.Session.SessionID,
				SessionStart = now
			});
		}
	}
}