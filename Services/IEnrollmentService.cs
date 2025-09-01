
using System.Threading.Tasks;
using SES.Models;

namespace SES.Services
{
    public interface IEnrollmentService
    {
        Task<(bool ok, string? error)> EnrollAsync(int studentId, int courseId);
        Task<(bool ok, string? error)> SetGradeAsync(int enrollmentId, float grade);
    }
}
