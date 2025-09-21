using LMS.Domain.Entities;
using LMS.Domain.Enums;
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
        public DbSet<TutorProfile> TutorProfiles { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleSession> ScheduleSessions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("lms");

            // Move Refresh Token entity into "auth" schema
            builder.Entity<RefreshToken>().ToTable("RefreshTokens", "auth");

            // Move Identity Framework entites into "auth" schema
            builder.Entity<AppUser>().ToTable("Users", "auth");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "auth");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "auth");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "auth");
            builder.Entity<IdentityRole>().ToTable("Roles", "auth");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "auth");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "auth");

            // Move other entities into "lms" schema
            builder.Entity<TutorProfile>().ToTable("TutorProfiles", "lms");
            builder.Entity<StudentProfile>().ToTable("StudentProfiles", "lms");
            builder.Entity<Course>().ToTable("Courses", "lms");
            builder.Entity<Enrollment>().ToTable("Enrollments", "lms");
            builder.Entity<Schedule>().ToTable("Schedules", "lms");
            builder.Entity<ScheduleSession>().ToTable("ScheduleSessions", "lms");
            builder.Entity<Prerequisite>().ToTable("Prerequisites", "lms");
            builder.Entity<Category>().ToTable("Categories", "lms");
            builder.Entity<Tag>().ToTable("Tags", "lms");
            builder.Entity<Notification>().ToTable("Notifications", "lms");
            builder.Entity<NotificationRecipient>().ToTable("NotificationRecipients", "lms");



            //// ENUM STRING MAPPING

            builder.Entity<Course>()
                .Property(c => c.Status)
                .HasConversion<string>();

            builder.Entity<Enrollment>()
                .Property(e => e.Status)
                .HasConversion<string>();

            builder.Entity<ScheduleSession>()
                .Property(s => s.DayOfWeek)
                .HasConversion<string>();



            //// COMPOSITE KEYS

            // Enrollment composite PK
            builder.Entity<Enrollment>().HasKey(e => new { e.StudentId, e.CourseId });

            // Prerequisite composite PK
            builder.Entity<Prerequisite>().HasKey(p => new { p.TargetCourseId, p.PrerequisiteCourseId });

            // NotificationRecipient composite PK 
            builder.Entity<NotificationRecipient>().HasKey(nr => new { nr.NotificationId, nr.RecipientId });




            //// RELATIONSHIPS
            
            // AppUser -> Role (many-to-one)
            builder.Entity<AppUser>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // can't delete a role while appUsers use it

            // AppUser -> TutorProfile (one-to-one)
            builder.Entity<AppUser>()
                .HasOne(u => u.TutorProfile)
                .WithOne(tp => tp.User)
                .HasForeignKey<TutorProfile>(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // delete tutor profile when appUser is deleted

            // AppUser -> StudentProfile (one-to-one)
            builder.Entity<AppUser>()
                .HasOne(u => u.StudentProfile)
                .WithOne(sp => sp.User)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // delete student profile when app user is deleted

            // AppUser -> NotificationRecipient (one-to-many)
            builder.Entity<AppUser>()
                .HasMany(u => u.NotificationRecipients)
                .WithOne(nr => nr.Recipient)
                .HasForeignKey(nr => nr.RecipientId)
                .OnDelete(DeleteBehavior.Cascade); // delete notification recipients when user is deleted

            // Notification -> NotificationRecipient (one-to-many)
            builder.Entity<Notification>()
                .HasMany(n => n.Recipients)
                .WithOne(nr => nr.Notification)
                .HasForeignKey(nr => nr.NotificationId)
                .OnDelete(DeleteBehavior.Cascade); // delete notification recipients when notification is deleted

            // TutorProfile -> Courses (many-to-many)
            builder.Entity<TutorProfile>()
                .HasMany(tp => tp.Courses)
                .WithMany(c => c.TutorProfiles);

            // StudentProfile -> Enrollments (one-to-many)
            builder.Entity<StudentProfile>()
                .HasMany(sp => sp.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade); // delete enrollments when student profile is deleted


            // Course -> Enrollments (one-to-many)
            builder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // delete enrollments when course is deleted


            // Course -> Prerequisites (one-to-many)
            builder.Entity<Course>()
                .HasMany(c => c.Prerequisites)
                .WithOne(p => p.TargetCourse)
                .HasForeignKey(p => p.TargetCourseId)
                .OnDelete(DeleteBehavior.NoAction); // delete prerequisite entries when target course is deleted

            // Course -> PrerequisiteFor (one-to-many)
            builder.Entity<Course>()
                .HasMany(c => c.PrerequisiteFor)
                .WithOne(p => p.PrerequisiteCourse)
                .HasForeignKey(p => p.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Cascade); // delete prerequiesite entries when prerequisite course is deleted


            // Course -> Category (many-to-one)
            builder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // can't delete category if courses are using it


            // Course -> Tag (many-to-many)
            builder.Entity<Course>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Courses);

            
            // Course -> Schedule (many-to-one)
            builder.Entity<Course>()
                .HasOne(c => c.Schedule)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict); // can't delete the schedule if a course is using it

            // Schedule -> Sessions (one-to-many)
            builder.Entity<Schedule>()
                .HasMany(s => s.Sessions)
                .WithOne(ss => ss.Schedule)
                .HasForeignKey(ss => ss.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade); // delete Sessions when the Schedule is deleted





            //// INDEXES 
            // ==== SEARCH AND FILTERING INDEXES ====

            // Course search and filtering (most important)
            builder.Entity<Course>().HasIndex(c => c.Title).IsUnique().HasDatabaseName("IX_Courses_Title");
            builder.Entity<Course>().HasIndex(c => c.CategoryId).HasDatabaseName("IX_Courses_CategoryId");
            builder.Entity<Course>().HasIndex(c => new { c.Status, c.CategoryId }).HasDatabaseName("IX_Courses_Status_CategoryId");

            // Category hierarchy queries
            builder.Entity<Category>().HasIndex(c => c.Name).IsUnique().HasDatabaseName("IX_Categories_Name");

            // Tag search
            builder.Entity<Tag>().HasIndex(t => t.Name).IsUnique().HasDatabaseName("IX_Tags_Name");

            // User authentication and profile lookups
            builder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_AppUsers_Email");


            // ==== NOTIFICATION SYSTEM INDEXES ====

            // Notification recipient queries (unread notifications, etc.)
            builder.Entity<NotificationRecipient>().HasIndex(nr => new { nr.RecipientId, nr.Opened }).HasDatabaseName("IX_NotificationRecipients_RecipientId_Opened");
            builder.Entity<NotificationRecipient>().HasIndex(nr => new { nr.RecipientId, nr.CreatedAt }).HasDatabaseName("IX_NotificationRecipients_RecipientId_CreatedAt");


            // ==== ENROLLMENT AND COURSE MANAGEMENT ====

            // Enrollment status queries
            builder.Entity<Enrollment>().HasIndex(e => new { e.StudentId, e.Status }).HasDatabaseName("IX_Enrollments_StudentId_Status");
            builder.Entity<Enrollment>().HasIndex(e => new { e.CourseId, e.Status }).HasDatabaseName("IX_Enrollments_CourseId_Status");


            // ==== SCHEDULING INDEXES ====

            // Session time-based queries
            builder.Entity<ScheduleSession>().HasIndex(s => new { s.ScheduleId, s.StartTime }).HasDatabaseName("IX_Sessions_ScheduleId_StartTime");
            builder.Entity<ScheduleSession>().HasIndex(s => new { s.ScheduleId, s.DayOfWeek }).HasDatabaseName("IX_Sessions_ScheduleId_DayOfWeek");


            // ==== PREREQUISITE QUERIES ====

            // Composite index for prerequisite lookups (both directions)
            builder.Entity<Prerequisite>().HasIndex(p => p.TargetCourseId).HasDatabaseName("IX_Prerequisites_TargetCourseId");
            builder.Entity<Prerequisite>().HasIndex(p => p.PrerequisiteCourseId).HasDatabaseName("IX_Prerequisites_PrerequisiteCourseId");


            // ==== AUDIT AND TEMPORAL QUERIES ====
            builder.Entity<Course>().HasIndex(c => c.CreatedAt).HasDatabaseName("IX_Courses_CreatedAt");
            builder.Entity<Enrollment>().HasIndex(e => e.EnrolledAt).HasDatabaseName("IX_Enrollments_EnrolledAt");
            builder.Entity<AppUser>().HasIndex(u => u.CreatedAt).HasDatabaseName("IX_AppUsers_CreatedAt");

        }
    }
}
