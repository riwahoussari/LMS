using AutoMapper;
using LMS.Application.DTOs;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

            CreateMap<RefreshToken, RefreshTokenDto>();

            // Categories - Tags
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<Tag, TagResponseDto>();

            // Courses
            CreateMap<Course, CourseResponseDto>();
            CreateMap<Course, PrerequisiteCourseResponseDto>();

            // Schedules
            CreateMap<Schedule, ScheduleResponseDto>();
            CreateMap<ScheduleSession, ScheduleSessionResponseDto>();

        }

    }
}
