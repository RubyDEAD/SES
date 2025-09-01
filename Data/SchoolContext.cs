// Data/SchoolContext.cs
using Microsoft.EntityFrameworkCore;
using SES.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SES.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student
            modelBuilder.Entity<Student>(b =>
            {
                b.Property(p => p.FirstName).IsRequired().HasMaxLength(40);
                b.Property(p => p.LastName).IsRequired().HasMaxLength(40);
            });

            // Course
            modelBuilder.Entity<Course>(b =>
            {
                b.Property(p => p.Title).IsRequired().HasMaxLength(80);
                b.Property(p => p.Credits).IsRequired();
            });

            // Enrollment (use generic overload to avoid design-time cast issues)
            modelBuilder.Entity<Enrollment>(b =>
            {
                b.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

                b.HasOne(e => e.Student)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(e => e.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Course)
                 .WithMany(c => c.Enrollments)
                 .HasForeignKey(e => e.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // User
            modelBuilder.Entity<User>(b =>
            {
                b.Property(u => u.Email).IsRequired().HasMaxLength(80);
                b.HasIndex(u => u.Email).IsUnique();

                b.Property(u => u.FirstName).IsRequired().HasMaxLength(40);
                b.Property(u => u.LastName).IsRequired().HasMaxLength(40);
                b.Property(u => u.PasswordHash).IsRequired();

                // Optional 1–1 link to Student (nullable FK)
                // EF makes the FK indexed and unique for WithOne by default when configured as 1–1.
                b.HasOne(u => u.Student)
                 .WithOne()
                 .HasForeignKey<User>(u => u.StudentId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Do NOT force a unique index yourself if targeting SQLite,
                // because multiple NULLs are allowed and EF/Core providers handle filtered uniqueness differently.
                // If you need to guarantee only one non-null StudentId per user, the 1–1 above already enforces it.
                // If you want to ensure a student cannot be linked to multiple users, keep this as-is.
            });
        }

        public override int SaveChanges() => base.SaveChanges();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);
    }
}
