using FluentValidation;
using LMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Validators
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator() {

            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");

            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("CategoryId is required");

            RuleFor(x => x.Schedule)
                .NotNull().WithMessage("Schedule is required")
                .SetValidator(new ScheduleDtoValidator());
        }
    }

    public class ScheduleDtoValidator : AbstractValidator<ScheduleDto> 
    { 
        public ScheduleDtoValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start Date is required")
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$").WithMessage("Start Date should have yyyy-mm-dd format");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End Date is required")
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$").WithMessage("End Date should have yyyy-mm-dd format");

            RuleFor(x => x)
                .Must(x => IsEndDateAfterStartDate(x.StartDate, x.EndDate))
                .WithMessage("End Date must be after Start Date")
                .When(x => IsValidDateFormat(x.StartDate) && IsValidDateFormat(x.EndDate));


            // Validate the Sessions array using ScheduleSessionDtoValidator
            RuleFor(x => x.Sessions)
                .NotNull().WithMessage("Sessions cannot be null")
                .NotEmpty().WithMessage("At least one session is required");

            // Validate each session in the Sessions array
            RuleForEach(x => x.Sessions)
                .SetValidator(new ScheduleSessionDtoValidator());
        }

        private bool IsValidDateFormat(string date)
        {
            return DateTime.TryParseExact(date, "yyyy-MM-dd", null,
                   System.Globalization.DateTimeStyles.None, out _);
        }

        private bool IsEndDateAfterStartDate(string startDate, string endDate)
        {
            if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", null,
                   System.Globalization.DateTimeStyles.None, out var start) &&
                DateTime.TryParseExact(endDate, "yyyy-MM-dd", null,
                   System.Globalization.DateTimeStyles.None, out var end))
            {
                return end > start;
            }
            return false;
        }

    }

    public class ScheduleSessionDtoValidator : AbstractValidator<ScheduleSessionDto>
    {
        public ScheduleSessionDtoValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .Must(BeValidDayOfWeek)
                .WithMessage("DayOfWeek must be a valid day (0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday)");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("StartTime is required")
                .Must(BeValidTimeFormat)
                .WithMessage("StartTime must be in HH:mm format (e.g., '09:30', '14:00')");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("EndTime is required")
                .Must(BeValidTimeFormat)
                .WithMessage("EndTime must be in HH:mm format (e.g., '09:30', '14:00')");

            RuleFor(x => x)
                .Must(x => IsEndTimeAfterStartTime(x.StartTime, x.EndTime))
                .WithMessage("EndTime must be after StartTime")
                .When(x => BeValidTimeFormat(x.StartTime) && BeValidTimeFormat(x.EndTime));

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location cannot be empty");
        }

        private bool BeValidDayOfWeek(int dayOfWeek)
        {
            return Enum.IsDefined(typeof(DayOfWeek), dayOfWeek);
        }

        private bool BeValidTimeFormat(string time)
        {
            return TimeSpan.TryParseExact(time, @"hh\:mm", null, out _);
        }

        private bool IsEndTimeAfterStartTime(string startTime, string endTime)
        {
            if (TimeSpan.TryParseExact(startTime, @"hh\:mm", null, out var start) &&
                TimeSpan.TryParseExact(endTime, @"hh\:mm", null, out var end))
            {
                return end > start;
            }
            return false;
        }
    }

}
