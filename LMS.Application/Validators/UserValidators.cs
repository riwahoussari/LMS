using FluentValidation;
using LMS.Application.DTOs;
using LMS.Infrastructure.Constants;
using LMS.Infrastructure.Seeding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Validators
{
    // Read
    public class GetUsersQueryDtoValidator : AbstractValidator<GetUsersQueryDto>
    {
        public GetUsersQueryDtoValidator()
        {
            RuleFor(x => x.Role)
                .Must(r => string.IsNullOrEmpty(r) || RoleConstants.List.Contains(r.ToLower()))
                .WithMessage("Invalid role specified.");

        }
    }

    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        
        // Update
        public UserUpdateDtoValidator()
        {

            RuleFor(x => x.BirthDate)
                .Must(birthdate => 
                    string.IsNullOrWhiteSpace(birthdate) || 
                    System.Text.RegularExpressions.Regex.IsMatch(birthdate, @"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$")
                )
                .WithMessage("Birthdate should have yyyy-mm-dd format");
  
        }
    }

    public class SuspendUserDtoValidator : AbstractValidator<SuspendUserDto>
    {
        public SuspendUserDtoValidator()
        {
            RuleFor(x => x.IsSuspended)
                .Must(isSuspended => isSuspended == true || isSuspended == false)
                .WithMessage("IsSuspended boolean field is required");
        }

    }



}
