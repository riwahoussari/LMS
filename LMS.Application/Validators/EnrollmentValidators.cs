using FluentValidation;
using LMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Validators
{
    public class GetEnrollmentsQueryDtoValidator : AbstractValidator<GetEnrollmentsQueryDto>
    {
        public GetEnrollmentsQueryDtoValidator() 
        {
            RuleFor(x => x.StudentProfileId).NotEmpty().WithMessage("StudentProfileId is required");
        }
    }
}
