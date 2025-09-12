using FluentValidation;
using LMS.Application.DTOs;
using LMS.Infrastructure.Seeding;

namespace LMS.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"(?=.*[a-z])")
                .WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"(?=.*[A-Z])")
                .WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"(?=.*\d)")
                .WithMessage("Password must contain at least one digit")
                .Matches(@"(?=.*[^\w\s])")
                .WithMessage("Password must contain at least one special character");


            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role is required")
                .Must(r => RoleSeeder.Roles.Contains(r.ToLower()))
                .WithMessage($"Role must be one of: {string.Join(", ", RoleSeeder.Roles)}");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required");
        }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }

}
