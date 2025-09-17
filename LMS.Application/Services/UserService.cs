using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }


        // READ
        public async Task<UserResponseDto?> GetByIdAsync(string id, bool withProfile = false)
        {
            AppUser? user = withProfile ?
                await _uow.Users.GetByIdWithProfileAsync(id) :
                await _uow.Users.GetByIdAsync(id);

            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync(bool withProfile = false)
        {
            var users = withProfile ?
                await _uow.Users.GetAllWithProfilesAsync() :
                await _uow.Users.GetAllAsync();

            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<IEnumerable<UserResponseDto>> GetByRoleAsync(string roleName, bool withProfile = false)
        {
            var users = withProfile ?
                await _uow.Users.FindWithProfilesAsync(u => u.Role.Name == roleName) :
                await _uow.Users.FindAsync(u => u.Role.Name == roleName);
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetByEmailAsync(string email, bool withProfile = false)
        {
            AppUser? user = withProfile ?
                await _uow.Users.FindSingleWithProfileAsync(u => u.Email == email.ToLower()) :
                await _uow.Users.FindSingleAsync(u => u.Email == email.ToLower());

            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        // UPDATE
        public async Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return null;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.BirthDate = dto.BirthDate;

            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
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

}
