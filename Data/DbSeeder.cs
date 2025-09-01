// Data/DbSeeder.cs
using System.Linq;
using Microsoft.AspNetCore.Identity;
using SES.Models;

namespace SES.Data
{
    public static class DbSeeder
    {
        public static void Seed(SchoolContext context)
        {
            // Ensure DB has no pending tracked changes before user seeding
            if (context.ChangeTracker.HasChanges())
            {
                context.SaveChanges();
            }

            // Seed only Users with secure password hashes if none exist
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();

                // Admin user
                var admin = new User
                {
                    Email = "admin@example.com",
                    FirstName = "System",
                    LastName = "Admin",
                    Role = UserRole.Admin
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345!");
                context.Users.Add(admin);

                // Optionally create a student user only if there is an existing Student row to link
                var firstStudent = context.Students.FirstOrDefault();
                if (firstStudent != null)
                {
                    var studentUser = new User
                    {
                        Email = "student1@example.com",
                        FirstName = firstStudent.FirstName,
                        LastName = firstStudent.LastName,
                        Role = UserRole.Student,
                        StudentId = firstStudent.Id
                    };
                    studentUser.PasswordHash = hasher.HashPassword(studentUser, "Student@12345!");
                    context.Users.Add(studentUser);
                }

                context.SaveChanges();
            }

            // Do not create Enrollments; keep them empty unless inserted by the app/workflows.
        }
    }
}
