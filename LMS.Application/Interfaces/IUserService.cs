using LMS.Application.DTOs;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    
    public interface IUserService
    {

        // READ
        Task<UserResponseDto?> GetByIdAsync(string id, bool withProfile = false);
        Task<IEnumerable<UserResponseDto>> GetAllAsync(GetUsersQueryDto dto, bool withProfile = false);


        // UPDATE
        Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto);

        // DELETE
        Task<bool> DeleteAsync(string id);

    }

}
