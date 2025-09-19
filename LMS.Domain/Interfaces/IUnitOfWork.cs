using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEnrollmentRepository Enrollments { get; }
        //ICourseRepository Courses { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IUserRepository Users { get; }
        ITutorProfileRepository TutorProfiles { get; }
        IStudentProfileRepository StudentProfiles { get; }

        Task<int> CompleteAsync();
    }

}
