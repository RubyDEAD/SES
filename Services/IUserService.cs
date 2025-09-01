using System.Threading.Tasks;
using SES.Models;

namespace SES.Services
{
    public interface IUserService
    {
        Task<(bool ok, string? error)> RegisterAsync(string email, string firstName, string lastName, string password);
        Task<(bool ok, string? error, User? user)> LoginAsync(string email, string password);
    }
}
