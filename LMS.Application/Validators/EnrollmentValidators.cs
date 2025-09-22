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
                
        }
    }
}
