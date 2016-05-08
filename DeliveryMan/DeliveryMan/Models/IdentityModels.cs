using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DataLayer;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DeliveryMan.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public System.Data.Entity.DbSet<Contact> contacts { get; set; }
        public System.Data.Entity.DbSet<Deliveryman> deliverymen { get; set; }
        public System.Data.Entity.DbSet<Order> orders { get; set; }
        public System.Data.Entity.DbSet<Restaurant> restaurants { get; set; }
        public System.Data.Entity.DbSet<Review> reviews { get; set; }
        public System.Data.Entity.DbSet<Blacklist> blacklists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            // modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}