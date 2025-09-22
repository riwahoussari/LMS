using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth - Users - Profiles
            CreateMap<AppUser, UserResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "unknown"));

            CreateMap<StudentProfile, StudentProfileResponseDto>();
            CreateMap<TutorProfile, TutorProfileResponseDto>();
            CreateMap<TutorProfile, TutorProfileExtendedResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<RefreshToken, RefreshTokenDto>();

            // Categories - Tags
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<Tag, TagResponseDto>();

            // Courses
            CreateMap<Course, PartialCourseResponseDto>();
            CreateMap<Course, CourseResponseDto>()
                .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => src.Prerequisites.Select(p => p.PrerequisiteCourse)));

            // Schedules
            CreateMap<Schedule, ScheduleResponseDto>();
            CreateMap<ScheduleSession, ScheduleSessionResponseDto>();

            // Enrollments
            CreateMap<Enrollment, EnrollmentResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Student.User.Id));

        }

    }
}
