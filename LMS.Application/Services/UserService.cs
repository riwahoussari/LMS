using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Strategies.Sorting;
using LMS.Application.Validators;
using LMS.Common.Helpers;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly SortStrategyFactory<AppUser> _sorter;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, SortStrategyFactory<AppUser> sorter)
        {
            _uow = unitOfWork;
            _mapper = mapper;
            _sorter = sorter;
        }


        // READ
        public async Task<UserResponseDto?> GetByIdAsync(string id, bool withProfile = false)
        {
            AppUser? user = withProfile ?
                await _uow.Users.GetByIdWithProfileAsync(id) :
                await _uow.Users.GetByIdAsync(id);

            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync(GetUsersQueryDto dto, bool withProfile = false)
        {
            IQueryable<AppUser> query = withProfile
            ? _uow.Users.QueryWithProfiles()
            : _uow.Users.Query();

            query = UserFilter.Apply(query, dto);
            
            if (dto.SortBy != null)
            {
                query = _sorter.Apply(query, dto.SortBy, dto.SortAsc != false);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }


        // UPDATE
        public async Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto)
        {
            var user = await _uow.Users.GetByIdWithProfileAsync(id);
            if (user == null) return null;

            // common for all users
            user.FirstName = string.IsNullOrEmpty(dto.FirstName) ? user.FirstName : dto.FirstName;
            user.LastName = string.IsNullOrEmpty(dto.LastName) ? user.LastName : dto.LastName;
            user.BirthDate = string.IsNullOrEmpty(dto.BirthDate) ? user.BirthDate : dto.BirthDate;

            // tutor only
            if (user.Role.Name == RoleConstants.Tutor && user.TutorProfile != null)
            {
                user.TutorProfile.Bio = string.IsNullOrEmpty(dto.Bio) ? user.TutorProfile.Bio : dto.Bio;
                user.TutorProfile.Expertise = string.IsNullOrEmpty(dto.Expertise) ? user.TutorProfile.Expertise : dto.Expertise;
            }

            // student only
            else if (user.Role.Name == RoleConstants.Student && user.StudentProfile != null)
            {
                user.StudentProfile.Major = string.IsNullOrEmpty(dto.Major) ? user.StudentProfile.Major : dto.Major;
            }

            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<int> ToggleSuspendedAsync(string id, bool isSuspended)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return 404;

            if (user.Role.Name == RoleConstants.Admin) return 403;

            user.Suspended = isSuspended;
            await _uow.CompleteAsync();
            return 200;
        }


        // DELETE
        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return false;

            _uow.Users.Remove(user);
            await _uow.CompleteAsync();
            return true;
        }

    }

    // helpers
    internal class UserFilter
    {
        public static IQueryable<AppUser> Apply(IQueryable<AppUser> query, GetUsersQueryDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Role))
                query = query.Where(u => u.Role != null && u.Role.Name == dto.Role);

            if (!string.IsNullOrWhiteSpace(dto.Email))
                query = query.Where(u => u.Email != null &&
                                         u.Email.ToLower().Contains(dto.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                query = query.Where(u => u.FirstName != null && u.LastName != null &&
                                         (u.FirstName + " " + u.LastName).ToLower()
                                            .Contains(dto.FullName.ToLower()));

            return query;
        }
    }
  


}
