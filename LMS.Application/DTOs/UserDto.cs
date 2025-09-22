using LMS.Application.Strategies.Sorting;
using LMS.Domain.Entities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.DTOs
{
    // Read

    public class GetUsersQueryDto
    {
        public string? Role {  get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? SortBy { get; set; }
        public bool? SortAsc { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    public class UserUpdateDto
    {
        // all users
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BirthDate { get; set; }

        // tutor only
        public string? Bio {get; set; }
        public string? Expertise { get; set; }

        // student only
        public string? Major { get; set; }
    }

    public class SuspendUserDto
    {
        public bool IsSuspended { get; set; }
    }


    // Response
    public class UserResponseDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string BirthDate { get; set; } = null!;
        public bool Suspended { get; set; }

        public StudentProfileResponseDto StudentProfile { get; set; } = null!;
        public TutorProfileResponseDto TutorProfile { get; set; } = null!;
    }

    public class StudentProfileResponseDto
    {
        public string Id { get; set; } = null!;
        public string Major { get; set; } = null!;
    }

    public class TutorProfileResponseDto
    {
        public string Id { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Expertise { get; set; } = null!;
    }

    public class TutorProfileExtendedResponseDto : TutorProfileResponseDto
    { 
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

}
