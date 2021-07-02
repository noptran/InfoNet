using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infonet.Data;
using Infonet.Web.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Infonet.Web {
	public class EmailService : IIdentityMessageService {
		public Task SendAsync(IdentityMessage message) {
			// Plug in your email service here to send an email.
			return Task.FromResult(0);
		}
	}

	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
	public class ApplicationUserManager : UserManager<ApplicationUser, int> {
		public ApplicationUserManager(IUserStore<ApplicationUser, int> store) : base(store) { }

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) {
			var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser, int>(manager) {
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = false
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator {
				RequiredLength = 8,
				RequireDigit = true
			};

			// Configure user lockout defaults 
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;
			manager.EmailService = new EmailService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
				manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));

			return manager;
		}
	}

	// Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
	public class ApplicationRoleManager : RoleManager<ApplicationRole, int> {
		public ApplicationRoleManager(IRoleStore<ApplicationRole, int> roleStore) : base(roleStore) { }

		public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context) {
			return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
		}
	}

	// This is useful if you do not want to tear down the database each time you run the application.
	// public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
	// This example shows you how to create a new database if the Model changes
	public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext> {
		protected override void Seed(ApplicationDbContext context) {
			InitializeIdentityForEF(context);
			base.Seed(context);
		}

#pragma warning disable 612
		public static void InitializeIdentityForEF(ApplicationDbContext db) {
			ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(db));
			ApplicationRoleManager roleManager = new ApplicationRoleManager(new ApplicationRoleStore(db));
			userManager.UserValidator = new UserValidator<ApplicationUser, int>(userManager) {
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = false
			};
			userManager.PasswordValidator = new PasswordValidator {
				RequiredLength = 1
			};
			if (roleManager.Roles.Any() || userManager.Users.Any())
				return;

			using (var dbContext = new InfonetServerContext()) {
				var roles = dbContext.SecurityRoles;
				foreach (var each in roles) {
					if (each.Code == "SYSDEVELOPER" || each.Code == "SYSUSER" || each.Code == "DHSADMIN"  || each.Code == "DHSDATAENTRY")
						continue;
					roleManager.Create(new ApplicationRole(each.Code) { Description = each.Description });
				}
				foreach (var eachIdentity in dbContext.SecurityIdentities) {
					var user = new ApplicationUser();
					user.CenterId = eachIdentity.SecurityPrincipal.Properties.CenterId;
					user.UserName = eachIdentity.SigninName;
					user.Email = eachIdentity.SecurityPrincipal.Email;
					var userresult = userManager.Create(user, eachIdentity.Pwd);
					if (!userresult.Succeeded)
						throw new Exception("Failed upon creating User: " + string.Concat(userresult.Errors));
					foreach (var eachRole in eachIdentity.SecurityPrincipal.SecurityRoles) {
						if (eachRole.Code == "SYSDEVELOPER" || eachRole.Code == "SYSUSER" || eachRole.Code == "DHSADMIN" || eachRole.Code == "DHSDATAENTRY")
							continue;
						userManager.AddToRole(user.Id, eachRole.Code);
					}
				}
			}
		}
	}
#pragma warning restore 612

	// Configure the application sign-in manager which is used in this application.
	public class ApplicationSignInManager : SignInManager<ApplicationUser, int> {
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager) { }

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user) {
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context) {
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}
	}
}