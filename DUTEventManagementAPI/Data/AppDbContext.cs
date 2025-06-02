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
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<EventFacultyScope> EventFacultyScopes { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed faculties
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "101",
                FacultyName = "K. Cơ khí",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "102",
                FacultyName = "K. Công nghệ Thông tin",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "103",
                FacultyName = "K. Cơ khí Giao thông",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "104",
                FacultyName = "K. Công nghệ Nhiệt - Điện lạnh",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "105",
                FacultyName = "K. Điện",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "106",
                FacultyName = "K. Điện tử Viễn thông",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "107",
                FacultyName = "K. Hóa",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "109",
                FacultyName = "K. Xây dựng Cầu - Đường",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "110",
                FacultyName = "K. Xây dựng Dân dụng - Công nghiệp",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "111",
                FacultyName = "K. Xây dựng công trình thủy",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "117",
                FacultyName = "K. Môi trường",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "118",
                FacultyName = "K. Quản lý dự án",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "121",
                FacultyName = "K. Kiến trúc",
            });
            builder.Entity<Faculty>().HasData(new Faculty
            {
                FacultyId = "123",
                FacultyName = "K. Khoa học Công nghệ tiên tiến",
            });


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
