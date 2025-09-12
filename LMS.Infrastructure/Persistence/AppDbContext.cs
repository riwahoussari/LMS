using LMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LMS.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string> // Replace User with your Identity user entity
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tables
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //public DbSet<TutorProfile> TutorProfiles { get; set; }
        //public DbSet<StudentProfile> StudentProfiles { get; set; }
        //public DbSet<Course> Courses { get; set; }
        //public DbSet<Schedule> Schedules { get; set; }
        //public DbSet<ScheduleSession> ScheduleSessions { get; set; }
        //public DbSet<Category> Categories { get; set; }
        //public DbSet<Tag> Tags { get; set; }
        //public DbSet<CourseTag> CourseTags { get; set; }
        //public DbSet<Prerequisite> Prerequisites { get; set; }
        //public DbSet<CourseTutor> CourseTutors { get; set; }
        //public DbSet<Enrollment> Enrollments { get; set; }
        //public DbSet<Notification> Notifications { get; set; }
        //public DbSet<NotificationRecipient> NotificationRecipients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships, composite keys, enums, etc.
            //builder.Entity<Enrollment>()
            //    .HasKey(e => new { e.StudentProfileId, e.CourseId });

            //builder.Entity<CourseTag>()
            //    .HasKey(ct => new { ct.CourseId, ct.TagId });

            //builder.Entity<Prerequisite>()
            //    .HasKey(p => new { p.TargetCourseId, p.PrerequisiteCourseId });

            //builder.Entity<CourseTutor>()
            //    .HasKey(ct => new { ct.CourseId, ct.TutorProfileId });

            //// Map enums to strings
            //builder.Entity<Course>()
            //    .Property(c => c.Status)
            //    .HasConversion<string>();

            //builder.Entity<Enrollment>()
            //    .Property(e => e.Status)
            //    .HasConversion<string>();

            //builder.Entity<ScheduleSession>()
            //    .Property(s => s.DayOfWeek)
            //    .HasConversion<string>();

            // Add any indexes you need
            builder.Entity<AppUser>().HasIndex(u => u.RoleId);
            //builder.Entity<Course>().HasIndex(c => c.CategoryId);
        }
    }
}
