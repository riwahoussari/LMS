using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators
{
    // Register
    public class RegisterUserBaseDtoValidator : AbstractValidator<RegisterUserBaseDto>
    {
        public RegisterUserBaseDtoValidator()
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


            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required");

            RuleFor(x => x.BirthDate)
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$").WithMessage("Birthdate should have yyyy-mm-dd format");
        }
    }

    public class RegisterStudentDtoValidator : AbstractValidator<RegisterStudentDto>
    {
        public RegisterStudentDtoValidator()
        {

            RuleFor(x => x.Major)
                .NotEmpty().WithMessage("Major is required");
        }
    }

    public class RegisterTutorDtoValidator : AbstractValidator<RegisterTutorDto>
    {
        public RegisterTutorDtoValidator()
        {
            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required");

            RuleFor(x => x.Expertise)
                .NotEmpty().WithMessage("Expertise is required");
        }
    }


    // Login
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


    // Tokens
    public class RefreshRequestDtoValidator : AbstractValidator<RefreshRequestDto>
    {
        public RefreshRequestDtoValidator() 
        { 
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh Token is required");   
        }
    }
  
}
