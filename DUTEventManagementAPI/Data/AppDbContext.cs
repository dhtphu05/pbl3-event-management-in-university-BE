using Microsoft.EntityFrameworkCore;
using DUTEventManagementAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DUTEventManagementAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventImage> EventImages { get; set; }
        public DbSet<Attendance> Attendances { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed 3 roles
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "Admin"
            });
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "User"
            });
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "3",
                Name = "Organizer",
                NormalizedName = "Organizer"
            });
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "4",
                Name = "Union",
                NormalizedName = "Union"
            });
            //builder.Entity<AppUser>();
            //.HasMany(e => e.Events)
            //.WithOne(e => e.AppUser)
            //.HasForeignKey(e => e.AppUserId)
            //.OnDelete(DeleteBehavior.Cascade);
        }
    }
}
