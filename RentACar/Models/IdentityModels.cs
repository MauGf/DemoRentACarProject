using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace RentACar.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class MyUser : IdentityUser<int, MyUserLogin, MyUserRole, MyUserClaim>
    {
        public virtual UserDetails UserDetails { get; set; }
        public virtual IEnumerable<Rent> Rents { get; set; }
        public virtual IEnumerable<Reservation> Reservations { get; set; }

        public bool AddUserDetails(string firstName, string lastName)
        {
            try
            {
                UserDetails userDetails = new UserDetails(this.Id, firstName, lastName);
                using (var db = new ApplicationDbContext())
                {
                    db.UserDetails.Add(userDetails);
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveUserDetails()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    UserDetails userDetails = db.UserDetails.Find(this.Id);
                    db.Entry(userDetails).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddRole(string role)
        {
            var userManager = new Microsoft.AspNet.Identity.UserManager<MyUser, int>(new MyUserStore(new ApplicationDbContext()));

            try
            {
                userManager.AddToRole(this.Id, role);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<MyUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class MyUserLogin : IdentityUserLogin<int> { }

    public class MyUserClaim : IdentityUserClaim<int> { }

    public class MyUserRole : IdentityUserRole<int> { }

    public class MyRole : IdentityRole<int, MyUserRole>
    {
        public MyRole() { }

        public MyRole(string name) { Name = name; }
    }

    public class MyUserStore : UserStore<MyUser, MyRole, int, MyUserLogin, MyUserRole, MyUserClaim>
    {
        public MyUserStore(ApplicationDbContext context)
            : base(context)
        {

        }
    }

    public class MyRoleStore : RoleStore<MyRole, int, MyUserRole>
    {
        public MyRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class ApplicationDbContext : IdentityDbContext<MyUser, MyRole, int, MyUserLogin, MyUserRole, MyUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map entities to tables
            modelBuilder.Entity<MyUser>().ToTable("Users");
            modelBuilder.Entity<MyRole>().ToTable("Roles");
            modelBuilder.Entity<MyUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<MyUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<MyUserClaim>().ToTable("UserClaims");

            // Primary key autoincrement
            modelBuilder.Entity<MyUser>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MyRole>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MyUserClaim>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Changed primary key names in tables
            modelBuilder.Entity<MyUser>().Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<MyRole>().Property(p => p.Id).HasColumnName("RoleId");
            modelBuilder.Entity<MyUserClaim>().Property(p => p.Id).HasColumnName("ClaimId");
        }
    }
}