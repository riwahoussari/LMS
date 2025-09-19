using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IEnrollmentRepository Enrollments { get; }
        //public ICourseRepository Courses { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IUserRepository Users { get; }
        public ITutorProfileRepository TutorProfiles { get; }
        public IStudentProfileRepository StudentProfiles { get; }

        public UnitOfWork(AppDbContext context,
                          IEnrollmentRepository enrollments,
                          IRefreshTokenRepository refreshTokens,
                          //ICourseRepository courses,
                          IUserRepository users,
                          ITutorProfileRepository tutorProfiles,
                          IStudentProfileRepository studentProfiles)
        {
            _context = context;
            Enrollments = enrollments;
            //Courses = courses;
            RefreshTokens = refreshTokens;
            Users = users;
            TutorProfiles = tutorProfiles;
            StudentProfiles = studentProfiles;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
