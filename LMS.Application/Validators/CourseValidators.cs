using FluentValidation;
using LMS.Application.DTOs;
using LMS.Domain.Enums;
using LMS.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Validators
{
    // Create
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {

            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");

            RuleFor(x => x.MaxCapacity).Must(mc => mc >= 0).WithMessage("Max Capacity cannot be negative.");

            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("CategoryId is required");

            RuleFor(x => x.Schedule)
                .NotNull().WithMessage("Schedule is required")
                .SetValidator(new ScheduleDtoValidator());
        }
    }

    public class AssignTutorDtoValidator : AbstractValidator<AssignTutorDto>
    {
        public AssignTutorDtoValidator()
        {

            RuleFor(x => x.TutorId).NotEmpty().WithMessage("TutorId is required");
        }
    }

    // Read
    public class GetCoursesQueryDtoValidator : AbstractValidator<GetCoursesQueryDto>
    {
        public GetCoursesQueryDtoValidator()
        {
            RuleFor(x => x.Limit)
                .Must(l => l == null || l > 0)
                .WithMessage("Limit must be bigger than 0");

            RuleFor(x => x.Offset)
                .Must(o => o == null || o >= 0)
                .WithMessage("Offset must be positive");
        }
        

    }

    // Update
    public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseDtoValidator()
        {
            RuleFor(x => x.Schedule)
                .SetValidator(new ScheduleDtoValidator())
                .When(x => x.Schedule != null);

            RuleFor(x => x.MaxCapacity).Must(mc => mc == null || mc >= 0).WithMessage("Max Capacity cannot be negative.");
        }
    }

    
}
