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
            CreateMap<AppUser, UserResponseDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        }

    }
}
