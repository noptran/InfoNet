using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Infonet.Web.Models.Identity {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			userIdentity.AddClaim(new Claim("CenterId", this.CenterId.ToString()));
			// Add custom user claims here
			return userIdentity;
        }
		public int CenterId { get; set; }
		//PRC DO comment back in and update identity database
		//public bool PasswordReset { get; set; }
	}

	public class ApplicationRole : IdentityRole<int, ApplicationUserRole> {
		public ApplicationRole() : base() { }
		public ApplicationRole(string name) { Name = name; }
		public string Description { get; set; }
	}
	
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base("InfonetIdentityContext")
        {
        }

		static ApplicationDbContext() {
			// Set the database intializer which is run once during application start
			// This seeds the database with admin user credentials and admin role
			Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
		}

		public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

	public class ApplicationUserRole : IdentityUserRole<int> { }
	public class ApplicationUserClaim : IdentityUserClaim<int> { }
	public class ApplicationUserLogin : IdentityUserLogin<int> { }

	public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, int,
		ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim> {
		public ApplicationUserStore(ApplicationDbContext context)
			: base(context) {
		}
	}

	public class ApplicationRoleStore : RoleStore<ApplicationRole, int, ApplicationUserRole> {
		public ApplicationRoleStore(ApplicationDbContext context)
			: base(context) {
		}
	}
}