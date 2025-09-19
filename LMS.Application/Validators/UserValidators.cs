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
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required");

            RuleFor(x => x.BirthDate)
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$").WithMessage("Birthdate should have yyyy-mm-dd format");
        }
    }

    

}
