using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Enrollments
        IEnrollmentRepository Enrollments { get; }

        // Schedules
        IScheduleRepository Schedules { get; }
        IScheduleSessionRepository ScheduleSessions { get; }

        // Courses
        ICategoryRepository Category { get; }
        ITagRepository Tag { get; }

        ICourseRepository Courses { get; }
        IPrerequisiteRepository Prerequisites { get; }

        // Auth - Users
        IRefreshTokenRepository RefreshTokens { get; }
        IUserRepository Users { get; }
        ITutorProfileRepository TutorProfiles { get; }
        IStudentProfileRepository StudentProfiles { get; }

        Task<int> CompleteAsync();
    }

}
