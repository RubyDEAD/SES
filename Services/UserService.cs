using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models;

namespace SES.Services
{
    public class UserService : IUserService
    {
        private readonly SchoolContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(SchoolContext db) => _db = db;

        public async Task<(bool ok, string? error)> RegisterAsync(string email, string firstName, string lastName, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password are required.");

            var exists = await _db.Users.AnyAsync(u => u.Email == email);
            if (exists) return (false, "Email already registered.");
            
            var student = new Student {
            FirstName = firstName.Trim(), 
            LastName = lastName.Trim(), 
            DateCreated = DateTime.Now
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            var user = new User
            {
                Email = email.Trim(),
                FirstName = firstName?.Trim() ?? string.Empty,
                LastName = lastName?.Trim() ?? string.Empty,
                Role = UserRole.Student
            };

            user.PasswordHash = _hasher.HashPassword(user, password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            
            return (true, null);
        }

        public async Task<(bool ok, string? error, User? user)> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password are required.", null);

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return (false, "Invalid credentials.", null);

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                return (false, "Invalid credentials.", null);



            return (true, null, user);
        }
    }
}
